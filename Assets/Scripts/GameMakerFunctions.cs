using UnityEngine;

namespace BROINK
{
    public static class GameMakerFunctions
    {
        public static float point_distance(float x1, float y1, float x2, float y2)
        {
            return point_distance(new(x1, y1), new(x2, y2));
        }

        public static float point_distance(Vector2 p1, Vector2 p2)
        {
            return (p2 - p1).magnitude;
        }

        public static float point_direction(float x1, float y1, float x2, float y2)
        {
            return point_direction(new(x1, y1), new(x2, y2));
        }

        public static float point_direction(Vector2 p1, Vector2 p2)
        {
            var direction = (p2 - p1).normalized;
            direction.y *= -1;
            var angle = Vector2.SignedAngle(new(1, 0), direction);
            if (angle < 0)
                angle += 360;
            return angle;
        }

        public static float abs(float val)
        {
            return Mathf.Abs(val);
        }

        public static float angle_difference(float dest, float src)
        {
            var diff = dest - src;
            if (abs(diff) < 180)
                return diff;
            return diff + 360 * -sign(diff);
        }

        public static Vector2 lengthdir(float len, float dir)
        {
            return new(lengthdir_x(len, dir), lengthdir_y(len, dir));
        }

        public static float lengthdir_x(float len, float dir)
        {
            return len * dcos(dir);
        }

        public static float lengthdir_y(float len, float dir)
        {
            return len * -dsin(dir);
        }

        public static float dcos(float val)
        {
            return Mathf.Cos(val * Mathf.Deg2Rad);
        }

        public static float dsin(float val)
        {
            return Mathf.Sin(val * Mathf.Deg2Rad);
        }

        public static T choose<T>(params T[] vals)
        {
            return vals[Random.Range(0, vals.Length)];
        }

        public static float sign(float n)
        {
            return Mathf.Sign(n);
        }

        public static void show_debug_message(object value)
        {
            Debug.Log(value);
        }

        public static void show_debug_message(string value, params object[] args)
        {
            Debug.Log(string.Format(value, args));
        }

        public static float lerp(float a, float b, float amt)
        {
            return Mathf.LerpUnclamped(a, b, amt);
        }
    }
}