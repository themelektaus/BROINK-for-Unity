using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class AISettings : ScriptableObject
    {
        public static AISettings active;

        [Header("Opening Mode")]
        [Range(0, 2)] public float openingTotalDuration = .55f;
        [Range(0, 1)] public float openingBackstepDuration = .4f;
        public Vector2 openingY = new(-1, 1);
        [Range(0, 10)] public float openingDodgeThreshold = 5;

        [Header("Defense")]
        [Range(0, 20)] public float openingDefenseHardness = 2;
        [Range(0, 20)] public float defenseHardness = 4;

        [Header("Out of Bounce Emergency Break")]
        [Range(0, 2)] public float breakAccelerationFactor = 1.15f;

        [Header("Bot 1")]
        [Range(0, 1)] public float bot1SpeedDamping = .2f;

        [Header("Bot 2")]
        public Player_Bot.Config bot2Config = new();

        [Header("Bot 3")]
        public Player_Bot.Config bot3Config = new();
        [Range(0, 500)] public float bot3ModeSwitchGameRadiusThreshold = 300;

        [Header("Bot 4")]
        public Player_Bot.Config bot4Config = new() { speedOffset = 6, outwardsFactor = 50 };
        [Range(-100, 100)] public float bot4MinPositionScore = 75;

        [Header("Bot 5")]
        public Player_Bot.Config bot5Config = new() { outwardsFactor = 30 };
        [Range(-100, 100)] public float bot5MinPositionScore = 0;

        [Header("Bot 6")]
        public Player_Bot.Config bot6Config = new();
        [Range(0, 500)] public float bot6ModeSwitchGameRadiusThreshold = 200;

        [Header("Bot 7")]
        public Player_Bot.Config bot7Config = new() { speedOffset = 1, outwardsFactor = 30 };
        [Range(-100, 100)] public float bot7MinPositionScore = -10;
    }
}