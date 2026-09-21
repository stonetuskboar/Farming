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
    private Vector2 OgPosition;
    public Vector2 HoverPositionDelta;
    protected override void Awake()
    {
        base.Awake();
        OgPosition = rectTransform.anchoredPosition;
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
        base.ChangeStateToIdle();
        DoAnchoredPositionTween(OgPosition, normalTime, type: EaseType.M3Spring);
    }
    public override void ChangeStateToIdleImmediately()
    {
        base.ChangeStateToIdleImmediately();
        SetAnchoredPosition(OgPosition);
    }
    public override void ChangeStateToHover()
    {
        base.ChangeStateToHover();
        DoAnchoredPositionTween(OgPosition + HoverPositionDelta /3, hoverTime, type: EaseType.M3Spring);
    }
    public override void ChangeStateToSelected()
    {
        base.ChangeStateToSelected();
        DoAnchoredPositionTween(OgPosition + HoverPositionDelta, hoverTime, type: EaseType.M3Spring);
    }
    public override void ChangeStateToSelectedImmediately()
    {
        base.ChangeStateToSelectedImmediately();
        SetAnchoredPosition(OgPosition + HoverPositionDelta);
    }
    public override void ChangeStateToDisable()
    {
        base.ChangeStateToDisable();
        DoAnchoredPositionTween(OgPosition, normalTime, type: EaseType.M3Spring);
    }

    public override void ChangeStateToDisableImmediately()
    {
        base.ChangeStateToDisableImmediately();
        SetAnchoredPosition(OgPosition);
    }
}
