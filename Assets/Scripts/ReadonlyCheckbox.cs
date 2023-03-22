using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class ReadonlyCheckbox : MonoBehaviour
    {
        [SerializeField] Sprite uncheckedSprite;
        [SerializeField] Sprite checkedSprite;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public abstract bool IsChecked();

        void Update()
        {
            if (IsChecked())
            {
                _renderer.sprite = checkedSprite;
                return;
            }

            _renderer.sprite = uncheckedSprite;
        }
    }
}