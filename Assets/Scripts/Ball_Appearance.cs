using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(AudioSource))]
    public class Ball_Appearance : MonoBehaviour
    {
        [SerializeField] AudioSource rollAudioSource;
        [SerializeField] GameObject winText;
        [SerializeField] SoundEffect winSoundEffect;

        public BallColor color;

        public Ball ball { get; set; }

        void Reset()
        {
            rollAudioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (ball.hasDropped)
            {
                rollAudioSource.volume = 0;
                return;
            }

            var _velocity = ball.velocity.magnitude;
            rollAudioSource.pitch = _velocity * 2 + .8f;
            rollAudioSource.volume = _velocity * 10 - .3f;
        }

        public GameObject ShowWinText()
        {
            winSoundEffect.Play();
            winText.SetActive(true);
            winText.transform.SetParent(null);
            winText.transform.position = new();
            return winText;
        }
    }
}