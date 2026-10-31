// =====================================================================
// 【AI TEMP-002】四季树配置数据类型（临时补丁）
// 搜索关键字：AI TEMP-002
//
// 说明：这个文件只定义配置用的数据结构，由
//       Assets/Editor/AI_TEMP_SeasonTreeSetup.cs 自动生成
//       Assets/Resources/AI_TEMP/SeasonTreeConfig.asset。
//
// 删除方法：删掉本文件、AI_TEMP_SeasonTreeSprites.cs、
//           AI_TEMP_SeasonTreeSetup.cs 和 Assets/Resources/AI_TEMP/ 即可。
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

// 单棵树的四季配置：下标 0=春 1=夏 2=秋 3=冬
[System.Serializable]
public class AI_TEMP_SeasonTreeEntry
{
    public int treeId;
    public Sprite[] stage2 = new Sprite[4];
    public Sprite[] stage3 = new Sprite[4];
}

public class AI_TEMP_SeasonTreeConfig : ScriptableObject
{
    public int version;
    public List<AI_TEMP_SeasonTreeEntry> trees = new List<AI_TEMP_SeasonTreeEntry>();
}
