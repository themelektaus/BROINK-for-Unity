using UnityEngine;
using UnityEngine.Audio;

namespace BROINK
{
    [CreateAssetMenu]
    public class SoundEffect : ScriptableObject
    {
        [SerializeField] AudioMixerGroup group;
        [SerializeField] Vector2 volume = new(.8f, .8f);
        [SerializeField] Vector2 pitch = new(1, 1);
        [SerializeField] AudioClip[] clips;

        public void Play(Vector2? position = null)
        {
            Play(Utils.Choose(clips), position);
        }

        public void Play(float clipIndex, Vector2? position = null)
        {
            var index = (int) ((clips.Length - 1) * Mathf.Clamp01(clipIndex));
            Play(clips[index], position);
        }

        void Play(AudioClip clip, Vector2? position)
        {
            var gameObject = new GameObject(clip.name);
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = group;
            audioSource.volume = Random.Range(volume.x, volume.y);
            audioSource.pitch = Random.Range(pitch.x, pitch.y);
            audioSource.clip = clip;
            if (position.HasValue)
            {
                audioSource.volume *= .8f;
                audioSource.spatialBlend = 1;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.minDistance = 50;
                audioSource.maxDistance = 100;
                audioSource.transform.position = position.Value;
            }
            audioSource.Play();
            Destroy(gameObject, 10);
        }
    }
}