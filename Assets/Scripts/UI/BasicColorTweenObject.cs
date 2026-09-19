using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasicColorTweenObject : BasicTweenObject
{
    protected RectTransform rectTransform;
    public Color normalColor;
    public Color hoverColor;
    protected Graphic graphic = null;
    protected int ColorId = 0;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();
        SetToNormal();
    }

    public bool RaycastTarget
    {
        get
        {
            return graphic.raycastTarget;
        }
        set
        {
            graphic.raycastTarget = value;
        }
    }

    public Color color
    {
        get
        {
            return graphic.color;
        }
        set
        {
            graphic.color = value;
        }
    }
    public virtual void SetToNormal()
    {
        SetColor(normalColor);
    }
    public virtual void SetToHover()
    {
        SetColor(hoverColor);
    }

    public virtual void HoverTween(float time = 0.2f)
    {
        DoColorTween(hoverColor, time);
    }
    public virtual void NormalTween(float time = 0.3f)
    {
        DoColorTween(normalColor, time);
    }
    public void SetColor(Color color)
    {
        ColorId++;
        graphic.color = color;
    }
    public void InitAlpha(float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        SetColor(c);
    }
    public virtual void SetAlpha(float targetAlpha)
    {
        if(graphic == null)
        {
            graphic = GetComponent<Graphic>();
        }
        Color c = graphic.color;
        c.a = targetAlpha;
        SetColor(c);
    }
    public virtual void DoColorTween(Color TargetColor, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (graphic == null)
        {
            graphic = GetComponent<Graphic>();
        }
        if (time <= 0 || gameObject.activeInHierarchy == false)
        {
            SetColor(TargetColor);
            callBack?.Invoke();
            return;
        }
        else
        {
            StartCoroutine(ColorTween(graphic, TargetColor, time, callBack, type, IsCallbackRequiredOnComplete));
        }
    }
    public virtual void DoAlphaTween(float targetAlpha, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (graphic == null)
        {
            Debug.LogError("graphic没有初始化");
            return;
        }
        else
        {
            if (time <= 0 || gameObject.activeInHierarchy == false)
            {
                SetAlpha(targetAlpha);
                callBack?.Invoke();
                return;
            }
            else
            {
                StartCoroutine(ColorTween(graphic, new Color(graphic.color.r, graphic.color.g, graphic.color.b, targetAlpha)
                                , time, callBack, type, IsCallbackRequiredOnComplete));
            }
        }
    }


    IEnumerator ColorTween(Graphic graphic,  Color targetColor, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        ColorId++;
        int id = ColorId;
        Color startColor = graphic.color;

        if (targetColor.a > 0 || graphic.color.a > 0)
        {
            graphic.enabled = true;
        }

        float nowTime = 0;
		nowTime += GetTimeDelta();
        Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
        while (nowTime < time && id == ColorId)
        {
			graphic.color = Color.LerpUnclamped(startColor, targetColor , EaseFunc(nowTime / time));
            yield return null;
			nowTime += GetTimeDelta();
		}

        if (id == ColorId)
        {
            if (IsToTarget(type) == true)
            {
                graphic.color = targetColor;
            }
            else
            {
                graphic.color = startColor;
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
            //被中断
        }
    }
}
