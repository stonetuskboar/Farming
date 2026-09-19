using UnityEngine;

// Y轴排序：Y 越小（越靠屏幕下方）→ 越靠前
// 用法：挂在需要 Y 排序的物体上，这些物体要放在同一个「排序图层」里
[ExecuteAlways] // 关键：让脚本在「编辑模式」也实时生效，不点运行也能看到遮挡效果
public class YSort : MonoBehaviour
{
    [Tooltip("精度，默认100即可。排序闪烁就把数字改大")]
    public int precision = 100;

    SpriteRenderer 渲染器;

    void Awake()
    {
        渲染器 = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (渲染器 == null) return;

        // 注意：transform.position 是世界坐标，不管物体套了几层父物体都按实际位置算
        渲染器.sortingOrder = Mathf.RoundToInt(-transform.position.y * precision);
    }
}
