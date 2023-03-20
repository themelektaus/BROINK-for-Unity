using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LevelIcon : MonoBehaviour
    {
        [SerializeField] Sprite empty;
        [SerializeField] Sprite full;

        [SerializeField] Game game;
        [SerializeField] int level;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _renderer.sprite = game.mastered || game.level > level ? full : empty;
        }
    }
}