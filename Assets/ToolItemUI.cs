using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolItemUI : MonoBehaviour
{
    public ToolItem toolItem;
    public Image icon;
    [NonSerialized]
    public UpgradeToolPage toolPage;

    public ImageSwapButton Button;

    public void Init(UpgradeToolPage page)
    {
        this.toolPage = page;
    }
    public void Set(ToolItem item)
    {
        this.toolItem = item;
        Sprite sprite = item.itemData.Icon;
        icon.sprite = sprite;
    }
    public void Start()
    {
        Button.PointerEntered += OnPointerEnter;
        Button.PointerExited += OnPointerExit;
        Button.PointerClicked += SelectThis;
    }
    public void OnPointerExit()
    {
        if (toolPage.NowSelectedTool == null)
        {
            toolPage.HideToolInfo();
        }

    }
    public void OnPointerEnter()
    {
        if (toolPage.NowSelectedTool == null)
        {
            toolPage.HoverTool(this.toolItem);
        }
    }
    public void SelectThis()
    {
        if (toolPage.NowSelectedTool != this.toolItem)
        {
            toolPage.SelectTool(this.toolItem);
            Button.ChangeStateToSelected();
        }
        else
        {
            toolPage.HideToolInfo();
            Button.ChangeStateToHover();
        }
    }

}
