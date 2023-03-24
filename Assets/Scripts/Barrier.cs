using System.Collections;

using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Barrier : MonoBehaviour
    {
        [SerializeField] SoundEffect ballBounceSoundEffect;
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
            currentLifetime = GlobalSettings.active.barrierLifetime;
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

        public void BallBounce()
        {
            StopCoroutine(ref fizzleCoroutine);

            if (currentLifetime == 0)
                return;

            ballBounceSoundEffect.Play();
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
                var color = _renderer.color;
                color.a = _renderer.color.a - Time.deltaTime * 5;
                _renderer.color = color;
                transform.localScale = new(_renderer.color.a, 1, 1);
                yield return null;
            }
            fadeOutCoroutine = null;
        }

        IEnumerator FizzleRoutine()
        {
            for (int i = 0; i < 10; i++)
            {
                var color = _renderer.color;
                color.a = Random.value * .5f + .5f;
                _renderer.color = color;
                yield return new WaitForSeconds(.05f);
            }
            fizzleCoroutine = null;
        }
    }
}