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
using Retro80Utilities.Palette;

namespace Retro80Colorizer.ColorReducer.SimpleLChDistanceReducer
{
    /// <summary>
    /// LCh空間の距離（WeightedDistance）を使って減色処理を行うサンプル。
    /// 指定された距離しきい値以下の色は同一クラスタとしてまとめる。
    /// </summary>
    public static class SimpleLChDistanceReducer
    {
        /// <summary>
        /// 入力画像から代表色を抽出し、減色されたBitmapを返す。
        /// </summary>
        /// <param name="bmp">入力画像</param>
        /// <param name="threshold">LCh距離のしきい値（例: 10.0）</param>
        /// <returns>減色されたBitmap</returns>
        public static Bitmap Reduce(Bitmap bmp, double threshold)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            var reduced = new Bitmap(width, height);

            var colorMap = new Dictionary<LChColor, Color>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (pixel.A < 255) continue;

                    var lab = LabColor.FromRgb(pixel);
                    var lch = LChColor.FromLab(lab);

                    // 近似クラスタを探す
                    LChColor? nearest = null;
                    foreach (var cluster in colorMap.Keys)
                    {
                        if (LChColor.WeightedDistance(lch, cluster) <= threshold)
                        {
                            nearest = cluster;
                            break;
                        }
                    }

                    // 見つかればそれを使う、なければ新クラスタを登録
                    if (nearest != null)
                    {
                        reduced.SetPixel(x, y, colorMap[nearest.Value]);
                    }
                    else
                    {
                        colorMap[lch] = pixel;
                        reduced.SetPixel(x, y, pixel);
                    }
                }
            }

            return reduced;
        }

        /// <summary>
        /// 画像から代表色を取得（LChクラスタを返す）
        /// </summary>
        /// <param name="bmp">入力画像</param>
        /// <param name="threshold">LCh距離のしきい値（例: 10.0）</param>
        /// <returns>クラスタ代表色のリスト</returns>
        public static List<Color> GetRepresentativeColors(Bitmap bmp, double threshold)
        {
            var colorMap = new List<Tuple<LChColor, Color>>();

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (pixel.A < 255) continue;

                    var lab = LabColor.FromRgb(pixel);
                    var lch = LChColor.FromLab(lab);

                    bool matched = false;
                    foreach (var cluster in colorMap)
                    {
                        if (LChColor.WeightedDistance(lch, cluster.Item1) <= threshold)
                        {
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        colorMap.Add(Tuple.Create(lch, pixel));
                    }
                }
            }

            var result = new List<Color>();
            foreach (var item in colorMap)
            {
                result.Add(item.Item2);
            }

            return result;
        }
    }
}
