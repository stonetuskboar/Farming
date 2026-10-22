using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList", menuName = "Farm/ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> dataList = new List<ItemData>();
    public List<ToolData> toolDataList = new List<ToolData>();
    public ItemData GetItemDataById(int id)
    {
        return dataList.Find(item => item.Id == id);
    }
    public ToolData GetToolDataById(int id)
    {
        return toolDataList.Find(item => item.Id == id);
    }
}

[System.Serializable]
public class ItemData
{
    public string Name;
    public int Id;
    public Sprite Icon;

    [TextArea]
    public string Description;
    public int Price = 1;
}
[System.Serializable]
public class ToolData
{
    public int Id;
    public int CosEnergy;
    public Vector2Int Size = new Vector2Int(1,1);
    public bool CouldUpgrade = true;
    public string UpgradeDescription;
    public int UpgradeCost = 200;
    public int UpgradeId = -1;

    public bool CanUpgrade()
    {
        if(UpgradeId == - 1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
}