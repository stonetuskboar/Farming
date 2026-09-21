using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoe : UsableItem
{
    public Hoe()
    {
        Id = 0;
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
        else if(farm.GetFarmTileData(cell).soilState != SoilState.Dirt)
        {
            return false;
        }else if(farm.GetFarmTileData(cell).cropObject != null)
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
            farm.Till(cell);
        }
        GameManager.ConsumeEnergy(4);
    }

}
