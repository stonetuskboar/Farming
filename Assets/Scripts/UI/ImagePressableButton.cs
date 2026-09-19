using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePressableButton : BasicPressableButton
{
    public ImageColorTweenObj image;
    public override void ChangeStateToIdle()
    {
        base.ChangeStateToIdle();
        image.NormalTween(normalTime);
    }
    public override void ChangeStateToHover()
    {
        base.ChangeStateToHover();
        image.HoverTween(hoverTime);
    }
    public override void ChangeStateToSelected()
    {
        base.ChangeStateToSelected();
        image.HoverTween(hoverTime);
    }
    public override void ChangeStateToDisable()
    {
        state = State.disable;
        if (GetCanvasGroup() != null)
        {
            DoAlphaTween(0.5f, normalTime);
            image.NormalTween(normalTime);
        }
        else
        {
            image.DoAlphaTween(image.color.a / 2, normalTime);
        }
        if (HoverSize != Vector2.one)
        {
            DoSizeTween(OgSize, normalTime + 0.1f, type: EaseType.M3Spring);
        }
    }

    public override void ChangeStateToDisableImmediately()
    {
        state = State.disable;
        if (GetCanvasGroup() != null)
        {
            SetCanvasGroupAlpha(0.5f);
            image.SetToNormal();
        }
        else
        {
            image.SetAlpha(image.color.a / 2);
        }
        SetSize(OgSize);
    }
}