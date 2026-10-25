
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridCursorController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public CameraController cameraController;
    [Header("Cursor")]
    [SerializeField] private Transform cursorBoxParent;
    [SerializeField] private SpriteRenderer cursorBoxPrefab;
    // 当前生成出来的 CursorBox
    private readonly List<SpriteRenderer> cursorBoxes = new();
    [SerializeField] private Sprite validSprite;
    [SerializeField] private Sprite invalidSprite;
    [Header("Input Actions")]
    [SerializeField] private InputActionReference clickDownAction;
    [SerializeField] private InputActionReference clickUpAction;
    public Sprite PointerSprite;
    public Image CursorImage;
    public ImageOutline ImageOuntline;
    public BasicTweenObject ImageTween;

    //这三个是为了移动端适配增加的。cickdownAction先于update触发，所以IsPointerClickUI更准确
    //如果玩家按下位置不是UI，即使之后遇到了UI，也不影响，操作更顺滑一些
    private bool IsPointerClickUI = true;
    private Vector2 LastPointerPosition;
    private bool IsDragging = false;
    // 曾经用过的格子
    private List<Vector3Int> UsedCells = new();
    // 当前范围内所有格子
    public List<Vector3Int> CurrentCells { get; private set; } = new();
    public bool IsUsable { get; private set; }
    private bool IsValid = false;

    private void Awake()
    {
        Cursor.visible = false;
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.visible = false;
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
    private void OnEnable()
    {
        clickDownAction.action.Enable();
        clickUpAction.action.Enable();
        clickDownAction.action.performed += OnClick;
        clickUpAction.action.performed += OnClickUp;
        if (Application.isMobilePlatform)
        {
            CursorImage.gameObject.SetActive(false);
        }
    }



    private void OnDisable()
    {
        clickDownAction.action.performed -= OnClick;
        clickUpAction.action.performed -= OnClickUp;
        clickDownAction.action.Disable();
        clickUpAction.action.Disable();
    }


    private void Update()
    {
        UpdateCursor();
        if (clickDownAction.action.IsPressed() && IsUsable && false == UsedCells.SequenceEqual(CurrentCells))
        {
            UsedCells.Clear();
            UsedCells.AddRange(CurrentCells);
            GetNowTool().Use(CurrentCells);
        }
        DragToMoveCam();
    }
    public UsableItem GetNowTool()
    {
        return gameManager.hotbarController.NowTool;
    }
    public void ChangeCursorToPointer()
    {
        ChangeCursorSprite(PointerSprite);
        ImageOuntline.HideOutlineWidth();   
    }

    public void ChangeCursorSprite(Sprite sprite)
    {
        if(Application.isMobilePlatform)
        {
            return;
        }
        CursorImage.sprite = sprite;
        SetPivot();
        ImageTween.SetLocalScale(Vector3.one * 1.5f);
        ImageTween.DoLocalScaleTween(Vector3.one, 0.4f, type:EaseType.EaseOutBack);
    }
    private void UpdateCursor(bool IsCheckUI = true)
    {
        IsValid = false;
        if (Pointer.current == null)
        {
            IsUsable = false;
            HideCursorBoxes();
            return;
        }
        Vector3 screenPosition =
            Pointer.current.position.ReadValue();
        screenPosition.z = CursorImage.transform.position.z - cameraController.cam.transform.position.z;
        Vector3 cursorWorldPosition =
            cameraController.cam.ScreenToWorldPoint(screenPosition);
        
        CursorImage.transform.position = cursorWorldPosition;

        if (IsCheckUI == true && EventSystem.current.IsPointerOverGameObject())
        {
            IsUsable = false;
            HideCursorBoxes();
            return;
        }
        UsableItem tool = GetNowTool();

        // 没有工具
        if (tool == null)
        {
            IsUsable = false;
            HideCursorBoxes();
            return;
        }
        CurrentCells = GetNearestCells(gameManager.FarmLandManager.grid,cursorWorldPosition, GetNowTool().size);
        for(int i = 0; i < CurrentCells.Count; i++)
        {
            if (i >= cursorBoxes.Count)
            {
                SpriteRenderer box = Instantiate(cursorBoxPrefab, cursorBoxParent);
                cursorBoxes.Add(box);
            }
            cursorBoxes[i].gameObject.SetActive(true);
            cursorBoxes[i].transform.position = gameManager.FarmLandManager.grid.GetCellCenterWorld(CurrentCells[i]);

            if(tool.CanUseThisCell(CurrentCells[i]))
            {
                cursorBoxes[i].sprite = validSprite;
            }
            else
            {
                cursorBoxes[i].sprite = invalidSprite;
            }
        }
        for(int i = CurrentCells.Count; i < cursorBoxes.Count; i++)
        {
            cursorBoxes[i].gameObject.SetActive(false);
        }
        IsValid = true;
        if (tool.CanUse(CurrentCells))
        {
            IsUsable = true;
        }
        else
        {
            IsUsable = false;
        }
    }
    public bool CheckIfPointerOverUI()
    {
        if (EventSystem.current == null ||
            Pointer.current == null)
            return false;

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Pointer.current.position.ReadValue()
        };
        List<RaycastResult> results = new();
        results.Clear();

        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
    private void HideCursorBoxes()
    {
        foreach (SpriteRenderer box in cursorBoxes)
        {
            box.gameObject.SetActive(false);
        }
    }
    public static List<Vector3Int> GetNearestCells(Grid grid,Vector3 worldPos,Vector2Int size)
    {
        Vector3 localPos = grid.WorldToLocal(worldPos);
        localPos.z = 0;
        Vector3 cellPos = grid.LocalToCellInterpolated(localPos);
        int m = size.x;
        int n = size.y;
        int startX = Mathf.FloorToInt(cellPos.x - (m - 1) * 0.5f);
        int startY = Mathf.FloorToInt(cellPos.y - (n - 1) * 0.5f);
        int z = Mathf.FloorToInt(cellPos.z);
        var result = new List<Vector3Int>(m * n);

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < m; x++)
            {
                result.Add(new Vector3Int(startX + x, startY + y, z));
            }
        }
        return result;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        IsPointerClickUI = CheckIfPointerOverUI();
        UpdateCursor(IsCheckUI: false);
        if (IsPointerClickUI == false && IsValid == true && IsUsable == false)
        {
            GameDataManager.Instance.PlayAudio("Failed");
        }
        UsedCells.Clear();
        if (Application.isMobilePlatform == false)
        {
            ImageTween.DoLocalRotationTween(new Vector3(0, 0, -35), 0.4f, type: EaseType.EaseOutBack);
        }
    }
    private void OnClickUp(InputAction.CallbackContext context)
    {
        UsableItem tool = GetNowTool();
        // 没有工具
        if (tool != null)
        {
            if(tool.Amount <= 0)
            {
                gameManager.hotbarController.RemoveItemById(tool.Id);
            }
        }
        if (Application.isMobilePlatform == false)
        {
            ImageTween.DoLocalRotationTween(new Vector3(0, 0, 0), 0.4f, type: EaseType.EaseOutBack);
        }

    }

    public void SetPivot()
    {
        Sprite sprite = CursorImage.sprite;
        RectTransform rt = CursorImage.rectTransform;

        Vector2 spritePivot = sprite.pivot;
        Vector2 spriteSize = sprite.rect.size;

        rt.pivot = new Vector2(
            spritePivot.x / spriteSize.x,
            spritePivot.y / spriteSize.y
        );
    }
    public void DragToMoveCam()
    {
        if (Pointer.current == null 
            || IsPointerClickUI == true 
            || clickDownAction.action.IsPressed()  ==false
            || gameManager.hotbarController.IsPointer() == false)
        {
            IsDragging = false;
            return;
        }
        Vector2 currentPosition =
                Pointer.current.position.ReadValue();
        if (IsDragging == false)
        {
            IsDragging = true;
            LastPointerPosition = currentPosition;
            return;
        }
        Vector2 delta = currentPosition - LastPointerPosition;
        LastPointerPosition = currentPosition;

        float scale = cameraController.cam.orthographicSize * 2f
                      / cameraController.cam.pixelHeight;

        Vector3 movement =
            new Vector3(-delta.x, -delta.y, 0f) * scale;
        cameraController.MoveCam(movement);
    }
}
