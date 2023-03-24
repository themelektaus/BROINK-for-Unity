using UnityEngine;

namespace BROINK
{
    public class PlayingField : MonoBehaviour
    {
        static float size => GlobalSettings.active.playingFieldSize;
        static float maxLifetime => GlobalSettings.active.playingFieldLifetime;

        public Barrier barrier;

        float currentLifetime;

        public float radius => size / 2 * GetScale();

        void OnEnable()
        {
            currentLifetime = maxLifetime;
        }

        public void ResetTransform()
        {
            transform.localScale = GetBaseScale();
            barrier.ResetTransform();
        }

        void Update()
        {
            if (!barrier.enabled)
                currentLifetime = Mathf.Max(0, currentLifetime - Time.deltaTime);

            transform.localScale = GetBaseScale() * GetScale();
        }

        Vector3 GetBaseScale()
        {
            return Vector3.one * (size / 10);
        }

        float GetScale()
        {
            return currentLifetime / maxLifetime * .95f + .05f;
        }
    }
}