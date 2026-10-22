
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] List<Sound> musicFiles;
    [SerializeField] List<Sound> sfxFiles;

    [Header("Attributes")]
    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float fadeOutDuration = 1f;


    private Coroutine musicFadeCoroutine;
    private Coroutine sfxFadeCoroutine;

    [YarnCommand("play_music")]
    public void PlayMusic(string soundName)//, bool loop = false, float volume = 1)
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        Sound soundToPlay = FindMusicFile(soundName);
        if (soundToPlay == null) return;
        musicFadeCoroutine = StartCoroutine(
            ChangeAudio(musicSource, soundToPlay.audioFile, soundToPlay.loop, soundToPlay.volume)
        );
    }

    [YarnCommand("play_sfx")]
    public void PlaySFX(string soundName)//, bool loop = false, float volume = 1)
    {
        if (sfxFadeCoroutine != null)
            StopCoroutine(sfxFadeCoroutine);

        Sound soundToPlay = FindSFXFile(soundName);
        if (soundToPlay == null) return;
        sfxFadeCoroutine = StartCoroutine(
            ChangeAudio(sfxSource, soundToPlay.audioFile, soundToPlay.loop, soundToPlay.volume)
        );
    }

    [YarnCommand("play_sfx_one_shot")]
    public void PlaySFXOneShot(string soundName)//, bool loop = false, float volume = 1)
    {
        Sound soundToPlay = FindSFXFile(soundName);
        if (soundToPlay == null) return;
        sfxSource.PlayOneShot(soundToPlay.audioFile, soundToPlay.volume);
    }

    Sound FindMusicFile(string soundName)
    {
        return musicFiles.Find(sound => sound.soundName == soundName);
    }

    Sound FindSFXFile(string soundName)
    {
        return sfxFiles.Find(sound => sound.soundName == soundName);
    }

    private IEnumerator ChangeAudio(AudioSource audioSource, AudioClip newClip, bool loop, float volume)
    {
        // Fade out the previous song
        if (audioSource.isPlaying)
            yield return FadeVolume(audioSource, 0f, fadeOutDuration);

        // Switch songs
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.volume = 0f;
        audioSource.Play();

        // Fade in the new song
        yield return FadeVolume(audioSource, volume, fadeInDuration);

    }

    [YarnCommand("stop_music")]
    public void StopMusic()
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(FadeOutAndStop(musicSource));
    }

    [YarnCommand("stop_sfx")]
    public void StopSFX()
    {
        if (sfxFadeCoroutine != null)
            StopCoroutine(sfxFadeCoroutine);

        sfxFadeCoroutine = StartCoroutine(FadeOutAndStop(sfxSource));
    }

    private IEnumerator FadeOutAndStop(AudioSource audioSource)
    {
        yield return FadeVolume(audioSource, 0f, fadeOutDuration);
        audioSource.Stop();
    }

    private IEnumerator FadeVolume(AudioSource audioSource, float targetVolume, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            audioSource.volume = Mathf.Lerp(
                startVolume, targetVolume, t
            );

            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
