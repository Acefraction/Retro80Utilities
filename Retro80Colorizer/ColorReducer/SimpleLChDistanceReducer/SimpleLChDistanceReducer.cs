// @language: C#
// @language-version: 7.3
// @framework: .NET Framework 4.8
// @platform: Windows Forms
// @dependencies: Newtonsoft.Json, NLog
// @purpose: Perceptual color reduction using LCh (Lightness, Chroma, Hue) space distance
// @constraints:
//   - Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
//   - All color operations must use RGB order unless otherwise specified.
//   - Bitwise color formats must preserve RGB structure.
//   - Use NLog for trace-level diagnostics in complex logic sections.
//   - All public classes, methods, and critical fields should include AI-readable XML comments
//     describing their purpose, input/output contracts, and design assumptions.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Retro80Utilities.Palette;

namespace Retro80Colorizer.ColorReducer.SimpleLChDistanceReducer
{
    public static class SimpleLChDistanceReducer
    {
        public static void ReduceWithLabels(Bitmap bmp, double clusterDistance, double quantizeDistance,
            out Bitmap reduced, out int[,] labelMap, out List<Color> clusterColors)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            reduced = new Bitmap(width, height);
            labelMap = new int[width, height];

            List<List<Color>> clusterMembers = new List<List<Color>>();
            List<LChColor> clusterCenters = new List<LChColor>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (pixel.A < 255) { reduced.SetPixel(x, y, Color.Transparent); labelMap[x, y] = -1; continue; }

                    LChColor lch = LChColor.FromLab(LabColor.FromRgb(pixel));

                    int matchIndex = -1;
                    double minDist = double.MaxValue;
                    for (int i = 0; i < clusterCenters.Count; i++)
                    {
                        double dist = LChColor.WeightedDistance(clusterCenters[i], lch);
                        if (dist < clusterDistance && dist < minDist)
                        {
                            minDist = dist;
                            matchIndex = i;
                        }
                    }

                    if (matchIndex == -1)
                    {
                        clusterCenters.Add(lch);
                        clusterMembers.Add(new List<Color> { pixel });
                        labelMap[x, y] = clusterCenters.Count - 1;
                    }
                    else
                    {
                        clusterMembers[matchIndex].Add(pixel);
                        labelMap[x, y] = matchIndex;
                    }
                }
            }

            clusterColors = new List<Color>();
            for (int i = 0; i < clusterMembers.Count; i++)
            {
                Color avg = AverageLChColor(clusterMembers[i]);
                clusterColors.Add(avg);
                clusterCenters[i] = LChColor.FromLab(LabColor.FromRgb(avg));
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int clusterId = labelMap[x, y];
                    if (clusterId < 0 || clusterId >= clusterColors.Count)
                    {
                        reduced.SetPixel(x, y, Color.Transparent);
                        continue;
                    }

                    Color pixel = bmp.GetPixel(x, y);
                    LChColor lch = LChColor.FromLab(LabColor.FromRgb(pixel));
                    Color quantized = clusterColors[clusterId];
                    double dist = LChColor.WeightedDistance(lch, LChColor.FromColor(quantized));

                    if (dist <= quantizeDistance)
                        reduced.SetPixel(x, y, quantized);
                    else
                        reduced.SetPixel(x, y, pixel);
                }
            }
        }

        /// <summary>
        /// 指定されたRGBカラーリストのLCh空間での平均を計算し、
        /// 人間の知覚に基づく代表的な色を求めます。
        /// 色相は円環構造を考慮してベクトル平均し、
        /// 明度と彩度は単純平均します。
        /// </summary>
        /// <param name="colors">平均化対象の色のリスト（System.Drawing.Color）</param>
        /// <returns>平均された代表色（RGBカラー）</returns>
        public static Color AverageLChColor(List<Color> colors)
        {
            if (colors == null || colors.Count == 0)
                return Color.Transparent;

            double sumL = 0, sumC = 0, sumH_sin = 0, sumH_cos = 0;
            foreach (var c in colors)
            {
                var lch = new LChColor(c);
                sumL += lch.L;
                sumC += lch.C;

                double hRad = lch.H * Math.PI / 180.0;
                sumH_cos += Math.Cos(hRad);
                sumH_sin += Math.Sin(hRad);
            }

            double avgL = sumL / colors.Count;
            double avgC = sumC / colors.Count;
            double avgH = Math.Atan2(sumH_sin, sumH_cos) * 180.0 / Math.PI;
            if (avgH < 0) avgH += 360.0;

            return new LChColor(avgL, avgC, avgH).ToColor();
        }

        public static void ExportClusterLabelMapAsImage(int[,] labelMap, List<Color> clusterColors, string outputPath)
        {
            int width = labelMap.GetLength(0);
            int height = labelMap.GetLength(1);
            Bitmap map = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = labelMap[x, y];
                    if (idx >= 0 && idx < clusterColors.Count)
                        map.SetPixel(x, y, clusterColors[idx]);
                    else
                        map.SetPixel(x, y, Color.Transparent);
                }
            }

            map.Save(outputPath, ImageFormat.Png);
        }

        public static Bitmap UpscaleWith2x2Dither(int[,] labelMap, List<Color> clusterColors, int maxColorsPerCluster, double diversityDistance)
        {
            int width = labelMap.GetLength(0);
            int height = labelMap.GetLength(1);
            Bitmap result = new Bitmap(width * 2, height * 2);
            int[,] bayer2x2 = new int[,] { { 0, 2 }, { 3, 1 } };

            Dictionary<int, List<Color>> clusterDitherColors = new Dictionary<int, List<Color>>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = labelMap[x, y];
                    if (id < 0 || id >= clusterColors.Count) continue;

                    if (!clusterDitherColors.ContainsKey(id))
                        clusterDitherColors[id] = new List<Color>();

                    Color candidate = clusterColors[id];
                    LChColor candidateLch = LChColor.FromColor(candidate);
                    bool far = true;
                    foreach (var existing in clusterDitherColors[id])
                    {
                        if (LChColor.WeightedDistance(candidateLch, LChColor.FromColor(existing)) < diversityDistance)
                        {
                            far = false;
                            break;
                        }
                    }
                    if (far) clusterDitherColors[id].Add(candidate);
                    if (clusterDitherColors[id].Count >= maxColorsPerCluster) continue;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = labelMap[x, y];
                    Color baseColor = (id >= 0 && id < clusterColors.Count) ? clusterColors[id] : Color.Black;
                    Color altColor = baseColor;

                    if (clusterDitherColors.ContainsKey(id))
                    {
                        LChColor baseLch = LChColor.FromColor(baseColor);
                        double minDist = double.MaxValue;
                        foreach (var c in clusterDitherColors[id])
                        {
                            if (c == baseColor) continue;
                            double dist = LChColor.WeightedDistance(baseLch, LChColor.FromColor(c));
                            if (dist < minDist)
                            {
                                minDist = dist;
                                altColor = c;
                            }
                        }
                    }

                    for (int dy = 0; dy < 2; dy++)
                    {
                        for (int dx = 0; dx < 2; dx++)
                        {
                            int bx = dx;
                            int by = dy;
                            int bayerVal = bayer2x2[by, bx];
                            Color dithered = (bayerVal < 2) ? baseColor : altColor;
                            result.SetPixel(x * 2 + dx, y * 2 + dy, dithered);
                        }
                    }
                }
            }

            return result;
        }
    }
}   