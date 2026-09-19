using UnityEngine;

/// <summary>
/// 运行时镜头控制（挂到 Main Camera 上）
/// WASD / 方向键 = 移动镜头，滚轮 = 缩放
/// </summary>
[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 10f;

    [Header("缩放速度")]
    public float zoomSpeed = 3f;

    [Header("镜头大小下限")]
    public float minSize = 1f;

    [Header("镜头大小上限")]
    public float maxSize = 20f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v -= 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v += 1f;

        if (h != 0f || v != 0f)
        {
            Vector3 dir = new Vector3(h, v, 0f).normalized;
            transform.position += dir * moveSpeed * Time.unscaledDeltaTime;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed, minSize, maxSize);
        }
    }
}
