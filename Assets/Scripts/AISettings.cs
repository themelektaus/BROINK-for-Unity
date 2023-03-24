using UnityEngine;

namespace BROINK
{
    [CreateAssetMenu]
    public class AISettings : ScriptableObject
    {
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

        [Header("Randomness")]
        public bool randomOpening = true;
        [Range(0, 90)] public float randomRotationRangeAgainstAI = 20;
    }
}