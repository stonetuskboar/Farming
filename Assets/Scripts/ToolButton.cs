using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ToolButton : ImageSwapButton
{
    public Image ToolIcon;
    public GameObject AmountObj;
    public TextMeshProUGUI AmountText;
    private HotBarController ToolController;
    [NonSerialized]
    public int itemId;
    protected override void Awake()
    {
        base.Awake();
        PointerClicked += OnClick;
    }

    public void Init(HotBarController controller)
    {
        ToolController = controller;
    }
    public void Set( BasicItem item)
    {
        itemId = item.itemData.Id;
        ToolIcon.sprite = item.itemData.Icon;
        if(item is BasicSeed)
        {
            AmountText.text = item.Amount.ToString();
            AmountObj.gameObject.SetActive(true);
        }
        else
        {
            AmountObj.gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        ToolController.SelectNowTool(this);
    }
}
