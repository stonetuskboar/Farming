// =====================================================================
// 【AI TEMP-004】季节换皮预览（临时工具·仅编辑器）
// 搜索关键字：AI TEMP-004
//
// 作用：Play 模式下点按钮，直接把画面切成春/夏/秋/冬，方便看贴图。
//
// 只做换皮：
//   - 不改月份、不改 UI、不推进回合、不动浇水/成长/金钱/存档；
//   - 只改 GameManager.season（内存），并刷新显示；
//   - 退出 Play 自动还原，不会写进场景或资源。
//
// 删除方法：删掉本文件（和 .meta）即可，工程回到原样。
// =====================================================================
#if UNITY_EDITOR
using skner.DualGrid;
using UnityEditor;
using UnityEngine;

public class AI_TEMP_SeasonPreview : EditorWindow
{
    private Season originalSeason = Season.春;
    private bool hasOriginal;
    private string status = "";

    [MenuItem("AI TEMP/季节换皮预览")]
    public static void Open()
    {
        GetWindow<AI_TEMP_SeasonPreview>("季节预览").Show();
    }

    private void OnEnable()
    {
        hasOriginal = false;
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "只换显示：不改月份 / 不改 UI / 不推进回合 / 不写存档。\n" +
            "退出 Play 自动还原。删除本文件即可完全移除。",
            MessageType.Info);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("请先进入 Play 模式，再点下面的按钮。", MessageType.Warning);
            return;
        }

        GameManager gm = Object.FindObjectOfType<GameManager>();
        if (gm == null)
        {
            EditorGUILayout.HelpBox("场景里找不到 GameManager。", MessageType.Warning);
            return;
        }

        if (!hasOriginal)
        {
            originalSeason = gm.season;
            hasOriginal = true;
        }

        GUILayout.Space(6);
        GUILayout.Label("当前显示季节：" + gm.season, EditorStyles.boldLabel);
        GUILayout.Space(6);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("春", GUILayout.Height(40))) Apply(gm, Season.春);
        if (GUILayout.Button("夏", GUILayout.Height(40))) Apply(gm, Season.夏);
        if (GUILayout.Button("秋", GUILayout.Height(40))) Apply(gm, Season.秋);
        if (GUILayout.Button("冬", GUILayout.Height(40))) Apply(gm, Season.冬);
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        if (GUILayout.Button("满精力（作弊）", GUILayout.Height(28)))
        {
            FillEnergy(gm);
        }

        bool lockOn = AI_TEMP_EnergyLock.Enabled;
        bool newLock = EditorGUILayout.ToggleLeft(
            "锁定精力（F8 也可切换）—— 当前：" + (lockOn ? "开" : "关"),
            lockOn);

        if (newLock != lockOn)
        {
            AI_TEMP_EnergyLock.Enabled = newLock;
        }

        if (GUILayout.Button("刷新当前季节", GUILayout.Height(26)))
        {
            Apply(gm, gm.season);
        }

        if (GUILayout.Button("恢复进入 Play 时的季节", GUILayout.Height(26)))
        {
            Apply(gm, originalSeason);
        }

        GUILayout.Space(8);
        if (!string.IsNullOrEmpty(status))
        {
            EditorGUILayout.HelpBox(status, MessageType.None);
        }

        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "说明：树/树叶/大树由 TEMP-002/003 在 0.5 秒内自动跟上；\n" +
            "草地由本工具直接换。冬天目前只有草地和覆雪差分，\n" +
            "普通的冬树图还没有，会回退到夏秋图。",
            MessageType.None);
    }

    private void FillEnergy(GameManager gm)
    {
        if (gm == null) return;

        gm.NowEnergy = gm.MaxEnergy;

        if (gm.energyBar != null)
        {
            gm.energyBar.SetEnergy(gm.MaxEnergy, gm.MaxEnergy);
        }

        status = "精力已补满：" + gm.NowEnergy + " / " + gm.MaxEnergy;
    }

    private void Apply(GameManager gm, Season season)
    {
        FarmLandManager farm = gm.FarmLandManager;
        if (farm == null)
        {
            status = "找不到 FarmLandManager。";
            return;
        }

        gm.season = season;

        int cropCount = 0;
        if (farm.crops != null)
        {
            foreach (BasicCrop crop in farm.crops)
            {
                if (crop == null) continue;
                crop.SetByGrowthProgress();
                cropCount++;
            }
        }

        bool grassOk = ApplyGrass(farm, season);

        status = "已切到「" + season + "」：作物 " + cropCount + " 株，草地" +
                 (grassOk ? "已换。" : "跳过（字段没找到或为空）。");
    }

    // 通过 SerializedObject 读取 FarmLandManager 的私有序列化字段，不反射、不改代码
    private static bool ApplyGrass(FarmLandManager farm, Season season)
    {
        try
        {
            SerializedObject so = new SerializedObject(farm);

            SerializedProperty moduleProp = so.FindProperty("grassTilemap");
            if (moduleProp == null || moduleProp.objectReferenceValue == null) return false;

            DualGridTilemapModule module = moduleProp.objectReferenceValue as DualGridTilemapModule;
            if (module == null) return false;

            string fieldName = "springTile";
            if (season == Season.夏) fieldName = "summerTile";
            else if (season == Season.秋) fieldName = "fallTile";
            else if (season == Season.冬) fieldName = "winterTile";

            SerializedProperty tileProp = so.FindProperty(fieldName);
            if (tileProp == null || tileProp.objectReferenceValue == null) return false;

            DualGridRuleTile tile = tileProp.objectReferenceValue as DualGridRuleTile;
            if (tile == null) return false;

            module.RenderTile = tile;
            module.RefreshRenderTilemap();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[AI TEMP-004] 草地换图失败：" + e.Message);
            return false;
        }
    }
}
#endif
