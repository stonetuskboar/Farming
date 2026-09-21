using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : UsableItem
{
    public Axe()
    {
        Id = 3;
    }
    public override bool CanUse(List<Vector3Int> cells)
    {
        foreach (Vector3Int cell in cells)
        {
            if (CanUseThisCell(cell) == false)
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
        else
        {
            BasicCrop crop = farm.GetFarmTileData(cell).cropObject;
            if (crop == null || crop.cropData is not TreeData || crop.IsMature() == false)
            {
                return false;
            }
        }

        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        HashSet<BasicCrop> cropsToHarvest = new HashSet<BasicCrop>();
        foreach (Vector3Int cell in cells)
        {
            BasicCrop crop = farm.GetFarmTileData(cell).cropObject;
            cropsToHarvest.Add(crop);
        }
        foreach (BasicCrop crop in cropsToHarvest)
        {
            if(crop.cropData is not TreeData)
            {
                continue;
            }
            farm.Harvest(crop);
            TreeData data = crop.cropData as TreeData;
            TreeData stumpData = GameDataManager.Instance.cropDataList.GetTreeDataById(data.stumpId);
            if(stumpData != null)
            {
                farm.Plant(crop.TileCells, stumpData);
            }
        }
        GameManager.ConsumeEnergy(6);
    }
}
