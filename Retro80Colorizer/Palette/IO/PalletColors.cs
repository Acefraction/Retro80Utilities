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

namespace Retro80Utilities.Palette.IO
{
    public class PaletteColors
    {
        /// <summary>
        /// カテゴリー（ワークフロー分類）
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// パレット名（セマンティック名）
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 表示する色のリスト（最大16色推奨）
        /// </summary>
        public List<Color> Colors { get; set; }
    }
}
