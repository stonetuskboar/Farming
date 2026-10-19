using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public BasicUIView uiView;
    public BasicUIView calendarView;
    public TimeView TimeView;
    public TimeView ScreenTimeView;
    public GameManager gameManager;

    public void Awake()
    {
        uiView.SetCanvasGroupAlpha(0f);
        uiView.SetCanvasGroupUnblock();
        calendarView.SetCanvasGroupAlpha(0f);
        TimeView.Set(gameManager.season, gameManager.Month, gameManager.year);
    }

    public void StartShow()
    {
        uiView.SetCanvasGroupBlockable();
        if(gameManager.Month % 3 == 0)
        {
            gameManager.TimePast();
            uiView.DoAlphaTween(1f, 1.5f,OnSeasonPass);
            calendarView.DoDelay(0.6f, ShowCalendar);
        }
        else
        {
            gameManager.TimePast();
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
        uiView.SetCanvasGroupUnblock();
        uiView.DoAlphaTween(0f, 0.8f,type:EaseType.EaseInQuad);
        calendarView.DoDelay(0.3f,HideCalendar);
    }
    public void HideCalendar()
    {
        calendarView.DoAlphaTween(0f, 0.2f);
    }

    public void OnSeasonPass()
    {
        GameDataManager.Instance.PlayAudio("SeasonChange");
        calendarView.DoLocalPositionTween(calendarView.transform.localPosition + new Vector3(0,-200,0),0.4f,type:EaseType.EaseInBack,callBack: AfterShake);

    }
    public void AfterShake()
    {
        SetTime();
        calendarView.DoLocalPositionTween(calendarView.transform.localPosition + new Vector3(0, 200, 0), 0.3f, type: EaseType.EaseOutBack,callBack: AfterShow);
    }

}
