using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicPopWindow : BasicUIView
{
    public State nowState;
    public enum State
    {
        Hide,
        Showing,
    }
    private int showId = 0;
    public RectTransform RectTransformByVisibleJudge;
    protected override void Awake()
    {
        base.Awake();
        if (nowState == State.Hide)
        {
            Hide(0f);
        }
        else
        {
            Show(0f);
        }
    }
    public override void GetRectMinMax(out Vector3 minCard, out Vector3 maxCard, bool Isoverlay = false)
    {
        GetRectMinMax(RectTransformByVisibleJudge, out minCard, out maxCard, Isoverlay);
    }
    public int Show(Vector3 worldPosition, float ShowTweenTime = 0.2f)
    {
        showId++;
        SetCanvasGroupAlpha(1f);
        SetCanvasGroupBlockable();
        nowState = State.Showing;
        StopEveryTween();
        transform.localScale = 1f * Vector3.one;
        transform.position = worldPosition;
        LimitRectVisible(new RectOffset(20,20, 20, 20));
        transform.localRotation = Quaternion.Euler(60, -45, 0);
        transform.localScale = 0.8f * Vector3.one;
        DoLocalRotationTween(Quaternion.identity, ShowTweenTime, type: EaseType.EaseOutBack);
        DoLocalScaleTween(Vector3.one, ShowTweenTime, type: EaseType.EaseOutBack);
        return showId;
    }
    public int Show(float ShowTweenTime = 0.2f)
    {
        return Show(transform.position, ShowTweenTime);
    }
    public void ShowThenHide(float showTweenTime = 0.3f, float HideTime = 1.2f)
    {
        int id = Show( showTweenTime);
        DoDelay(HideTime, () =>
        {
            Hide(id);
        });
    }
    public void ShowThenHide(Vector3 worldPosition,float showTweenTime = 0.3f, float HideTime = 1f)
    {
        int id = Show(worldPosition, showTweenTime);
        DoDelay(HideTime, () =>
        {
            Hide(id);
        });
    }
    public void Hide(int id)
    {
        if (id == showId)
        {
            Hide();
        }
    }
    public void Hide(float hideTime = 0.15f)
    {
        nowState = State.Hide;
        SetCanvasGroupUnblock();
        DoLocalScaleTween(Vector3.one * 0.5f, hideTime, type: EaseType.EaseInOutBack);
        DoAlphaTween(0f, hideTime);
    }

    public bool IsShowing()
    {
        if (nowState == State.Showing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
