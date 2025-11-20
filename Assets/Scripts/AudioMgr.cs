using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  引用SFX
        AudioMgr.instance.PlaySFX("");
    引用BGM
        AudioMgr.instance.PlayBGM("",3f);
    SFX來源
        Resources/Audios/SFX
    BGM來源
        Resources/Audios/BGM
 */

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr instance { get; private set; }

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private AudioClip[] bgmClips;
    private AudioClip[] sfxClips;
    
    private float bgmVolume = .8f;
    private float sfxVolume = .8f;

    [Header("UI Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        instance = this;
        
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;

        bgmClips = Resources.LoadAll<AudioClip>("Audios/BGM");
        sfxClips = Resources.LoadAll<AudioClip>("Audios/SFX");

        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVolume;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }
    

    #region Play BGM（支援淡入）
    public void PlayBGM(string clipName, float fadeTime = 1f)
    {
        AudioClip clip = System.Array.Find(bgmClips, c => c.name == clipName);
        if (clip == null) return;
        
        StopAllCoroutines();
        StartCoroutine(FadeInBGM(clip, fadeTime));
    }

    // 停止 BGM（淡出）
    public void StopBGM(float fadeTime = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutBGM(fadeTime));
    }
    
    private IEnumerator FadeInBGM(AudioClip newClip, float duration)
    {
        if (bgmSource.isPlaying)
            yield return FadeOutBGM(duration);

        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / duration);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = bgmVolume; // 重置音量
    }
    #endregion

    #region Play SFX
    public void PlaySFX(string clipName)
    {
        AudioClip clip = System.Array.Find(sfxClips, c => c.name == clipName);
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    #endregion

    #region 調整音量
    // 調整 BGM 音量
    private void SetBGMVolume(float value)
    {
        bgmVolume = value;
        bgmSource.volume = bgmVolume;
    }

    // 調整 SFX 音量
    private void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = sfxVolume;
    }
    #endregion
}
