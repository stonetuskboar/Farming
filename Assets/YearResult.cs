using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class YearResult : MonoBehaviour
{
    public BasicUIView uiView;
    public TextMeshProUGUI YearText;
    public TextMeshProUGUI DebtText;
    public TextMeshProUGUI EarnText;
    public TextMeshProUGUI PaidText;
    public TextMeshProUGUI ResultMoneyText;

    public List<ResultItem> PlantPlants;
    public List<ResultItem> HarvestPlants;
    public List<ResultItem> WiltPlants;
    public GameManager gameManager;
    public ImageSwapButton ConfirmButton;
    protected void Awake()
    {
        uiView.SetCanvasGroupAlpha(0f);
        uiView.SetCanvasGroupUnblock();
        ConfirmButton.PointerClicked += Confirm;
    }
    public void Set()
    {
        int year = gameManager.year;
        if ( year == 1)
        {
            YearText.text = "第一年";
        }
        else if (year == 2)
        {
            YearText.text = "第二年";
        }
        else if (year == 3)
        {
            YearText.text = "第三年";
        }
        else if (year == 4)
        {
            YearText.text = "第四年";
        }
        else if (year == 5)
        {
            YearText.text = "第五年";
        }
        DebtText.text = gameManager.GetDebt().ToString();
        EarnText.text = gameManager.EarnThisYear.ToString();
        PaidText.text = gameManager.PaidThisYear.ToString();
        ResultMoneyText.text = (gameManager.Money - gameManager.GetDebt()).ToString();


        List<KeyValuePair<int,int>> topPlant = gameManager.FarmLandManager.PlantDic
            .OrderByDescending(kv => kv.Value)
            .Take(PlantPlants.Count)
            .ToList();
        for (int i = 0; i < topPlant.Count && i < PlantPlants.Count; i++ )
        {
            CropData data = GameDataManager.Instance.cropDataList.GetCropDataById(topPlant[i].Key);
            if(data == null)
            {
                data = GameDataManager.Instance.cropDataList.GetTreeDataById(topPlant[i].Key);
            }
            PlantPlants[i].gameObject.SetActive(true);
            PlantPlants[i].Set(data.GetGrownSprite(), topPlant[i].Value);
        }

        for( int i = topPlant.Count; i < PlantPlants.Count; i++)
        {
            PlantPlants[i].gameObject.SetActive(false);
        }

        topPlant = gameManager.FarmLandManager.HarvestDic
            .OrderByDescending(kv => kv.Value)
            .Take(HarvestPlants.Count)
            .ToList();
        for (int i = 0; i < topPlant.Count && i < HarvestPlants.Count; i++)
        {
            CropData data = GameDataManager.Instance.cropDataList.GetCropDataById(topPlant[i].Key);
            if (data == null)
            {
                data = GameDataManager.Instance.cropDataList.GetTreeDataById(topPlant[i].Key);
            }
            HarvestPlants[i].gameObject.SetActive(true);
            HarvestPlants[i].Set(data.GetGrownSprite(), topPlant[i].Value);
        }

        for (int i = topPlant.Count; i < HarvestPlants.Count; i++)
        {
            HarvestPlants[i].gameObject.SetActive(false);
        }

            topPlant = gameManager.FarmLandManager.WiltDic
        .OrderByDescending(kv => kv.Value)
        .Take(WiltPlants.Count)
        .ToList();
        for (int i = 0; i < topPlant.Count && i < WiltPlants.Count; i++)
        {
            CropData data = GameDataManager.Instance.cropDataList.GetCropDataById(topPlant[i].Key);
            if (data == null)
            {
                data = GameDataManager.Instance.cropDataList.GetTreeDataById(topPlant[i].Key);
            }
            WiltPlants[i].gameObject.SetActive(true);
            WiltPlants[i].Set(data.GetGrownSprite(), topPlant[i].Value);
        }

        for (int i = topPlant.Count; i < WiltPlants.Count; i++)
        {
            WiltPlants[i].gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        GameDataManager.Instance.PlayAudio("ShopOpen");
        gameObject.SetActive(true);
        uiView.SetCanvasGroupAlpha(1f);
        uiView.SetCanvasGroupBlockable();
        uiView.SetLocalRotation(Quaternion.Euler(30, -25, 0));
        uiView.SetLocalScale(0.8f * Vector3.one);
        uiView.DoLocalRotationTween(Quaternion.identity, 0.4f, type: EaseType.EaseOutBack);
        uiView.DoLocalScaleTween(Vector3.one, 0.5f, type: EaseType.EaseOutBack);
        uiView.DoAlphaTween(1f, 0.4f);
    }
    public void Hide(Action callBack = null)
    {
        GameDataManager.Instance.PlayAudio("ShopClose");
        uiView.DoLocalScaleTween(Vector3.one * 0.6f, 0.3f, type: EaseType.EaseInOutBack,callBack: callBack);
        uiView.SetCanvasGroupUnblock();
        uiView.DoAlphaTween(0f, 0.3f, Unactive);
    }
    public void Unactive()
    {
        gameObject.SetActive(false);
    }
    public void Confirm()
    {
        Hide(ShowResult); 

    }
    public void ShowResult()
    {
        if (gameManager.CouldPaidTheBill() == true && gameManager.year < 3)
        { 
            gameManager.ClearStatData();
            gameManager.FarmLandManager.ClearStatData();
            gameManager.PaidTheBill();
            gameManager.TimePast();
            gameManager.transitionController.ShowBill(IsSeasonChange: true);
        }
        else
        {
            gameManager.hotbarController.SwitchToCursor();
            gameManager.transitionController.endController.ShowResulView();
        }
    }
}
