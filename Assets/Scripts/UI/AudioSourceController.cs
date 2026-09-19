using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    public List<AudioSource> sourceList;
    private AudioManager manager;
    private AudioSource nowSource = null;
    public Action LoopEnd;
    private int BgmTweenId;
    private int loopStartSamples;
    private int loopEndSamples = int.MaxValue;
    private int loopLengthSamples;

    public void Init(AudioManager auidoManager)
    {
        this.manager = auidoManager;
    }
    public void Awake()
    {
        nowSource = sourceList[0];
    }
    private void Update()
    {
        if (nowSource.timeSamples >= loopEndSamples)
        {
            AudioSource lastSource = nowSource;
            SwitchSource();
            nowSource.volume = lastSource.volume;
            PlayMusic( manager.GetRandomBgmData());
            LoopEnd?.Invoke();
        }
    }

    public AudioSource GetNowSource()
    {
        return nowSource; 
    }

    public AudioSource SwitchSource()
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            if (sourceList[i] != nowSource)
            {
                nowSource = sourceList[i];
                break;
            }
        }
        return nowSource;
    }

    public AudioSourceController SetVolume(float value)
    {
        BgmTweenId++;
        nowSource.volume = value;
        return this;
    }

    public float GetVolume()
    {
        return nowSource.volume;
    }
    public void StopMusic()
    {
        for (int i = 0; i < sourceList.Count; i++)
        {
            sourceList[i].Stop();
        }
    }
    public void DoVolumeTween(float targetVolume, float duration, Action callback = null)
    {
        StartCoroutine(BgmVolumeTween(targetVolume, duration,callback));
    }
    IEnumerator BgmVolumeTween(float targetVolume, float duration , Action callback)
    {
        BgmTweenId++;
        int id = BgmTweenId;

        float startVolume = nowSource.volume;
        float delta = targetVolume - startVolume;
        float time = Time.deltaTime;

        while (time < duration && BgmTweenId == id)
        {
            nowSource.volume = startVolume + (time / duration) * delta;
            yield return null;
            time += Time.deltaTime;
        }
        if (BgmTweenId == id)
        {
            callback?.Invoke();
            nowSource.volume = targetVolume;
        }
    }
    public void SetbgmLoopSamples(BgmData bgm)
    {
        loopStartSamples = (int)(bgm.loopStartTime * nowSource.clip.frequency);
        loopEndSamples = (int)(bgm.loopEndTime * nowSource.clip.frequency);
        loopLengthSamples = loopEndSamples - loopStartSamples;
    }

    public AudioSourceController PlayMusic(BgmData data)
    {
        GetNowSource().clip = data.clip;
        SetbgmLoopSamples(data);
        nowSource.timeSamples = loopStartSamples;
        nowSource.Play();
        return this;
    }
    public AudioSourceController PlayMusic(string name)
    {
        BgmData data = manager.FindBgmData(name);
        GetNowSource().clip = data.clip;
        SetbgmLoopSamples(data);
        nowSource.timeSamples = loopStartSamples;
        nowSource.Play();
        return this;
    }
    public void SetRandomTimeSample()
    {
        nowSource.timeSamples = UnityEngine.Random.Range(loopStartSamples, loopEndSamples);
    }

}
