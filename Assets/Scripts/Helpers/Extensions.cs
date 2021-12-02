using UnityEngine;

namespace Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Only changes Z value of the vector.
        /// </summary>
        /// <param name="vector">Vector3 to update.</param>
        /// <param name="val">New z value.</param>
        /// <returns>Vector3 with updated Z.</returns>
        public static Vector3 SetZ(this Vector3 vector, float val)
        {
            return new Vector3(vector.x, vector.y, val);
        }

        /// <summary>
        /// Returns Vector2 as Vector2Int
        /// </summary>
        /// <param name="vector">Vector2 to get values from.</param>
        /// <returns>given Vector2 as Vector2Int</returns>
        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static bool IsBetween(this int val, int min, int max)
        {
            return val >= min && val <= max;
        }
    
        public static bool IsBetween(this float val, float min, float max)
        {
            return val >= min && val <= max;
        }

        /// <summary>
        /// Deletes all children of transform.
        /// </summary>
        /// <param name="transform">Transform to delete children.</param>
        public static void DeleteAllChildren(this Transform transform)
        {
            while (transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}