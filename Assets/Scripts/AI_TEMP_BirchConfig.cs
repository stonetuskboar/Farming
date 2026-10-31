// =====================================================================
// 【AI TEMP-007】白桦（临时）配置数据类型
// 搜索关键字：AI TEMP-007
//
// 说明：只定义数据结构，由 AI_TEMP_BirchSetup.cs 自动按名字抓图生成。
// 删除方法：删掉 AI_TEMP_Birch*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using UnityEngine;

public class AI_TEMP_BirchConfig : ScriptableObject
{
    public int version;

    public Sprite spring;
    public Sprite summer;
    public Sprite autumn;
    public Sprite winter;

    public Sprite Get(int season)
    {
        if (season == 0) return spring;
        if (season == 1) return summer;
        if (season == 2) return autumn;
        if (season == 3) return winter;
        return null;
    }
}
