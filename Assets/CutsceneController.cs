using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour,  IPointerClickHandler
{
    public CircleTransition transition;
    public List<Sprite> SpriteList;
    private int NowIndex = 0;
    public Image Image;
    public BasicUIView UIView;

    public Action EndAction;
    private Vector2 OgAnchorPosition;
    private bool IsClickable = false;

    public void Start()
    {
        OgAnchorPosition = UIView.GetAnchoredPosition();
    }
    public void StartShow()
    {
        NowIndex = 0;
        IsClickable = false;
        transition.StartClose(1.2f,InitShowSprite);
        UIView.SetCanvasGroupAlpha(0f);
    }

    public void InitShowSprite()
    {
        IsClickable = false;
        Image.sprite = SpriteList[NowIndex];
        Image.SetNativeSize();
        UIView.SetCanvasGroupAlpha(0f);
        UIView.DoAlphaTween(1f, 0.8f);
        UIView.DoDelay(0.5f, SetClickable);
    }
    public void ShowSprite()
    {
        IsClickable = false;
        Image.sprite = SpriteList[NowIndex];
        Image.SetNativeSize();
        UIView.SetCanvasGroupAlpha(0f);
        UIView.DoAlphaTween(1f, 0.4f, SetClickable);
        UIView.SetAnchoredPosition(OgAnchorPosition + new Vector2(0, -200));
        UIView.DoAnchoredPositionTween(OgAnchorPosition, 0.5f, type:EaseType.M3Spring);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsClickable == true)
        {
            NowIndex++;
            if(NowIndex < SpriteList.Count)
            {
                ShowSprite();
            }
            else
            {
                UIView.DoAlphaTween(0f, 0.4f);
                transition.StartOpen(0.8f);
                EndAction?.Invoke();
            }
        }

    }

    public void SetClickable()
    {
        IsClickable = true;
    }
}
