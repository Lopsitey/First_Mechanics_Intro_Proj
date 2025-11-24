#region

using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Utilities
{
    /// <summary>
    /// A static toolbox for generic helper functions used across the project.
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        /// Checks if a value is close to zero within a specific threshold.
        /// Useful for velocity checks, float comparisons, and dead-zones.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="threshold">The tolerance range (default 0.1f).</param>
        /// <returns>True if the value is within the threshold of zero.</returns>
        public static bool ALMOST_ZERO(float value, float threshold = 0.1f)
        {
            return Mathf.Abs(value) < threshold;
        }

        /// <summary>
        /// For 2D Vectors
        /// </summary>
        /// <param name="value"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool ALMOST_ZERO(Vector2 value, float threshold = 0.1f)
        {
            bool bothBelow = Mathf.Abs(value.x) < threshold && Mathf.Abs(value.y) < threshold;
            return bothBelow;
        }

        /// <summary>
        /// Remaps a value from one range to another (like Arduino's map function).
        /// I might need this for UI bars, volume sliders, or colour blending.
        /// </summary>
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary>
        /// Checks if a specific layer is inside a LayerMask.
        /// Useful for trigger checks.
        /// </summary>
        public static bool IsLayerInMask(int layer, LayerMask mask)
        {
            return (mask == (mask | (1 << layer)));
        }
    }
}