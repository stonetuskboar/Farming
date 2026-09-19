using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BasicTip : BasicUIView
{
    public TextMeshProUGUI ReminderText;
    public State nowState;
    public enum State
    {
        Hide,
        Showing,
    }
    private int showId = 0;
    public RectTransform RectTransformByVisibleJudge;
    public LayoutElement LayoutElement;
    private float DefaultPreferWidth;
    protected override void Awake()
    {
        base.Awake();
        DefaultPreferWidth = LayoutElement.preferredWidth;
        if(nowState == State.Hide)
        {
            Hide(0f);
        }
    }
    public override void GetRectMinMax(out Vector3 minCard, out Vector3 maxCard , bool Isoverlay = false)
    {
        GetRectMinMax(RectTransformByVisibleJudge, out minCard, out maxCard, Isoverlay);
    }
    public int Show(Vector3 worldPosition, string text = null, float ShowTweenTime = 0.2f)
    {
        showId++;
        SetCanvasGroupAlpha(1f);
        nowState = State.Showing;
        StopEveryTween();
        if (text != null)
        {
            ReminderText.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        transform.localScale = 1f * Vector3.one;
        transform.position = worldPosition;
        LimitRectVisible(new RectOffset(0,0,120, 120));

        if(rectTransform.rect.height > LayoutElement.preferredWidth)
        {
            transform.localRotation = Quaternion.Euler(30, -25, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(60, -45, 0);
        }
        transform.localScale = 0.8f * Vector3.one;
        DoLocalRotationTween(Quaternion.identity, ShowTweenTime, type: EaseType.EaseOutBack);
        DoLocalScaleTween(Vector3.one, ShowTweenTime, type: EaseType.EaseOutBack);
        return showId;
    }
    public int Show( string text = null, float ShowTweenTime = 0.2f)
    {
        return Show(transform.position, text, ShowTweenTime);
    }
    public void ShowThenHide(string text = null, float showTweenTime = 0.3f, float HideTime = 1.2f)
    {
        int id = Show(text, showTweenTime);
        DoDelay(HideTime, () =>
        {
            Hide(id);
        });
    }
    public void ShowThenHide(Vector3 worldPosition, string text = null, float showTweenTime = 0.3f, float HideTime = 1f)
    {
        int id = Show(worldPosition, text, showTweenTime);
        DoDelay(HideTime, () =>
        { 
            Hide(id);
        });
    }
    public void Hide(int id)
    {
        if(id == showId)
        {
            Hide();
        }
    }
    public void Hide(float hideTime = 0.15f)
    {
        nowState = State.Hide;
        DoLocalScaleTween(Vector3.one * 0.5f, hideTime, type: EaseType.EaseInOutBack);
        DoAlphaTween(0f, hideTime);
    }

    public void SetPreferedWidth(int PreferedWidth)
    {
        if (PreferedWidth <= 0)
        {
            LayoutElement.preferredWidth = DefaultPreferWidth;
        }
        else
        {
            LayoutElement.preferredWidth = PreferedWidth;
        }
    }

    public bool IsShowing()
    {
        if(nowState == State.Showing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetText(string text)
    {
        ReminderText.text = text;
    }
}
