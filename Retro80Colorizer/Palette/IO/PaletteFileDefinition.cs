// @language-version: 7.3
// @framework: .NET Framework 4.8
// @target: Windows Forms
// @dependencies: Newtonsoft.Json
// @color-order: RGB
// @bitwise-color: RGB-aligned only
// @constraints:
// - Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
// - All color operations must use RGB order.
// - Bitwise color formats must preserve RGB structure unless explicitly required.
// - Avoid BRG, BGR, or platform-specific layouts unless documented.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Retro80Colorizer.Palette.IO
{
    /// <summary>
    /// This class represents a fixed-size color palette grid (32×32 cells) loaded from a PNG image.
    /// Each cell is 16×16 pixels, and the central pixel color is used as the representative color.
    /// Transparency (alpha=0) is treated as "no color" and skipped.
    /// </summary>
    public class PaletteFileDefinition
    {
        // Grid size constants (fixed layout)
        public const int GRID_HEIGHT = 32;
        public const int GRID_WIDTH = 32;
        public const int GRID_CELL_SIZE = 16;
        public const int ACTUAL_DATA_START_ROW = 1;

        /// <summary>
        /// このパレットが読み込まれた元のPNGファイルパス
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// 2D color list: outer list is rows, inner list is non-transparent colors in each row.
        /// Each color represents the center pixel of a 16×16 cell.
        /// </summary>
        public List<List<Color>> Colors { get; private set; }

        /// <summary>
        /// Default constructor (does not load anything).
        /// </summary>
        public PaletteFileDefinition() { }

        /// <summary>
        /// Constructor that loads and parses a PNG file immediately.
        /// </summary>
        /// <param name="PNGFilePath">Path to the palette PNG image</param>
        public PaletteFileDefinition(string PNGFilePath)
        {
            this.Filename = PNGFilePath;
            this.Colors = ReadFromFile(PNGFilePath);
        }

        /// <summary>
        /// Reads a PNG palette file and extracts the center color of each 16×16 cell in a 32×32 grid.
        /// Transparency is skipped. Grid size is strictly validated.
        /// </summary>
        /// <param name="PNGFilePath">Path to the palette PNG image</param>
        /// <returns>A 2D list of Color objects per row</returns>
        public List<List<Color>> ReadFromFile(string PNGFilePath)
        {
            if (!File.Exists(PNGFilePath))
            {
                MessageBox.Show($"ファイルが見つかりません: {PNGFilePath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new FileNotFoundException("Palette image file not found.", PNGFilePath);
            }

            this.Filename = PNGFilePath; // ← ここでも一応保持（単独呼び出し時の保険）

            Bitmap bitmap = new Bitmap(PNGFilePath);
            int expectedWidth = GRID_WIDTH * GRID_CELL_SIZE;
            int expectedHeight = GRID_HEIGHT * GRID_CELL_SIZE;

            if (bitmap.Width != expectedWidth || bitmap.Height != expectedHeight)
            {
                MessageBox.Show(
                    $"画像サイズが正しくありません。\n必要サイズ: {expectedWidth}×{expectedHeight}\n実サイズ: {bitmap.Width}×{bitmap.Height}",
                    "画像サイズエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidDataException("Bitmap dimensions do not match expected grid size.");
            }

            List<List<Color>> result = new List<List<Color>>();

            for (int row = ACTUAL_DATA_START_ROW; row < GRID_HEIGHT; row++)
            {
                List<Color> rowColors = new List<Color>();
                for (int col = 0; col < GRID_WIDTH; col++)
                {
                    // Calculate center of the cell
                    int centerX = col * GRID_CELL_SIZE + GRID_CELL_SIZE / 2;
                    int centerY = row * GRID_CELL_SIZE + GRID_CELL_SIZE / 2;

                    Color pixelColor = bitmap.GetPixel(centerX, centerY);

                    // Skip transparent (alpha = 0)
                    if (pixelColor.A == 0)
                        continue;

                    rowColors.Add(pixelColor);
                }

                // Always add the row, even if it's empty
                result.Add(rowColors);
            }

            this.Colors = result;
            return result;
        }

        /// <summary>
        /// 指定した行番号（0ベース）のカラーデータを取得します。
        /// 存在しない場合はMessageBoxを表示してIndexOutOfRangeExceptionをスローします。
        /// </summary>
        /// <param name="rowIndex">行インデックス（0〜31）</param>
        /// <returns>Colorのリスト（横方向）</returns>
        public List<Color> GetRow(int rowIndex)
        {
            if (Colors == null || rowIndex < 0 || rowIndex >= Colors.Count)
            {
                MessageBox.Show($"指定された行 {rowIndex} は存在しません。\n現在の行数: {Colors?.Count ?? 0}",
                                "パレット行エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IndexOutOfRangeException($"Row index {rowIndex} is out of bounds.");
            }

            return Colors[rowIndex];
        }
    }
}
