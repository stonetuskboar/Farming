using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class HotBarController : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private InputActionReference scrollAction;
    [SerializeField] private InputActionReference shiftAction;
    public UsableItem NowTool;
    public List<UsableItem> ItemList = new List<UsableItem>();
    public List<ToolButton> ButtonList = new();
    public ImageSwapButton CursorSwitchButton;
    public int NowPage = 0;
    public TextTip Tip;
    public void Start()
    {
        AddItem(0, 1);
        AddItem(1, 1);
        AddItem(3, 1);
        AddItem(4, 1);
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
            BasicItem item = new BasicWeedTreeSeed(i,i);
            AddItem(item, 999);
        }
        InitButton();
        CursorSwitchButton.PointerClicked += OnSwitchToCursor;
        CursorSwitchButton.PointerEntered += OnCursorPointEnter;
        CursorSwitchButton.PointerExited += OnCursorPointerExit;
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

    public void OnGameStart()
    {
        OnSwitchToCursor();
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
            if (NowTool != null && ButtonList[i].itemId == NowTool.Id)
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
            SelectNowTool(ItemList[ItemList.Count - 1].Id);
            return false;
        }
        else if(index < 0)
        {
            SelectNowTool(ItemList[0].Id);
            return false;
        }
        else 
        {
            SelectNowTool(ItemList[index].Id);
            return true;
        }
    }


    public void SelectNowTool(int itemId)
    {
        if (NowTool != null && NowTool.Id == itemId)
        {
            return;
        }
        UsableItem item = FindItemById(itemId); 
        if(item == null)
        {
            return;
        }
        if (NowTool == null)
        {
            gameManager.GridCursorController.ImageOuntline.ShowOutlineWidth();
            CursorSwitchButton.ChangeStateToIdle();
        }
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (NowTool != null && ButtonList[i].itemId == NowTool.Id && ButtonList[i].gameObject.activeInHierarchy == true)
            {
                ButtonList[i].ChangeStateToIdle();
            }
            if (ButtonList[i].itemId == itemId && ButtonList[i].gameObject.activeInHierarchy == true)
            {
                ButtonList[i].ChangeStateToSelected();
            }
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
    public void AddItem(BasicItem item, int amount = 0)
    {
        item.SetAmount(amount);
        item.Init(gameManager);
        ItemList.Add(item as UsableItem);

        FreshButton();
        if (gameManager.ShopPageController.buySeedPage.gameObject.activeInHierarchy == true)
        {
            gameManager.ShopPageController.buySeedPage.FreshButton();
        }
    }
    public void DecreaseItem(int id, int amount)
    {
        int index = FindItemIndexById(id);
        BasicItem item;
        if (index >= 0)
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
        if(NowTool == null)
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

    public void OnSwitchToCursor()
    {
        if(NowTool == null)
        {
            return;
        }
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (NowTool != null && ButtonList[i].itemId == NowTool.Id && ButtonList[i].gameObject.activeInHierarchy == true)
            {
                ButtonList[i].ChangeStateToIdle();
            }
        }
        CursorSwitchButton.ChangeStateToSelected();
        gameManager.GridCursorController.ChangeCursorToPointer();
        NowTool = null;
    }
    public void OnCursorPointEnter()
    {
        Vector3[] corners = new Vector3[4];
        CursorSwitchButton.GetRectTransform().GetWorldCorners(corners);
        Vector3 leftTop = corners[1];
        Vector3 rightBottom = corners[2];

        // 上侧侧边缘的中点（世界坐标）
        Vector3 upCenter = (rightBottom + leftTop) / 2f;

        string str = "鼠标，在点击地面时不会进行操作，以避免误触。";
        Tip.DoDelayShow(0.4f, upCenter, str, 0.2f);
    }

    public void OnCursorPointerExit()
    {
        Tip.Hide();
    }
}
