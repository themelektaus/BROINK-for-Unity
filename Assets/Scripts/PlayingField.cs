using UnityEngine;

namespace BROINK
{
    public class PlayingField : MonoBehaviour
    {
        static float size => Settings.active.playingFieldSize;
        static float maxLifetime => Settings.active.playingFieldLifetime;

        public Barrier barrier;

        float currentLifetime;

        public float radius => size / 2 * GetScale();

        void OnEnable()
        {
            currentLifetime = maxLifetime;
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
            return currentLifetime / maxLifetime * .95f + .05f;
        }
    }
}