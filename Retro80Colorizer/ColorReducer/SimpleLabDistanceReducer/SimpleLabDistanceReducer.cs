using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Retro80Utilities.Palette.IO;
using Retro80Colorizer.ColorReducer;
using Retro80Colorizer.ColorReducer.SimpleLabDistanceReducer;

namespace Retro80Colorizer.ColorReducer.SimpleLabDistanceReducer
{
    /// <summary>
    /// SimpleLabDistanceReducer reduces image colors using a single palette,
    /// selecting the closest color in Lab space (ΔE) for each pixel.
    /// This reducer is simple and intended for testing or baseline comparison.
    /// </summary>
    public class SimpleLabDistanceReducer : IColorReducer
    {
        /// <summary>
        /// The name of this reducer, used for UI and logging purposes.
        /// </summary>
        public string Name => "Simple Lab Distance Reducer";

        /// <summary>
        /// Applies Lab-based nearest color reduction using a single palette.
        /// If multiple palettes are provided, a warning is shown and null is returned.
        /// </summary>
        /// <param name="source">The full-color source image.</param>
        /// <param name="palettes">A list containing exactly one PaletteColors item.</param>
        /// <returns>A dictionary containing one reduced bitmap with the palette name as key, or null.</returns>
        public Dictionary<string, Bitmap> Reduce(Bitmap source, List<PaletteColors> palettes)
        {
            if (palettes == null || palettes.Count != 1)
            {
                MessageBox.Show(
                    "SimpleLabDistanceReducer は1つのパレットしか扱えません。",
                    "減色処理エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return null;
            }

            var palette = palettes[0];
            var reduced = new Bitmap(source.Width, source.Height);

            // キャッシュ：パレット内の Lab変換済みカラー
            var paletteLab = new List<LabColor>();
            foreach (var c in palette.Colors)
                paletteLab.Add(LabColor.FromRgb(c));

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var original = source.GetPixel(x, y);
                    var originalLab = LabColor.FromRgb(original);

                    double minDistance = double.MaxValue;
                    int bestIndex = 0;

                    for (int i = 0; i < paletteLab.Count; i++)
                    {
                        var dist = LabColor.DeltaE(originalLab, paletteLab[i]);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            bestIndex = i;
                        }
                    }

                    reduced.SetPixel(x, y, palette.Colors[bestIndex]);
                }
            }

            return new Dictionary<string, Bitmap>
            {
                { palette.name, reduced }
            };
        }
    }
}
