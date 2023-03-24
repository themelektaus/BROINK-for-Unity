using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FadeIn : MonoBehaviour
    {
        SpriteRenderer _renderer;
        float alpha;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            alpha = 0;
            UpdateRenderer();
        }

        void Update()
        {
            alpha = Mathf.Min(1, alpha + Time.deltaTime * 10);
            UpdateRenderer();
        }

        void UpdateRenderer()
        {
            transform.localScale = Vector3.one * (alpha * .2f + .8f);
            _renderer.color = new(1, 1, 1, alpha);
        }
    }
}