using UnityEngine;

namespace BROINK
{
    public static class Utils
    {
        public static float AbsDeltaAngle(float current, Vector2 target)
        {
            return AbsDeltaAngle(current, target.ToAngle());
        }

        public static float AbsDeltaAngle(float current, float target)
        {
            return Mathf.Abs(Mathf.DeltaAngle(current, target));
        }

        public static void SetAlpha(ref Color color, float alpha)
        {
            color.a = alpha;
        }

        public static T Choose<T>(params T[] items)
        {
            return items[Random.Range(0, items.Length)];
        }
    }
}