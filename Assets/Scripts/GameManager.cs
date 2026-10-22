using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public GameDataManager GameDataManager;
    public GridCursorController GridCursorController;
    public FarmLandManager FarmLandManager;


    public HotBarController hotbarController;
    public EnergyBar energyBar;

    public TransitionController transitionController;

    public HarvestEffectManager HarvestEffectManager;
    public ShopPageController ShopPageController;
    public CatTip catTip;
    public float NowEnergy = 200;
    public float MaxEnergy = 200;

    public int Money = 200;
    public int year = 1;
    public Season season = Season.春;
    public int Month = 1;
    public List<int> Debts;
    [NonSerialized]
    public int EarnThisYear = 0;
    [NonSerialized]
    public int PaidThisYear = 0;
    public TextColorTweenObj MoneyText;

    public List<BasicItem> SellItemList = new List<BasicItem>();

    public void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
        GameDataManager.Instance.audioData.MusicVolume.SetGlobalValue(30);
    }
    public void ChangeVolume(float value)
    {
        GameDataManager.Instance.audioData.MusicVolume.SetGlobalValue(value);
    }
    public void GameStart()
    {
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
        UpdateMoneyText();
        SetSeasonAudio();
    }
    public bool CouldPaidTheBill()
    {
        if(Money < GetDebt())
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void PaidTheBill()
    {
        Money -= GetDebt();
        UpdateMoneyText();
    }

    public void ClearStatData()
    {
        EarnThisYear = 0;
        PaidThisYear = 0;
    }
    public void SetSeasonAudio()
    {
        Season newSeason = season;

        string wwiseSeason;

        switch (newSeason)
        {
            case Season.春:
                wwiseSeason = "Spring";
                break;

            case Season.夏:
                wwiseSeason = "Summer";
                break;

            // 程序里叫 Fall，Wwise 里叫 Autumn
            case Season.秋:
                wwiseSeason = "Autumn";
                break;

            case Season.冬:
                // Wwise 目前还没有 Winter
                //Debug.LogWarning("Wwise 还没有添加 Winter 音频");
                return;

            default:
                wwiseSeason = "Spring";
                break;
        }

        AkUnitySoundEngine.SetSwitch(
            "Season_Transition",
            wwiseSeason,
            GameDataManager.gameObject
        );
    }

    public void ConsumeEnergy(float amount)
    {
        NowEnergy -= amount;
        NowEnergy = Mathf.Clamp(NowEnergy, 0, MaxEnergy);
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
    }
    public void ReadyForNextTurn()
    {
        TimePast();
        if (Month % 3 == 1)
        {
            transitionController.StartShow(IsSeasonChange: true);
        }
        else
        {
            transitionController.StartShow(IsSeasonChange: false);
        }

    }
    public void NextTurn()
    {
        if(season == Season.冬)
        {
            return;
        }
        if(Month % 3 == 1)
        {
            SetSeasonAudio();
        }
        FarmLandManager.OnNextTurn(year,season,Month);
        NowEnergy = MaxEnergy;
        energyBar.SetEnergy(NowEnergy, MaxEnergy);
    }
    public int GetDebt()
    {
        return Debts[year - 1];
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
        if(value > 0)
        {
            EarnThisYear += value;
        }
        else
        {
            PaidThisYear += Mathf.Abs(value);
        }
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
            if (ShopPageController.itemSellPage.gameObject.activeInHierarchy == true)
            {
                ShopPageController.itemSellPage.FreshButton();
            }
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
        if(season == Season.冬)
        {
            Month = 1;
            season = Season.春;
            year++;
        }
        if (beforeMonth % 3 == 0)
        {
            if (season == Season.冬)
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
            else if (season == Season.秋)
            {
                season = Season.冬;
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