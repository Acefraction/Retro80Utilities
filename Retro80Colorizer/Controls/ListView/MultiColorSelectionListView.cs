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
using System.Windows.Forms;

namespace Retro80Colorizer.Controls.ListView
{
    /// <summary>
    /// チェックボックス付きで複数選択可能な、色つきリストビュー
    /// </summary>
    public class MultiColorSelectionListView : System.Windows.Forms.ListView
    {
        public MultiColorSelectionListView()
        {
            this.View = View.Details;
            this.CheckBoxes = true;
            this.FullRowSelect = true;
            this.OwnerDraw = true;

            this.Columns.Add("名前", 120);
            this.Columns.Add("色", 320); // 色表示用（16×12 + 隙間ぶん）

            this.DrawItem += (s, e) => { /* suppress checkbox redraw */ };
            this.DrawColumnHeader += (s, e) => e.DrawDefault = true;
            this.DrawSubItem += MultiColorSelectionListView_DrawSubItem;
        }

        /// <summary>
        /// データをリストビューにバインドする
        /// </summary>
        /// <param name="data">表示データリスト</param>
        public void SetData(List<MultiColorSelectionListDataSource> data)
        {
            this.Items.Clear();

            int maxTextWidth = 0;
            using (Graphics g = this.CreateGraphics())
            {
                foreach (var entry in data)
                {
                    var item = new ListViewItem(entry.Name);
                    item.SubItems.Add(""); // 色用カラム
                    item.Tag = entry;
                    this.Items.Add(item);

                    // フォントで文字列幅を測定
                    SizeF size = g.MeasureString(entry.Name, this.Font);
                    maxTextWidth = Math.Max(maxTextWidth, (int)size.Width);
                }
            }

            // 最小幅 + α（マージン込みで調整）
            this.Columns[0].Width = Math.Min(Math.Max(maxTextWidth + 20, 100), 300);
        }


        private void MultiColorSelectionListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawDefault = true;
                return;
            }

            var data = e.Item.Tag as MultiColorSelectionListDataSource;
            if (data == null || data.Colors == null) return;

            Rectangle bounds = e.SubItem.Bounds;
            int squareSize = 12;
            int spacing = 4;

            for (int i = 0; i < data.Colors.Count && i < 16; i++)
            {
                Rectangle rect = new Rectangle(
                    bounds.Left + i * (squareSize + spacing),
                    bounds.Top + (bounds.Height - squareSize) / 2,
                    squareSize,
                    squareSize
                );

                using (Brush brush = new SolidBrush(data.Colors[i]))
                {
                    e.Graphics.FillRectangle(brush, rect);
                    e.Graphics.DrawRectangle(Pens.Black, rect);
                }
            }
        }

        /// <summary>
        /// 現在チェックされているデータソースを返す
        /// </summary>
        public List<MultiColorSelectionListDataSource> GetCheckedItems()
        {
            var result = new List<MultiColorSelectionListDataSource>();
            foreach (ListViewItem item in this.Items)
            {
                if (item.Checked && item.Tag is MultiColorSelectionListDataSource data)
                {
                    result.Add(data);
                }
            }
            return result;
        }
    }
}