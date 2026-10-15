
using UnityEngine;
using UnityEngine.UI;

public class ShopPageButton : ImageSwapButton
{
    public GameObject Page;
    public ShopPageController shopPageController;
    protected override void Awake()
    {
        base.Awake();
        PointerClicked += PointerClick;
    }

    public void PointerClick()
    {
        if(shopPageController.nowSelectedButton != this)
        {
            shopPageController.nowSelectedButton.UnSelectThis();
            shopPageController.nowSelectedButton = this;
            this.SelectThis();
        }
    }
    public void SelectThis()
    {
        ChangeStateToSelected();
        Page.SetActive(true);
    }
    public void SelectThisImmediately()
    {
        ChangeStateToSelectedImmediately();
        Page.SetActive(true);
    }
    public void UnSelectThis()
    {
        ChangeStateToIdle();
        Page.SetActive(false);
    }
}
