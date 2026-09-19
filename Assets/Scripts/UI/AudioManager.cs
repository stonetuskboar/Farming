using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource SfxSource;
    public AudioSourceController mainBgm;
    public List<Audios> audioList = new();
    public List<BgmData> bgmDataList = new();
    public static AudioManager instance = null;
    private List<BgmData> PlayedBgm = new();
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        mainBgm.Init(this);
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("BgmVolume") == true)
        {
            float bgmVolume = PlayerPrefs.GetFloat("BgmVolume");
            mainBgm.SetVolume(bgmVolume);
        }
        if (PlayerPrefs.HasKey("SFXVolume") == true)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            SetSfxVolume(sfxVolume);
        }
        //mainBgm.PlayMusic(GetRandomBgmData());
    }

    //为了避免bgm过于重复，记录已经播放过的bgm，直到播放过的bgm数量达到bgm总数的一半时才会开始丢弃最早播放过的bgm
    public BgmData GetRandomBgmData()
    {
        if(PlayedBgm.Count >= (bgmDataList.Count /2))
        {
            PlayedBgm.RemoveAt(0);
        }
        BgmData data = bgmDataList[UnityEngine.Random.Range(0, bgmDataList.Count)];
        while(PlayedBgm.Contains(data))
        {
            data = bgmDataList[UnityEngine.Random.Range(0, bgmDataList.Count)];
        }
        PlayedBgm.Add(data);
        return data;
    }
    public void PlayBGM(string name)
    {
        BgmData data = FindBgmData(name);
        mainBgm.PlayMusic(data);
    }

    public void SetSfxVolume(float value)
    {
        SfxSource.volume = value;
    }

    public BgmData FindBgmData(string name)
    {
        for (int i = 0; i < bgmDataList.Count; i++)
        {
            if (bgmDataList[i].name == name)
            {
                return bgmDataList[i];
            }
        }
        Debug.Log("没找到音乐"+ name);
        return bgmDataList[0];
    }

    public void playSfxSound(string name)
    {
        if(name == null || name == "")
        {
            return;
        }
        for (int i = 0; i < audioList.Count; i++)
        {
            if(audioList[i].name == name)
            {
                SfxSource.PlayOneShot(audioList[i].clip);
                return;
            }
        }
    }
    public float GetSfxVolume()
    {
        return SfxSource.volume;
    }
}

[Serializable]
public class Audios
{
    public string name;
    public AudioClip clip;
}

[Serializable]
public class BgmData
{
    public string name;
    public AudioClip clip;
    public double loopStartTime;
    public double loopEndTime;
}