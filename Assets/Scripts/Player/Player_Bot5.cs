using UnityEngine;

namespace BROINK
{
    public class Player_Bot5 : Player_Bot
    {
        [SerializeField, Range(-100, 100)]
        float minPositionScore = 0;

        void Reset()
        {
            outwardsFactor = 30;
        }

        public override void Process(ref Vector2 output)
        {
            if (playingField.barrier.enabled)
            {
                if (ModeOpening(ref output))
                    return;

                ModeOpeningDodge(ref output);
                OutOfBoundsEmergencyBreak(ref output);
                return;
            }
            
            var score = CalculatePositionScore();
            if (score > minPositionScore)
                ModeOffensive(ref output);
            else
                ModeDefensive(ref output, advanced: true);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}