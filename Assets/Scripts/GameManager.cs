using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ak.wwise;

public class GameManager : MonoBehaviour
{
    public GameDataManager GameDataManager;
    public GridCursorController GridCursorController;
    public FarmLandManager FarmLandManager;

    public HotBarController hotbarController;
    public EnergyBar energyBar;

    public CircleTransition circleTransition;
    public TextColorTweenObj TransitionText;

    public HarvestEffectManager HarvestEffectManager;
    public ShopPageController ShopPageController;
    public float NowEnergy = 200;
    public float MaxEnergy = 200;

    public int Money = 200;
    public int year = 1;
    public Season season = Season.春;
    public int Month = 1;

    public TextColorTweenObj MoneyText;

    public List<BasicItem> SellItemList = new List<BasicItem>();

    public void Start()
    {
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
        GameDataManager.Instance.PlayAudio("SpringAmb");
        GameDataManager.Instance.PlayAudio("SpringMusic");
        TransitionText.SetAlpha(0f);
        UpdateMoneyText();
    }

    public void ConsumeEnergy(float amount)
    {
        NowEnergy -= amount;
        NowEnergy = Mathf.Clamp(NowEnergy, 0, MaxEnergy);
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
    }

    public void ReadyForNextTurn()
    {
        GameDataManager.Instance.PlayAudio("NextTurn");
        circleTransition.StartClose(1.2f , NextTurn);
        TransitionText.SetAlpha(0f);
        TransitionText.DoDelay(0.2f, () => { TransitionText.DoAlphaTween(1f, 0.4f); });
        TimePast();
        TransitionText.SetText($"第{year}年<br>{Month}月<br>{GetSeasonText(season)}");
    }
    public void NextTurn()
    {
        FarmLandManager.OnNextTurn(year,season,Month);
        circleTransition.StartOpen(0.8f);
        TransitionText.DoDelay(0.4f, () => { TransitionText.DoAlphaTween(0f, 0.3f); });
        NowEnergy = MaxEnergy;
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
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
    public void ChangeMoney(int value)
    {
        Money += value;
        UpdateMoneyText();

        MoneyText.SetLocalRotation(Quaternion.identity);
        MoneyText.SetLocalScale(Vector3.one);
        MoneyText.DoLocalRotationTween(Quaternion.Euler(30, -25, 0), 0.4f, type: EaseType.PunchDefault);
        MoneyText.DoLocalScaleTween(Vector3.one * 1.3f, 0.6f, type: EaseType.ShakeDefault);
        MoneyText.SetToHover();
        MoneyText.NormalTween(0.4f);
    }
    public void UpdateMoneyText()
    {
        MoneyText.text.text = Money.ToString();
    }
    public void AddItem(int id, int amount)
    {
        BasicItem item = FindItem(id);
        if (item == null)
        {
            item = new BasicItem();
            item.SetId(id);
            item.SetAmount(amount);
            item.Init(this);
            SellItemList.Add(item);
        }
        else
        {
            item.Amount += amount;
        }
        if(ShopPageController.itemSellPage.gameObject.activeInHierarchy == true)
        {
            ShopPageController.itemSellPage.FreshButton();
        }
    }
    public void DecreaseItem(int id, int amount)
    {
        BasicItem item = FindItem(id);
        if (item != null)
        {
            item.Amount -= amount;
            if(item.Amount <= 0)
            {
                SellItemList.Remove(item);
            }
        }
        if (ShopPageController.itemSellPage.gameObject.activeInHierarchy == true)
        {
            ShopPageController.itemSellPage.FreshButton();
        }
    }
    public BasicItem FindItem(int id)
    {
        for (int i = 0; i < SellItemList.Count; i++)
        {
            if (SellItemList[i].Id == id)
            {
                return SellItemList[i];
            }
        }
        return null;
    }

    public void TimePast()
    {
        int beforeMonth = Month;
        Month++;
        if (beforeMonth == 9)
        {
            season = Season.春;
            year++;
            Month = 1;
        }
        else
        {
            if (beforeMonth % 3 == 0)
            {
                if (season == Season.秋)
                {
                    Month = 1;
                    season = Season.春;
                    year++;
                }
                else if (season == Season.春)
                {
                    season = Season.夏;
                }
                else if (season == Season.夏)
                {
                    season = Season.秋;
                }
            }
        }
    }

    public string GetSeasonText(Season season)
    {
        if(season == Season.春)
        {
            return "春";
        }
        else if (season == Season.夏)
        {
            return "夏";
        }
        else if (season == Season.秋)
        {
            return "秋";
        }
        else if (season == Season.冬)
        {
            return "冬";
        }
        return "";
    }
}


public enum Season
{
    春,
    夏,
    秋,
    冬

}