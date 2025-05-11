using Retro80Colorizer.ColorReducer;

namespace Retro80Colorizer.ColorReducer.SimpleLabDistanceReducer
{
    /// <summary>
    /// ColorReducerProvider is a factory-style class that provides an instance of
    /// the SimpleLabDistanceReducer. This is intended to encapsulate the logic for
    /// creating this specific color reducer implementation.
    /// </summary>
    public static class ColorReducerProvider
    {
        /// <summary>
        /// Creates and returns a new instance of SimpleLabDistanceReducer.
        /// Use this method when only this specific reducer strategy is desired.
        /// </summary>
        /// <returns>An instance of IColorReducer implemented by SimpleLabDistanceReducer.</returns>
        public static IColorReducer Create()
        {
            return new SimpleLabDistanceReducer();
        }
    }
}
