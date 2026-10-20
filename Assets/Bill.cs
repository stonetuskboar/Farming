using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bill : BasicUIView
{
    public TextMeshProUGUI MoneyText;
    public Vector2 OgAnchorPosition;

    public void Start()
    {
        OgAnchorPosition = GetAnchoredPosition();
    }

    public void Set(int num)
    {
        MoneyText.text = num.ToString();
    }
}
