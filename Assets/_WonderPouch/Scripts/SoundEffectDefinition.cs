using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectDefinition", menuName = "Scriptable Objects/SoundEffectDefinition")]
public class SoundEffectDefinition : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private AudioClip[] _clips;
    [SerializeField][Range(0f, 1f)] private float _volume = 1f;
    [SerializeField][MinMaxRange(0.1f, 3f)] private Vector2 pitchRange = new Vector2(1f, 1f);
    [SerializeField] private bool spatialSound = false;

    [System.Serializable]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public MinMaxRangeAttribute(float min, float max) { this.min = min; this.max = max; }
    }

    public void Play()
    {
        if (_clips.IsNullOrEmpty())
        {
            return;
        }

        AudioClip clip = _clips[Random.Range(0, _clips.Length)];
        GameObject tempAudioSource = new GameObject("TempAudio_" + clip.name);

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = _volume;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.spatialBlend = spatialSound ? 1f : 0f;
        audioSource.Play();

        float destroyTime = clip.length;
        Destroy(tempAudioSource, destroyTime);
    }
}
