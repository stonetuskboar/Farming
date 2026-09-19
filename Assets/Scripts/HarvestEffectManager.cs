using UnityEngine;

public class HarvestEffectManager : MonoBehaviour
{

    [Header("Harvest Effect")]
    [SerializeField] private ParticleSystem harvestParticlePrefab;

    [Header("Item Drop")]
    [SerializeField] private ItemDrop itemDropPrefab;

    [SerializeField] private float dropRadius = 0.8f;


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

        for (int i = 0; i < amount; i++)
        {
            // 随机落点
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

            ItemDrop drop =
                Instantiate(
                    itemDropPrefab,
                    position,
                    Quaternion.identity
                );

            drop.Initialize(
                itemId,
                1,
                targetPosition
            );
        }
    }
}