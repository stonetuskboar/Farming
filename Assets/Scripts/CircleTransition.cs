using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CircleTransition : MonoBehaviour
{
    public Image transitionImage;

    private Material material;

    private static readonly int RadiusID = Shader.PropertyToID("_Radius");

    private void Awake()
    {
        material = transitionImage.material;
    }
    public void StartClose(float duration, Action callback = null)
    {
        StartCoroutine(Close(duration,callback));
    }
    public void StartOpen(float duration, Action callback = null)
    {
        StartCoroutine(Open(duration, callback));
    }
    protected IEnumerator Close(float duration, Action callback)
    {
        transitionImage.raycastTarget = true;
        float start = 0f;
        float end = GetMaxRadius();

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // 平滑动画
            t = Mathf.SmoothStep(0, 1, t);

            material.SetFloat(
                RadiusID,
                Mathf.Lerp(start, end, t)
            );

            yield return null;
        }
        callback?.Invoke();
        material.SetFloat(RadiusID, end);
    }

    protected IEnumerator Open(float duration, Action callback = null)
    {
        transitionImage.raycastTarget = false;
        float start = GetMaxRadius();
        float end = 0f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            t = Mathf.SmoothStep(0, 1, t);

            material.SetFloat(
                RadiusID,
                Mathf.Lerp(start, end, t)
            );

            yield return null;
        }
        callback?.Invoke();
        material.SetFloat(RadiusID, 0);
    }

    private float GetMaxRadius()
    {
        float aspect = (float)Screen.width / Screen.height;

        return Mathf.Sqrt(
            aspect * aspect + 1
        ) * 0.5f;
    }
}