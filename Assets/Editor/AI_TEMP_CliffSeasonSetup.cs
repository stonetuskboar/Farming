// =====================================================================
// 【AI TEMP-005】悬崖 / 悬崖底部 四季配置自动生成（临时补丁·编辑器部分）
// 搜索关键字：AI TEMP-005
//
// 作用：读取 Assets/Art/Tiles/悬崖 下的四季资源，生成
//       Assets/Resources/AI_TEMP/CliffSeasonConfig.asset。
//
// 版本：ConfigVersion 变化时自动重建。
// 删除方法：删掉 AI_TEMP_CliffSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AI_TEMP_CliffSeasonSetup
{
    private const string ConfigPath = "Assets/Resources/AI_TEMP/CliffSeasonConfig.asset";
    private const string ConfigFolder = "Assets/Resources/AI_TEMP";
    private const int ConfigVersion = 1;
    private const string Folder = "Assets/Art/Tiles/悬崖";

    static AI_TEMP_CliffSeasonSetup()
    {
        EditorApplication.delayCall += AutoCreate;
    }

    [MenuItem("AI TEMP/重新生成悬崖四季配置")]
    public static void Recreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[AI TEMP-005] 请先退出运行模式，再重新生成配置。");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<AI_TEMP_CliffSeasonConfig>(ConfigPath) != null)
        {
            AssetDatabase.DeleteAsset(ConfigPath);
        }

        AutoCreate();
    }

    private static void AutoCreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        AI_TEMP_CliffSeasonConfig existing = AssetDatabase.LoadAssetAtPath<AI_TEMP_CliffSeasonConfig>(ConfigPath);
        if (existing != null && existing.version >= ConfigVersion) return;
        if (existing != null) AssetDatabase.DeleteAsset(ConfigPath);

        AI_TEMP_CliffSeasonConfig config = ScriptableObject.CreateInstance<AI_TEMP_CliffSeasonConfig>();
        config.version = ConfigVersion;

        config.cliffSpring = LoadRuleTile("悬崖_春_DualGridRuleTile.asset");
        config.cliffSummer = LoadRuleTile("悬崖_夏_DualGridRuleTile.asset");
        config.cliffAutumn = LoadRuleTile("悬崖_秋_DualGridRuleTile.asset");
        config.cliffWinter = LoadRuleTile("悬崖_冬_DualGridRuleTile.asset");

        config.bottomSpring = LoadSheet("悬崖底部.png");
        config.bottomSummer = LoadSheet("悬崖底部_夏.png");
        config.bottomAutumn = LoadSheet("悬崖底部_秋.png");
        config.bottomWinter = LoadSheet("悬崖底部_冬.png");

        Debug.Log("[AI TEMP-005] 悬崖：春 " + (config.cliffSpring != null) +
                  " / 夏 " + (config.cliffSummer != null) +
                  " / 秋 " + (config.cliffAutumn != null) +
                  " / 冬 " + (config.cliffWinter != null));
        Debug.Log("[AI TEMP-005] 悬崖底部：春 " + config.bottomSpring.Length +
                  " / 夏 " + config.bottomSummer.Length +
                  " / 秋 " + config.bottomAutumn.Length +
                  " / 冬 " + config.bottomWinter.Length + " 张。");

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        if (!AssetDatabase.IsValidFolder(ConfigFolder))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "AI_TEMP");
        }

        AssetDatabase.CreateAsset(config, ConfigPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[AI TEMP-005] 悬崖四季配置已生成：" + ConfigPath);
    }

    private static skner.DualGrid.DualGridRuleTile LoadRuleTile(string fileName)
    {
        string path = Folder + "/" + fileName;

        if (AssetDatabase.LoadAssetAtPath<skner.DualGrid.DualGridRuleTile>(path) == null)
        {
            Debug.LogWarning("[AI TEMP-005] 找不到 RuleTile：" + path);
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<skner.DualGrid.DualGridRuleTile>(path);
    }

    private static Sprite[] LoadSheet(string fileName)
    {
        string path = Folder + "/" + fileName;

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) == null)
        {
            Debug.LogWarning("[AI TEMP-005] 找不到贴图：" + path);
            return new Sprite[0];
        }

        Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);
        Dictionary<int, Sprite> map = new Dictionary<int, Sprite>();
        int max = -1;

        for (int i = 0; i < all.Length; i++)
        {
            Sprite sp = all[i] as Sprite;
            if (sp == null) continue;

            int index = IndexSuffix(sp.name);
            if (index < 0) continue;

            map[index] = sp;
            if (index > max) max = index;
        }

        if (max < 0) return new Sprite[0];

        Sprite[] result = new Sprite[max + 1];
        foreach (KeyValuePair<int, Sprite> pair in map)
        {
            result[pair.Key] = pair.Value;
        }
        return result;
    }

    private static int IndexSuffix(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return -1;

        int pos = spriteName.LastIndexOf('_');
        if (pos < 0 || pos >= spriteName.Length - 1) return -1;

        int index;
        if (int.TryParse(spriteName.Substring(pos + 1), out index)) return index;
        return -1;
    }
}
#endif
