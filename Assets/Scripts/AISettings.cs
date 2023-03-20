using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class AISettings : ScriptableObject
    {
        public static AISettings active;

        [Header("Opening Mode")]
        [Range(0, 2)] public float openingTotalDuration = .75f;
        [Range(0, 1)] public float openingBackstepDuration = .4f;
        public Vector2 openingY = new(-1, 1);

        [Header("Out of Bounce Emergency Break")]
        [Range(0, 2)] public float breakAccelerationFactor = 1.15f;

        [Header("Bot 1")]
        [Range(0, 1)] public float bot1SpeedDamping = .2f;

        [Header("Bot 2")]
        public float bot2SpeedOffset = 10;
        [Range(0, 100)] public float bot2OutwardsFactor = 0;

        [Header("Bot 3")]
        public float bot3SpeedOffset = 10;
        [Range(0, 100)] public float bot3OutwardsFactor = 0;
        [Range(0, 500)] public float bot3ModeSwitchGameRadiusThreshold = 300;

        [Header("Bot 4")]
        public float bot4SpeedOffset = 6;
        [Range(0, 100)] public float bot4OutwardsFactor = 50;
        [Range(-100, 100)] public float bot4MinPositionScore = 75;

        [Header("Bot 5")]
        public float bot5SpeedOffset = 10;
        [Range(0, 100)] public float bot5OutwardsFactor = 30;
        [Range(-100, 100)] public float bot5MinPositionScore = 0;

        [Header("Bot 6")]
        public float bot6SpeedOffset = 10;
        [Range(0, 100)] public float bot6OutwardsFactor = 0;
        [Range(0, 500)] public float bot6ModeSwitchGameRadiusThreshold = 200;

        [Header("Bot 7")]
        public float bot7SpeedOffset = 1;
        [Range(0, 100)] public float bot7OutwardsFactor = 30;
        [Range(-100, 100)] public float bot7MinPositionScore = -10;
    }
}