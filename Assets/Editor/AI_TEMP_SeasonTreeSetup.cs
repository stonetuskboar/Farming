// =====================================================================
// 【AI TEMP-002】四季树配置自动生成（临时补丁·编辑器部分）
// 搜索关键字：AI TEMP-002
//
// 作用：扫描 Assets/Art/Crops 下所有 Sprite，按名字自动挑出
//       「2阶段 / 3阶段(成熟) × 春·夏·秋·冬」的图，生成
//       Assets/Resources/AI_TEMP/SeasonTreeConfig.asset，供运行时读取。
//
// 命名匹配规则（同时兼容两套写法）：
//   成熟：松树_春 / 3阶段松树_秋季 / 松树_夏_秋_冬
//   2阶段：2阶段松树_春 / 2阶段松树_秋季
//   带「_覆雪」的图不会被选中（落雪逻辑目前不存在）。
//
// 版本：ConfigVersion 变化时会自动重建配置，不需要手动删资源。
// 删除方法：删掉 AI_TEMP_*.cs 三个脚本 + Assets/Resources/AI_TEMP/ 文件夹即可。
// =====================================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AI_TEMP_SeasonTreeSetup
{
    private const string ConfigPath = "Assets/Resources/AI_TEMP/SeasonTreeConfig.asset";
    private const string ConfigFolder = "Assets/Resources/AI_TEMP";
    private const int ConfigVersion = 3;

    private static readonly int[] TreeIds = { 15, 16, 17 };
    private static readonly string[] TreeNames = { "松树", "柏树", "橡树" };
    private static readonly string[] SeasonNames = { "春", "夏", "秋", "冬" };

    static AI_TEMP_SeasonTreeSetup()
    {
        EditorApplication.delayCall += AutoCreate;
    }

    [MenuItem("AI TEMP/重新生成四季树配置")]
    public static void Recreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[AI TEMP-002] 请先退出运行模式，再重新生成配置。");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<AI_TEMP_SeasonTreeConfig>(ConfigPath) != null)
        {
            AssetDatabase.DeleteAsset(ConfigPath);
        }

        AutoCreate();
    }

    private static void AutoCreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        AI_TEMP_SeasonTreeConfig existing = AssetDatabase.LoadAssetAtPath<AI_TEMP_SeasonTreeConfig>(ConfigPath);
        if (existing != null && existing.version >= ConfigVersion) return;
        if (existing != null) AssetDatabase.DeleteAsset(ConfigPath);

        Dictionary<string, Sprite> sprites = LoadAllSprites();

        AI_TEMP_SeasonTreeConfig config = ScriptableObject.CreateInstance<AI_TEMP_SeasonTreeConfig>();
        config.version = ConfigVersion;
        int found = 0;

        for (int i = 0; i < TreeNames.Length; i++)
        {
            AI_TEMP_SeasonTreeEntry entry = new AI_TEMP_SeasonTreeEntry();
            entry.treeId = TreeIds[i];
            entry.stage2 = new Sprite[4];
            entry.stage3 = new Sprite[4];

            for (int s = 0; s < SeasonNames.Length; s++)
            {
                entry.stage3[s] = PickSprite(sprites, TreeNames[i], string.Empty, SeasonNames[s]);
                entry.stage2[s] = PickSprite(sprites, TreeNames[i], "2阶段", SeasonNames[s]);

                if (entry.stage3[s] != null) found++;
                if (entry.stage2[s] != null) found++;
            }

            config.trees.Add(entry);
            Debug.Log("[AI TEMP-002] " + TreeNames[i] + "：成熟 4 季取到 " + CountValid(entry.stage3) +
                      " 张，2阶段 4 季取到 " + CountValid(entry.stage2) + " 张。");
        }

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

        Debug.Log("[AI TEMP-002] 四季树配置已生成：" + ConfigPath + "（贴图引用 " + found + " / 24）。");
    }

    private static int CountValid(Sprite[] arr)
    {
        int n = 0;
        if (arr == null) return 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null) n++;
        }
        return n;
    }

    // 把 Assets/Art/Crops 下所有 Sprite 按名字收进字典（同名只取第一个）
    private static Dictionary<string, Sprite> LoadAllSprites()
    {
        Dictionary<string, Sprite> map = new Dictionary<string, Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art/Crops" });

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);

            for (int j = 0; j < all.Length; j++)
            {
                Sprite sp = all[j] as Sprite;
                if (sp != null && !map.ContainsKey(sp.name))
                {
                    map.Add(sp.name, sp);
                }
            }
        }

        return map;
    }

    // stageWord："" = 成熟（3阶段），"2阶段" = 2阶段
    private static Sprite PickSprite(Dictionary<string, Sprite> map, string tree, string stageWord, string season)
    {
        // 成熟允许「3阶段XXX」和「XXX」两种写法；2阶段只认「2阶段XXX」
        string[] stageWords = stageWord == "2阶段" ? new[] { "2阶段" } : new[] { "3阶段", string.Empty };

        for (int i = 0; i < stageWords.Length; i++)
        {
            string sw = stageWords[i];
            string[] names =
            {
                sw + tree + "_" + season + "季",   // 3阶段松树_秋季
                sw + tree + "_" + season,          // 3阶段松树_秋 / 松树_春
            };

            for (int n = 0; n < names.Length; n++)
            {
                Sprite s;
                if (map.TryGetValue(names[n], out s)) return s;
            }

            // 秋/冬合在一张图的写法：3阶段柏树_秋冬
            if (season == "秋" || season == "冬")
            {
                Sprite combo;
                if (map.TryGetValue(sw + tree + "_秋冬", out combo)) return combo;
            }
        }

        // 常青树：夏/秋/冬 共用一张
        for (int i = 0; i < stageWords.Length; i++)
        {
            Sprite s;
            if (map.TryGetValue(stageWords[i] + tree + "_夏_秋_冬", out s)) return s;
        }

        return null;
    }
}
#endif
