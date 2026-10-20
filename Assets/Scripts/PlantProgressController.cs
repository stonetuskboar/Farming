using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlantProgressController : MonoBehaviour
{
    public BasicUIView UiView;
    public PlantProgress Prefab;
    public List<PlantProgress> PlantProgressList = new List<PlantProgress>();

    public TextMeshProUGUI SeasonText;
    public TextMeshProUGUI GrowthTimeText;
    public RectTransform LayoutRect;


    public void Set(CropData data)
    {
        SeasonText.text = data.GetSeasonText();
        GrowthTimeText.text = data.matureTurns + "月";
        List<GrowthStage> stages = data.growthStages;
        if(stages.Count > PlantProgressList.Count)
        {
            for (int i = PlantProgressList.Count; i < stages.Count; i++)
            {
                CreateProgress();
            }
        }

        for(int i = 0; i < stages.Count; i++)
        {
            PlantProgressList[i].gameObject.SetActive(true);
            PlantProgressList[i].image.sprite = stages[i].GrowthSprite;
        }
        for(int i = stages.Count; i < PlantProgressList.Count; i++)
        {
            PlantProgressList[i].gameObject.SetActive(false);
        }
    }

    public PlantProgress GetProgress()
    {
        for(int i = 0; i < PlantProgressList.Count; i++)
        {
            if (PlantProgressList[i].gameObject.activeSelf == false)
            {
                PlantProgressList[i].gameObject.SetActive(true);
                return PlantProgressList[i];
            }
        }
        return CreateProgress();
    }
    public PlantProgress CreateProgress()
    {
        PlantProgress newProgress = Instantiate(Prefab, LayoutRect);
        newProgress.gameObject.SetActive(false);
        PlantProgressList.Add(newProgress);
        return newProgress;
    }

    public void Hide(float time)
    {
        UiView.DoAlphaTween(0f, time);
    }
    public void Show(float time)
    {
        UiView.DoAlphaTween(1f, time);
    }
}
