using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WinIcon : MonoBehaviour
    {
        [SerializeField] Sprite empty;
        [SerializeField] Sprite full;

        [SerializeField] int wins;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (GameSettings.active.wins >= wins)
            {
                _renderer.sprite = full;
                return;
            }

            _renderer.sprite = empty;
        }
    }
}