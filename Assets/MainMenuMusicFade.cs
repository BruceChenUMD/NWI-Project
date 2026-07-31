using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusicFade : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float targetVolume = 0.5f;

    [SerializeField]
    private float startDelay = 0.1f;

    [SerializeField]
    private float fadeDuration = 1f;

    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();

        // Prevent Play On Awake from producing a pop.
        musicSource.Stop();
        musicSource.playOnAwake = false;
        musicSource.volume = 0f;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
    }

    private IEnumerator Start()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                startDelay
            );
        }

        musicSource.Play();

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            musicSource.volume = Mathf.SmoothStep(
                0f,
                targetVolume,
                progress
            );

            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}