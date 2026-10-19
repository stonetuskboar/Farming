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
    public bool HideOutLine = false;
    private Image image;

    private Material materialInstance;

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
        materialInstance = image.material;
        UpdateProperties();
        UpdateSpriteUV();
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
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        if (image == null)
            image = GetComponent<Image>();

        materialInstance = image.material;
        UpdateProperties();
        UpdateSpriteUV();
    }
#endif

    private void UpdateProperties()
    {
        materialInstance.SetColor(
            OutlineColorID,
            outlineColor
        );

        if(HideOutLine == true)
        {
            materialInstance.SetFloat(
                OutlineWidthID,
                0f
            );
        }
        else
        {
            materialInstance.SetFloat(
                OutlineWidthID,
                outlineWidth
            );
        }

    }

    private void UpdateSpriteUV()
    {

        Sprite sprite = image.sprite;

        lastSprite = sprite;

        Vector4 outerUV =
            DataUtility.GetOuterUV(sprite);

        // DataUtility.GetOuterUV:
        //
        // x = min U
        // y = min V
        // z = max U
        // w = max V

        materialInstance.SetVector(
            SpriteUVRectID,
            outerUV
        );
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        UpdateProperties();
    }

    public void SetOutlineWidth(float width)
    {
        Debug.Log("here");
        outlineWidth = Mathf.Max(0f, width);
        UpdateProperties();
    }
    public void HideOutlineWidth()
    {
        HideOutLine = true;
        UpdateProperties();
    }
    public void ShowOutlineWidth()
    {
        HideOutLine = false;
        UpdateProperties();
    }
}