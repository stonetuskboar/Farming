using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class CropShaderController : MonoBehaviour
{
    private static readonly int UVRectID =
        Shader.PropertyToID("_UVRect");

    private static readonly int BottomLockID =
        Shader.PropertyToID("_Bottom");

    private readonly int PositionID =
        Shader.PropertyToID("_Position");
    private readonly int StrengthID =
    Shader.PropertyToID("_Strength");
    private readonly int BendPowerID =
Shader.PropertyToID("_BendPower");
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
                uvMax.x,
                uvMax.y
            )
        );

        propertyBlock.SetFloat(
            BottomLockID,
            pivotY01
        );
        propertyBlock.SetVector(
            PositionID,
            transform.position
        );
        spriteRenderer.SetPropertyBlock(propertyBlock);

        lastSprite = sprite;
    }

    public void UpdateWindForce(WindType type) 
    {
        float strength = 0;
        float bendPower = 1;
        if(type == WindType.静止)
        {
            strength = 0;
        }else if(type == WindType.微动)
        {
            bendPower = 2.5f;
            strength = 0.2f;
        }else if(type == WindType.正常)
        {
            strength = 0.5f;
            bendPower = 2f;
        }else if(type == WindType.剧烈)
        {
            strength = 0.8f;
            bendPower = 1.5f;
        }

        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(
            BendPowerID,
            bendPower
        );
        propertyBlock.SetFloat(
            StrengthID,
            strength
        );
        spriteRenderer.SetPropertyBlock(propertyBlock);

    }
}