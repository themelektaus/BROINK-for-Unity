using UnityEngine;
using UnityEngine.Events;

namespace BROINK
{
    public class Button : MonoBehaviour
    {
        [SerializeField] SoundEffect soundEffect;
        [SerializeField] UnityEvent onClick;

        bool hover;
        float scale = 1;

        void OnMouseEnter() => hover = true;
        void OnMouseExit() => hover = false;

        void OnMouseDown()
        {
            soundEffect.Play();
            onClick.Invoke();
        }

        void OnDisable()
        {
            scale = 1;
            hover = false;
        }

        void Update()
        {
            scale = Mathf.Lerp(scale, 1 + (hover ? 1 : 0) * .1f, Time.deltaTime * 40);
            transform.localScale = Vector3.one * scale;
        }
    }
}