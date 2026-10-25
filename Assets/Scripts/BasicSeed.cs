using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BasicSeed : UsableItem
{
    public CropData cropData;
    public int seedId = 0;
    public BasicSeed(int id, int seedId)
    {
        Id = id;
        this.seedId = seedId;
    }
    public override void Init(GameManager manager)
    {
        base.Init(manager);
        cropData = GameDataManager.Instance.cropDataList.GetCropDataById(seedId);
        size = cropData.size;
    }
    public void BaseInit(GameManager manager)
    {
        base.Init(manager);
    }
    public override bool CanUse(List<Vector3Int> cells)
    {
        foreach (Vector3Int cell in cells)
        {
            if(CanUseThisCell(cell) == false)
            {
                return false;
            }
        }
        return true;
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        if (Amount <= 0)
        {
            return false;
        }
        else if (farm.CanFarmHere(cell) == false)
        {
            return false;
        }
        else 
        {
            FarmTileData data = farm.GetFarmTileData(cell);

            if (data.soilState != SoilState.Tilled && data.soilState != SoilState.Watered)
            {
                return false;
            }
            else if (data.cropObject != null)
            {
                return false;
            }
            else if (cropData.IsContainSeason(GameManager.season) == false)
            {
                return false;
            }
        }
        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        GameDataManager.Instance.PlayAudio("Plant");
        FarmLandManager farm = GameManager.FarmLandManager;
        farm.Plant(cells, cropData);
        if (farm.PlantDic.TryAdd(cropData.id, 1) == false)
        {
            farm.PlantDic[cropData.id] += 1;
        }
        GameManager.hotbarController.DecreaseItem(cropData.id,1);
    }
}
