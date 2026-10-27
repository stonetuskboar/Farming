using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HotBarController : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private InputActionReference scrollAction;
    [SerializeField] private InputActionReference shiftAction;
    public UsableItem NowTool;
    public List<UsableItem> ItemList = new List<UsableItem>();
    public List<ToolButton> ButtonList = new();
    public ToolButton CursorSwitchButton;
    public int NowPage = 0;
    public TextTip Tip;
    public void Start()
    {
        UsableItem item = new PointerTool();
        item.Init(gameManager);
        item.SetAmount(1);
        CursorSwitchButton.Set(item);
        CursorSwitchButton.Init(this);
        AddItem(0, 1);
        AddItem(1, 1);
        AddItem(3, 1);
        AddItem(4, 1);
        AddItem(8, 10);
        AddItem(9, 5);

        InitButton();
        SwitchToCursor();
    }
    void OnEnable()
    {
        scrollAction.action.performed += OnScroll;
        shiftAction.action.Enable();
        scrollAction.action.Enable();
    }


    void OnDisable()
    {
        scrollAction.action.performed -= OnScroll;
        scrollAction.action.Disable();
        shiftAction.action.Disable();
    }
    public void GiveCheatSeed()
    {
        gameManager.ChangeMoney(3000);
        for (int i = 5; i <= 14; i++)
        {
            AddItem(i, 999);
        }
        for (int i = 31; i <= 42; i++)
        {
            AddItem(i, 999);
        }
        for (int i = 15; i <= 17; i++)
        {
            UsableItem item = new BasicWeedTreeSeed(i, i);
            AddItem(item, 999);
        }
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
            ButtonList[i].Set(ItemList[i + startIndex]);
            if (ButtonList[i].item == NowTool)
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
    public bool SelectToolByIndex(int index)
    {
        if (index >= ItemList.Count)
        {
            SelectNowTool(ItemList[ItemList.Count - 1]);
            return false;
        }
        else if(index < 0)
        {
            SelectNowTool(ItemList[0]);
            return false;
        }
        else 
        {
            SelectNowTool(ItemList[index]);
            return true;
        }
    }


    public void SelectNowTool(UsableItem item)
    {
        if (item == null)
        {
            return;
        }
        if (NowTool == item)
        {
            return;
        }
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (ButtonList[i].gameObject.activeInHierarchy == true && ButtonList[i].item == NowTool)
            {
                ButtonList[i].ChangeStateToIdle();
            }
            if (ButtonList[i].gameObject.activeInHierarchy == true && ButtonList[i].item == item)
            {
                ButtonList[i].ChangeStateToSelected();
            }
        }

        if (IsPointer() == true)
        {
            CursorSwitchButton.ChangeStateToIdle();
            gameManager.GridCursorController.ImageOuntline.ShowOutlineWidth();
        }
        NowTool = item;
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
    public void AddItem(UsableItem item, int amount = 0)
    {
        item.SetAmount(amount);
        item.Init(gameManager);
        ItemList.Add(item);

        FreshButton();
        if (gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }
    public void OnUpgradeItem(UsableItem item)
    {
        if(NowTool == item)
        {
            gameManager.GridCursorController.ChangeCursorSprite(NowTool.itemData.Icon);
        }

        FreshButton();
        if (gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }
    public void DecreaseItem(int id, int amount)
    {
        UsableItem item = FindItemById(id);
        if (item == null)
        {
            return;
        }
        item.Amount -= amount;
        FreshButton();
        if (gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }

    public void RemoveItemById(int id)
    {
        int index = FindItemIndexById(id);
        ItemList.RemoveAt(index);
        if (ItemList.Count > index)
        {
            SelectNowTool(ItemList[index]);
        }
        else
        {
            SelectNowTool(ItemList[ItemList.Count - 1]);
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
        if ((NowPage + 1) * ButtonList.Count < ItemList.Count)
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
    private void OnScroll(InputAction.CallbackContext context)
    {
        if(IsPointer() == true)
        {
            return;
        }
        if(shiftAction.action.IsPressed() == true)
        {
            return;
        }
        Vector2 scroll = context.ReadValue<Vector2>();
        int index = FindItemIndexById(NowTool.Id);
        if(scroll.y > 0)
        {
            bool result = SelectToolByIndex(index - 1);
            if(result == true)
            {
                GameDataManager.Instance.PlayAudio("ItemSelect");
            }
        }
        else if (scroll.y <0)
        {
            bool result = SelectToolByIndex(index + 1);
            if (result == true)
            {
                GameDataManager.Instance.PlayAudio("ItemSelect");
            }
        }
    }

    public void SwitchToCursor()
    {
        if (IsPointer() == true)
        {
            return;
        }
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (ButtonList[i].gameObject.activeInHierarchy == true && ButtonList[i].item == NowTool)
            {
                ButtonList[i].ChangeStateToIdle();
            }
        }
        CursorSwitchButton.ChangeStateToSelected();
        gameManager.GridCursorController.ChangeCursorToPointer();
        NowTool = CursorSwitchButton.item;
    }
    public bool IsPointer()
    {
        if(NowTool != null && NowTool == CursorSwitchButton.item)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
