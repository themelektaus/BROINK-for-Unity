using UnityEngine;

namespace BROINK
{
    public class PlayingField : MonoBehaviour
    {
        public Barrier barrier;

        [SerializeField] float size = 10;
        [SerializeField] float maxLifetime = 28;

        float currentLifetime;

        public float radius => size / 2 * GetScale();

        void OnEnable()
        {
            barrier.enabled = true;
            currentLifetime = maxLifetime;
            ResetTransform();
        }

        public void ResetTransform()
        {
            transform.localScale = Vector3.one;
            barrier.ResetTransform();
        }

        void Update()
        {
            if (!barrier.enabled)
                currentLifetime = Mathf.Max(0, currentLifetime - Time.deltaTime);

            transform.localScale = Vector3.one * GetScale();
        }

        float GetScale()
        {
            return currentLifetime / maxLifetime * .8f + .2f;
        }
    }
}