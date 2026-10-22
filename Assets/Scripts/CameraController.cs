
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

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
    public float scrollSpeed = 3f;
    public float zoomSpeed = 300;

    [Header("镜头大小下限")]
    public float minSize = 1f;

    [Header("镜头大小上限")]
    public float maxSize = 20f;
    [Header("边缘检测")]
    public float edgeSize = 30f; // 距离屏幕边缘多少像素开始移动
    [SerializeField] private InputActionReference scrollAction;
    [SerializeField] private InputActionReference moveAction;
    public BoxCollider2D mapBounds;
    public Camera cam;
    private float lastPinchDistance;

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
        EnhancedTouchSupport.Enable();
        scrollAction.action.performed += OnScroll;
        scrollAction.action.Enable();
        moveAction.action.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        scrollAction.action.performed -= OnScroll;
        scrollAction.action.Disable();
        moveAction.action.Disable();
    }
    void Update()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
        #else
                MobileControll();
        #endif



        if (moveAction.action.IsPressed() == true )
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();

            if (input.sqrMagnitude > 0f)
            {
                Vector3 dir = new Vector3(input.x, input.y, 0f).normalized;

                cam.transform.position += dir
                    * cam.orthographicSize
                    * moveSpeed
                    * Time.unscaledDeltaTime;

                ClampCamera();
            }
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
                cam.orthographicSize - scroll * scrollSpeed, minSize, maxSize);
            ClampCamera();
        }
    }

    private void MobileControll()
    {
        var touches = Touch.activeTouches;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
            // 双指缩放
         if (touches.Count >= 2)
        {
            Vector2 p1 = touches[0].screenPosition;
            Vector2 p2 = touches[1].screenPosition;

            float currentDistance = Vector2.Distance(p1, p2);

            if (lastPinchDistance > 0f)
            {
                float delta = currentDistance - lastPinchDistance;

                if (cam != null && cam.orthographic)
                {
                    // 正交相机：调整视野大小
                    cam.orthographicSize -= delta * scrollSpeed /zoomSpeed;
                    cam.orthographicSize = Mathf.Clamp(
                        cam.orthographicSize,
                        minSize,
                        maxSize
                    );
                }
                else
                {
                    // 透视相机：沿观察方向前后移动
                    transform.position += transform.forward
                        * delta * scrollSpeed;
                }
            }

            lastPinchDistance = currentDistance;
            return;
        }

        // 单指拖动
        lastPinchDistance = 0f;
        ClampCamera();
    }
    public void MoveCam(Vector3 delta)
    {
        transform.position += delta;
        ClampCamera();
    }
}
