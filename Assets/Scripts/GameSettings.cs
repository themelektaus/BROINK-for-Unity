using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class GameSettings : ScriptableObject
    {
        public static GameSettings active;

        [Header("Game Progression")]
        [Range(0, 3)] public int wins;
        [Range(1, 7)] public int level = 1;
        public bool mastered;

        [Header("Ball")]
        public float ballRadius = .4f;
        public float ballAcceleration = .2f;

        [Header("Playing Field")]
        public float playingFieldSize = 10;
        public float playingFieldLifetime = 28;

        [Header("Start Wall")]
        public float barrierWidth = .1f;
        public float barrierLifetime = 2;

        public void Win()
        {
            wins = Mathf.Min(3, wins + 1);

            if (wins < 3)
                return;

            if (level == 7)
            {
                mastered = true;
                return;
            }

            wins = 0;
            level++;
        }

        public void Lose()
        {
            wins = Mathf.Max(0, wins - 1);
            mastered = false;
        }
    }
}