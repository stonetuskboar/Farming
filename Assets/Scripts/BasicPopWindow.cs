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
    public RectOffset offSet;
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
        LimitRectVisible(offSet);
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
    public void ShowWithTime(float ShowTweenTime = 0.2f)
    {
        Show(transform.position, ShowTweenTime);
    }
    public void DoDelayShow(float delayTime, Vector3 worldPosition, float ShowTweenTime = 0.2f)
    {
        StartCoroutine(DelayShow(delayTime, worldPosition, ShowTweenTime));
    }
    public void DoDelayShow(float delayTime, float ShowTweenTime = 0.2f)
    {
        StartCoroutine(DelayShow(delayTime, transform.position,ShowTweenTime));
    }
    public IEnumerator DelayShow(float delayTime, Vector3 worldPosition, float ShowTweenTime)
    {
        int id = showId;
        float nowTime = 0;
        nowTime += GetUnScaledTimeDelta();
        while (id == showId && nowTime < delayTime)
        {
            yield return null;
            nowTime += GetUnScaledTimeDelta();
        }
        if (id == showId)
        {
            Show(worldPosition, ShowTweenTime);
        }
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
        showId++;
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
