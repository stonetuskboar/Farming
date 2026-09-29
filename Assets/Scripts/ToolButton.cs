using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : ImagePressableButton
{
    public Image ToolIcon;
    private HotBarController ToolController;
    [NonSerialized]
    public int index;
    public Sprite DefaultSprite;
    public Sprite SelectedSprite;
    protected override void Awake()
    {
        base.Awake();
        PointerClicked += OnClick;
    }

    public void Init(HotBarController controller)
    {
        ToolController = controller;
    }
    public void Set(int i)
    {
        index = i;
    }

    public void OnClick()
    {
        ToolController.SelectNowTool(this);
    }
    public override void ChangeStateToIdle()
    {
        image.sprite = DefaultSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToIdle();
    }
    public override void ChangeStateToIdleImmediately()
    {
        image.sprite = DefaultSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToIdleImmediately();
    }
    public override void ChangeStateToHover()
    {
        image.sprite = DefaultSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToHover();
    }
    public override void ChangeStateToSelected()
    {
        image.sprite = SelectedSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToSelected();
    }
    public override void ChangeStateToSelectedImmediately()
    {
        image.sprite = SelectedSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToSelectedImmediately();
    }
    
    public override void ChangeStateToDisable()
    {
        image.sprite = DefaultSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToDisable();
    }

    public override void ChangeStateToDisableImmediately()
    {
        image.sprite = DefaultSprite;
        image.image.SetNativeSize();
        ResetOgSize();
        base.ChangeStateToDisableImmediately();
    }
}
