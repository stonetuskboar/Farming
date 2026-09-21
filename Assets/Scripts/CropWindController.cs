using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAtlasUVNormalizer : MonoBehaviour
{
    private static readonly int UVRectID =
        Shader.PropertyToID("_UVRect");

    private static readonly int BottomLockID =
        Shader.PropertyToID("_Bottom Lock");

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;

    private Sprite lastSprite;

    private void OnEnable()
    {
        Initialize();
        UpdateSpriteData();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Initialize();
        UpdateSpriteData();
    }
#endif

    private void LateUpdate()
    {
        if (spriteRenderer != null &&
            spriteRenderer.sprite != lastSprite)
        {
            UpdateSpriteData();
        }
    }

    private void Initialize()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    private void UpdateSpriteData()
    {
        if (spriteRenderer == null)
            return;

        Sprite sprite = spriteRenderer.sprite;

        if (sprite == null)
            return;

        // -----------------------------
        // 1. 计算 Atlas UV Rect
        // -----------------------------

        Vector2[] uvs = sprite.uv;

        if (uvs == null || uvs.Length == 0)
            return;

        Vector2 uvMin = uvs[0];
        Vector2 uvMax = uvs[0];

        for (int i = 1; i < uvs.Length; i++)
        {
            uvMin = Vector2.Min(uvMin, uvs[i]);
            uvMax = Vector2.Max(uvMax, uvs[i]);
        }

        Vector2 uvSize = uvMax - uvMin;

        uvSize.x = Mathf.Max(uvSize.x, 0.000001f);
        uvSize.y = Mathf.Max(uvSize.y, 0.000001f);


        // -----------------------------
        // 2. Pivot -> 0~1 高度
        // -----------------------------

        float pivotY01 =
            sprite.pivot.y / sprite.rect.height;

        pivotY01 = Mathf.Clamp01(pivotY01);


        // -----------------------------
        // 3. 设置 Shader 参数
        // -----------------------------

        spriteRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetVector(
            UVRectID,
            new Vector4(
                uvMin.x,
                uvMin.y,
                uvSize.x,
                uvSize.y
            )
        );

        propertyBlock.SetFloat(
            BottomLockID,
            pivotY01
        );

        spriteRenderer.SetPropertyBlock(propertyBlock);

        lastSprite = sprite;
    }
}