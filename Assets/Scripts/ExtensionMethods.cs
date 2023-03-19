using UnityEngine;

namespace BROINK
{
    public static class ExtensionMethods
    {
        public static float ToAngle(this Vector2 direction)
        {
            return Vector2.SignedAngle(new(1, 0), direction);
        }

        public static Vector2 ToDirectionVector(this float angle)
        {
            return new(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
        }
    }
}