using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(Ball))]
    public abstract class Player : MonoBehaviour
    {
        public abstract bool isHuman { get; }

        public Ball ball { get; private set; }
        public Ball opponentBall { get; set; }
        public PlayingField playingField { get; set; }

        protected virtual void Awake()
        {
            ball = GetComponent<Ball>();
        }
    }
}