// @language-version: 7.3
// @framework: .NET Framework 4.8
// @target: Windows Forms
// @dependencies: Newtonsoft.Json, NLog
// @color-order: RGB
// @bitwise-color: RGB-aligned only
// @constraints:
// - Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
// - All color operations must use RGB order.
// - Bitwise color formats must preserve RGB structure unless explicitly required.
// - Avoid BRG, BGR, or platform-specific layouts unless documented.
// - If the processing logic is sufficiently complex, use NLog for debug logging and traceability.
// - All classes, methods, and important variables should include AI-friendly XML comments
//   describing their role, input/output behavior, and structural context.



using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Retro80Colorizer.Controls.ListView
{
    /// <summary>
    /// 複数色を持つ1行分のデータモデル
    /// </summary>
    public class MultiColorSelectionListDataSource
    {
        /// <summary>
        /// 表示名（テキストラベル）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表示する色のリスト（最大16色推奨）
        /// </summary>
        public List<Color> Colors { get; set; }

        /// <summary>
        /// コンストラクタ（空のリストで初期化）
        /// </summary>
        public MultiColorSelectionListDataSource()
        {
            Colors = new List<Color>();
        }

        /// <summary>
        /// コンストラクタ（初期データ付き）
        /// </summary>
        public MultiColorSelectionListDataSource(string name, IEnumerable<Color> colors)
        {
            Name = name;
            Colors = new List<Color>(colors ?? new List<Color>());
        }
    }
}