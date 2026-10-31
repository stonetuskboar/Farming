// =====================================================================
// 【AI TEMP-005】悬崖 / 悬崖底部 四季配置数据类型（临时补丁）
// 搜索关键字：AI TEMP-005
//
// 说明：只定义数据结构，资源由 AI_TEMP_CliffSeasonSetup.cs 自动生成。
// 删除方法：删掉 AI_TEMP_CliffSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using skner.DualGrid;
using UnityEngine;

public class AI_TEMP_CliffSeasonConfig : ScriptableObject
{
    public int version;

    // 悬崖：四季各一张 RuleTile（直接换 RenderTile）
    public DualGridRuleTile cliffSpring;
    public DualGridRuleTile cliffSummer;
    public DualGridRuleTile cliffAutumn;
    public DualGridRuleTile cliffWinter;

    // 悬崖底部：四季各一组 Sprite（按 _0 ~ _15 下标对齐）
    public Sprite[] bottomSpring;
    public Sprite[] bottomSummer;
    public Sprite[] bottomAutumn;
    public Sprite[] bottomWinter;

    public DualGridRuleTile GetCliff(int season)
    {
        if (season == 0) return cliffSpring;
        if (season == 1) return cliffSummer;
        if (season == 2) return cliffAutumn;
        if (season == 3) return cliffWinter;
        return null;
    }

    public Sprite[] GetBottom(int season)
    {
        if (season == 0) return bottomSpring;
        if (season == 1) return bottomSummer;
        if (season == 2) return bottomAutumn;
        if (season == 3) return bottomWinter;
        return null;
    }
}
