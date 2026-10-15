using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarController : MonoBehaviour
{
    public GameManager gameManager;
    public UsableItem NowTool;
    private int NowSelectId = -1;
    public List<UsableItem> ItemList = new List<UsableItem>();
    public List<ToolButton> ButtonList = new();
    public int NowPage = 0;

    public void Start()
    {
        AddItem(0, 1);
        AddItem(1, 1);
        AddItem(3, 1);
        AddItem(4, 1);
        //for (int i = 5; i <= 17; i++)
        //{
        //    AddItem(i, 0);
        //}
        //for (int i = 31; i <= 41; i++)
        //{
        //    AddItem(i, 0);
        //}
        //for (int i = 42; i <= 43; i++)
        //{
        //    AddItem(i, 0);
        //}
        InitButton();
        SelectNowTool(ButtonList[0]);
    }

    public void InitButton()
    {
        for(int i = 0; i < ButtonList.Count; i++)
        {
            ButtonList[i].Init(this);
        }
        FreshButton();
    }
    public void FreshButton()
    {
        CheckPage();
        int startIndex = NowPage * ButtonList.Count;
        for (int i = 0; (i + startIndex) < ItemList.Count && i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(true);
            Sprite sprite = ItemList[i + startIndex].itemData.Icon;
            ButtonList[i].Set(ItemList[i + startIndex]);
            if (ButtonList[i].itemId == NowSelectId)
            {
                ButtonList[i].ChangeStateToSelectedImmediately();
            }
            else
            {
                ButtonList[i].ChangeStateToIdleImmediately();
            }
        }
        for (int i = ItemList.Count - startIndex; i >=0 && i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
    }
    public void SelectLastTool()
    {

    }
    public void SelectNowTool(ToolButton button)
    {
        SelectNowTool(button.itemId);
    }
    public void SelectNowTool(int itemId)
    {
        if (NowSelectId == itemId)
        {
            return;
        }
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (ButtonList[i].itemId == NowSelectId && ButtonList[i].gameObject.activeInHierarchy == true)
            {
                ButtonList[i].ChangeStateToIdle();
            }
            if (ButtonList[i].itemId == itemId && ButtonList[i].gameObject.activeInHierarchy == true)
            {
                ButtonList[i].ChangeStateToSelected();
            }
        }
        NowSelectId = itemId;
        NowTool = FindItemById(itemId);
        gameManager.GridCursorController.ChangeCursorSprite(NowTool.itemData.Icon);
    }
    public void AddItem(int id, int amount = 0)
    {
        int index = FindItemIndexById(id);
        BasicItem item;
        if (index < 0)
        {
            item = GameDataManager.Instance.NewItemById(id);
            item.SetAmount(amount);
            item.Init(gameManager);
            ItemList.Add(item as UsableItem);
        }
        else
        {
            item = ItemList[index];
            item.Amount += amount;
        }

        FreshButton();
        if(gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }
    public void DecreaseItem(int id, int amount)
    {
        int index = FindItemIndexById(id);
        BasicItem item;
        if (index > 0)
        {
            item = ItemList[index];
            item.Amount -= amount;
            if(item.Amount <= 0)
            {
                ItemList.RemoveAt(index);
                if(ItemList.Count > index)
                {
                    SelectNowTool(ItemList[index].Id);
                }
                else
                {
                    SelectNowTool(ItemList[ItemList.Count -1].Id);
                }
            }
        }  

        FreshButton();
        if (gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }
    public int FindItemIndexById(int id)
    {
        for(int i =0; i < ItemList.Count; i++)
        {
            if (ItemList[i].Id == id)
            {
                return i;
            }
        }
        return -1;
    }
    public UsableItem FindItemById(int id)
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].Id == id)
            {
                return ItemList[i];
            }
        }
        return null;
    }
    public void DownPage()
    {
        GameDataManager.Instance.PlayAudio("UI_Click");
        if ((NowPage + 1) * ButtonList.Count < ItemList.Count)
        {
            NowPage++;
            FreshButton();
        }
    }
    public void UpPage()
    {
        GameDataManager.Instance.PlayAudio("UI_Click");
        if(NowPage >0)
        {
            
            NowPage--;
            FreshButton();
        }
    }
    public void CheckPage()
    {
        if (NowPage * ButtonList.Count >= ItemList.Count)
        {
            NowPage = (ItemList.Count - 1 )/ ButtonList.Count;
        }
        else if (NowPage < 0)
        {
            NowPage = 0;
        }
    }
}
