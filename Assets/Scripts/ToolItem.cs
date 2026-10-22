using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolItem : UsableItem
{
    public ToolData toolData;
    public int EnergyValue = 0;
    public override void Init(GameManager manager)
    {
        base.Init(manager);
        toolData = GameDataManager.Instance.itemDataList.GetToolDataById(Id);
        size = toolData.Size;
        EnergyValue = toolData.CosEnergy;
    }
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

    public void Upgrade()
    {
        itemData = GameDataManager.Instance.itemDataList.GetItemDataById(toolData.UpgradeId);
        toolData = GameDataManager.Instance.itemDataList.GetToolDataById(toolData.UpgradeId);
        size = toolData.Size;
        EnergyValue = toolData.CosEnergy;
    }
}
