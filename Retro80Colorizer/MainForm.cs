// Project: Retro80Colorizer
// Target Language Version: C# 7.3
// Platform: .NET Framework 4.8
// Note: Do not use features introduced after C# 7.3 (e.g., records, switch expressions, etc.)
// This project uses RGB order for all color array indexing and conversions.
// Any bitwise color definitions should also respect the RGB convention, unless otherwise noted.
// Do NOT use BRG, BGR, or machine-specific orders unless explicitly required.
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

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
    }
}
