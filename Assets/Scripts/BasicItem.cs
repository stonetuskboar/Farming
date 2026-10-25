using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicItem
{
    //用Id查找itemData，和itemData里的id一致
    public int Id;
    public int Amount;
    public ItemData itemData;

    public GameManager GameManager;

    public void SetId(int id)
    {
        Id = id;
    }
    public void SetAmount(int amount)
    {
        Amount = amount;
    }
    public virtual void Init(GameManager manager)
    {
        GameManager = manager;
        itemData = GameDataManager.Instance.itemDataList.GetItemDataById(Id);
    }
}
