using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    public BasicUIView uiView;
    public BasicUIView calendarView;
    public TimeView TimeView;
    //屏幕右上角也有一个时间UI
    public TimeView ScreenTimeView;
    public Bill bill;
    public Bill screenBill;
    public EndController endController;
    public GameManager gameManager;

    public YearResult yearResult;

    public void Awake()
    {
        uiView.SetCanvasGroupAlpha(0f);
        uiView.SetCanvasGroupUnblock();
        bill.SetCanvasGroupAlpha(0f);
        calendarView.SetCanvasGroupAlpha(0f);
        TimeView.Set(gameManager.season, gameManager.Month, gameManager.year);
    }

    public void StartShow(bool IsSeasonChange = false)
    {
        uiView.SetCanvasGroupBlockable();
        if(IsSeasonChange == true)
        {
            GameDataManager.Instance.PlayAudio("SeasonChange");
            uiView.DoAlphaTween(1f, 1.5f,OnSeasonPass);
            calendarView.DoDelay(0.6f, ShowCalendar);
        }
        else
        {
            GameDataManager.Instance.PlayAudio("NextTurn");
            uiView.DoAlphaTween(1f, 1f);
            SetTime();
            calendarView.DoDelay(0.5f, ShowCalendar);
            uiView.DoDelay(1.6f, AfterShow);
        }
    }
    public void SetTime()
    {
        TimeView.Set(gameManager.season, gameManager.Month, gameManager.year);
    }
    public void ShowCalendar()
    {
        calendarView.DoAlphaTween(1f,0.3f);
    }

    public void AfterShow()
    {
        ScreenTimeView.Set(gameManager.season, gameManager.Month, gameManager.year);
        gameManager.NextTurn();

        if(gameManager.Month > 9)
        {
            HideCalendar();
            yearResult.Set();
            yearResult.Show();
        }
        else
        {
            Hide();
        }
    }
    public void Hide()
    {
        uiView.SetCanvasGroupUnblock();
        uiView.DoAlphaTween(0f, 0.8f, type: EaseType.EaseInQuad);
        calendarView.DoDelay(0.3f, HideCalendar);
    }
    public void HideCalendar()
    {
        calendarView.DoAlphaTween(0f, 0.2f);
    }

    public void OnSeasonPass()
    {
        calendarView.DoLocalPositionTween(calendarView.transform.localPosition + new Vector3(0,-200,0),0.4f,type:EaseType.EaseInBack,callBack: AfterShake);

    }
    public void AfterShake()
    {
        SetTime();
        calendarView.DoLocalPositionTween(calendarView.transform.localPosition + new Vector3(0, 200, 0), 0.3f, type: EaseType.EaseOutBack,callBack: AfterShow);
    }
    public void ShowBill(bool IsSeasonChange = true)
    {
        screenBill.Set(gameManager.GetDebt());
        bill.Set(gameManager.GetDebt());
        bill.SetCanvasGroupAlpha(0f);
        bill.DoAlphaTween(1f,0.3f);
        bill.SetAnchoredPosition(bill.OgAnchorPosition + new Vector2(-180,0));
        bill.DoAnchoredPositionTween(bill.OgAnchorPosition,0.4f, type:EaseType.EaseOutBack);
        bill.DoDelay(1f, () => { HideBill(IsSeasonChange); });
    }
    public void HideBill(bool IsSeasonChange = true)
    {
        StartShow(IsSeasonChange);
        bill.DoAlphaTween(0f, 0.3f);
        bill.DoAnchoredPositionTween(bill.OgAnchorPosition + new Vector2(-180, 0), 0.3f ,type:EaseType.EaseInBack);
    }
}
