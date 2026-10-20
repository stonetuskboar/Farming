using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolItem : UsableItem
{
    public int EnergyValue = 0;

    public override bool CanUse(List<Vector3Int> cells)
    {
        if (GameManager.NowEnergy < EnergyValue)
        {
            return false;
        }
        foreach (Vector3Int cell in cells)
        {
            if (CanUseThisCell(cell) == false)
            {
                return false;
            }
        }
        return true;
    }
}
