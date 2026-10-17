using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPageController : BasicUIView
{
    private bool IsShow = false;
    public ItemSellPage itemSellPage;
    public BuySeedPage buySeedPage;
    public ShopPageButton nowSelectedButton;
    public GameManager gameManager;
    protected override void Awake()
    {
        base.Awake();
        if(gameObject.activeInHierarchy == true)
        {
            IsShow = true;
        }
        else
        {
            IsShow = false;
        }
    }

    public void Start()
    {
        if (nowSelectedButton != null)
        {
            nowSelectedButton.SelectThisImmediately();
        }
    }

    public void SwitchShow()
    {
        if (IsShow == true)
        {
            GameDataManager.Instance.PlayAudio("ShopClose");
            Hide();
        }
        else
        {
            GameDataManager.Instance.PlayAudio("ShopOpen");
            Show();
        }
    }
    public void Show() 
    {
        gameManager.GridCursorController.ChangeCursorToPointer();
        IsShow = true;
        gameObject.SetActive(true);
        SetCanvasGroupAlpha(1f);
        SetCanvasGroupBlockable();
        transform.localRotation = Quaternion.Euler(30, -25, 0);
        transform.localScale = 0.8f * Vector3.one;
        DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
        DoAlphaTween(1f, 0.3f);
    }
    public void Hide()
    {
        gameManager.GridCursorController.BackToNormalSprite();
        IsShow = false;
        DoLocalScaleTween(Vector3.one * 0.6f, 0.2f, type: EaseType.EaseInOutBack);
        SetCanvasGroupUnblock();
        DoAlphaTween(0f, 0.2f, Unactive);

    }
    public void Unactive()
    {
        gameObject.SetActive(false);
    }
}
