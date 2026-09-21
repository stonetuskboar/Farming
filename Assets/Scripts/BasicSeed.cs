using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BasicSeed : UsableItem
{
    public CropData data;
    public int seedId = 0;
    public BasicSeed(int id, int seedId)
    {
        Id = id;
        this.seedId = seedId;
    }
    public override void Init(GameManager manager)
    {
        base.Init(manager);
        data = GameManager.GameDataManager.cropDataList.GetCropDataById(seedId);
        size = data.size;
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
        if (farm.CanFarmHere(cell) == false)
        {
            return false;
        }
        //else if (farm.GetFarmTileData(cell).soilState == SoilState.Dirt)
        //{
        //    return false;
        //}
        else if (farm.GetFarmTileData(cell).cropObject != null)
        {
            return false;
        }
        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        foreach (Vector3Int cell in cells)
        {
            farm.Plant(cell, data);
        }
    }

}
