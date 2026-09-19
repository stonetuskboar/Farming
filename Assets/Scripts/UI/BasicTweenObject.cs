using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 需要确保对象上有canvasGroup
/// </summary>
public class BasicTweenObject : MonoBehaviour
{

    protected event Action<Vector3> RotationChanged;

    protected int PositionTweenId = 0;

    private int ScaleTweenId = 0;

    private int LocalRotationId = 0;

    private bool IsTimeScaled = true;

    protected Func<float> GetTimeDelta = GetScaledTimeDelta;

    protected VibrationCustomData vibrationData = new();

    /// <summary>
    /// 如果不是DoTween，而是直接更改坐标，需要使用这个函数，避免异常情况
    /// </summary>
    public virtual void StopEveryTween()
    {
        PositionTweenId++;
        ScaleTweenId++;
        LocalRotationId++;
    }
    public void StopNowScaleTween()
    {
        ScaleTweenId++;
    }
    public void StopNowPositionTween()
    {
        PositionTweenId++;
    }
    public void SetWorldPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.position = p;
    }
    public virtual void SetLocalPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.localPosition = p;
    }
    public virtual void AddLocalPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.localPosition += p;
    }
    public void SetLocalScale(Vector3 scale)
    {
        transform.localScale = scale;
        ScaleTweenId++;
    }
    public void SetLocalRotation(Vector3 rotation)
    {
        transform.localRotation = Quaternion.Euler(rotation);
        RotationChanged?.Invoke(transform.rotation.eulerAngles);
        LocalRotationId = 0;
    }
    public void SetLocalRotation(quaternion quaternion)
    {
        transform.localRotation = quaternion;
        RotationChanged?.Invoke(transform.rotation.eulerAngles);
        LocalRotationId = 0;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public virtual void DoLocalPositionTween(Vector3 position, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if(gameObject.activeInHierarchy == true)
        {
            StartCoroutine(LocalPositionTween(position, time, callBack, type, IsCallbackRequiredOnComplete));
        }
        else
        {
            SetLocalPosition(position);
        }
    }
    public void DoPositionTween(Vector3 position, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(WorldPositionTween(position, time, callBack, type, IsCallbackRequiredOnComplete));
    }

    public void DoLocalScaleTween(Vector3 scale, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(LocalScaleTween(scale, time, callBack , type, IsCallbackRequiredOnComplete));
    }
    public void DoLocalRotationTween(Quaternion quaternion, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(LocalRotationTween(quaternion, time, callBack, type,IsCallbackRequiredOnComplete));
    }
    public void DoLocalRotationTween(Vector3 rotation, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        Quaternion quaternion = Quaternion.Euler(rotation);
        DoLocalRotationTween(quaternion, time, callBack, type, IsCallbackRequiredOnComplete);
    }

    public void DoDelay(float time, Action callBack = null)
    {
        StartCoroutine(Delay(time, callBack));
    }
    IEnumerator Delay(float time, Action callback)
    {
        if (IsTimeScaled == true)
        {
            yield return new WaitForSeconds(time);
        }
        else
        {
            yield return new WaitForSecondsRealtime(time);
        }
        callback?.Invoke();
    }



    IEnumerator LocalPositionTween(Vector3 targetPosition, float time, Action callback , EaseType type, bool IsCallbackRequiredOnComplete)
    {
        PositionTweenId++;
        int id = PositionTweenId;
        Vector3 startPositon = transform.localPosition;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == PositionTweenId)
            {
                transform.localPosition = Vector3.LerpUnclamped(startPositon, targetPosition, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = targetPosition - startPositon;
            while (nowTime < time && id == PositionTweenId)
            {
                transform.localPosition = startPositon + Vector3.Scale( V3ShakeFunc(nowTime / time) , Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }


        if (id == PositionTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localPosition = targetPosition;
            }
            else
            {
                transform.localPosition = startPositon;
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
    IEnumerator WorldPositionTween(Vector3 targetPosition, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        PositionTweenId++;
        int id = PositionTweenId;

        Vector3 startPositon = transform.position;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == PositionTweenId)
            {
                transform.position = Vector3.LerpUnclamped(startPositon, targetPosition, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = targetPosition - startPositon;
            while (nowTime < time && id == PositionTweenId)
            {
                transform.position = startPositon + Vector3.Scale(V3ShakeFunc(nowTime / time), Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == PositionTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = startPositon;
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
    IEnumerator LocalScaleTween(Vector3 TargetScale, float time, Action callback , EaseType type, bool IsCallbackRequiredOnComplete)
    {
        ScaleTweenId++;
        int id = ScaleTweenId;

        Vector3 startScale = transform.localScale;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == ScaleTweenId)
            {
                transform.localScale = Vector3.LerpUnclamped(startScale, TargetScale, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = TargetScale - startScale;
            while (nowTime < time && id == ScaleTweenId)
            {
                transform.localScale = startScale + Vector3.Scale(V3ShakeFunc(nowTime / time), Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == ScaleTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localScale = TargetScale;
            }
            else
            {
                transform.localScale = startScale;
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

    IEnumerator LocalRotationTween(Quaternion targetQuaternion, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        LocalRotationId++;
        int id = LocalRotationId;

        Quaternion StartQuaternion = transform.localRotation;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == LocalRotationId)
            {
                transform.localRotation = Quaternion.SlerpUnclamped(StartQuaternion, targetQuaternion, EaseFunc(nowTime / time));
                RotationChanged?.Invoke(transform.rotation.eulerAngles);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }//很不幸，根据四元数的规则，不能简单的将四个分量单独乘以随机数
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 startEuler = StartQuaternion.eulerAngles;
            Vector3 endEuler = targetQuaternion.eulerAngles;
            while (nowTime < time && id == LocalRotationId)
            {
                Vector3 v3Shake = V3ShakeFunc(nowTime / time);
                Vector3 resultEuler =
                    new Vector3(
                            LerpAngleUnclamped(startEuler.x, endEuler.x, v3Shake.x),
                            LerpAngleUnclamped(startEuler.y, endEuler.y, v3Shake.y),
                            LerpAngleUnclamped(startEuler.z, endEuler.z, v3Shake.z)
                         );
                transform.localRotation = Quaternion.Euler(resultEuler);
                RotationChanged?.Invoke(transform.rotation.eulerAngles);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == LocalRotationId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localRotation = targetQuaternion;
            }
            else
            {
                transform.localRotation = StartQuaternion;
            }
            RotationChanged?.Invoke(transform.rotation.eulerAngles);
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
    /// <summary>
    /// 如果是false，在esc菜单弹出来后依然有动画。如果是true，esc弹出后停止运动
    /// </summary>
    /// <param profs="IsScaled"></param>
    public void SetTimeScaled(bool IsScaled)
    {
        IsTimeScaled = IsScaled;
        if(IsScaled == false)
        {
            GetTimeDelta = GetUnScaledTimeDelta;
        }
        else
        {
            GetTimeDelta = GetScaledTimeDelta;
        }
    }
    public static float GetScaledTimeDelta()
    {
        return Time.deltaTime;
    }
    public static float GetUnScaledTimeDelta()
    { 
        return Time.unscaledDeltaTime; 
    }

    public Func<float, float> GetEaseFuncByType(EaseType type, float time)
    {
        if (type == EaseType.VibrationCustom)
        {
            return VibrationCustom;
        }
        else
        {
            return EasingFunc.GetEaseFuncByType(type, time);
        }
    }

    public Func<float, Vector3> GetVector3ShakeFuncByType(EaseType type)
    {
        if (type == EaseType.VibrationCustom)
        {
            return V3VibrationCustom;
        }else
        {
            return EasingFunc.GetVector3ShakeFuncByType(type);
        }
    }
    public Func<float, Vector2> GetVector2ShakeFuncByType(EaseType type)
    {
        if (type == EaseType.VibrationCustom)
        {
            return V2VibrationCustom;
        }
        else
        {
            return EasingFunc.GetVector2ShakeFuncByType(type);
        }
    }
    public bool IsToTarget(EaseType type)
    {
        if (type == EaseType.PunchDefault || type == EaseType.ShakeDefault)
        {
            return false;
        }
        else if(type == EaseType.VibrationCustom)
        {
            if(vibrationData.IsToTarget == false)
            {
                    return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
    public bool IsRandomShake(EaseType type)
    {
        if(type == EaseType.ShakeDefault || type == EaseType.ShakeToTarget)
        {
            return true;
        }else if(type == EaseType.VibrationCustom)
        {
            if(vibrationData.IsRandomVibrate == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    public Vector3 V3VibrationCustom(float X)
    {
        Vector3 v3 = new(VibrationCustom(X, 0), VibrationCustom(X, 1f), VibrationCustom(X, 2f));
        return v3;
    }
    public Vector2 V2VibrationCustom(float X)
    {
        Vector2 v2 = new(VibrationCustom(X, 0), VibrationCustom(X, 1f));
        return v2;
    }
    public float VibrationCustom(float X)
    {
        return vibrationData.VibrationCustom(X);
    }
    public float VibrationCustom(float X, float SeedPlus)
    {
        return vibrationData.VibrationCustom(X, SeedPlus);
    }


    public VibrationCustomData GetVibrationData()
    {
        return vibrationData;
    }
    /// <summary>
    /// 改了下mathf的lerpAngle，取消了t的clamp01限制
    /// </summary>
    public static float LerpAngleUnclamped(float a, float b, float t)
    {
        float num = Mathf.Repeat(b - a, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * t;
    }
}

public class VibrationCustomData
{
    public bool IsRandomVibrate = false;
    public bool IsToTarget = false;
    /// <summary>
    /// Frequency乘以dampingRation等于20左右最好
    /// </summary>
    public float Frequency = 6;
    private float angularFrequency;
    public float DampingRatio = 3f;
    private float dampingFactor;
    /// <summary>
    /// 不能是整数
    /// </summary>
    public static float DefaultRandomSeed = 1.2f;
    public float RandomSeed = DefaultRandomSeed;
    protected Func<float, float> ResultFunc;
    public VibrationCustomData(int Frequency = 6, float DampingRatio = 3f, bool IsRandomSeed = false, bool isRandom = false, bool isToTarget = false)
    {
        Set(Frequency, DampingRatio, IsRandomSeed, isRandom, isToTarget);
    }

    public void Set(int frequency = 6, float dampingRatio = 3f, bool IsRandomSeed = false, bool isRandom = false, bool isToTarget = false)
    {
        if (IsRandomSeed == true)
        {
            SetNewRandomSeed();
        }
        this.Frequency = frequency;
        this.DampingRatio = dampingRatio;
        IsRandomVibrate = isRandom;
        IsToTarget = isToTarget;

        if (IsToTarget == true)
        {
            this.Frequency += 0.5f;
            ResultFunc = ToTargetResult;
        }
        else
        {
            ResultFunc = UnToTargetResult;
        }
        angularFrequency = this.Frequency * Mathf.PI; //从原点开始
        dampingFactor = this.DampingRatio * this.Frequency / (2f * Mathf.PI);
    }

    public void SetNewRandomSeed()
    {
        RandomSeed = UnityEngine.Random.Range(1f, 100f);
        while (RandomSeed % 1 == 0)
        {
            RandomSeed = UnityEngine.Random.Range(1f, 100f);
        }
    }
    public float UnToTargetResult(float X)
    {
        return Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
    }
    public float ToTargetResult(float X)
    {
        return Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Cos(Mathf.PI + angularFrequency * X);
    }
    public float VibrationCustom(float X, float SeedPlus = 0)
    {
        float result = ResultFunc(X);

        if (IsRandomVibrate == true)
        {
            float frequencyNumber;
            if (IsToTarget == true)
            {
                //round会将0.4999以下舍为0，将0.50001至1.49999舍为1
                frequencyNumber = MathF.Round(X * Frequency);
            }
            else
            {
                frequencyNumber = (int)(X * Frequency);
            }
            if (IsToTarget == false || frequencyNumber != 0)
            {
                float realRandomSeed = RandomSeed + SeedPlus;
                float nextRandom = 2 * Mathf.PerlinNoise(realRandomSeed, frequencyNumber * 0.2f); //返回0-1随机数
                result *= nextRandom;
            }
        }
        if (IsToTarget == true)
        {
            result += 1;
        }
        return result;
    }
}


