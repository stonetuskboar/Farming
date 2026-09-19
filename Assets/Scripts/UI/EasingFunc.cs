using System;
using Unity.Mathematics;
using UnityEngine;

public static class EasingFunc
{
    public static float Line(float x) 
    {
        return x;
    }
    public static float EaseOutQuad(float x)
    {
        return 1 - Mathf.Pow(1 - x, 2);
    }

    /// <summary>
    /// 比EaseOutQuad运动更快
    /// </summary>
    /// <param profs="x"></param>
    /// <returns></returns>
    public static float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
    public static float EaseInQuad(float x)
    {
        return x * x;
    }
    public static float EaseInCubic(float x)
    {
        return x * x * x;
    }
    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;
        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }

    public static float EaseOutElastic(float x)
    {
        float c4 = 2 * MathF.PI * x;

        if (x == 0)
        {
            return 0;
        }
        else if (x == 1)
        {
            return 1;
        }
        else
        {
            return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }
    }

    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return c3 * Mathf.Pow(x, 3) - c1 * Mathf.Pow(x, 2);
    }
    /// <summary>
    /// //来自https://m3.material.io/styles/motion/overview/specs 
    /// bezier 参数为：0.42, 1.67, 0.21, 0.90. Duration =  350ms
    /// 此为参数拟合结果，我实在不知道spring的原理
    /// </summary>
    public static float M3SpringFast(float x)
    {
        if (x < 0.313f)
        {
            return -17.62056f * Mathf.Pow(x, 3) + 4.68289f * Mathf.Pow(x, 2) + 3.61229f * x + 0.00547024f;
        }
        else if (x < 0.5f)
        {
            return -67.31153f * Mathf.Pow(x, 4) + 132.13945f * Mathf.Pow(x, 3) - 97.58087f * Mathf.Pow(x, 2) + 31.75381f * x - 2.73107f;
        }
        else
        {
            return -0.628211f * Mathf.Pow(x, 4) + +1.76657f * Mathf.Pow(x, 3) - 1.27041f * Mathf.Pow(x, 2) - 0.130715f * x + 1.2626f;
        }
    }
    /// <summary>
    /// //来自https://m3.material.io/styles/motion/overview/specs 
    /// bezier 参数为：	0.38, 1.21, 0.22, 1.00. Duration =  500ms
    /// 此为参数拟合结果，我实在不知道spring的原理
    /// </summary>
    public static float M3SpringDefault(float x)
    {
        if (x < 0.4f)
        {
            return -12.86932f * Mathf.Pow(x, 3) + 3.81265f * Mathf.Pow(x, 2) + 2.97105f * x + 0.00245274f;
        }
        else if (x < 0.7f)
        {
            return -8.35137f * Mathf.Pow(x, 4) + 21.2143f * Mathf.Pow(x, 3) - 20.33852f * Mathf.Pow(x, 2) + 8.68378f * x - 0.375128f;
        }
        else
        {
            return 0.0969583f * Mathf.Pow(x, 3) - 0.156501f * Mathf.Pow(x, 2) + 0.0233681f * x + 1.03619f;
        }
    }
    /// <summary>
    /// //来自https://m3.material.io/styles/motion/overview/specs 
    /// bezier 参数为：	0.39, 1.29, 0.35, 0.98. Duration =  650ms
    /// 此为参数拟合结果，我实在不知道spring的原理
    /// </summary>
    public static float M3SpringSlow(float x)
    {
        if (x < 0.6f)
        {
            return 15.01864f * Mathf.Pow(x, 4) - 18.84855f * Mathf.Pow(x, 3) + 3.77053f * Mathf.Pow(x, 2) + 2.96921f * x + 0.0055831f;
        }
        else
        {
            return -1.09108f * Mathf.Pow(x, 4) + 3.88852f * Mathf.Pow(x, 3) - 4.95606f * Mathf.Pow(x, 2) + 2.63687f * x + 0.521703f;
        }
    }

    public static float PunchDefault(float X)
    {
        int frequency = 6;
        float dampingRatio = 3f;
        float angularFrequency = frequency * Mathf.PI; //从原点开始
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
        return result;
    }
    public static float PunchToTarget(float X)
    {
        float frequency = 6.5f;
        float dampingRatio = 3f;
        float angularFrequency = frequency * Mathf.PI; //从原点开始
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = 1 + Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Cos(Mathf.PI + angularFrequency * X);
        return result;
    }
    public static Vector3 V3ShakeDefault(float X)
    {
        Vector3 v3 = new Vector3(ShakeDefault(X, VibrationCustomData.DefaultRandomSeed),
            ShakeDefault(X, VibrationCustomData.DefaultRandomSeed + 1f),
            ShakeDefault(X, VibrationCustomData.DefaultRandomSeed + 2f));
        return v3;
    }
    public static Vector2 V2ShakeDefault(float X)
    {
        Vector2 v2 = new Vector2(ShakeDefault(X, VibrationCustomData.DefaultRandomSeed),
            ShakeDefault(X, VibrationCustomData.DefaultRandomSeed + 1f));
        return v2;
    }
    public static float ShakeDefault(float X)
    {
        return ShakeDefault(X, VibrationCustomData.DefaultRandomSeed);
    }
    public static float ShakeDefault(float X, float RandomSeed)
    {
        int frequency = 4;
        float dampingRatio = 5f;
        //（int)会向下取整
        float frequencyNumber = (int)(X * frequency);
        float angularFrequency = frequency * Mathf.PI; //从原点开始
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
        float nextRandom = 2 * Mathf.PerlinNoise(RandomSeed, frequencyNumber * 0.2f); //返回0-1随机数
        result = result * nextRandom;
        return result;
    }
    public static Vector3 V3ShakeToTarget(float X)
    {
        Vector3 v3 = new Vector3(ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed),
            ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed + 1f),
            ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed + 2f));
        return v3;
    }
    public static Vector2 V2ShakeToTarget(float X)
    {
        Vector2 v2 = new Vector2(ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed),
            ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed + 1f));
        return v2;
    }
    public static float ShakeToTarget(float X)
    {
        return ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed);
    }
    public static float ShakeToTarget(float X, float RandomSeed)
    {
        float frequency = 4.5f;
        float dampingRatio = 5f;
        //round会将0.4999以下舍为0，将0.50001至1.49999舍为1，符合Cos曲线
        float frequencyNumber = Mathf.Round(X * frequency);
        float angularFrequency = frequency * Mathf.PI; //从原点开始
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Cos(Mathf.PI + angularFrequency * X);

        float nextRandom = 1;
        if (frequencyNumber > 0)
        {
            nextRandom = 2 * Mathf.PerlinNoise(RandomSeed, frequencyNumber * 0.2f); //返回0-1随机数
        }
        result = 1 + result * nextRandom;
        return result;
    }

    public static Func<float, float> GetEaseFuncByType(EaseType type, float time)
    {
        if(type == EaseType.Line)
        {
            return Line;
        }
        else if (type == EaseType.EaseOutQuad)
        {
            return EaseOutQuad;
        }
        else if (type == EaseType.EaseOutCubic)
        {
            return EaseOutCubic;
        }
        else if (type == EaseType.EaseInQuad)
        {
            return EaseInQuad;
        }
        else if (type == EaseType.EaseInCubic)
        {
            return EaseInCubic;
        }
        else if (type == EaseType.EaseOutBounce)
        {
            return EaseOutBounce;
        }
        else if (type == EaseType.EaseOutElastic)
        {
            return EaseOutElastic;
        }
        else if (type == EaseType.EaseInOutBack)
        {
            return EaseInOutBack;
        }
        else if (type == EaseType.EaseInOutCubic)
        {
            return EaseInOutCubic;
        }
        else if (type == EaseType.EaseOutBack)
        {
            return EaseOutBack;
        }
        else if (type == EaseType.EaseInBack)
        {
            return EaseInBack;
        }
        else if (type == EaseType.M3Spring)
        {
            if (time <= 0.35f)
            {
                return M3SpringFast;
            }
            else if (time < 0.65f)
            {
                return M3SpringDefault;
            }
            else
            {
                return M3SpringSlow;
            }
        }
        else if (type == EaseType.PunchDefault)
        {
            return PunchDefault;
        }
        else if (type == EaseType.PunchToTarget)
        {
            return PunchToTarget;
        }
        else if (type == EaseType.ShakeDefault)
        {
            return ShakeDefault;
        }
        else if (type == EaseType.ShakeToTarget)
        {
            return ShakeToTarget;
        }
        else
        {
            return EaseOutQuad; //默认
        }
    }

    public static Func<float, Vector3> GetVector3ShakeFuncByType(EaseType type)
    {
        if (type == EaseType.ShakeDefault)
        {
            return EasingFunc.V3ShakeDefault;
        }
        else if (type == EaseType.ShakeToTarget)
        {
            return EasingFunc.V3ShakeToTarget;
        }
        else
        {
            Debug.Log("此函数不应该执行到此。");
            return EasingFunc.V3ShakeDefault;
        }
    }

    public static Func<float, Vector2> GetVector2ShakeFuncByType(EaseType type)
    {
        if (type == EaseType.ShakeDefault)
        {
            return EasingFunc.V2ShakeDefault;
        }
        else if (type == EaseType.ShakeToTarget)
        {
            return EasingFunc.V2ShakeToTarget;
        }
        else
        {
            Debug.Log("此函数不应该执行到此。");
            return EasingFunc.V2ShakeDefault;
        }
    }
}
public enum EaseType
{
    Line,
    EaseOutQuad,
    EaseOutCubic,
    EaseInQuad,
    EaseInCubic,
    /// <summary>
    /// 弹跳，还没被用过
    /// </summary>
    EaseOutBounce,
    /// <summary>
    /// 缓出弹性，还没被用过
    /// </summary>
    EaseOutElastic,
    /// <summary>
    /// 有缓冲的运动，经常使用
    /// </summary>
    EaseInOutBack,
    /// <summary>
    /// 缓入缓出三次方，还没被用过
    /// </summary>
    EaseInOutCubic,
    /// <summary>
    /// 大概在0.55左右到达1
    /// </summary>
    EaseOutBack,
    EaseInBack,
    /// <summary>
    /// 根据时间分为三种ease Line 350ms, 500ms, 650ms
    /// 小于350时，第一次到达1为0.28 ≈100ms
    /// 500ms时，第一次到达为0.43 ≈ 210ms
    /// 650ms时，第一次到达为0.45 ≈ 300ms
    /// </summary>
    M3Spring,
    /// <summary>
    /// Default代表在原地震动，toTarget表示会增加值到目标
    /// </summary>
    PunchDefault,
    PunchToTarget,
    /// <summary>
    /// Punch代表幅度固定，Shake代表每次振幅幅度随机
    /// </summary>
    ShakeDefault,
    ShakeToTarget,
    VibrationCustom,
}