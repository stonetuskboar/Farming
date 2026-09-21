using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem:BasicItem
{
    public  Vector2Int size = new Vector2Int(1, 1);
    public abstract bool CanUse(List<Vector3Int> cells);
    public abstract bool CanUseThisCell(Vector3Int cell);
    public abstract void Use(List<Vector3Int> cells);

}
