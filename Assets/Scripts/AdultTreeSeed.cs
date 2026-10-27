using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 作弊用：种下就是成年树，可以正常砍伐
public class AdultTreeSeed : BasicWeedTreeSeed
{
    public AdultTreeSeed(int id, int seedId) : base(id, seedId)
    {
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        if (Amount <= 0)
        {
            return false;
        }
        if (farm.CanFarmHere(cell) == false)
        {
            return false;
        }
        if (farm.GetFarmTileData(cell).cropObject != null)
        {
            return false;
        }
        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        GameDataManager.Instance.PlayAudio("Plant");
        FarmLandManager farm = GameManager.FarmLandManager;
        BasicCrop crop = farm.Plant(cells, cropData);
        crop.growthProgress = cropData.matureTurns;   // 直接成年
        crop.SetByGrowthProgress();
        GameManager.hotbarController.DecreaseItem(Id, 1);
    }
}
