using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    public static float bgm_volume = 0.2f;
    public static float se_volume = 0.2f;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource onlySource;
    public AudioSource loopSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // すでにインスタンスがある場合は削除
            return;
        }
    }

    public async UniTask StopAllAudioUT()
    {
        float startVolume = bgmSource.volume;
        float startVolume2 =sfxSource.volume;
        float time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / 0.5f);
            sfxSource.volume = Mathf.Lerp(startVolume2, 0f, time / 0.5f);
            await UniTask.Yield(PlayerLoopTiming.Update); // 毎フレーム待機
        }

        bgmSource.volume = 0f;
        sfxSource.volume = 0f;

        bgmSource.Stop();
        sfxSource.Stop(); 
    }

    public void StopALLAudio()
    {
        bgmSource.volume = 0f;
        sfxSource.volume = 0f;

        bgmSource.Stop();
        sfxSource.Stop();
    }

    /// <summary>
    /// 指定したBGMを再生する
    /// </summary>
    public void PlayBGM(int index,float duration = 1)
    {
        Debug.Log("BGM再生");
        if (index < 0 || index >= bgmClips.Length || CardDataController.isCardUses[3]) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// 効果音を再生する
    /// </summary>
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length || CardDataController.isCardUses[3]) return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }


    public void PlayOnlySFX(AudioClip[] clips, int dex = 99)
    {
        if (CardDataController.isCardUses[3]) return;
        int index = dex;
        if (dex == 99)
        {
            index = Random.Range(0, clips.Length);
        }
        onlySource.PlayOneShot(clips[index]);
    }

    public void SetVolume()
    {
        bgmSource.volume = bgm_volume;
        sfxSource.volume = se_volume;
        if(loopSource != null) loopSource.volume = bgm_volume;
    }

    public async UniTask FadeInAsync(float duration, float targetVolume = 1f, CancellationToken token = default)
    {
        float vol = bgmSource.volume;
        bgmSource.volume = 0f;
        if (!bgmSource.isPlaying)
            bgmSource.Play();

        await FadeVolumeAsync(0f, vol, duration, token);
    }

    public async UniTask FadeOutAsync(float duration, CancellationToken token = default)
    {
        float startVolume = bgmSource.volume;
        await FadeVolumeAsync(startVolume, 0f, duration, token);
        bgmSource.Stop();
    }

    public async UniTask FadeVolumeAsync(float from, float to, float duration, CancellationToken token)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (token.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            bgmSource.volume = Mathf.Lerp(from, to, t);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        bgmSource.volume = to;
    }
}
