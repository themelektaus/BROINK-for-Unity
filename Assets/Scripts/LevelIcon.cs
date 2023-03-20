using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LevelIcon : MonoBehaviour
    {
        [SerializeField] Sprite empty;
        [SerializeField] Sprite full;

        [SerializeField] int level;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (GameSettings.active.mastered || GameSettings.active.level > level)
            {
                _renderer.sprite = full;
                return;
            }

            _renderer.sprite = empty;
        }
    }
}