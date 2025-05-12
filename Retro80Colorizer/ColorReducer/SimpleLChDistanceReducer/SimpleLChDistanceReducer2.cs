using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Retro80Utilities.Palette;

namespace Retro80Colorizer.ColorReducer.SimpleLChDistanceReducer
{
    public static class SimpleLChDistanceReducer2
    {
        public static void ReduceWithLabels(Bitmap bmp, double clusterDistance, double quantizeDistance,
            out Bitmap reduced, out int[,] labelMap, out List<Color> clusterColors, out List<List<Color>> clusterMembers)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            reduced = new Bitmap(width, height);
            labelMap = new int[width, height];

            clusterMembers = new List<List<Color>>();
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

        public static Dictionary<int, Bitmap> GenerateClusterDitherTiles(List<List<Color>> clusterMembers, List<Color> clusterColors, int maxColorsPerCluster, double diversityDistance)
        {
            int clusterCount = clusterColors.Count;
            Dictionary<int, Bitmap> ditherTiles = new Dictionary<int, Bitmap>();
            int[,] bayer2x2 = new int[,] { { 0, 2 }, { 3, 1 } };

            for (int id = 0; id < clusterCount; id++)
            {
                var colorSamples = clusterMembers[id];
                List<Color> uniqueColors = new List<Color>();

                foreach (var c in colorSamples)
                {
                    var candidateLch = LChColor.FromColor(c);
                    bool far = true;
                    foreach (var existing in uniqueColors)
                    {
                        if (LChColor.WeightedDistance(candidateLch, LChColor.FromColor(existing)) < diversityDistance)
                        {
                            far = false;
                            break;
                        }
                    }
                    if (far) uniqueColors.Add(c);
                    if (uniqueColors.Count >= maxColorsPerCluster) break;
                }

                Bitmap tile = new Bitmap(2, 2);
                Color baseColor = clusterColors[id];
                Color altColor = baseColor;

                // swap: base は常に「暗い色」にする 
                {
                    // baseColor, altColor の決定後
                    var lchBase = LChColor.FromColor(baseColor);
                    var lchAlt = LChColor.FromColor(altColor);

                    if (lchAlt.L < lchBase.L)
                    {
                        var tmp = baseColor;
                        baseColor = altColor;
                        altColor = tmp;
                    }
                }

                if (uniqueColors.Count > 1)
                {
                    LChColor baseLch = LChColor.FromColor(baseColor);
                    double minDist = double.MaxValue;
                    foreach (var c in uniqueColors)
                    {
                        double dist = LChColor.WeightedDistance(baseLch, LChColor.FromColor(c));
                        if (dist > 0.1 && dist < minDist)
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
                        int bayerVal = bayer2x2[dy, dx];
                        Color dithered = (bayerVal < 2) ? baseColor : altColor;
                        tile.SetPixel(dx, dy, dithered);
                    }
                }

                ditherTiles[id] = tile;
            }

            return ditherTiles;
        }

        public static Bitmap RenderDitheredImage(int[,] highResLabelMap, Dictionary<int, Bitmap> ditherTiles)
        {
            int width = highResLabelMap.GetLength(0);
            int height = highResLabelMap.GetLength(1);
            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int clusterId = highResLabelMap[x, y];
                    if (ditherTiles.ContainsKey(clusterId))
                    {
                        Bitmap tile = ditherTiles[clusterId];
                        int tx = x % tile.Width;
                        int ty = y % tile.Height;
                        result.SetPixel(x, y, tile.GetPixel(tx, ty));
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            return result;
        }

        private static Color AverageLChColor(List<Color> colors)
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
    }
}