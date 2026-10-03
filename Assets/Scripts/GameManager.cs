using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameDataManager GameDataManager;
    public GridCursorController GridCursorController;
    public FarmLandManager FarmLandManager;

    public HotBarController hotbarController;
    public EnergyBar energyBar;

    public CircleTransition circleTransition;

    public HarvestEffectManager HarvestEffectManager;

    public float NowEnergy = 200;
    public float MaxEnergy = 200;

    public int year = 1;
    public Season season = Season.Spring;
    public int Round = 1;


    public void Start()
    {
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
        GameDataManager.PlaySpringAmb.Post(Camera.main.gameObject);
    }

    public void ConsumeEnergy(float amount)
    {
        NowEnergy -= amount;
        NowEnergy = Mathf.Clamp(NowEnergy, 0, MaxEnergy);
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
    }

    public void ReadyForNextTurn()
    {
        GameDataManager.Instance.NextTurn.Post(Camera.main.gameObject);
        circleTransition.StartClose(1.2f , NextTurn);
    }
    public void NextTurn()
    {
        FarmLandManager.OnNextTurn();
        circleTransition.StartOpen(0.8f);
        NowEnergy = MaxEnergy;
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
    }
    //这个是临时写的
    public void UIClick()
    {
        GameDataManager.Instance.UIClick.Post(Camera.main.gameObject);
    }
    public void DoDelay(float time, Action callBack = null)
    {
        StartCoroutine(Delay(time, callBack));
    }
    IEnumerator Delay(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

}


public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}