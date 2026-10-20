using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultItem : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI NumberText;


    public void Set(Sprite icon,int num)
    {
        Icon.sprite = icon;
        NumberText.text = num.ToString();
    }
}
