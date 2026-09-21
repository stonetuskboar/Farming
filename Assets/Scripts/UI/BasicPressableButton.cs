using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BasicPressableButton : BasicUIView, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string ClickedSoundName;
    public event Action PointerEntered;
    public event Action PointerClicked;
    [SerializeField]
    protected UnityEvent PointerClickedEvent;
    public event Action PointerExited;
    protected State state = State.idle;
    public enum State
    {
        idle,
        hover,
        disable,
        selected,
    }
    public float hoverTime = 0.2f;
    public float normalTime = 0.3f;

    protected Vector2 OgSize;
    public Vector2 HoverSize = new Vector2(1.2f, 1f);

    protected override void Awake()
    {
        base.Awake();
        OgSize = rectTransform.sizeDelta;
    }

    public State GetState()
    {
        return state; 
    }
    public Vector2 GetOgSize()
    {
        return OgSize; 
    }
    public void SetOgSize(Vector2 size)
    {
        OgSize = size;
    }
    protected void OnPointerEntered()
    {
        PointerEntered?.Invoke();
    }

    protected void OnPointerExited()
    {
        PointerExited?.Invoke();
    }
    protected void OnPointerClicked()
    {
        PointerClicked?.Invoke();
        PointerClickedEvent?.Invoke();
    }

    public virtual void ChangeStateToIdle()
    {
        state = State.idle;
        if(HoverSize != Vector2.one)
        {
            DoSizeTween(OgSize, normalTime + 0.1f, type: EaseType.M3Spring);
        }
    }
    public virtual void ChangeStateToIdleImmediately()
    {
        state = State.idle;
        if (HoverSize != Vector2.one)
        {
            SetSize(OgSize);
        }
    }
    public virtual void ChangeStateToHover()
    {
        state = State.hover;
        if (HoverSize != Vector2.one)
        {
            DoSizeTween(HoverSize * OgSize, hoverTime + 0.1f, type: EaseType.M3Spring);
        }
    }
    public virtual void ChangeStateToSelected()
    {
        state = State.selected;
        if (HoverSize != Vector2.one)
        {
            DoSizeTween(OgSize, normalTime + 0.1f, type: EaseType.M3Spring);
        }
    }
    public virtual void ChangeStateToSelectedImmediately()
    {
        state = State.selected;
        if (HoverSize != Vector2.one)
        {
            SetSize(OgSize);
        }
    }
    public virtual void ChangeStateToDisable()
    {
        state = State.disable;
        if(GetCanvasGroup() != null )
        {
            DoAlphaTween(0.5f, normalTime);
        }
        if (HoverSize != Vector2.one)
        {
            DoSizeTween(OgSize, normalTime + 0.1f, type: EaseType.M3Spring);
        }
    }
    public virtual void ChangeStateToDisableImmediately()
    {
        state = State.disable;
        if(GetCanvasGroup() != null )
        {
            SetCanvasGroupAlpha(0.5f);
        }
        SetSize(OgSize);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (state != State.disable)
        {
            OnPointerClicked();
            AudioManager.instance.playSfxSound(ClickedSoundName);
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (state == State.idle)
        {
            ChangeStateToHover();
            OnPointerEntered();
        }
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (state == State.hover)
        {
            ChangeStateToIdle();
            OnPointerExited();
        }
    }


    public void Show(float showTime = 0.2f)
    {
        SetCanvasGroupAlpha(1f);
        transform.localRotation = Quaternion.Euler(-30, 25, 0);
        transform.localScale = 0.8f * Vector3.one;
        DoLocalRotationTween(Quaternion.identity, showTime, type: EaseType.EaseOutBack);
        DoLocalScaleTween(Vector3.one, showTime, type: EaseType.EaseOutBack);
        SetCanvasGroupBlockable();
    }
    public void Hide(float hideTime = 0.15f)
    {
        DoLocalScaleTween(Vector3.one * 0.5f, hideTime, type: EaseType.EaseInOutBack);
        DoAlphaTween(0f, hideTime);
        SetCanvasGroupUnblock();
    }

    public void SetToHide()
    {
        SetCanvasGroupAlpha(0f);
        SetLocalScale(Vector3.one * 0.5f);
        SetCanvasGroupUnblock();
    }
}
