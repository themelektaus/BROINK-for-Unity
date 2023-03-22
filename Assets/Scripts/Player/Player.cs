using System.Collections.Generic;
using UnityEngine;

namespace BROINK
{
    public abstract class Player : MonoBehaviour
    {
        public Ball ball { get; set; }
        public PlayingField playingField { get; set; }
        
        protected List<Player> players { get; private set; }

        void OnDisable()
        {
            ball.icon = null;
        }

        public Player AddTo(List<Player> players)
        {
            this.players = players;
            this.players.Add(this);
            return this;
        }
    }
}