using UnityEngine;

namespace BROINK
{
    public class Ball_Icons : MonoBehaviour
    {
        public Sprite guard;
        public Sprite attack;
        public Sprite dodge;
        public Sprite warning;

        public Player_Bot bot { get; set; }

        void Update()
        {
            bot.ball.icon = bot.mode switch
            {
                Player_Bot.Mode.CampCenter => guard,
                Player_Bot.Mode.Offensive => attack,
                Player_Bot.Mode.Defensive => dodge,
                Player_Bot.Mode.Emergency => warning,
                _ => null
            };
        }
    }
}