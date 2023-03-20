using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Barrier : MonoBehaviour
    {
        public float width = .1f;
        public float maxLifetime = 2;

        [SerializeField] SoundEffect fadeOutSoundEffect;

        public float currentLifetime { get; private set; }

        SpriteRenderer _renderer;

        Coroutine fadeOutCoroutine;
        Coroutine fizzleCoroutine;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            currentLifetime = maxLifetime;
            ResetTransform();
        }

        void OnDisable()
        {
            StopCoroutine(ref fadeOutCoroutine);
            StopCoroutine(ref fizzleCoroutine);
            ResetTransform();
        }

        public void ResetTransform()
        {
            transform.localScale = Vector3.one;
            _renderer.color = Color.white;
        }

        void Update()
        {
            if (currentLifetime == 0)
                return;

            currentLifetime = Mathf.Max(0, currentLifetime - Time.deltaTime);
            
            if (currentLifetime != 0)
                return;

            StopCoroutine(ref fizzleCoroutine);
            StartCoroutine(ref fadeOutCoroutine, FadeOutRoutine());
        }

        public void Fizzle()
        {
            StopCoroutine(ref fizzleCoroutine);

            if (currentLifetime == 0)
                return;

            StartCoroutine(ref fizzleCoroutine, FizzleRoutine());
        }

        void StartCoroutine(ref Coroutine coroutine, IEnumerator routine)
        {
            StopCoroutine(ref coroutine);
            coroutine = StartCoroutine(routine);
        }

        void StopCoroutine(ref Coroutine coroutine)
        {
            if (coroutine is null)
                return;

            StopCoroutine(coroutine);
            coroutine = null;
        }

        IEnumerator FadeOutRoutine()
        {
            fadeOutSoundEffect.Play();

            enabled = false;
            while (_renderer.color.a > 0)
            {
                _renderer.color = _renderer.color.WithAlpha(_renderer.color.a - Time.deltaTime * 5);
                transform.localScale = new(_renderer.color.a, 1, 1);
                yield return null;
            }
            fadeOutCoroutine = null;
        }

        IEnumerator FizzleRoutine()
        {
            for (int i = 0; i < 10; i++)
            {
                _renderer.color = _renderer.color.WithAlpha(Random.value * .5f + .5f);
                yield return new WaitForSeconds(.05f);
            }
            fizzleCoroutine = null;
        }
    }
}