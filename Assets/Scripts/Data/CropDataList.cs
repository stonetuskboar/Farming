using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDataList", menuName = "Farm/CropDataList")]
public class CropDataList : ScriptableObject
{
    public List<CropData> crops = new List<CropData>();
    public List<TreeData> trees = new List<TreeData>();
    public CropData GetCropDataById(int id)
    {
        return crops.Find(crop => crop.id == id);
    }
    public TreeData GetTreeDataById(int id)
    {
        return trees.Find(tree => tree.id == id);
    }
}

[System.Serializable]
public class TreeData:CropData
{
    //砍树会留下树干
    public int stumpId;
    public TreeData()
    {
        size = new Vector2Int(2, 2);
    }
}

[System.Serializable]
public class CropData
{
    public string Name;
    public int id;

    // 从种下到成熟需要多少回合
    public int matureTurns;
    public List<GrowthStage> growthStages = new List<GrowthStage>();

    public Vector2Int size = new Vector2Int(1,1);

    public int DropedItemId;
}

[System.Serializable]
public class GrowthStage
{
    public int GrowthProgress;
    public Sprite GrowthSprite;
    public Sprite shadow;
}