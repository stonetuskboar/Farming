using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public ItemDataList itemDataList;
    public CropDataList cropDataList;
    [Header("Wwise")]
    public AK.Wwise.Event PlayWater;
    public AK.Wwise.Event PlayChopTree;
    public AK.Wwise.Event PlayHarvest;
    public AK.Wwise.Event ItemPickUp;
    public AK.Wwise.Event Plant;
    public AK.Wwise.Event Till;
    public AK.Wwise.Event NextTurn;
    public AK.Wwise.Event UIClick;
    public AK.Wwise.Event PlaySpringAmb;
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
}
