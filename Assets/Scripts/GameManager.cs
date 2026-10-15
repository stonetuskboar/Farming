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
    public TextColorTweenObj TransitionText;

    public HarvestEffectManager HarvestEffectManager;
    public ShopPageController ShopPageController;
    public float NowEnergy = 200;
    public float MaxEnergy = 200;

    public int Money = 200;
    public int year = 1;
    public Season season = Season.春;
    public int Month = 1;

    public List<BasicItem> SellItemList = new List<BasicItem>();

    public void Start()
    {
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
        GameDataManager.Instance.PlayAudio("SpringAmb");
        TransitionText.SetAlpha(0f);
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

    public void AddItem(int id, int amount)
    {
        int index = FindItemIndex(id);
        BasicItem item;
        if (index < 0)
        {
            item = new BasicItem();
            item.SetId(id);
            item.SetAmount(amount);
            item.Init(this);
            SellItemList.Add(item);
        }
        else
        {
            item = SellItemList[index];
        }
        item.Amount += amount;
        if(ShopPageController.itemSellPage.gameObject.activeInHierarchy == true)
        {
            ShopPageController.itemSellPage.FreshButton();
        }
    }
    public void DecreaseItem(int id, int amount)
    {
        int index = FindItemIndex(id);
        BasicItem item;
        if (index >= 0)
        {
            item = SellItemList[index];
            item.Amount -= amount;
            if(item.Amount <= 0)
            {
                SellItemList.RemoveAt(index);
            }
        }
        if (ShopPageController.itemSellPage.gameObject.activeInHierarchy == true)
        {
            ShopPageController.itemSellPage.FreshButton();
        }
    }
    public int FindItemIndex(int id)
    {
        for (int i = 0; i < SellItemList.Count; i++)
        {
            if (SellItemList[i].Id == id)
            {
                return i;
            }
        }
        return -1;
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