using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bill : BasicUIView
{
    public TextMeshProUGUI YearText;
    public TextMeshProUGUI MoneyText;
    public Vector2 OgAnchorPosition;

    public void Start()
    {
        OgAnchorPosition = GetAnchoredPosition();
    }

    public void Set(int num, int year)
    {
        MoneyText.text = num.ToString();
        if (year == 1)
        {
            YearText.text = "一";
        }
        else if (year == 2)
        {
            YearText.text = "二";
        }
        else if (year == 3)
        {
            YearText.text = "三";
        }
        else if (year == 4)
        {
            YearText.text = "四";
        }
    }
}
