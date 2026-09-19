using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemDrop : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private int itemId;
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

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        int itemId,
        int amount,
        Vector3 targetPosition)
    {
        this.itemId = itemId;
        spriteRenderer.sprite = GameDataManager.Instance.itemDataList.GetItemDataById(itemId).Icon;
        this.amount = amount;

        this.spawnPosition =
            transform.position;

        this.targetPosition =
            targetPosition;
        IsPickAble = false;
        StartCoroutine(SpawnAnimation());
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

            // 抛物线高度
            position.y +=
                Mathf.Sin(t * Mathf.PI)
                * spawnHeight;

            transform.position =
                position;

            yield return null;
        }
        IsPickAble = true;
        transform.position =
            end;
    }

    private void Update()
    {
        if (isPickedUp || IsPickAble == false)
            return;

        CheckPickup();
    }

    private void CheckPickup()
    {
        if (Pointer.current == null)
        {
            return;
        }
        Vector2 screenPosition =
            Pointer.current.position.ReadValue();
        Vector3 cursorWorldPosition =
            Camera.main.ScreenToWorldPoint(screenPosition);

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

        Destroy(gameObject);
    }

    private void AddToInventory()
    {

        // 你的背包系统：
        // Inventory.AddItem(itemId, amount);

        // 同时可以通知 UI：
        // ItemGainUI.Show(itemId, amount);
    }
}