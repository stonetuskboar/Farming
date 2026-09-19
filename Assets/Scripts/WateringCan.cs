using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : UsableItem
{
    public WateringCan()
    {
        Id = 1;
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
        else if (farm.GetFarmTileData(cell).soilState != SoilState.Tilled)
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
            farm.Water(cell);
        }
        GameManager.ConsumeEnergy(2);
    }

}
