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

using System.Collections.Generic;
using System.Drawing;
using Retro80Utilities.Palette.IO;

namespace Retro80Colorizer.ColorReducer
{
    /// <summary>
    /// IColorReducer defines a strategy for reducing the colors of an image
    /// by applying named palette blocks independently.
    /// </summary>
    public interface IColorReducer
    {
        /// <summary>
        /// Reduces a full-color image into multiple blocks, each mapped to its own palette.
        /// </summary>
        /// <param name="source">The input image to be reduced.</param>
        /// <param name="palettes">A list of named palette definitions to apply.</param>
        /// <returns>A dictionary mapping palette names to their respective reduced bitmaps.</returns>
        Dictionary<string, Bitmap> Reduce(Bitmap source, List<PaletteColors> palettes);

        /// <summary>
        /// The name of this reduction algorithm.
        /// </summary>
        string Name { get; }
    }
}
