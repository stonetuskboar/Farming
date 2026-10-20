using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeToolPage : MonoBehaviour
{
    public BasicPopWindow ToolDescriptionPopWindow;
    public TextMeshProUGUI PopWindowText;
    public ToolItemUI ToolUIPrefab;
    public List<ToolItemUI> ToolItemUIList = new ();

    public RectTransform LayoutRect;

    public ImageColorTweenObj BeforeUpgradeImage;
    public ImageColorTweenObj AfterUpgradeImage;
    [NonSerialized]
    public ToolItem NowSelectedTool = null;
    public GameManager gameManager;
    public void OnEnable()
    {
        FreshButton();
        HidePlantImmediately();
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
        ToolDescriptionPopWindow.Hide(0f);
        BeforeUpgradeImage.SetAlpha(0f);
        AfterUpgradeImage.SetAlpha(0f);
        FindUIById(NowSelectedTool)?.Button.ChangeStateToIdleImmediately();
        NowSelectedTool = null;
    }
    public void HideToolInfo()
    {
        ToolDescriptionPopWindow.Hide(0.3f);
        BeforeUpgradeImage.DoAlphaTween(0f,0.3f);
        AfterUpgradeImage.DoAlphaTween(0f, 0.3f);
        NowSelectedTool = null;
    }
    public void HoverTool(ToolItem item)
    {
        PopWindowText.text = item.itemData.Description;
        GameDataManager.Instance.PlayAudio("Popup");
        ToolDescriptionPopWindow.Show(0.2f);
        BeforeUpgradeImage.sprite = item.itemData.Icon;
        TweenShow(BeforeUpgradeImage);
    }
    public void SelectTool(ToolItem item)
    {
        PopWindowText.text = item.itemData.Description;
        if (NowSelectedTool != null)
        {
            ToolDescriptionPopWindow.Show(0.3f);
            TweenShow(BeforeUpgradeImage);
            FindUIById(NowSelectedTool)?.Button.ChangeStateToIdle();
        }
        else
        {
        }
        BeforeUpgradeImage.sprite = item.itemData.Icon;
        NowSelectedTool = item;
    }
    public void TweenShow(BasicColorTweenObject tweenObj)
    {
        tweenObj.SetLocalRotation(Quaternion.Euler(-30, 25, 0));
        tweenObj.SetLocalScale(0.8f * Vector3.one);
        tweenObj.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
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
}
