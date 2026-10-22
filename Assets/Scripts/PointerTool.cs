using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTool : UsableItem
{

    public PointerTool()
    {
        Id = 51;
        size = new Vector2Int(0,0);
    }
    public override bool CanUse(List<Vector3Int> cells)
    {
        return true;
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        return;
    }

}
