using System.Collections.Generic;
using System.Linq;

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

            var transferImpact = true;

            var nextBalls = balls.ToList();
            foreach (var ball in balls)
            {
                nextBalls.RemoveAt(0);
                foreach (var other in nextBalls)
                {
                    if (ball.CollidesWith(other))
                    {
                        ball.UpdateCollision(other, transferImpact);
                        transferImpact = false;
                    }
                }
            }

            foreach (var ball in balls)
                ball.UpdateTransformPosition();
        }
    }
}