using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : ToolItem
{
    public WateringCan()
    {
        Id = 1;
        EnergyValue = 2;
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
        else if (farm.GetFarmTileData(cell).soilState != SoilState.Tilled)
        {
            return false;
        }
        else
        {
            BasicCrop crop = farm.GetFarmTileData(cell).cropObject;
            if(crop != null)
            {
                if (crop.IsMature() == true)
                {
                    return false;
                }
                else if (crop.cropData is TreeData)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        foreach (Vector3Int cell in cells)
        {
            farm.Water(cell);
        }
        GameManager.ConsumeEnergy(EnergyValue);
        GameDataManager.Instance.PlayAudio("Water");
    }

}
