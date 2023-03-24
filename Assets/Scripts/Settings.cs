using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class Settings : ScriptableObject
    {
        static Settings _active;
        public static Settings active
        {
            get
            {
                if (!_active)
                    _active = CreateInstance<Settings>();
                return _active;
            }
        }

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