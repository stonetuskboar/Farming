using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ToolButton : ImageSwapButton
{
    public Image ToolIcon;
    public GameObject AmountObj;
    public TextMeshProUGUI AmountText;
    private HotBarController HotBarController;
    [NonSerialized]
    public int itemId;
    protected override void Awake()
    {
        base.Awake();
        PointerClicked += OnClick;
        PointerEntered += OnPointEnter;
        PointerExited += OnPointerExit;
    }

    public void Init(HotBarController controller)
    {
        HotBarController = controller;
    }
    public void Set( BasicItem item)
    {
        itemId = item.Id;
        ToolIcon.sprite = item.itemData.Icon;
        if(item is BasicSeed)
        {
            AmountText.text = item.Amount.ToString();
            AmountObj.gameObject.SetActive(true);
        }
        else
        {
            AmountObj.gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        HotBarController.SelectNowTool(itemId);
    }

    public void OnPointEnter()
    {
        Vector3[] corners = new Vector3[4];
        GetRectTransform().GetWorldCorners(corners);
        Vector3 leftTop = corners[1];
        Vector3 rightBottom = corners[2];

        // 上侧侧边缘的中点（世界坐标）
        Vector3 upCenter = (rightBottom + leftTop) / 2f;
        UsableItem item = HotBarController.FindItemById(itemId);
        string str = "";
        if (item is BasicSeed)
        {
            BasicSeed seed = item as BasicSeed;
            ItemData data = GameDataManager.Instance.itemDataList.GetItemDataById(seed.cropData.DropedItemId);
            str = $"{seed.itemData.Name},{seed.cropData.Name}的种子。<br>可生长季节：{seed.cropData.GetSeasonText()}<br>生长时间：{seed.cropData.matureTurns}个月。";
                
            if(seed is BasicTreeSeed)
            {
                str += $"<br>不需要浇水也可生长。";
            }
            str += $"<br>可收获{seed.cropData.DropedAmountMin}-{seed.cropData.DropedAmountMax}个{data.Name}。售价：{data.Price}。";
        }
        else
        {
            str = item.itemData.Description;
        }
        HotBarController.Tip.DoDelayShow(0.4f, upCenter,str,0.2f);
    }

    public void OnPointerExit()
    {
        HotBarController.Tip.Hide();
    }
}
