using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeedItemUI : MonoBehaviour
{
    public int Id;
    public Image icon;
    public TextMeshProUGUI Amount;
    [NonSerialized]
    public BuySeedPage buySeedPage;

    public ImageSwapButton Button;

    public void Init(BuySeedPage page)
    {
        this.buySeedPage = page;
    }
    public void Set(ItemData data,int amount)
    {
        this.Id = data.Id;
        icon.sprite = data.Icon;
        Amount.text = amount.ToString();
    }
    public void Start()
    {
        Button.PointerEntered += OnPointerEnter;
        Button.PointerExited += OnPointerExit;
        Button.PointerClicked += SelectThis;
    }
    public void OnPointerExit()
    {
        if(buySeedPage.SelectedItemId == -1)
        {
            buySeedPage.HidePlantInfo();
        }

    }
    public void OnPointerEnter()
    {
        if (buySeedPage.SelectedItemId == -1)
        {
            buySeedPage.HoverPlantInfo(Id);
        }
    }
    public void SelectThis()
    {
        if (buySeedPage.SelectedItemId != Id)
        {
            buySeedPage.SelectPlant(Id);
            Button.ChangeStateToSelected();
        }
        else
        {
            buySeedPage.HidePlantInfo();
            Button.ChangeStateToHover();
        }
    }

}
