using UnityEngine;

namespace BROINK
{
    public class PlayingField : MonoBehaviour
    {
        public Barrier barrier;

        [SerializeField] float size = 10;
        [SerializeField] float lifetime = 120;

        float _lifetime;

        public float lifetimeFactor => _lifetime / lifetime;
        public float radius => size / 2 * GetScale();

        void OnEnable()
        {
            barrier.enabled = true;
            _lifetime = lifetime;
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
                _lifetime = Mathf.Max(0, _lifetime - Time.deltaTime);

            transform.localScale = Vector3.one * GetScale();
        }

        float GetScale()
        {
            return lifetimeFactor * .8f + .2f;
        }
    }
}