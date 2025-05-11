using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

// Project: Retro80Colorizer
// Target Language Version: C# 7.3
// Platform: .NET Framework 4.8
// Note: Do not use features introduced after C# 7.3 (e.g., records, switch expressions, etc.)
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