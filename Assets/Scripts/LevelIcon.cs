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
            var color = _renderer.color;
            color.g = level > Game.MAX_LEVEL ? 0 : 1;
            color.b = level > Game.MAX_LEVEL ? 0 : 1;
            _renderer.color = color;

            _renderer.sprite = (game.mastered && level <= Game.MAX_LEVEL) || game.level > level ? full : empty;
        }
    }
}