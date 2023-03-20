using System.Collections.Generic;

using UnityEngine;

namespace BROINK
{
    public static class PhysicsSystem
    {
        public static readonly List<Ball> balls = new();

        static (float current, float target, float velocity, float smoothTime) timeScale;

        static PhysicsSystem()
        {
            ResetTimeScale();
        }

        public static void ResetTimeScale()
        {
            timeScale = (1, 1, 0, .1f);
        }

        public static void StartSlowMotion()
        {
            timeScale.target = .1f;
        }

        public static void ExitSlowMotion()
        {
            timeScale.smoothTime = 1;
            timeScale.target = 1;
        }

        public static void NormalUpdate()
        {
            timeScale.current = Mathf.SmoothDamp(
                timeScale.current,
                timeScale.target,
                ref timeScale.velocity,
                timeScale.smoothTime
            );

            foreach (var ball in balls)
                ball.NormalUpdate(timeScale.current);
        }

        public static void PhysicsUpdate(Barrier barrier)
        {
            foreach (var ball in balls)
                ball.PhysicsUpdate(timeScale.current);

            if (barrier.enabled)
                foreach (var ball in balls)
                    ball.UpdateCollision(barrier);

            if (balls.Count == 2)
                if (balls[0].CollidesWith(balls[1]))
                    balls[0].UpdateCollision(balls[1]);

            foreach (var ball in balls)
                ball.UpdateTransformPosition();
        }
    }
}