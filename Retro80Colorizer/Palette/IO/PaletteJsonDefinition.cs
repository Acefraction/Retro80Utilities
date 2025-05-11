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
using System.Linq;
using System.Text;


namespace Retro80Utilities.Palette.IO
{
    public class PaletteJsonDefinition
    {
        /// <summary>
        /// パレットのワークフローカテゴリー
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// パレットファイル名（PNG形式）
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// パレットファイルの説明（人間への説明用、処理には使わない）
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// パレットファイルにパレットの一覧（ＰＮＧファイル内で行方向に並んでいる）
        /// </summary>
        public List<string> names { get; set; }
    }
}
