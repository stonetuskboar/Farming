using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTreeSeed : BasicSeed
{
    public BasicTreeSeed(int Id, int seedId):base(Id,seedId)
    {
    }
    public override void Init(GameManager manager)
    {
        BaseInit(manager);
        data = GameManager.GameDataManager.cropDataList.GetTreeDataById(seedId);
        size = data.size;
    }

    public override void Use(List<Vector3Int> cells)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        farm.Plant(cells, data);
    }
}
