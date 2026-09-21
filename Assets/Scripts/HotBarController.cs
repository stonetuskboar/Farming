using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarController : MonoBehaviour
{
    public GameManager gameManager;
    public UsableItem NowTool;
    private int NowSelectIndex = -1;
    public List<BasicItem> ItemList = new List<BasicItem>();
    public List<ToolButton> ButtonList = new();
    public int NowPage = 0;

    public void Start()
    {
        ItemList.Add(new Hoe());
        ItemList.Add(new WateringCan());
        ItemList.Add(new Sickle());
        ItemList.Add(new Axe());
        for(int i = 5; i <= 14; i ++)
        {
            ItemList.Add(new BasicSeed(i, i));
        }
        for (int i = 15; i <= 17; i++)
        {
            ItemList.Add(new BasicTreeSeed(i, i));
        }
        for (int i = 31; i <= 39; i++)
        {
            ItemList.Add(new BasicSeed(i, i));
        }
        for (int i = 40; i <= 41; i++)
        {
            ItemList.Add(new BasicTreeSeed(i, i));
        }
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemList[i].Init(gameManager);
        }
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
        int startIndex = NowPage * ButtonList.Count;
        int index = NowSelectIndex - startIndex;
        for (int i = 0; (i + startIndex) < ItemList.Count && i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(true);
            Sprite sprite = ItemList[i + startIndex].itemData.Icon;
            ButtonList[i].ToolIcon.sprite = sprite;
            ButtonList[i].Set(i + startIndex);
            if (ButtonList[i].index == NowSelectIndex)
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

    public void SelectNowTool(ToolButton button)
    {
        if (ItemList[button.index] is not UsableItem)
        {
            return;
        }
        for(int i = 0; i < ButtonList.Count; i++)
        {
            if (ButtonList[i].index == NowSelectIndex)
            {
                ButtonList[i].ChangeStateToIdle();
            }
        }
        button.ChangeStateToSelected();
        NowSelectIndex = button.index;
        NowTool = ItemList[button.index] as UsableItem;
        gameManager.GridCursorController.ChangeCursorSprite(NowTool.itemData.Icon);
    }
    public void AddItem(int id)
    {
        int index = FindItemIndex(id);
        BasicItem item;
        if (index < 0)
        {
            item = new BasicItem();
            item.SetId(id);
            item.Init(gameManager);
            ItemList.Add(item);
        }
        else
        {
            item = ItemList[index];
        }
        item.Amount++;

        int startIndex = index - NowPage * ButtonList.Count;
        if (startIndex >= 0 && startIndex <= ButtonList.Count)
        {
            FreshButton();
        }
    }

    public int FindItemIndex(int id)
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

    public void DownPage()
    {
        if (NowPage * ButtonList.Count < ItemList.Count)
        {
            NowPage++;
            FreshButton();
        }
    }
    public void UpPage()
    {
        if(NowPage >0)
        {
            NowPage--;
            FreshButton();
        }
    }
}
