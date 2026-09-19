using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem
{
    public int Id;
    public ItemData itemData;
    public GameManager GameManager;
    public  Vector2Int size = new Vector2Int(1, 1);
    public abstract bool CanUse(List<Vector3Int> cells);
    public abstract bool CanUseThisCell(Vector3Int cell);
    public abstract void Use(List<Vector3Int> cells);

    public virtual void Init(GameManager manager)
    {
        GameManager = manager;
        itemData = GameManager.GameDataManager.itemDataList.GetItemDataById(Id);
    }
}
