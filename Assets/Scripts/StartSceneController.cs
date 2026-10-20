using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    public BasicUIView UIView;
    public CutsceneController cutScene;
    private Vector2 OgAnchorPosition;
    public GameManager gameManager;
    public GameObject UI;

    public void Start()
    {
        OgAnchorPosition = UIView.GetAnchoredPosition();
        GameDataManager.Instance.PlayAudio("StartMusic");
        UI.SetActive(false);
    }

    public void OnExitButton()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer =
                   new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject activity =
                       unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call<bool>("moveTaskToBack", true);
                }
            }
    #else
            Application.Quit();
    #endif
    }

    public void OnStartButtonClick()
    {
        Vector2 targetPosition = OgAnchorPosition;
        targetPosition.y -= UIView.GetRectTransform().sizeDelta.y;
        UIView.DoAnchoredPositionTween(targetPosition ,0.6f);
        UIView.DoDelay(0.2f,cutScene.StartShow);
        UIView.SetCanvasGroupUnblock();
        cutScene.EndAction = ShowBill;

    }

    public void ShowBill()
    {
        gameManager.GameStart();
        cutScene.Hide();
        UI.SetActive(true);
        UIView.gameObject.SetActive(false);
        gameManager.transitionController.uiView.SetCanvasGroupAlpha(1f);
        gameManager.transitionController.uiView.SetCanvasGroupBlockable();
        gameManager.transitionController.ShowBill(IsSeasonChange: false);
        gameManager.transitionController.uiView.DoDelay(2f,PlayStartMusic);
    }

    public void PlayStartMusic()
    {
        GameDataManager.Instance.PlayAudio("StopStartMusic");
        GameDataManager.Instance.PlayAudio("SeasonAudio");
    }
}
