// @language-version: 7.3
// @framework: .NET Framework 4.8
// @target: Windows Forms
// @dependencies: Newtonsoft.Json
// @color-order: RGB
// @bitwise-color: RGB-aligned only
// @constraints:
/// Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
/// All color operations must use RGB order.
/// Bitwise color formats must preserve RGB structure unless explicitly required.
/// Avoid BRG, BGR, or platform-specific layouts unless documented.

using System;
using System.Drawing;

namespace Retro80Colorizer.Palette.IO
{
    /// <summary>
    /// パレットテンプレートの内部表現クラスです。
    /// 各セルの色情報（32×32）と、縦方向の機能ラベル（最大32行）を管理します。
    /// </summary>
    public class PaletteFileDefinition
    {
        public const int GridSize = 32;

        /// <summary>
        /// 色の2次元配列。[x, y] でアクセス（0〜31）
        /// 各要素はグリッドセルの中央色です。
        /// </summary>
        public Color[,] Colors { get; private set; }

        /// <summary>
        /// 各Y行（縦方向）に対応する機能ラベル（例："肌：明るめ"など）。
        /// 未使用行は空文字列として扱います。
        /// </summary>
        public string[] RowLabels { get; private set; }

        /// <summary>
        /// カラーとラベルを指定してパレット定義を構築します。
        /// </summary>
        /// <param name="colors">32×32の色情報</param>
        /// <param name="rowLabels">最大32個の行ラベル（null可）</param>
        public PaletteFileDefinition(Color[,] colors, string[] rowLabels)
        {
            if (colors == null) throw new ArgumentNullException(nameof(colors));
            if (colors.GetLength(0) != GridSize || colors.GetLength(1) != GridSize)
                throw new ArgumentException("カラーグリッドのサイズは32×32である必要があります。");

            this.Colors = colors;

            // 行ラベルの初期化（足りない分は空文字列）
            this.RowLabels = new string[GridSize];
            for (int y = 0; y < GridSize; y++)
            {
                this.RowLabels[y] = (rowLabels != null && y < rowLabels.Length)
                    ? rowLabels[y] ?? ""
                    : "";
            }
        }

        /// <summary>
        /// 指定した行のカラー一覧（横方向）を取得します。
        /// </summary>
        public Color[] GetRowColors(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= GridSize)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            var row = new Color[GridSize];
            for (int x = 0; x < GridSize; x++)
                row[x] = Colors[x, rowIndex];

            return row;
        }

        /// <summary>
        /// 指定した行のラベルを取得します。
        /// 空文字列またはnullの場合は未使用行とみなします。
        /// </summary>
        public string GetLabel(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= GridSize) return null;
            return RowLabels[rowIndex];
        }
    }
}
