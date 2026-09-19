using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BasicUIView : BasicTweenObject
{
    //canvasGroup不一定挂载在这个gameobj上。
    [SerializeField]
    private CanvasGroup canvasGroup;
    /// <summary>
    /// 只用来处理UI组件，一般使用transform(以后出现问题再改),可能在更改屏幕分辨率，Camera大小，canvas模式后出错
    /// </summary>
    protected RectTransform rectTransform;
    private int AlphaId = 0;
    private int SizeTweenId = 0;
    //有时候挂载在自身上，有时在父对象上，在LimitPosition里会用到
    protected Canvas canvas;

    protected virtual void Awake()
    {
        if(transform is RectTransform)
        {
            rectTransform = (RectTransform)transform;
        }

        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
    }
    public RectTransform GetRectTransform()
    {
        if(rectTransform == null)
        {
            rectTransform = (RectTransform)transform;
        }
        return rectTransform;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
    public CanvasGroup GetCanvasGroup()
    {
        return canvasGroup;
    }
    public override void StopEveryTween()
    {
        base.StopEveryTween();
        AlphaId++;
        SizeTweenId++;
    }
    public void StopAlphaTween()
    {
        AlphaId++;
    }
    public void SetCanvasGroupAlpha(float alpha)
    {
        AlphaId++;
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.alpha = alpha;
    }
    public void SetSize(Vector2 Size)
    {
        SizeTweenId++;
        SetRealSize(Size);
    }
    public Vector2 GetAnchoredPosition()
    {
        return GetRectTransform().anchoredPosition;
    }
    public void SetAnchoredPosition(Vector2 p)
    {
        PositionTweenId++;
        GetRectTransform().anchoredPosition = p;
    }
    public void AddAnchoredPosition(Vector2 p)
    {
        PositionTweenId++;
        GetRectTransform().anchoredPosition += p;
    }
    public virtual void DoAnchoredPositionTween(Vector2 anchoredPosition, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (gameObject.activeInHierarchy == true)
        {
            StartCoroutine(AnchoredPositionTween(anchoredPosition, time, callBack, type, IsCallbackRequiredOnComplete));
        }
        else
        {
            SetLocalPosition(anchoredPosition);
        }
    }
    IEnumerator AnchoredPositionTween(Vector2 targetPosition, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        PositionTweenId++;
        int id = PositionTweenId;
        Vector2 startPositon = GetRectTransform().anchoredPosition;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == PositionTweenId)
            {
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPositon, targetPosition, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector2> V2ShakeFunc = GetVector2ShakeFuncByType(type);
            Vector2 Delta = targetPosition - startPositon;
            while (nowTime < time && id == PositionTweenId)
            {
                rectTransform.anchoredPosition = startPositon + Vector2.Scale(V2ShakeFunc(nowTime / time), Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }


        if (id == PositionTweenId)
        {
            if (IsToTarget(type) == true)
            {
                rectTransform.anchoredPosition = targetPosition;
            }
            else
            {
                rectTransform.anchoredPosition = startPositon;
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
    public void DoAlphaTween(float targetAlpha, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("canvasGroup没有初始化");
            return;
        }
        else
        {
            StartCoroutine(AlphaTween(targetAlpha, time, callBack, type, IsCallbackRequiredOnComplete));
        }
    }


    IEnumerator AlphaTween(float targetAlpha, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        AlphaId++;
        int id = AlphaId;

        float startAlpha = canvasGroup.alpha;
        Func<float, float> EaseFunc = GetEaseFuncByType(type, time);

        float nowTime = 0;
		nowTime += GetTimeDelta();

        while (nowTime < time && id == AlphaId)
        {
            canvasGroup.alpha = startAlpha + (targetAlpha - startAlpha) * EaseFunc(nowTime / time);
            yield return null;
			nowTime += GetTimeDelta();
		}

        if (id == AlphaId)
        {
            if (IsToTarget(type) == true)
            {
                canvasGroup.alpha = targetAlpha;
            }
            else
            {
                canvasGroup.alpha = startAlpha;
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
    public void DoSizeTween(Vector2 TargetSize, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
            StartCoroutine(SizeTween(TargetSize, time,  type, callBack, IsCallbackRequiredOnComplete));
    }
    IEnumerator SizeTween(Vector2 TargetSize, float time, EaseType type, Action callback, bool IsCallbackRequiredOnComplete)
    {
        SizeTweenId++;
        int id = SizeTweenId;

        Vector2 startSize = rectTransform.rect.size;

        float nowTime = 0;
		nowTime += GetTimeDelta();

		if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == SizeTweenId)
            {
                SetRealSize( Vector2.LerpUnclamped(startSize, TargetSize, EaseFunc(nowTime / time)));
                yield return null;
				nowTime += GetTimeDelta();
			}
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector2 Delta = TargetSize - startSize;
            while (nowTime < time && id == SizeTweenId)
            {
                SetRealSize( startSize + Vector2.Scale(V3ShakeFunc(nowTime / time), Delta));
                yield return null;
				nowTime += GetTimeDelta();
			}
        }

        if (id == SizeTweenId)
        {
            if (IsToTarget(type) == true)
            {
                SetRealSize(TargetSize );
            }
            else
            {
                SetRealSize( startSize );
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
        }
    }

    private void SetRealSize(Vector2 size)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }
    public void SetCanvasGroupUnblock()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void SetCanvasGroupBlockable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.Log("没有找到CanvasGroup然而却试图设置它的交互性");
        }
    }
    public float GetHeight()
    {
        return rectTransform.rect.height;
    }
    public Vector3 LimitRectVisible()
    {
        return LimitRectVisible(new RectOffset(0, 0, 0, 0));
    }
    public Vector3 LimitRectVisible(RectOffset margin)
    {
        margin = new RectOffset((int)(margin.left * canvas.scaleFactor)
            , (int)(margin.right * canvas.scaleFactor)
            , (int)(margin.bottom * canvas.scaleFactor)
            , (int)(margin.top * canvas.scaleFactor));

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 worldDelta = GetLimitDelta(margin, Vector3.zero, new Vector3(Screen.width, Screen.height, 0) ,IsOverlay: true);
            //老代码，如果新的有bug，使用老代码试试
            //rectTransform.anchoredPosition += new Vector2(worldDelta.x, worldDelta.y);
            SetWorldPosition(transform.position + new Vector3(worldDelta.x, worldDelta.y ,0));
            return worldDelta;
        }
        else
        {
            Vector2 screenDelta = GetLimitDelta(margin, Vector3.zero, new Vector3(Screen.width, Screen.height, 0));
            Camera camera = canvas.worldCamera;
            Vector3 newPos = camera.WorldToScreenPoint(transform.position);
            newPos += new Vector3(screenDelta.x,screenDelta.y);
            SetWorldPosition(camera.ScreenToWorldPoint(newPos));
            return screenDelta;
        }
    }
    public Vector2 GetLimitDelta(RectOffset margin , Vector3 min,Vector3 max ,bool IsOverlay = false)
    {
        Vector3 minScreenPos;
        Vector3 maxScreenPos;
        GetRectMinMax(out minScreenPos, out maxScreenPos,IsOverlay);
        minScreenPos.x -= margin.left;
        minScreenPos.y -= margin.bottom;
        maxScreenPos.x += margin.right;
        maxScreenPos.y += margin.top;

        Vector2 delta = Vector3.zero;
        if (minScreenPos.x < min.x) //卡的左下角不能小于摄像机左下角
        {
            delta.x  = min.x - minScreenPos.x;
        }
        if (minScreenPos.y < (min.y)) //卡的左下角不能小于摄像机左下角
        {
            delta.y = min.y - minScreenPos.y;
        }
        if (maxScreenPos.x > max.x) //卡的右上角不能大于摄像机右上角
        {
            delta.x = max.x - maxScreenPos.x;
        }
        if (maxScreenPos.y > max.y) //卡的右上角不能大于摄像机右上角
        {
            delta.y = max.y - maxScreenPos.y;
        }
        return delta;
    }
    public virtual void GetRectMinMax(out Vector3 minVec, out Vector3 maxVec, bool IsOverlay = false)
    {
        GetRectMinMax(rectTransform, out minVec, out maxVec, IsOverlay);
    }
    public void GetRectMinMax(RectTransform JudgedRectTransform, out Vector3 minVec, out Vector3 maxVec , bool IsOverlay = false)
    {
        Vector3[] worldCorners = new Vector3[4];
        JudgedRectTransform.GetWorldCorners(worldCorners);
        if (IsOverlay == false)
        { 
            for (int i = 0; i < worldCorners.Length; i++)
            {
                worldCorners[i] = canvas.worldCamera.WorldToScreenPoint(worldCorners[i]);
            }
        }
        GetCornerMinMax(worldCorners, out minVec, out maxVec);
    }

    public void GetCornerMinMax(Vector3[] worldCorners,  out Vector3 minVec, out Vector3 maxVec)
    {
        float xMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (xMin > worldCorners[i].x)
            {
                xMin = worldCorners[i].x;
            }
        }
        float yMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (yMin > worldCorners[i].y)
            {
                yMin = worldCorners[i].y;
            }
        }
        float ZMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (ZMin > worldCorners[i].z)
            {
                ZMin = worldCorners[i].z;
            }
        }
        minVec = new Vector3(xMin, yMin, ZMin);


        float xMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (xMax < worldCorners[i].x)
            {
                xMax = worldCorners[i].x;
            }
        }
        float yMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (yMax < worldCorners[i].y)
            {
                yMax = worldCorners[i].y;
            }
        }
        float zMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (zMax < worldCorners[i].z)
            {
                zMax = worldCorners[i].z;
            }
        }
        maxVec = new Vector3(xMax, yMax, zMax);
    }

}
