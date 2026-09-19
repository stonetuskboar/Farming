using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ImageOutline : MonoBehaviour
{
    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 1f;

    private Image image;

    private Material runtimeMaterial;
    private Material originalMaterial;

    private Sprite lastSprite;

    private static readonly int SpriteUVRectID =
        Shader.PropertyToID("_SpriteUVRect");

    private static readonly int OutlineColorID =
        Shader.PropertyToID("_OutlineColor");

    private static readonly int OutlineWidthID =
        Shader.PropertyToID("_OutlineWidth");

    private void OnEnable()
    {
        image = GetComponent<Image>();

        CreateMaterial();
        UpdateMaterial();
    }

    private void OnDisable()
    {
        CleanupMaterial();
    }

    private void OnDestroy()
    {
        CleanupMaterial();
    }

    private void LateUpdate()
    {
        if (image == null)
            return;

        // Sprite 被动态替换时，自动重新计算 Atlas UV
        if (lastSprite != image.sprite)
        {
            UpdateSpriteUV();
        }

        UpdateProperties();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        if (image == null)
            image = GetComponent<Image>();

        if (runtimeMaterial == null)
            CreateMaterial();

        UpdateMaterial();
    }
#endif

    private void CreateMaterial()
    {
        if (image == null)
            return;

        if (runtimeMaterial != null)
            return;

        originalMaterial = image.material;

        Shader shader =
            Shader.Find("UI/WhiteOutlineSafeAtlas");

        if (shader == null)
        {
            Debug.LogError(
                "找不到 Shader: UI/WhiteOutlineSafeAtlas",
                this
            );

            return;
        }

        // 每个 Image 使用独立材质。
        // 因为不同 Sprite 的 Atlas UV Rect 不一样。
        runtimeMaterial = new Material(shader);

        runtimeMaterial.name =
            $"{name}_WhiteOutline_Runtime";

        runtimeMaterial.hideFlags =
            HideFlags.HideAndDontSave;

        image.material = runtimeMaterial;
    }

    private void UpdateMaterial()
    {
        if (runtimeMaterial == null)
            return;

        UpdateSpriteUV();
        UpdateProperties();

        image.SetMaterialDirty();
    }

    private void UpdateProperties()
    {
        if (runtimeMaterial == null)
            return;

        runtimeMaterial.SetColor(
            OutlineColorID,
            outlineColor
        );

        runtimeMaterial.SetFloat(
            OutlineWidthID,
            outlineWidth
        );
    }

    private void UpdateSpriteUV()
    {
        if (runtimeMaterial == null)
            return;

        Sprite sprite = image.sprite;

        lastSprite = sprite;

        if (sprite == null)
        {
            runtimeMaterial.SetVector(
                SpriteUVRectID,
                new Vector4(0, 0, 1, 1)
            );

            return;
        }

        Vector4 outerUV =
            DataUtility.GetOuterUV(sprite);

        // DataUtility.GetOuterUV:
        //
        // x = min U
        // y = min V
        // z = max U
        // w = max V

        runtimeMaterial.SetVector(
            SpriteUVRectID,
            outerUV
        );

        image.SetMaterialDirty();
    }

    private void CleanupMaterial()
    {
        if (image != null &&
            image.material == runtimeMaterial)
        {
            image.material = originalMaterial;
        }

        if (runtimeMaterial == null)
            return;

        if (Application.isPlaying)
        {
            Destroy(runtimeMaterial);
        }
        else
        {
            DestroyImmediate(runtimeMaterial);
        }

        runtimeMaterial = null;
        lastSprite = null;
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        UpdateProperties();
    }

    public void SetOutlineWidth(float width)
    {
        outlineWidth = Mathf.Max(0f, width);
        UpdateProperties();
    }
}