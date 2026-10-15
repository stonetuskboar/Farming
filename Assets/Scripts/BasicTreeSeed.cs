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
        cropData = GameManager.GameDataManager.cropDataList.GetTreeDataById(seedId);
        size = cropData.size;
    }

}
