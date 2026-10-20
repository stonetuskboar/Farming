using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellableItemUI : MonoBehaviour
{
    public BasicItem sellAbleItem;
    public Image icon;
    public TextMeshProUGUI Amount;
    [NonSerialized]
    public ItemSellPage itemSellPage;

    public ImageSwapButton SellButton;

    public void Init(ItemSellPage page)
    {
        this.itemSellPage = page;
    }
    public void Set(BasicItem item)
    {
        this.sellAbleItem = item;
        Sprite sprite = item.itemData.Icon;
        icon.sprite = sprite;
        Amount.text = item.Amount.ToString();
    }
    public void Start()
    {
        SellButton.PointerClicked += SelectThis;
    }
    public void SelectThis()
    {
        if(itemSellPage.sellingPopWindow.itemId == sellAbleItem.Id)
        {
            itemSellPage.sellingPopWindow.itemId = -1;
            itemSellPage.sellingPopWindow.popWindow.Hide();
        }
        else
        {
            Vector3[] corners = new Vector3[4];
            SellButton.GetRectTransform().GetWorldCorners(corners);
            Vector3 leftBottom = corners[0];
            Vector3 leftTop = corners[1];

            // 左侧边缘的中点（世界坐标）
            Vector3 leftCenter = (leftBottom + leftTop) / 2f;

            itemSellPage.sellingPopWindow.Set(sellAbleItem);
            itemSellPage.sellingPopWindow.popWindow.Show(leftCenter, 0.3f);
        }

    }
}
