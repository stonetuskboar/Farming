using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSellPage : MonoBehaviour
{
    public List<SellableItemUI> SellItemUIList = new();
    public SellingPopWindow sellingPopWindow;

    public GameManager gameManager;
    public void Awake()
    {
        for (int i = 0; i < SellItemUIList.Count; i++)
        {
            SellItemUIList[i].Init(this);
        }
    }
    public void OnEnable()
    {
        FreshButton();
        sellingPopWindow.popWindow.Hide(0f);
        sellingPopWindow.itemId = -1;
    }
    public void FreshButton()
    {
        for (int i = 0; i< gameManager.SellItemList.Count && i < SellItemUIList.Count; i++)
        {
            SellItemUIList[i].gameObject.SetActive(true);
            SellItemUIList[i].Set(gameManager.SellItemList[i]);

        }
        for (int i = gameManager.SellItemList.Count; i < SellItemUIList.Count; i++)
        {
            SellItemUIList[i].gameObject.SetActive(false);
        }
    }

    public SellableItemUI FindUIById(int id)
    {
        for(int i = 0; i < SellItemUIList.Count; i ++)
        {
            if (SellItemUIList[i].sellAbleItem.Id == id)
            {
                return SellItemUIList[i];
            }
        }
        return null;
    }
    public void OnHide()
    {
        if(sellingPopWindow.popWindow.nowState == BasicPopWindow.State.Showing)
        {
            sellingPopWindow.popWindow.Hide(0f);
        }
    }
}
