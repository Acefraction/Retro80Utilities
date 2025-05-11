// Project: Retro80Colorizer
// Target Language Version: C# 7.3
// Platform: .NET Framework 4.8
// Note: Do not use features introduced after C# 7.3 (e.g., records, switch expressions, etc.)
// This project uses RGB order for all color array indexing and conversions.
// Any bitwise color definitions should also respect the RGB convention, unless otherwise noted.
// Do NOT use BRG, BGR, or machine-specific orders unless explicitly required.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Retro80Colorizer.Controls.ListView;
using Retro80Utilities.Palette.IO;

namespace Retro80Utilities
{

    public partial class frmPalletesChoice : Form
    {
        private MultiColorSelectionListView listView;
        private ToolStripStatusLabel statusLabel;

        public frmPalletesChoice()
        {
            InitializeComponent();

            // コントロール初期化
            {

                listView = new MultiColorSelectionListView
                {
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel1.Controls.Add(listView, 0, 0);

                btnClose.Click += btnClose_Click;
            }

            // データ投入（例）
            listView.SetData(new List<MultiColorSelectionListDataSource>
            {
                new MultiColorSelectionListDataSource
                {
                    Name = "パレットA（キャラクター）",
                    Colors = new List<Color> { Color.Red, Color.Green, Color.Blue }
                },
                new MultiColorSelectionListDataSource
                {
                    Name = "パレットB",
                    Colors = new List<Color> { Color.Cyan, Color.Magenta, Color.Yellow }
                }
            });
            {
                statusLabel = new ToolStripStatusLabel();

                statusStrip1.Items.Add(statusLabel);
                this.Controls.Add(statusStrip1);
                statusLabel.Text = "使用する色を選択してください。";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var checkedItems = listView.GetCheckedItems();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("何も選択されていません。");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in checkedItems)
            {
                sb.AppendLine($"■ {item.Name}");
                foreach (var color in item.Colors)
                {
                    string name = GetColorName(color);
                    sb.AppendLine("　- " + name);
                }
            }

            MessageBox.Show(sb.ToString(), "選択された色一覧");
        }

        private string GetColorName(Color color)
        {
            if (color.IsNamedColor)
            {
                return color.Name;
            }
            return $"R:{color.R}, G:{color.G}, B:{color.B}";
        }
        public  void SetPaletteSource(List<PaletteColors> palettes)
        {
            var data = palettes.Select(p => new MultiColorSelectionListDataSource
            {
                Name = $"{p.name}({p.category})",
                Colors = p.Colors ?? new List<Color>()
            }).ToList();

            listView.SetData(data);
        }

        public List<MultiColorSelectionListDataSource> SelectedItems
        {
            get { return listView.GetCheckedItems(); }
        }

    }
}
