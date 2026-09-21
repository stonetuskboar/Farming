using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicItem
{
    public int Id;
    public int Amount;
    public ItemData itemData;

    public GameManager GameManager;

    public void SetId(int id)
    {
        Id = id;
    }
    public virtual void Init(GameManager manager)
    {
        GameManager = manager;
        itemData = GameManager.GameDataManager.itemDataList.GetItemDataById(Id);
    }
}
