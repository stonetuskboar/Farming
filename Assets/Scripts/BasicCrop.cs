using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCrop : MonoBehaviour
{
    public CropData cropData;
    public SpriteRenderer cropSr;
    public SpriteRenderer shadowSr;
    //这个作物占据哪几块地
    public List<Vector3Int> TileCells = new();
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
        TileCells.Clear();
        TileCells.AddRange(tilePositions);
        growthProgress = 0;
        SetByGrowthProgress();
    }

    public void SetFarmLand(FarmLandManager farmLand)
    {
        this.farmLandManager = farmLand;
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
        if(currentStage.shadow != null)
        {
            shadowSr.enabled = true;
            shadowSr.sprite = currentStage.shadow;
        }
        else
        {
            shadowSr.enabled = false;
        }
    }
    public virtual void OnNextTurn()
    {
        bool IsWatered = true;
        for (int i = 0; i < TileCells.Count; i++)
        {
            if (farmLandManager.GetFarmTileData(TileCells[i]).soilState != SoilState.Watered)
            {
                IsWatered = false;
                break;
            }
        }
        if (IsWatered == true || cropData is TreeData)
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
