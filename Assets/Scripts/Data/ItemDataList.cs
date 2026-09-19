using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList", menuName = "Farm/ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> dataList = new List<ItemData>();

    public ItemData GetItemDataById(int id)
    {
        return dataList.Find(item => item.Id == id);
    }
}

[System.Serializable]
public class ItemData
{
    public string Name;
    public int Id;
    public Sprite Icon;
}
