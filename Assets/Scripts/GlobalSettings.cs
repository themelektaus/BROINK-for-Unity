using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class GlobalSettings : ScriptableObject
    {
        public static GlobalSettings active;

        [Header("General")]
        public int requiredWins = 3;

        [Header("Ball")]
        public float ballRadius = .4f;
        public float ballAcceleration = .2f;

        [Header("Playing Field")]
        public float playingFieldSize = 10;
        public float playingFieldLifetime = 28;

        [Header("Start Wall")]
        public float barrierWidth = .1f;
        public float barrierLifetime = 1.8f;
    }
}