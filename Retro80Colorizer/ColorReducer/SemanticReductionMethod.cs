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

namespace Retro80Colorizer.ColorReducer
{
    /// <summary>
    /// SemanticReductionMethod defines the selectable semantic compression strategies.
    /// These methods determine how input pixels are grouped and reduced using the provided palettes.
    /// </summary>
    public enum SemanticReductionMethod
    {
        /// <summary>
        /// Heuristic region-based reduction using noise estimation and palette matching.
        /// Suitable for structured UI or clean image inputs.
        /// </summary>
        HeuristicSemanticRegionReducer = 0,

        /// <summary>
        /// Thermodynamic reduction using Monte Carlo simulation and pixel energy stabilization.
        /// Best for complex or noisy images where gradual convergence is required.
        /// </summary>
        SemanticThermodynamicReducer = 1,

        /// <summary>
        /// Basic color reduction using Lab color distance matching.
        /// Intended for single-palette scenarios and simple nearest-color mapping.
        /// </summary>
        SimpleLabDistanceReducer = 2
    }
}
