using UnityEngine;

namespace BROINK
{
    public class BallMessage : MonoBehaviour
    {
        [SerializeField] SpriteRenderer shadowRenderer;
        [SerializeField] SpriteRenderer backgroundRenderer;
        [SerializeField] SpriteRenderer iconRenderer;

        public Sprite icon;
        public bool visible;

        (float current, float target, float velocity) alpha = (0, 0, 0);

        void Update()
        {
            if (visible)
                iconRenderer.sprite = icon;

            alpha.target = visible ? 1 : 0;
            alpha.current = Mathf.SmoothDamp(alpha.current, alpha.target, ref alpha.velocity, .05f);

            SetAlpha(shadowRenderer, alpha.current);
            SetAlpha(backgroundRenderer, alpha.current);
            SetAlpha(iconRenderer, alpha.current);

            transform.localPosition = new(0, alpha.current - 1, 0);
        }

        void SetAlpha(SpriteRenderer renderer, float alpha)
        {
            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}