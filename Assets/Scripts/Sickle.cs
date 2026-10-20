using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sickle : ToolItem
{
    public Sickle()
    {
        Id = 3;
        EnergyValue = 4;
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
            if (crop == null || crop.cropData is TreeData)
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
        bool IsStone = false; //临时写的
        foreach (BasicCrop crop in cropsToHarvest)
        {
            farm.Harvest(crop);
            //临时写的，硬编码
            if(crop.cropData.id == 37 || crop.cropData.id == 38 || crop.cropData.id == 40)
            {
                IsStone = true;
            }
        }
        if(IsStone == true)
        {
            GameDataManager.Instance.PlayAudio("MineRock");
        }
        else
        {
            GameDataManager.Instance.PlayAudio("Harvest");
        }

        GameManager.ConsumeEnergy(EnergyValue);
    }
}
