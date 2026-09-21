using System.Collections.Generic;
using UnityEngine;

public class HarvestEffectManager : MonoBehaviour
{
    public GameManager gameManager;
    public Transform ItemParent;

    [Header("Harvest Effect")]
    [SerializeField] private ParticleSystem harvestParticlePrefab;

    [Header("Item Drop")]
    [SerializeField] private ItemDrop itemDropPrefab;

    [SerializeField] private float dropRadius = 0.8f;

    [Header("Object Pool")]
    [SerializeField] private int initialPoolSize = 20;

    private readonly Queue<ItemDrop> itemDropPool = new Queue<ItemDrop>();

    private void Awake()
    {
        InitializePool();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePool()
    {
        if (itemDropPrefab == null)
            return;

        for (int i = 0; i < initialPoolSize; i++)
        {
            ItemDrop drop = CreateItemDrop();
            itemDropPool.Enqueue(drop);
        }
    }

    /// <summary>
    /// 创建新的掉落物实例
    /// </summary>
    private ItemDrop CreateItemDrop()
    {
        ItemDrop drop = Instantiate(itemDropPrefab,ItemParent);

        drop.gameObject.SetActive(false);

        return drop;
    }

    /// <summary>
    /// 从对象池获取掉落物
    /// </summary>
    private ItemDrop GetItemDrop()
    {
        ItemDrop drop;

        if (itemDropPool.Count > 0)
        {
            drop = itemDropPool.Dequeue();
        }
        else
        {
            // 池子不够时自动扩容
            drop = CreateItemDrop();
        }

        drop.gameObject.SetActive(true);

        return drop;
    }

    /// <summary>
    /// 将掉落物归还对象池
    /// </summary>
    private void ReturnItemDrop(ItemDrop drop)
    {
        if (drop == null)
            return;

        drop.gameObject.SetActive(false);

        itemDropPool.Enqueue(drop);
        gameManager.hotbarController.AddItem(drop.itemId);
    }

    /// <summary>
    /// 在指定位置播放收割特效，并生成掉落物
    /// </summary>
    public void PlayHarvestEffect(
        Vector3 position,
        int itemId,
        int amount)
    {
        PlayParticle(position);

        SpawnDrops(
            position,
            itemId,
            amount
        );
    }

    /// <summary>
    /// 播放收割粒子
    /// </summary>
    private void PlayParticle(Vector3 position)
    {
        if (harvestParticlePrefab == null)
            return;

        ParticleSystem particle =
            Instantiate(
                harvestParticlePrefab,
                position,
                Quaternion.identity
            );

        particle.Play();

        Destroy(
            particle.gameObject,
            particle.main.duration +
            particle.main.startLifetime.constantMax
        );
    }

    /// <summary>
    /// 生成掉落物
    /// </summary>
    private void SpawnDrops(
        Vector3 position,
        int itemId,
        int amount)
    {
        if (itemDropPrefab == null)
            return;
        ItemData data = GameDataManager.Instance.itemDataList.GetItemDataById(itemId);
        if (data == null)
        {
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomOffset =
                Random.insideUnitCircle *
                dropRadius;

            Vector3 targetPosition =
                position +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            // 从对象池获取，而不是 Instantiate
            ItemDrop drop = GetItemDrop();

            drop.transform.position = position;
            drop.transform.rotation = Quaternion.identity;

            drop.Initialize(data, 1,targetPosition,ReturnItemDrop);
        }
    }
}