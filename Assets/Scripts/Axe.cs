using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : ToolItem
{
    public Axe()
    {
        Id = 4;
        EnergyValue = 6;
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        if (GameManager.NowEnergy < EnergyValue)
        {
            return false;
        }
        FarmLandManager farm = GameManager.FarmLandManager;
        if (farm.CanFarmHere(cell) == false)
        {
            return false;
        }
        else
        {
            BasicCrop crop = farm.GetFarmTileData(cell).cropObject;
            if (crop == null || crop.IsMature() == false || crop.cropData is not TreeData)
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
            if(crop.IsMature() == true)
            {
                TreeData data = crop.cropData as TreeData;
                TreeData stumpData = GameDataManager.Instance.cropDataList.GetTreeDataById(data.stumpId);
                if (stumpData != null)
                {
                    farm.Plant(crop.TileCells, stumpData, crop.transform.position);
                }
            }
        }
        GameDataManager.Instance.PlayAudio("ChopTree");
        GameManager.ConsumeEnergy(EnergyValue);
    }
}
