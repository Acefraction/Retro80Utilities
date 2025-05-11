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
    internal class PaletteDefinition
    {
        public class PaletteJsonDefinition
        {
            public string filename { get; set; }
            public string category { get; set; }
            public string name { get; set; }
            public List<string> rows { get; set; }
        }
    }
}
