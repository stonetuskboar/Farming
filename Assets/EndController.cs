using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndController : MonoBehaviour,IPointerClickHandler
{
    public Color FailedColor;
    public Color SucessColor;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI MoneyText;
    public Image RaycastImage;
    public BasicUIView ResultView;
    public BasicUIView ThanksPlay;
    public BasicUIView GoodEnding;
    public BasicUIView BadEnding;

    private bool IsWining = false;
    public GameManager gameMnager;

    private State state = State.Hide;

    enum State
    {
        Hide,
        ResultView,
        Ending,
        ThanksPlay
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(state == State.ResultView)
        {
            ShowEnding();
        }else if(state == State.Ending)
        {
            ShowThanksPlay();
        }else if(state == State.ThanksPlay)
        {
            state = State.Hide;
        }
    }

    public void Awake()
    {
        ResultView.SetCanvasGroupAlpha(0f);
        ResultView.SetCanvasGroupUnblock();
        GoodEnding.SetCanvasGroupAlpha(0f);
        GoodEnding.SetCanvasGroupUnblock();
        BadEnding.SetCanvasGroupAlpha(0f);
        BadEnding.SetCanvasGroupUnblock();
        ThanksPlay.SetCanvasGroupAlpha(0f);
        ThanksPlay.SetCanvasGroupUnblock();
        RaycastImage.raycastTarget = false;
    }
    public void ShowResulView()
    {
        gameMnager.hotbarController.OnSwitchToCursor();
        GameDataManager.Instance.PlayAudio("StopSeasonAudio");
        GameDataManager.Instance.PlayAudio("EndingMusic");
        state = State.ResultView;
        MoneyText.text = gameMnager.Money.ToString();
        debtText.text = gameMnager.GetDebt().ToString();
        if(gameMnager.CouldPaidTheBill() == false)
        {
            IsWining = false;
            MoneyText.color = FailedColor;
        }
        else
        {
            IsWining = true;
            MoneyText.color = SucessColor;
        }
        SetRaycastFalse();
        Vector2 OgAnchor = ResultView.GetAnchoredPosition();
        ResultView.SetAnchoredPosition(OgAnchor + new Vector2(0,-200));
        ResultView.DoAnchoredPositionTween(OgAnchor,0.6f,type:EaseType.EaseOutBack ,callBack: SetRaycastTrue);
        ResultView.DoAlphaTween(1f,0.5f);
    }
    public void ShowEnding()
    {
        SetRaycastFalse();
        ResultView.SetCanvasGroupAlpha(0f);
        state = State.Ending;
        if(IsWining == true)
        {
            GameDataManager.Instance.PlayAudio("BuySucess");
            Vector2 OgAnchor = GoodEnding.GetAnchoredPosition();
            GoodEnding.SetAnchoredPosition(OgAnchor + new Vector2(0, -200));
            GoodEnding.DoAnchoredPositionTween(OgAnchor, 0.4f, type: EaseType.EaseOutBack, callBack: SetRaycastTrue);
            GoodEnding.DoAlphaTween(1f, 0.3f);
        }
        else
        {
            GameDataManager.Instance.PlayAudio("GameOver");
            Vector2 OgAnchor = BadEnding.GetAnchoredPosition();
            BadEnding.SetAnchoredPosition(OgAnchor + new Vector2(0, -200));
            BadEnding.DoAnchoredPositionTween(OgAnchor, 0.4f, type: EaseType.EaseOutBack, callBack: SetRaycastTrue);
            BadEnding.DoAlphaTween(1f, 0.3f);
        }
    }
    public void ShowThanksPlay()
    {
        GameDataManager.Instance.PlayAudio("Popup");
        SetRaycastFalse();
        GoodEnding.SetCanvasGroupAlpha(0f);
        BadEnding.SetCanvasGroupAlpha(0f);
        state = State.ThanksPlay;
        Vector2 OgAnchor = ThanksPlay.GetAnchoredPosition();
        ThanksPlay.SetAnchoredPosition(OgAnchor + new Vector2(0, -200));
        ThanksPlay.DoAnchoredPositionTween(OgAnchor, 0.4f, type: EaseType.EaseOutBack, callBack: SetRaycastTrue);
        ThanksPlay.DoAlphaTween(1f, 0.3f);
        ThanksPlay.SetCanvasGroupBlockable();

    }
    public void SetRaycastTrue()
    {
        RaycastImage.raycastTarget = true;
    }
    public void SetRaycastFalse()
    {
        RaycastImage.raycastTarget = false;
    }
    public void Restart()
    {
        GameDataManager.Instance.PlayAudio("StopEndingMusic");
        SceneManager.LoadScene("GameScene");
    }

}
