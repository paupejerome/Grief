using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
    DEGAT1 = 0,
    DEGAT2 = 1,
    DEGAT3 = 2,
    ECRASE1 = 3,
    ECRASE2 = 4,
    MORT1 = 5,
    MORT2 = 6,
    MORT3 = 7,
    MORT4 = 8,
    MORT5 = 9,
    SAUT1 = 10,
    SAUT2 = 11,
    COIN1 = 12,
    COIN2 = 13,
    FEU1 = 14,
    FEU2 = 15,
    FEU3 = 16,
    GRAVITY1 = 17,
    GRAVITY2 = 18
}

public class MinigameSoundManager : MonoBehaviour
{
    static MinigameSoundManager instance;

    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioClip[] sfxClips;

    AudioSource audioSource;
    float baseVolume;

    public static MinigameSoundManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;
    }

    public void StartMusic(int day)
    {
        switch (MinigameManager.GetInstance().winCondition)
        {
            case WinCondition.TRAVEL_DISTANCE:
                audioSource.clip = audioClips[0];
                audioSource.Play(); // play pokemon music
                break;

            case WinCondition.COLLECT_GOLD:
                audioSource.PlayOneShot(audioClips[1]); // play lead
                StartCoroutine(WaitFinishAndPlay(audioClips[2])); // play the actual music
                audioSource.loop = true;
                break;

            case WinCondition.SURVIVE:
                StartCoroutine(WaitAndPlayMusic(2f, audioClips[3])); // Cool music
                audioSource.loop = true;
                break;
        }
    }

    public void StopSongSmoothly(float duration = 1f, float smoothness = 30f)
    {
        StartCoroutine(StopSongSmoothlyCoroutine(duration, smoothness));
    }

    IEnumerator StopSongSmoothlyCoroutine(float duration, float smoothness)
    {
        float coef = duration / smoothness;

        for (int i = 0; i < smoothness; i++)
        {
            audioSource.volume -= coef / 3f;
            yield return new WaitForSeconds(coef);
        }

        audioSource.Stop();
        audioSource.volume = baseVolume;
    }

    IEnumerator WaitFinishAndPlay(AudioClip audioClip)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        audioSource.clip = audioClip;
        audioSource.Play();

        yield break;
    }

    IEnumerator WaitAndPlayMusic(float time, AudioClip clip)
    {
        yield return new WaitForSeconds(time);
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlaySFX(int audioClip)
    {
        playerSource.PlayOneShot(sfxClips[audioClip]);
    }

    public void PlaySFX(SFX sfx)
    {
        int clip = (int)sfx;

        playerSource.PlayOneShot(sfxClips[clip]);
    }
}
