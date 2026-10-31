// =====================================================================
// 【AI TEMP-003】树叶/双层树叶 四季配置数据类型（临时补丁）
// 搜索关键字：AI TEMP-003
//
// 说明：只定义数据结构，实际资源由
//       Assets/Editor/AI_TEMP_LeafSeasonSetup.cs 自动生成到
//       Assets/Resources/AI_TEMP/LeafSeasonConfig.asset。
//
// 删除方法：删掉 AI_TEMP_LeafSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

// 一套树叶图（双层树叶 / 树叶顶）的四季 Sprite，下标 = 切图编号（_0 ~ _15）
[System.Serializable]
public class AI_TEMP_LeafSeasonEntry
{
    public string name;
    public Sprite[] spring;
    public Sprite[] summer;
    public Sprite[] autumn;
    public Sprite[] winter;

    public Sprite[] Get(int season)
    {
        if (season == 0) return spring;
        if (season == 1) return summer;
        if (season == 2) return autumn;
        if (season == 3) return winter;
        return null;
    }
}

public class AI_TEMP_LeafSeasonConfig : ScriptableObject
{
    public int version;
    public List<AI_TEMP_LeafSeasonEntry> entries = new List<AI_TEMP_LeafSeasonEntry>();

    // 场景里的大树（单张 SpriteRenderer，Assets/Art/room/大树.png 里的四季图）
    public Sprite treeSpring;
    public Sprite treeSummer;
    public Sprite treeAutumn;
    public Sprite treeWinter;

    public Sprite GetTreeSprite(int season)
    {
        if (season == 0) return treeSpring;
        if (season == 1) return treeSummer;
        if (season == 2) return treeAutumn;
        if (season == 3) return treeWinter;
        return null;
    }
}
