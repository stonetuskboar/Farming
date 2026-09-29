using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeedSeed : BasicSeed
{
    public BasicWeedSeed(int id, int seedId) : base(id, seedId)
    {
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        if (farm.CanFarmHere(cell) == false)
        {
            return false;
        }
        //else if (farm.GetFarmTileData(cell).soilState != SoilState.Tilled && farm.GetFarmTileData(cell).soilState != SoilState.Watered)
        //{
        //    return false;
        //}
        else if (farm.GetFarmTileData(cell).cropObject != null)
        {
            return false;
        }
        return true;
    }
}
