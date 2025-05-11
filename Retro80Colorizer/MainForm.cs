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

namespace Retro80Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Retro80Utilities.Palette.IO;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            {
                this.btnPalleteChoice.Click += (s, e) =>
                {


                    var palettes = new List<PaletteColors>
                    {
                        new PaletteColors
                        {
                            name = "基本色",
                            category = "UI",
                            Colors = new List<Color> { Color.Red, Color.Green, Color.Blue }
                        },
                        new PaletteColors
                        {
                            name = "補助色",
                            category = "アクセント",
                            Colors = new List<Color> { Color.Orange, Color.Purple }
                        }
                    };


                    var palletesChoiceForm = new frmPalletesChoice();
                    palletesChoiceForm.SetPaletteSource(palettes);

                    if (palletesChoiceForm.ShowDialog() == DialogResult.OK)
                    {
                        var result = palletesChoiceForm.SelectedItems;
                        foreach (var item in result)
                        {
                            Console.WriteLine(item.Name);
                            foreach (var color in item.Colors)
                            {
                                Console.WriteLine($" - {color}");
                            }
                        }
                    }
                };
            }
        }
    }
}
