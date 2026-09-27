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
    public bool IsStump;
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
    public int DropedAmount;
}

[System.Serializable]
public class GrowthStage
{
    public int GrowthProgress;
    public Sprite GrowthSprite;
    public Sprite shadow;

    public WindType 风力;
}

public enum WindType
{
    //石头、种子、树桩等不该动的属于静止
    静止,
    //很坚硬的物体属于微动，例如树苗等
    微动,
    //大部分正常的植物、树木
    正常,
    //草这种很柔软的植物属于剧烈
    剧烈
}