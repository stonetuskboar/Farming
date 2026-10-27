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
    public CropShaderController cropShaderController;
    public CropShaderController shadowShaderController;
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
        cropSr.sprite = GetSeasonSprite(currentStage);
        cropShaderController.UpdateWindForce(currentStage.风力);
        shadowShaderController.UpdateWindForce(currentStage.风力);
        if (currentStage.shadow != null)
        {
            shadowSr.enabled = true;
            shadowSr.sprite = currentStage.shadow;
        }
        else
        {
            shadowSr.enabled = false;
        }
    }
    public void SetSeason()
    {

    }
    public virtual void OnNextTurn(int year, Season season,int month)
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
        if (IsMature() == false && cropData.IsContainSeason(season) == false)
        {
            farmLandManager.ReplaceToWiltPlant(this);
        }
    }

    public bool IsMature()
    {
        return growthProgress >= cropData.matureTurns;
    }

    // 按当前季节取四季贴图；没有配四季图就用成长阶段图
    Sprite GetSeasonSprite(GrowthStage stage)
    {
        if (cropData.seasonSprites != null && cropData.seasonSprites.Count >= 4 && farmLandManager != null && farmLandManager.gameManager != null)
        {
            int idx = (int)farmLandManager.gameManager.season;   // 春0 夏1 秋2 冬3
            if (idx >= 0 && idx < cropData.seasonSprites.Count)
            {
                Sprite s = cropData.seasonSprites[idx];
                if (s != null) return s;
            }
        }
        return stage.GrowthSprite;
    }

}
