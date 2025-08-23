using UnityEngine;
using UnityEngine.Audio; // For mixers
using System.Collections.Generic; // For pooling

namespace ChainEmpires
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Groups")]
        [SerializeField] private AudioMixer mixer; // For volume/master control
        [SerializeField] private AudioClip[] bgmClips; // Realm-based music (e.g., stone age ambient)
        [SerializeField] private AudioClip[] sfxClips; // SFX pool (e.g., build, attack)

        [Header("Spatial & Adaptive")]
        [SerializeField] private float spatialBlend = 1f; // 3D spatial for battles
        [SerializeField] private float adaptiveFadeTime = 2f; // Fade on events (e.g., combat)

        // 2025 Opts: AI-Generated Sounds (Unity Generators package placeholder)
        [SerializeField] private bool useAIGenSounds = true; // Procedural via AI

        private AudioSource bgmSource;
        private ObjectPool<AudioSource> sfxPool; // Low-GC pooling
        private float currentVolume = 1f;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null) Instance = this; else Destroy(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];

            // Pool: 2025 DSP opt (reduce allocations, low-latency)
            sfxPool = new ObjectPool<AudioSource>(() =>
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
                src.spatialBlend = spatialBlend; // 3D spatial
                return src;
            }, 20); // Max concurrent SFX

            // Mobile Opt: Low-latency DSP (Unity 6.2 best practice)
            AudioSettings.SetDSPBufferSize(256, 2); // Reduce buffer for responsiveness
        }

        public static SoundManager Instance { get; private set; }

        public void PlayBGM(int realmIndex)
        {
            if (useAIGenSounds)
            {
                // 2025 AI Gen: Placeholder for Unity AI Generators (procedural sound)
                Debug.Log("Generating AI soundscape for realm " + realmIndex);
                // In real: Use Generators.GenerateSound("ambient_realm_" + realmIndex);
            }
            else bgmSource.clip = bgmClips[realmIndex % bgmClips.Length];

            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            var src = sfxPool.Get();
            src.transform.position = position; // Spatial positioning
            src.clip = clip;
            src.volume = volume * currentVolume;
            src.Play();
            StartCoroutine(ReleaseAfterPlay(src, clip.length));
        }

        private System.Collections.IEnumerator ReleaseAfterPlay(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay);
            sfxPool.Release(src);
        }

        // Adaptive: Fade on events (e.g., combat tension)
        public void AdaptMusic(float targetVolume)
        {
            StopCoroutine("FadeVolume");
            StartCoroutine(FadeVolume(targetVolume));
        }

        private System.Collections.IEnumerator FadeVolume(float target)
        {
            float start = currentVolume;
            for (float t = 0; t < adaptiveFadeTime; t += Time.deltaTime)
            {
                currentVolume = Mathf.Lerp(start, target, t / adaptiveFadeTime);
                bgmSource.volume = currentVolume;
                yield return null;
            }
            currentVolume = target;
        }

        // Opt: Update all active SFX volumes on master change
        public void SetMasterVolume(float volume) => mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    // Reuse ObjectPool from ARManager.cs (or duplicate if needed)
}