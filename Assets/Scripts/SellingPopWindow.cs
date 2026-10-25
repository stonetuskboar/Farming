using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ak.wwise;
using static UnityEngine.Rendering.DebugUI;

public class SellingPopWindow : MonoBehaviour
{
    public ItemSellPage Page;
    public BasicPopWindow popWindow;

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
    public int itemId = -1;
    public int currentValue = 1;


    public void Start()
    {
        IncreaseButton.PointerDown += HoldIncrease;
        IncreaseButton.PointerUp += StopHolding;
        IncreaseButton.PointerExited += StopHolding;
        DecreaseButton.PointerDown += HoldDecrease;
        DecreaseButton.PointerUp += StopHolding;
        DecreaseButton.PointerExited += StopHolding;

        ConfirmButton.PointerClicked += Sold;
        SetValue(0);
    }

    private void Update()
    {
        if (!isHeld)
            return;

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

    public void Set(BasicItem item)
    {
        itemId = item.Id;
        this.MaxValue = item.Amount;
        this.Price = item.itemData.Price;
        SetValue(1);
    }
    private void ChangeValue()
    {
        if (isIncrease)
            SetValue(currentValue + 1);
        else
            SetValue(currentValue - 1);
    }
    public void SetValue(int value)
    {
        currentValue = Mathf.Clamp(
            value, 0, MaxValue);
        amountText.text = currentValue.ToString();
        ValueText.text = (currentValue * Price).ToString();
    }

    public void HoldIncrease()
    {
        isHeld = true;
        isIncrease = true;
        holdTime = 0f;
        repeatTimer = initialDelay;

        // 按下时立即改变一次
        ChangeValue();
    }
    public void HoldDecrease()
    {
        isHeld = true;
        isIncrease = false;
        holdTime = 0f;
        repeatTimer = initialDelay;

        // 按下时立即改变一次
        ChangeValue();
    }
    private void StopHolding()
    {
        isHeld = false;
    }

    public void Sold()
    {
        Page.gameManager.ChangeMoney(currentValue * Price);
        Page.gameManager.DecreaseItem(itemId, currentValue);
        GameDataManager.Instance.PlayAudio("SellSucess");
        SellableItemUI ui = Page.FindUIById(itemId);
        if(ui != null && ui.gameObject.activeInHierarchy == true)
        {
            ui.SellButton.SetLocalRotation(Quaternion.identity);
            ui.SellButton.SetLocalScale(Vector3.one);
            ui.SellButton.DoLocalRotationTween(Quaternion.Euler(30, -25, 0), 0.4f, type: EaseType.PunchDefault);
            ui.SellButton.DoLocalScaleTween(Vector3.one * 1.3f, 0.6f, type: EaseType.ShakeDefault);
        }

        StopHolding();
        popWindow.Hide(0.2f);
    }
}
