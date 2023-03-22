using UnityEngine;

namespace BROINK
{
    public class Player_Bot1 : Player_Bot
    {
        [SerializeField, Range(0, 1)]
        float speedDamping = .2f;

        public override void Process(ref Vector2 output)
        {
            mode = Mode.Offensive;
            output = -playerSelf_pos / 10;
            output += -playerSelf_speed * speedDamping;
        }
    }
}