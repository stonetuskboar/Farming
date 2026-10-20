using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPageController : MonoBehaviour
{
    private bool IsShow = false;
    public Image CoverImage;
    public BasicUIView uiView;
    public ItemSellPage itemSellPage;
    public BuySeedPage buySeedPage;
    public ShopPageButton nowSelectedButton;
    public GameManager gameManager;
    protected void Awake()
    {
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
        uiView.SetCanvasGroupAlpha(1f);
        uiView.SetCanvasGroupBlockable();
        uiView.SetLocalRotation( Quaternion.Euler(30, -25, 0));
        uiView.SetLocalScale(0.8f * Vector3.one);
        uiView.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        uiView.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
        uiView.DoAlphaTween(1f, 0.3f);
    }
    public void Hide()
    {
        gameManager.GridCursorController.BackToNormalSprite();
        IsShow = false;
        uiView.DoLocalScaleTween(Vector3.one * 0.6f, 0.2f, type: EaseType.EaseInOutBack);
        uiView.SetCanvasGroupUnblock();
        uiView.DoAlphaTween(0f, 0.2f, Unactive);

    }
    public void Unactive()
    {
        gameObject.SetActive(false);
    }
}
