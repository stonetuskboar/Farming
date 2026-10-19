using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioDataList", menuName = "Farm/AudioDataList")]
public class AudioData : ScriptableObject
{
    public List<AudioEventData> audioEvents = new List<AudioEventData>();
    public AK.Wwise.RTPC MusicVolume;

    public AudioEventData GetAudioEventDataByName(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return null;
        }
        return audioEvents.Find(x => x.Name == name);
    }
}

[Serializable]
public class AudioEventData
{
    public string Name;
    public AK.Wwise.Event audioEvent;
}