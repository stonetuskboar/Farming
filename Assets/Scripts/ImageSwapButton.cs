using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSwapButton : BasicPressableButton
{
    public Image image;
    public Sprite DefaultSprite;
    public Sprite HoverSprite;
    public Sprite SelectedSprite;
    public Sprite PressedSprite;
    [NonSerialized]
    public float Scale = 1;
    protected override void Awake()
    {
        base.Awake();
        image.alphaHitTestMinimumThreshold = 0.1f;
    }
    public virtual void ResetOgSize()
    {
        Vector2 nativeSize = Scale * image.sprite.rect.size / image.pixelsPerUnit;
        SetOgSize(nativeSize);
    }
    public void OnValidate()
    {
        image.sprite = DefaultSprite;
    }
    public override void ChangeStateToIdle()
    {
        image.sprite = DefaultSprite;
        ResetOgSize();
        base.ChangeStateToIdle();
    }
    public override void ChangeStateToIdleImmediately()
    {
        image.sprite = DefaultSprite;
        ResetOgSize();
        base.ChangeStateToIdleImmediately();
    }
    public override void ChangeStateToHover()
    {
        image.sprite = HoverSprite;
        ResetOgSize();
        base.ChangeStateToHover();
    }
    public override void ChangeStateToSelected()
    {
        image.sprite = SelectedSprite;
        ResetOgSize();
        base.ChangeStateToSelected();
    }
    public override void ChangeStateToSelectedImmediately()
    {
        image.sprite = SelectedSprite;
        ResetOgSize();
        base.ChangeStateToSelectedImmediately();
    }

    public override void ChangeStateToDisable()
    {
        image.sprite = DefaultSprite;
        ResetOgSize();
        base.ChangeStateToDisable();
    }

    public override void ChangeStateToDisableImmediately()
    {
        image.sprite = DefaultSprite;
        ResetOgSize();
        base.ChangeStateToDisableImmediately();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDown();
        if (state == State.hover)
        {
            image.sprite = PressedSprite;
            ResetOgSize();
            DoSizeTween(PressedSize * OgSize, hoverTime + 0.1f, type: EaseType.M3Spring);
        }
        else if (state == State.selected)
        {
            image.sprite = PressedSprite;
            ResetOgSize();
            DoSizeTween(PressedSize * OgSize, hoverTime + 0.1f, type: EaseType.M3Spring);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUp();
        if (state == State.hover)
        {
            image.sprite = HoverSprite;
            ResetOgSize();
            DoSizeTween(HoverSize * OgSize, hoverTime, type: EaseType.M3Spring);
        }else if(state == State.selected)
        {
            image.sprite = SelectedSprite;
            ResetOgSize();
            DoSizeTween(OgSize, normalTime + 0.1f, type: EaseType.M3Spring);
        }
    }
}
