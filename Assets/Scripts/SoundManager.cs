
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
    public void PlayMusic(string soundName)
    {
        Sound soundToPlay = FindMusicFile(soundName);
        if (soundToPlay == null) return;

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(
            ChangeAudio(musicSource, soundToPlay)
        );
    }

    [YarnCommand("play_sfx")]
    public void PlaySFX(string soundName)
    {
        Sound soundToPlay = FindSFXFile(soundName);
        if (soundToPlay == null) return;

        if (sfxFadeCoroutine != null)
            StopCoroutine(sfxFadeCoroutine);

        sfxFadeCoroutine = StartCoroutine(
            ChangeAudio(sfxSource, soundToPlay)
        );
    }

    [YarnCommand("play_sfx_one_shot")]
    public void PlaySFXOneShot(string soundName)
    {
        Sound soundToPlay = FindSFXFile(soundName);
        if (soundToPlay == null) return;

        sfxSource.PlayOneShot(
            soundToPlay.audioFile,
            soundToPlay.volume
        );
    }

    Sound FindMusicFile(string soundName)
    {
        return musicFiles.Find(sound =>
            sound.soundName == soundName);
    }

    Sound FindSFXFile(string soundName)
    {
        return sfxFiles.Find(sound =>
            sound.soundName == soundName);
    }

    private IEnumerator ChangeAudio(
        AudioSource audioSource, Sound sound)
    {
        // Fade out previous audio
        if (audioSource.isPlaying)
            yield return FadeVolume(
                audioSource, 0f, fadeOutDuration);

        audioSource.Stop();

        bool hasSeparateLoop =
            sound.loop && sound.audioLoopFile != null;

        // Play the initial clip
        audioSource.clip = sound.audioFile;
        audioSource.loop = sound.loop && !hasSeparateLoop;
        audioSource.volume = 0f;
        audioSource.Play();

        float elapsed = 0f;
        bool waitingForLoop = hasSeparateLoop;

        // Fade in, and wait for the intro to finish
        // if a separate looping clip is assigned.
        while (elapsed < fadeInDuration || waitingForLoop)
        {
            // Once the intro ends, begin the loop
            if (waitingForLoop && !audioSource.isPlaying)
            {
                audioSource.clip = sound.audioLoopFile;
                audioSource.loop = true;
                audioSource.Play();

                waitingForLoop = false;
            }

            elapsed += Time.unscaledDeltaTime;

            float t = fadeInDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / fadeInDuration);

            audioSource.volume = Mathf.Lerp(
                0f, sound.volume, t);

            yield return null;
        }

        audioSource.volume = sound.volume;
    }

    [YarnCommand("stop_music")]
    public void StopMusic()
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(
            FadeOutAndStop(musicSource)
        );
    }

    [YarnCommand("stop_sfx")]
    public void StopSFX()
    {
        if (sfxFadeCoroutine != null)
            StopCoroutine(sfxFadeCoroutine);

        sfxFadeCoroutine = StartCoroutine(
            FadeOutAndStop(sfxSource)
        );
    }

    private IEnumerator FadeOutAndStop(
        AudioSource audioSource)
    {
        yield return FadeVolume(
            audioSource, 0f, fadeOutDuration);

        audioSource.Stop();
    }

    private IEnumerator FadeVolume(
        AudioSource audioSource,
        float targetVolume,
        float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / fadeDuration);

            audioSource.volume = Mathf.Lerp(
                startVolume, targetVolume, t);

            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
