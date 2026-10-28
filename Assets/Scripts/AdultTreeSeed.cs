// ===== AI TEMP-001 成年树作弊物品（不入正式版）=====
// 作用：种下即为成年树，可正常砍伐
// 对应物品 id：60 松树 / 61 柏树 / 62 橡树
// 依赖：GameDataManager.NewItemById() 的 60~62 分支
//       HotBarController.GiveCheatSeed() 的发放循环
//       ItemDataList.asset 中 id 60/61/62 三条
// 移除方式：删除本文件 + 删除上述三处 ===== AI TEMP-001 ===== 标记块
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
