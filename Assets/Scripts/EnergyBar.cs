
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Image currentFill;
    [SerializeField] private Image delayedFill;

    [SerializeField] private float bufferDelay = 0.15f;
    [SerializeField] private float bufferSpeed = 2f;

    private float energyPercent = 1f;
    private Coroutine bufferCoroutine;
    public TextMeshProUGUI NumText;

    public void InItEnergy(float current, float max)
    {
        energyPercent = Mathf.Clamp01(current / max);
        NumText.text = $"{current}/{max}";
        currentFill.fillAmount = energyPercent;
        delayedFill.fillAmount = energyPercent;
    }
    public void SetEnergy(float current, float max)
    {
        energyPercent = Mathf.Clamp01(current / max);
        NumText.text = $"{current}/{max}";
        // 实际精力立即变化
        currentFill.fillAmount = energyPercent;

        // 启动缓冲条动画
        if (bufferCoroutine != null)
            StopCoroutine(bufferCoroutine);

        bufferCoroutine = StartCoroutine(UpdateBuffer());
    }
    private IEnumerator UpdateBuffer()
    {
        // 稍微停顿一下
        yield return new WaitForSeconds(bufferDelay);

        // 平滑追赶实际值
        while (delayedFill.fillAmount > energyPercent)
        {
            delayedFill.fillAmount = Mathf.MoveTowards(
                delayedFill.fillAmount,
                energyPercent,
                bufferSpeed * Time.deltaTime
            );

            yield return null;
        }

        delayedFill.fillAmount = energyPercent;
        bufferCoroutine = null;
    }
}
