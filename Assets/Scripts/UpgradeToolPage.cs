using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ak.wwise;

public class UpgradeToolPage : MonoBehaviour
{
    public BasicPopWindow ToolDescriptionPopWindow;
    public TextMeshProUGUI PopWindowText;
    public ToolItemUI ToolUIPrefab;
    public List<ToolItemUI> ToolItemUIList = new ();

    public RectTransform LayoutRect;

    public ImageColorTweenObj BeforeUpgradeImage;
    public ImageColorTweenObj AfterUpgradeImage;
    public ImageSwapButton UpgradeButton;
    public TextMeshProUGUI MoneyText;
    public BasicUIView MoneyTextView;
    public Color FailedColor;
    public Color SucessColor;
    [NonSerialized]
    public ToolItem NowSelectedTool = null;
    public GameManager gameManager;
    public void OnEnable()
    {
        FreshButton();
        HidePlantImmediately();
    }
    public void Awake()
    {
        UpgradeButton.PointerClicked += UpgradeTool;
    }
    public void FreshButton()
    {
        List<ToolItem> toolList = new();

        for (int i = 0; i < gameManager.hotbarController.ItemList.Count; i++)
        {
            if (gameManager.hotbarController.ItemList[i] is ToolItem)
            {
                toolList.Add(gameManager.hotbarController.ItemList[i] as ToolItem);
            }
        }
        if (toolList.Count > ToolItemUIList.Count)
        {
            for (int i = ToolItemUIList.Count; i < toolList.Count; i++)
            {
                CreateProgress();
            }
        }

        for (int i = 0; i < toolList.Count; i++)
        {
            ToolItemUIList[i].gameObject.SetActive(true);
            ToolItemUIList[i].Set(toolList[i]);
        }
        for (int i = toolList.Count; i < ToolItemUIList.Count; i++)
        {
            ToolItemUIList[i].gameObject.SetActive(false);
        }
    }
    public ToolItemUI GetProgress()
    {
        for (int i = 0; i < ToolItemUIList.Count; i++)
        {
            if (ToolItemUIList[i].gameObject.activeSelf == false)
            {
                ToolItemUIList[i].gameObject.SetActive(true);
                return ToolItemUIList[i];
            }
        }
        return CreateProgress();
    }
    public ToolItemUI CreateProgress()
    {
        ToolItemUI newProgress = Instantiate(ToolUIPrefab, LayoutRect);
        newProgress.gameObject.SetActive(false);
        ToolItemUIList.Add(newProgress);
        newProgress.Init(this);
        return newProgress;
    }
    public void HidePlantImmediately()
    {
        MoneyTextView.SetCanvasGroupAlpha(0f);
        UpgradeButton.Hide(0f);
        ToolDescriptionPopWindow.Hide(0f);
        BeforeUpgradeImage.SetAlpha(0f);
        AfterUpgradeImage.SetAlpha(0f);
        FindUIById(NowSelectedTool)?.Button.ChangeStateToIdleImmediately();
        NowSelectedTool = null;
    }
    public void HideToolInfo()
    {
        MoneyTextView.DoAlphaTween(0f,0.3f);
        UpgradeButton.Hide(0.3f);
        ToolDescriptionPopWindow.Hide(0.3f);
        BeforeUpgradeImage.DoAlphaTween(0f,0.3f);
        AfterUpgradeImage.DoAlphaTween(0f, 0.3f);
        NowSelectedTool = null;
    }
    public void HoverTool(ToolItem item)
    {
        GameDataManager.Instance.PlayAudio("Popup");
        PopWindowText.text = $"{item.itemData.Description}<br>{item.toolData.UpgradeDescription}";
        ToolDescriptionPopWindow.Show(0.2f);
        BeforeUpgradeImage.sprite = item.itemData.Icon;
        TweenShow(BeforeUpgradeImage);
        if(item.toolData.CouldUpgrade == true)
        {
            ItemData data = GameDataManager.Instance.itemDataList.GetItemDataById(item.toolData.UpgradeId);
            AfterUpgradeImage.sprite = data.Icon;
            AfterUpgradeShow(AfterUpgradeImage);

            SetMoneyText(item.toolData.UpgradeCost);
            MoneyTextView.DoAlphaTween(1f, 0.2f);
        }
    }
    public void SelectTool(ToolItem item)
    {
        PopWindowText.text = $"{item.itemData.Description}<br>{item.toolData.UpgradeDescription}";
        if (NowSelectedTool != null)
        {
            ToolDescriptionPopWindow.Show(0.3f);
            TweenShow(BeforeUpgradeImage);
            if (item.toolData.CouldUpgrade == true)
            {
                UpgradeButton.Show(0.3f);
                AfterUpgradeShow(AfterUpgradeImage);
                SetMoneyText(item.toolData.UpgradeCost);
                MoneyTextView.DoAlphaTween(1f, 0.2f);
            }
            else
            {
                UpgradeButton.Hide(0.2f);
            }

            FindUIById(NowSelectedTool)?.Button.ChangeStateToIdle();
        }
        else
        {
            if (item.toolData.CouldUpgrade == true)
            {
                UpgradeButton.Show(0.3f);
                SetMoneyText(item.toolData.UpgradeCost);
                MoneyTextView.DoAlphaTween(1f, 0.2f);
            }

        }
        if (item.toolData.CouldUpgrade == true)
        {
            ItemData data = GameDataManager.Instance.itemDataList.GetItemDataById(item.toolData.UpgradeId);
            AfterUpgradeImage.sprite = data.Icon;
        }
        BeforeUpgradeImage.sprite = item.itemData.Icon;
        NowSelectedTool = item;
    }
    public void TweenShow(BasicColorTweenObject tweenObj)
    {
        tweenObj.SetLocalRotation(Quaternion.Euler(-30, -25, 0));
        tweenObj.SetLocalScale(0.8f * Vector3.one);
        tweenObj.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoAlphaTween(1f, 0.2f);
    }
    public void AfterUpgradeShow(BasicColorTweenObject tweenObj)
    {
        tweenObj.SetLocalRotation(Quaternion.Euler(30, 25, 0));
        tweenObj.SetLocalScale(1.2f * Vector3.one);
        tweenObj.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.M3Spring);
        tweenObj.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.M3Spring);
        tweenObj.DoAlphaTween(1f, 0.2f);
    }
    public ToolItemUI FindUIById(ToolItem item)
    {
        for (int i = 0; i < ToolItemUIList.Count; i++)
        {
            if (ToolItemUIList[i].toolItem == item && ToolItemUIList[i].gameObject.activeInHierarchy == true)
            {
                return ToolItemUIList[i];
            }
        }
        return null;
    }
    public void SetMoneyText(int value)
    {
        MoneyText.text = value.ToString();
        if (gameManager.Money < value)
        {
            MoneyText.color = FailedColor;
        }
        else
        {
            MoneyText.color = SucessColor;
        }
    }
    public void UpgradeTool()
    {
        if(gameManager.Money < NowSelectedTool.toolData.UpgradeCost)
        {
            GameDataManager.Instance.PlayAudio("Failed");
            return;
        }
        GameDataManager.Instance.PlayAudio("ToolUpgrade");
        gameManager.ChangeMoney(-NowSelectedTool.toolData.UpgradeCost);
        NowSelectedTool.Upgrade();
        gameManager.hotbarController.OnUpgradeItem(NowSelectedTool);
        MoneyTextView.DoAlphaTween(0f,0.2f);
        UpgradeButton.Hide(0.3f);
        PopWindowText.text = $"{NowSelectedTool.itemData.Description}<br>{NowSelectedTool.toolData.UpgradeDescription}";
        ToolDescriptionPopWindow.Show(0.3f);
        AfterUpgradeImage.SetAlpha(0f);
        BeforeUpgradeImage.image.sprite = NowSelectedTool.itemData.Icon;
        BeforeUpgradeImage.SetLocalRotation(Quaternion.identity);
        BeforeUpgradeImage.SetLocalScale(Vector3.one);
        BeforeUpgradeImage.DoLocalRotationTween(Quaternion.Euler(30, -25, 0), 0.4f, type: EaseType.PunchDefault);
        BeforeUpgradeImage.DoLocalScaleTween(Vector3.one * 1.3f, 0.6f, type: EaseType.ShakeDefault);
        FreshButton();
    }
}
