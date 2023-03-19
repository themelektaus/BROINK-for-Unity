using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WinIcon : MonoBehaviour
    {
        [SerializeField] Sprite empty;
        [SerializeField] Sprite full;

        [SerializeField] Game game;
        [SerializeField] int wins;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _renderer.sprite = game.wins >= wins ? full : empty;
        }
    }
}