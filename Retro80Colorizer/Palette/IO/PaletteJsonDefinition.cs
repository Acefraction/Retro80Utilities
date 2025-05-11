using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Project: Retro80Colorizer
// Target Language Version: C# 7.3
// Platform: .NET Framework 4.8
// Note: Do not use features introduced after C# 7.3 (e.g., records, switch expressions, etc.)
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
