using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuySeedPage : MonoBehaviour
{
    public List<SeedItemUI> SeedItemUIList = new();
    public List<int> SeedIdList = new();
    public BuyingSeedPanel buyingSeedPanel;
    public Image Icon;
    public BasicColorTweenObject IconTween;
    public TextMeshProUGUI PlantName;
    public BasicColorTweenObject NameTextTween;
    public int NowPage = 0;
    public TextMeshProUGUI PageText;
    public PlantProgressController plantProgressController;
    public GameManager gameManager;
    [NonSerialized]
    public int SelectedItemId = -1;
    public void Awake()
    {
        for (int i = 0; i < SeedItemUIList.Count; i++)
        {
            SeedItemUIList[i].Init(this);
        }
        SetPageText();
        buyingSeedPanel.ConfirmButton.PointerClicked += BuySeed;
    }
    public void OnEnable()
    {
        FreshButton();
        HidePlantImmediately();
    }
    public void FreshButton()
    {
        CheckPage();
        int startIndex = NowPage * SeedItemUIList.Count;
        int index = SeedItemUIList.Count - startIndex;
        for (int i = 0; (i + startIndex) < SeedIdList.Count && i < SeedItemUIList.Count; i++)
        {
            SeedItemUIList[i].gameObject.SetActive(true);
            ItemData data = gameManager.GameDataManager.itemDataList.GetItemDataById(SeedIdList[i + startIndex]);
            int amount = 0;
            BasicItem item = gameManager.hotbarController.FindItemById(SeedIdList[i + startIndex]);
            if (item != null)
            {
                amount = item.Amount;
            }
            SeedItemUIList[i].Set(data, amount);
            if (SeedItemUIList[i].Id == SelectedItemId)
            {
                SeedItemUIList[i].Button.ChangeStateToSelectedImmediately();
            }
            else
            {
                SeedItemUIList[i].Button.ChangeStateToIdleImmediately();
            }
        }
        for (int i = SeedIdList.Count - startIndex; i >= 0 && i < SeedItemUIList.Count; i++)
        {
            SeedItemUIList[i].gameObject.SetActive(false);
        }
    }

    public void IncreasePage()
    {

        if ((NowPage + 1) * SeedItemUIList.Count < SeedIdList.Count)
        {
            NowPage++;
            SetPageText();
            FreshButton();
        }
    }
    public void DecreasePage()
    {
        if (NowPage > 0)
        {
            NowPage--;
            SetPageText();
            FreshButton();
        }
    }
    public void CheckPage()
    {
        //现在是页面2（0，1，2），页面3用来显示25-36物品，如果物品数大于24，则正常。小于等于24则要调整
        if (NowPage * SeedItemUIList.Count >= SeedIdList.Count)
        {
            NowPage = (SeedIdList.Count - 1 )/ SeedItemUIList.Count;
            SetPageText();
        }
        else if (NowPage < 0)
        {
            NowPage = 0;
            SetPageText();
        }
    }
    public void SetPageText()
    {
        PageText.text = (NowPage + 1).ToString();
    }
    public void HidePlantImmediately()
    {
        ResetBuyingAmount();
        buyingSeedPanel.ConfirmButton.Hide(0f);
        IconTween.SetAlpha(0);
        NameTextTween.SetAlpha(0);
        plantProgressController.Hide(0f);
        FindUIById(SelectedItemId)?.Button.ChangeStateToIdleImmediately();
        SelectedItemId = -1;
    }
    public void ResetBuyingAmount()
    {
        buyingSeedPanel.Set(0,0);
        buyingSeedPanel.SetAmount(0);
    }
    public void HidePlantInfo()
    {
        buyingSeedPanel.ConfirmButton.Hide(0.3f);
        IconTween.DoAlphaTween(0f, 0.3f);
        NameTextTween.DoAlphaTween(0f, 0.3f);
        plantProgressController.Hide(0.3f);
        SelectedItemId = -1;
    }
    public void HoverPlantInfo(int id)
    {
        CropData data = GameDataManager.Instance.cropDataList.GetCropDataById(id);
        if(data == null)
        {
            data = GameDataManager.Instance.cropDataList.GetTreeDataById(id);
        }
        ItemData item = GameDataManager.Instance.itemDataList.GetItemDataById(data.DropedItemId);
        Icon.sprite = item.Icon;
        PlantName.text = data.Name;
        TweenShow(IconTween);
        TweenShow(NameTextTween);
        plantProgressController.Set(data);
        plantProgressController.Show(0.2f);
        buyingSeedPanel.Set(item.Price, gameManager.Money / item.Price);
        buyingSeedPanel.SetAmount(1);
    }
    public void SelectPlant(int Id)
    {
        CropData data = GameDataManager.Instance.cropDataList.GetCropDataById(Id);
        if (data == null)
        {
            data = GameDataManager.Instance.cropDataList.GetTreeDataById(Id);
        }
        ItemData item = GameDataManager.Instance.itemDataList.GetItemDataById(data.DropedItemId);
        buyingSeedPanel.Set(item.Price, gameManager.Money / item.Price);
        Icon.sprite = item.Icon;
        PlantName.text = data.Name;
        plantProgressController.Set(data);
        plantProgressController.Show(0.2f);
        if (SelectedItemId != -1)
        {
            TweenShow(IconTween);
            TweenShow(NameTextTween);
            FindUIById(SelectedItemId)?.Button.ChangeStateToIdle();
        }
        else
        {
            TweenSelectedShow(IconTween);
            TweenSelectedShow(NameTextTween);
            buyingSeedPanel.ConfirmButton.Show(0.3f);
        }
        SelectedItemId = Id;
    }

    public void TweenShow(BasicColorTweenObject tweenObj)
    {
        tweenObj.SetLocalRotation( Quaternion.Euler(-30, 25, 0));
        tweenObj.SetLocalScale( 0.8f * Vector3.one);
        tweenObj.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoAlphaTween(1f,0.2f);
    }
    public void TweenSelectedShow(BasicColorTweenObject tweenObj)
    {
        tweenObj.DoLocalRotationTween(Quaternion.identity, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoLocalScaleTween(Vector3.one, 0.3f, type: EaseType.EaseOutBack);
        tweenObj.DoAlphaTween(1f, 0.2f);
    }
    public void BuySeed()
    {
        if(buyingSeedPanel.currentAmount <= 0)
        {
            return;
        }
        gameManager.hotbarController.AddItem(SelectedItemId, buyingSeedPanel.currentAmount);
        ItemData data = GameDataManager.Instance.itemDataList.GetItemDataById(SelectedItemId);
        gameManager.ChangeMoney(-buyingSeedPanel.GetValue());
        buyingSeedPanel.Set(data.Price, gameManager.Money / data.Price);
        buyingSeedPanel.SetAmount(buyingSeedPanel.currentAmount);
        GameDataManager.Instance.PlayAudio("BuySucess");
        SeedItemUI ui = FindUIById(SelectedItemId);
        if (FindUIById(SelectedItemId) != null)
        {
            ui.Button.SetLocalRotation(Quaternion.identity);
            ui.Button.SetLocalScale(Vector3.one);
            ui.Button.DoLocalRotationTween(Quaternion.Euler(30, -25, 0), 0.4f, type: EaseType.PunchDefault);
            ui.Button.DoLocalScaleTween(Vector3.one * 1.3f, 0.6f, type: EaseType.ShakeDefault);
        }
    }

    public SeedItemUI FindUIById(int id)
    {
        for (int i = 0; i < SeedItemUIList.Count; i++)
        {
            if (SeedItemUIList[i].Id == id && SeedItemUIList[i].gameObject.activeInHierarchy == true)
            {
                return SeedItemUIList[i];
            }
        }
        return null;
    }
}
