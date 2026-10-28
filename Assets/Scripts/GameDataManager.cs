using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public ItemDataList itemDataList;
    public CropDataList cropDataList;

    public AudioData audioData;
    public static GameDataManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }


    public void PlayAudio(string name)
    {
        var audioEvent = audioData.GetAudioEventDataByName(name);
        if (audioEvent != null)
        {
            audioEvent.audioEvent.Post(gameObject);
        }
    }

    public BasicItem NewItemById(int id)
    {
        if (id == 0)
        {
            return new Hoe();
        }
        else if (id == 1)
        {
            return new WateringCan();
        }
        else if (id == 3)
        {
            return new Sickle();
        }
        else if (id == 4)
        {
            return new Axe();

        }
        else if (id >= 5 && id <= 14)
        {
            return new BasicSeed(id, id);

        }
        else if (id >= 15 && id <= 17)
        {
            return new BasicTreeSeed(id, id);
        }
        else if (id >= 31 && id <= 41)
        {
            return new BasicWeedSeed(id, id);
        }
        else if (id >= 42 && id <= 42)
        {
            return new BasicWeedTreeSeed(id, id);
        }
        // ===== AI TEMP-001 BEGIN 成年树作弊物品 =====
        else if (id >= 60 && id <= 62)
        {
            // 60 -> 松树15，61 -> 柏树16，62 -> 橡树17
            return new AdultTreeSeed(id, id - 45);
        }
        // ===== AI TEMP-001 END =====
        else
        {
            BasicItem item = new BasicItem();
            item.SetId(id);
            return item;
        }
    }
}
