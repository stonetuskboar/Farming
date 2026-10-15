using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextColorTweenObj : BasicColorTweenObject
{
    public TextMeshProUGUI text;
    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if(text == null )
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        graphic = text;
        SetToNormal();
    }
    //对于normal状态和hover状态透明度不一样的文字，会出现显示bug，但我太懒了，以后出问题再改吧
    public void SetTextWithAlpha(string str)
    {
        if (gameObject.activeInHierarchy == true && str != text.text)
        {
            float a = text.color.a;
            SetAlpha(text.color.a / 3);
            DoAlphaTween(normalColor.a, 0.3f);
        }
        SetText(str);
    }

    public virtual void SetText(string str)
    {
        text.text = str;
    }
}
