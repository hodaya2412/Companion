using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float targetVolume = 0.4f;

    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float initialTimerValue = 0f;

    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeSource = musicSourceA;
        inactiveSource = musicSourceB;

        activeSource.volume = targetVolume;
        inactiveSource.volume = minVolume;
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null) return;
        if (activeSource.clip == newClip) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Crossfade(newClip));
    }

    private IEnumerator Crossfade(AudioClip newClip)
    {
        inactiveSource.clip = newClip;
        inactiveSource.volume = minVolume;
        inactiveSource.loop = true;
        inactiveSource.Play();

        float timer = initialTimerValue;
        float startVolume = activeSource.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            activeSource.volume = Mathf.Lerp(startVolume, minVolume, t);
            inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = minVolume;

        inactiveSource.volume = targetVolume;

        AudioSource temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
    }
}