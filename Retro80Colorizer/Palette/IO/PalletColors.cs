using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

// Project: Retro80Colorizer
// Target Language Version: C# 7.3
// Platform: .NET Framework 4.8
// Note: Do not use features introduced after C# 7.3 (e.g., records, switch expressions, etc.)
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
