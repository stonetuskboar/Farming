
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridCursorController : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    [Header("Cursor")]
    [SerializeField] private Transform cursorBoxParent;
    [SerializeField] private SpriteRenderer cursorBoxPrefab;
    // 当前生成出来的 CursorBox
    private readonly List<SpriteRenderer> cursorBoxes = new();
    [SerializeField] private Sprite validSprite;
    [SerializeField] private Sprite invalidSprite;
    [Header("Input Actions")]
    [SerializeField] private InputActionReference pointAction;
    [SerializeField] private InputActionReference clickAction;

    public Image CursorImage;
    // 曾经用过的格子
    private List<Vector3Int> UsedCells = new();
    // 当前范围内所有格子
    public List<Vector3Int> CurrentCells { get; private set; } = new();
    public bool IsValid { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
    }
    private void OnDestroy()
    {
        Cursor.visible = true;
    }
    private void OnEnable()
    {
        pointAction.action.Enable();
        clickAction.action.Enable();
        clickAction.action.performed += OnClick;
    }



    private void OnDisable()
    {
        clickAction.action.performed -= OnClick;
        pointAction.action.Disable();
        clickAction.action.Disable();
    }

    public UsableItem GetNowTool()
    {
        return gameManager.hotbarController.NowTool;
    }
    private void Update()
    {
        UpdateCursor();

        if(clickAction.action.IsPressed() && IsValid && false == UsedCells.SequenceEqual(CurrentCells))
        {
            UsedCells.Clear();
            UsedCells.AddRange(CurrentCells);
            GetNowTool().Use(CurrentCells);
        }
    }
    public void ChangeCursorSprite(Sprite sprite)
    {
        CursorImage.sprite = sprite;
    }
    private void UpdateCursor()
    {
        if (Pointer.current == null)
        {
            IsValid = false;
            HideCursorBoxes();
            return;
        }
        Vector2 screenPosition =
            Pointer.current.position.ReadValue();
        Vector3 cursorWorldPosition =
            mainCamera.ScreenToWorldPoint(screenPosition);
        cursorWorldPosition.z = 0f;
        CursorImage.transform.position = cursorWorldPosition;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            IsValid = false;
            HideCursorBoxes();
            return;
        }
        UsableItem tool = GetNowTool();

        // 没有工具
        if (tool == null)
        {
            IsValid = false;
            HideCursorBoxes();
            return;
        }
        CurrentCells = GetNearestCells(gameManager.FarmLandManager.grid, cursorWorldPosition, GetNowTool().size.x, GetNowTool().size.y);

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

        if (tool.CanUse(CurrentCells))
        {
            IsValid = true;
        }
        else
        {
            IsValid = false;
        }
    }

    private void HideCursorBoxes()
    {
        foreach (SpriteRenderer box in cursorBoxes)
        {
            box.gameObject.SetActive(false);
        }
    }
    public static List<Vector3Int> GetNearestCells(Grid grid,Vector3 worldPos,int m,int n)
    {
        Vector3 localPos = grid.WorldToLocal(worldPos);
        Vector3 cellPos = grid.LocalToCellInterpolated(localPos);
        

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
        UsedCells.Clear();
    }
}
