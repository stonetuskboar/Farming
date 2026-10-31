// =====================================================================
// 【AI TEMP-007】白桦配置自动生成（临时补丁·编辑器部分）
// 搜索关键字：AI TEMP-007
//
// 作用：在 Assets/Art/Crops 下按名字找「白桦_春/夏/秋/冬」，
//       生成 Assets/Resources/AI_TEMP/BirchConfig.asset。
//       找不到图时不生成（避免生成空配置），切好图后点菜单重来即可。
//
// 命名要求：白桦_春 / 白桦_夏 / 白桦_秋 / 白桦_冬
//           （带「覆雪」的不会被选中）
//
// 删除方法：删掉 AI_TEMP_Birch*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AI_TEMP_BirchSetup
{
    private const string ConfigPath = "Assets/Resources/AI_TEMP/BirchConfig.asset";
    private const string ConfigFolder = "Assets/Resources/AI_TEMP";
    private const int ConfigVersion = 1;
    private const string SearchFolder = "Assets/Art/Crops";

    private static readonly string[] SeasonNames = { "春", "夏", "秋", "冬" };

    static AI_TEMP_BirchSetup()
    {
        EditorApplication.delayCall += AutoCreate;
    }

    [MenuItem("AI TEMP/重新生成白桦配置")]
    public static void Recreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[AI TEMP-007] 请先退出运行模式，再重新生成配置。");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<AI_TEMP_BirchConfig>(ConfigPath) != null)
        {
            AssetDatabase.DeleteAsset(ConfigPath);
        }

        AutoCreate();
    }

    private static void AutoCreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        AI_TEMP_BirchConfig existing = AssetDatabase.LoadAssetAtPath<AI_TEMP_BirchConfig>(ConfigPath);
        if (existing != null && existing.version >= ConfigVersion) return;
        if (existing != null) AssetDatabase.DeleteAsset(ConfigPath);

        Dictionary<string, Sprite> sprites = LoadBirchSprites();

        Sprite spring = Pick(sprites, "春");
        Sprite summer = Pick(sprites, "夏");
        Sprite autumn = Pick(sprites, "秋");
        Sprite winter = Pick(sprites, "冬");

        if (spring == null && summer == null && autumn == null && winter == null)
        {
            Debug.LogWarning("[AI TEMP-007] 还没找到「白桦_春/夏/秋/冬」的 Sprite。\n" +
                             "切好图后，点菜单 AI TEMP > 重新生成白桦配置。");
            return;
        }

        AI_TEMP_BirchConfig config = ScriptableObject.CreateInstance<AI_TEMP_BirchConfig>();
        config.version = ConfigVersion;
        config.spring = spring;
        config.summer = summer;
        config.autumn = autumn;
        config.winter = winter;

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

        Debug.Log("[AI TEMP-007] 白桦配置已生成：春 " + (spring != null) +
                  " / 夏 " + (summer != null) +
                  " / 秋 " + (autumn != null) +
                  " / 冬 " + (winter != null));
    }

    private static Dictionary<string, Sprite> LoadBirchSprites()
    {
        Dictionary<string, Sprite> map = new Dictionary<string, Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { SearchFolder });

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);

            for (int j = 0; j < all.Length; j++)
            {
                Sprite sp = all[j] as Sprite;
                if (sp == null) continue;
                if (!sp.name.Contains("白桦")) continue;
                if (sp.name.Contains("覆雪")) continue;
                if (map.ContainsKey(sp.name)) continue;

                map.Add(sp.name, sp);
            }
        }

        return map;
    }

    private static Sprite Pick(Dictionary<string, Sprite> map, string season)
    {
        Sprite s;

        // 优先精确名：白桦_春
        if (map.TryGetValue("白桦_" + season, out s)) return s;

        // 其次：名字里同时含「白桦」和季节
        foreach (KeyValuePair<string, Sprite> pair in map)
        {
            if (pair.Key.Contains(season)) return pair.Value;
        }

        return null;
    }
}
#endif
