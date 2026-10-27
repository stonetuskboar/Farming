using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyingSeedPanel : MonoBehaviour
{

    public ImageSwapButton IncreaseButton;
    public ImageSwapButton DecreaseButton;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI ValueText;
    public ImageSwapButton ConfirmButton;

    private bool isIncrease = true;
    [Header("按住加速设置")]
    [SerializeField] private float initialDelay = 0.35f;
    [SerializeField] private float startInterval = 0.15f;
    [SerializeField] private float minInterval = 0.025f;
    [SerializeField] private float accelerationDuration = 2f;

    private bool isHeld;
    private float holdTime;
    private float repeatTimer;
    private int MaxValue = 50;
    private int Price = 0;
    public int currentAmount = 0;

    public void Start()
    {
        IncreaseButton.PointerDown += HoldIncrease;
        IncreaseButton.PointerUp += StopHolding;
        DecreaseButton.PointerDown += HoldDecrease;
        DecreaseButton.PointerUp += StopHolding;
        if(Application.isMobilePlatform)
        {
            IncreaseButton.Scale = 2;
            DecreaseButton.Scale = 2;
            IncreaseButton.ChangeStateToIdleImmediately();
            DecreaseButton.ChangeStateToIdleImmediately();
        }
    }

    private void Update()
    {
        if (!isHeld)
            return;

        if(holdTime < minInterval && holdTime +Time.unscaledDeltaTime >= minInterval)
        {
            GameDataManager.Instance.PlayAudio("Hold");
        }
        holdTime += Time.unscaledDeltaTime;
        repeatTimer -= Time.unscaledDeltaTime;

        if (repeatTimer <= 0f)
        {
            ChangeValue();

            float progress = accelerationDuration > 0f
                ? Mathf.Clamp01(holdTime / accelerationDuration)
                : 1f;
            // 从初始间隔逐渐缩短到最快间隔
            float interval = Mathf.Lerp(
                startInterval,
                minInterval,
                progress
            );
            repeatTimer += interval;
        }
    }
    private void OnDisable()
    {
        StopHolding();
    }

    public void Set(int price, int maxValue)
    {
        this.Price = price;
        MaxValue = maxValue;
        SetAmount(currentAmount);
    }
    private void ChangeValue()
    {
        if (isIncrease)
            SetAmount(currentAmount + 1);
        else
            SetAmount(currentAmount - 1);
    }
    public void SetAmount(int value)
    {
        currentAmount = Mathf.Clamp(
            value, 0, MaxValue);
        amountText.text = currentAmount.ToString();
        ValueText.text = (GetValue()).ToString();
    }
    public int GetValue()
    {
        return currentAmount * Price;
    }

    public void HoldIncrease()
    {
        //GameDataManager.Instance.PlayAudio("UI_Click");
        isHeld = true;
        isIncrease = true;
        holdTime = 0f;
        repeatTimer = initialDelay;

        // 按下时立即改变一次
        ChangeValue();
    }
    public void HoldDecrease()
    {
        //GameDataManager.Instance.PlayAudio("UI_Click");
        isHeld = true;
        isIncrease = false;
        holdTime = 0f;
        repeatTimer = initialDelay;

        // 按下时立即改变一次
        ChangeValue();
    }
    private void StopHolding()
    {
        Debug.Log("StopHold");
        GameDataManager.Instance.PlayAudio("StopHold");
        isHeld = false;
    }

}
