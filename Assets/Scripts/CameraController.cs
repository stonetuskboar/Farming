
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private InputActionReference scrollAction;
    public BoxCollider2D mapBounds;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }
    void OnEnable()
    {
        scrollAction.action.performed += OnScroll;
        scrollAction.action.Enable();
    }

    void OnDisable()
    {
        scrollAction.action.performed -= OnScroll;
        scrollAction.action.Disable();
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
            cam.transform.position += dir * cam.orthographicSize * moveSpeed * Time.unscaledDeltaTime;
            ClampCamera();
        }
    }
    private void ClampCamera()
    {
        Bounds bounds = mapBounds.bounds;

        // 摄像机视野的半宽、半高
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // 摄像机中心允许移动的范围
        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;

        Vector3 pos = transform.position;

        pos.x = minX > maxX
            ? bounds.center.x
            : Mathf.Clamp(pos.x, minX, maxX);

        pos.y = minY > maxY
            ? bounds.center.y
            : Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
    private void OnScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<Vector2>().y;
        scroll = Mathf.Sign(scroll);
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed, minSize, maxSize);
            ClampCamera();
        }
    }
}
