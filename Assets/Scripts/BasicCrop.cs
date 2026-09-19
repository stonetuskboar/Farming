using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCrop : MonoBehaviour
{
    public CropData cropData;
    public SpriteRenderer cropSr;
    public SpriteRenderer shadowSr;
    //这个作物占据哪几块地
    public List<Vector3Int> TilePosition = new();
    public int growthProgress;
    private FarmLandManager farmLandManager;
    public void Init(FarmLandManager farmLandManager, CropData data, Vector3Int tilePositions)
    {
        Init(farmLandManager, data, new List<Vector3Int> { tilePositions });
    }
    public void Init(FarmLandManager farmLandManager, CropData data, List<Vector3Int> tilePositions)
    {
        this.farmLandManager = farmLandManager;
        cropData = data;
        TilePosition = tilePositions;
        growthProgress = 0;
        SetByGrowthProgress();
    }

    public void SetByGrowthProgress()
    {
        GrowthStage currentStage = null;

        foreach (GrowthStage stage in cropData.growthStages)
        {
            // 找到当前进度下，已经达到的最高阶段
            if (growthProgress >= stage.GrowthProgress)
            {
                if (currentStage == null || stage.GrowthProgress > currentStage.GrowthProgress)
                {
                    currentStage = stage;
                }
            }
        }
        cropSr.sprite = currentStage.GrowthSprite;
        shadowSr.sprite = currentStage.shadow;
    }
    public void OnNextTurn()
    {
        bool IsWatered = true;
        for (int i = 0; i < TilePosition.Count; i++)
        {
            if (farmLandManager.GetFarmTileData(TilePosition[i]).soilState != SoilState.Watered)
            {
                IsWatered = false;
                break;
            }
        }
        if (cropData is TreeData ||  IsWatered == true)
        {
            growthProgress++;
            SetByGrowthProgress();
        }
    }

    public bool IsMature()
    {
        return growthProgress >= cropData.matureTurns;
    }


}
