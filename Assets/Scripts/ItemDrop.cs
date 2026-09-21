using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemDrop : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] public int itemId;
    [SerializeField] private int amount = 1;

    [Header("Spawn")]
    [SerializeField] private float spawnDuration = 0.35f;
    [SerializeField] private float spawnHeight = 0.6f;

    [Header("Pickup")]
    [SerializeField] private float pickupDistance = 0.8f;
    [SerializeField] private float pickupDuration = 0.25f;
    [SerializeField] private float pickupHeight = 0.5f;

    private bool IsPickAble = false;

    private SpriteRenderer spriteRenderer;

    private Vector3 spawnPosition;
    private Vector3 targetPosition;

    private bool isPickedUp;

    /// <summary>
    /// 对象池回收回调
    /// </summary>
    private Action<ItemDrop> returnToPool;

    private Coroutine spawnCoroutine;
    private Coroutine pickupCoroutine;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 初始化掉落物
    /// </summary>
    public void Initialize(
        ItemData data,
        int amount,
        Vector3 targetPosition,
        Action<ItemDrop> returnToPool)
    {
        this.itemId = data.Id;
        this.amount = amount;
        this.targetPosition = targetPosition;
        this.returnToPool = returnToPool;

        isPickedUp = false;
        IsPickAble = false;

        // 停止上一次可能还没有结束的协程
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (pickupCoroutine != null)
        {
            StopCoroutine(pickupCoroutine);
            pickupCoroutine = null;
        }

        // 恢复颜色
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;

            spriteRenderer.sprite = data.Icon;
        }

        spawnPosition =
            transform.position;

        spawnCoroutine =
            StartCoroutine(
                SpawnAnimation()
            );
    }

    private IEnumerator SpawnAnimation()
    {
        Vector3 start =
            spawnPosition;

        Vector3 end =
            targetPosition;

        float time = 0f;

        while (time < spawnDuration)
        {
            time += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    time / spawnDuration
                );

            Vector3 position =
                Vector3.Lerp(
                    start,
                    end,
                    t
                );

            // 抛物线
            position.y +=
                Mathf.Sin(t * Mathf.PI)
                * spawnHeight;

            transform.position =
                position;

            yield return null;
        }

        transform.position = end;

        IsPickAble = true;

        spawnCoroutine = null;
    }

    private void Update()
    {
        if (isPickedUp || !IsPickAble)
            return;

        CheckPickup();
    }

    private void CheckPickup()
    {
        if (Pointer.current == null)
            return;

        if (Camera.main == null)
            return;

        Vector2 screenPosition =
            Pointer.current.position.ReadValue();

        Vector3 cursorWorldPosition =
            Camera.main.ScreenToWorldPoint(
                screenPosition
            );

        if (
            Vector2.Distance(
                transform.position,
                cursorWorldPosition
            ) <= pickupDistance
        )
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (isPickedUp)
            return;

        isPickedUp = true;
        IsPickAble = false;

        pickupCoroutine =
            StartCoroutine(
                PickupAnimation()
            );
    }

    private IEnumerator PickupAnimation()
    {
        Vector3 start =
            transform.position;

        Vector3 end =
            start +
            Vector3.up * pickupHeight;

        Color startColor =
            spriteRenderer.color;

        float time = 0f;

        while (time < pickupDuration)
        {
            time += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    time / pickupDuration
                );

            // Ease In
            float easedT =
                t * t;

            transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    easedT
                );

            Color color =
                startColor;

            color.a =
                Mathf.Lerp(
                    startColor.a,
                    0f,
                    t
                );

            spriteRenderer.color =
                color;

            yield return null;
        }

        AddToInventory();

        pickupCoroutine = null;

        // 不再 Destroy
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        IsPickAble = false;
        isPickedUp = false;

        if (returnToPool != null)
        {
            returnToPool.Invoke(this);
        }
        else
        {
            // 防止没有设置对象池时物体一直存在
            gameObject.SetActive(false);
        }
    }

    private void AddToInventory()
    {
        // 你的背包系统：
        // Inventory.AddItem(itemId, amount);

        // 同时可以通知 UI：
        // ItemGainUI.Show(itemId, amount);
    }
}