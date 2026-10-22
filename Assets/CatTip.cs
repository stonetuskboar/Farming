using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatTip : BasicPopWindow
{
    public LayoutGroup layouGroup;
    public TextMeshProUGUI text;
    public Image image;
    public Sprite FarmCat;
    public Sprite PointCat;
    public ImageSwapButton DeclineButton;
    public ImageSwapButton ConfirmButton;
    public List<TipInfos> BeginnerTipList;
    public bool HasStartBeginnerTip = false;
    private int NowIndex = 0;
    public enum Cat
    {
        point,
        farm
    }
    public void SetCat(Cat cat)
    {
        if(cat == Cat.point)
        {
            image.sprite = PointCat;
        }
        else
        {
            image.sprite = FarmCat;
        }
        image.SetNativeSize();
    }

    public void StartShowBeginnerTips()
    {
        ShowBeginnerTips();
    }
    public void ShowBeginnerTips()
    {
        if(NowIndex == 1)
        {
            DeclineButton.Hide(0.2f);
        }
        if (NowIndex >= BeginnerTipList.Count)
        {
            Hide(0.3f);
            return;
        }
        TipInfos info = BeginnerTipList[NowIndex];
        GetRectTransform().anchorMin = info.Anchor;
        GetRectTransform().anchorMax = info.Anchor;
        SetAnchoredPosition(info.position);
        SetCat(info.cat);
        text.text = info.str;
        Show(0.3f);
        if(info.IsLeft == true)
        {
            RectTransform rect = image.rectTransform;
            rect.anchoredPosition = new Vector2(-25,-52);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            layouGroup.childAlignment = TextAnchor.LowerLeft;
        }
        else
        {
            RectTransform rect = image.rectTransform;
            rect.anchoredPosition = new Vector2(77, 59);
            rect.anchorMin = new Vector2(1,0);
            rect.anchorMax = new Vector2(1, 0);
            layouGroup.childAlignment = TextAnchor.LowerRight;
        }
        NowIndex++;
    }
}

[System.Serializable]
public class TipInfos
{
    [TextArea]
    public string str;
    public Vector2 position;
    public Vector2 Anchor = new Vector2(0.5f, 0.5f);
    public CatTip.Cat cat = CatTip.Cat.farm;

    public bool IsLeft = true;
}