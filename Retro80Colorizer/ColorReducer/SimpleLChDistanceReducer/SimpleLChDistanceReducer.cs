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
//
// @notes:
//   LCh 距離のしきい値（threshold）の目安:
//     - 3.0: 非常に類似した色（肌の陰影や髪の影など微差）をまとめるレベル
//     - 5.0: 明確な差を感じるがグラデーションでつながる範囲（塗りの影・光）
//     - 10.0+: 完全に別の色として認識される差（パレット化や分類用途に最適）
//   - PNG画像のピクセルが半透明（alpha < 255）の場合、それらはすべて処理対象から除外されます。

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
            var clusters = new List<Tuple<LChColor, Color>>();
            var clusterMemberColors = new Dictionary<int, List<Color>>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (pixel.A < 255) { reduced.SetPixel(x, y, Color.Transparent); labelMap[x, y] = -1; continue; }

                    var lab = LabColor.FromRgb(pixel);
                    var lch = LChColor.FromLab(lab);

                    int matchedIndex = -1;
                    for (int i = 0; i < clusters.Count; i++)
                    {
                        if (LChColor.WeightedDistance(clusters[i].Item1, lch) <= clusterDistance)
                        {
                            matchedIndex = i;
                            break;
                        }
                    }

                    if (matchedIndex == -1)
                    {
                        clusters.Add(Tuple.Create(lch, pixel));
                        matchedIndex = clusters.Count - 1;
                    }

                    labelMap[x, y] = matchedIndex;

                    if (!clusterMemberColors.ContainsKey(matchedIndex))
                        clusterMemberColors[matchedIndex] = new List<Color>();
                    clusterMemberColors[matchedIndex].Add(pixel);
                }
            }

            clusterColors = new List<Color>();
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusterMemberColors.ContainsKey(i))
                {
                    clusterColors.Add(AverageColor(clusterMemberColors[i]));
                }
                else
                {
                    clusterColors.Add(clusters[i].Item2); // fallback
                }
            }

            // Second pass: assign quantized color to output image
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
                    var lab = LabColor.FromRgb(pixel);
                    var lch = LChColor.FromLab(lab);

                    Color quantizedColor = clusterColors[clusterId];
                    double dist = LChColor.WeightedDistance(lch, LChColor.FromColor(quantizedColor));

                    if (dist <= quantizeDistance)
                        reduced.SetPixel(x, y, quantizedColor);
                    else
                        reduced.SetPixel(x, y, pixel); // fallback
                }
            }
        }

        private static Color AverageColor(List<Color> colors)
        {
            long r = 0, g = 0, b = 0;
            foreach (var c in colors)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }

            int count = colors.Count;
            if (count == 0) return Color.Transparent;
            return Color.FromArgb((int)(r / count), (int)(g / count), (int)(b / count));
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

        public static Bitmap UpscaleWith2x2Dither(int[,] labelMap, List<Color> clusterColors, int maxColors, double diversityDistance)
        {
            int width = labelMap.GetLength(0);
            int height = labelMap.GetLength(1);
            Bitmap result = new Bitmap(width * 2, height * 2);
            int[,] bayer2x2 = new int[,] { { 0, 2 }, { 3, 1 } };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = labelMap[x, y];
                    Color baseColor = (id >= 0 && id < clusterColors.Count) ? clusterColors[id] : Color.Black;
                    Color altColor = baseColor;

                    // 同一クラスタから別の色がある場合に限定して選択
                    for (int i = 0; i < clusterColors.Count; i++)
                    {
                        if (i != id)
                            continue;
                        Color candidate = clusterColors[i];
                        if (candidate != baseColor)
                        {
                            altColor = candidate;
                            break;
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