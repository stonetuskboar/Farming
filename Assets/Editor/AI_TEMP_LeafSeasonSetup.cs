// =====================================================================
// 【AI TEMP-003】树叶 / 双层树叶 四季配置自动生成（临时补丁·编辑器部分）
// 搜索关键字：AI TEMP-003
//
// 作用：扫描 Assets/Art/Tiles/双网格瓦片图/树叶、河流 下的
//       双层树叶.png / _夏 / _秋 / _冬 与 树叶顶.png / _夏 / _秋 / _冬，
//       按切图编号（_0 ~ _15）整理成四季数组，生成
//       Assets/Resources/AI_TEMP/LeafSeasonConfig.asset。
//
// 版本：ConfigVersion 变化时自动重建，不需要手动删资源。
// 删除方法：删掉 AI_TEMP_LeafSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AI_TEMP_LeafSeasonSetup
{
    private const string ConfigPath = "Assets/Resources/AI_TEMP/LeafSeasonConfig.asset";
    private const string ConfigFolder = "Assets/Resources/AI_TEMP";
    private const int ConfigVersion = 2;
    private const string Folder = "Assets/Art/Tiles/双网格瓦片图/树叶、河流";
    private const string TreePath = "Assets/Art/room/大树.png";

    private static readonly string[] Systems = { "双层树叶", "树叶顶" };
    private static readonly string[] SeasonSuffix = { "", "_夏", "_秋", "_冬" };

    static AI_TEMP_LeafSeasonSetup()
    {
        EditorApplication.delayCall += AutoCreate;
    }

    [MenuItem("AI TEMP/重新生成四季树叶配置")]
    public static void Recreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[AI TEMP-003] 请先退出运行模式，再重新生成配置。");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<AI_TEMP_LeafSeasonConfig>(ConfigPath) != null)
        {
            AssetDatabase.DeleteAsset(ConfigPath);
        }

        AutoCreate();
    }

    private static void AutoCreate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        AI_TEMP_LeafSeasonConfig existing = AssetDatabase.LoadAssetAtPath<AI_TEMP_LeafSeasonConfig>(ConfigPath);
        if (existing != null && existing.version >= ConfigVersion) return;
        if (existing != null) AssetDatabase.DeleteAsset(ConfigPath);

        AI_TEMP_LeafSeasonConfig config = ScriptableObject.CreateInstance<AI_TEMP_LeafSeasonConfig>();
        config.version = ConfigVersion;

        for (int i = 0; i < Systems.Length; i++)
        {
            string system = Systems[i];
            AI_TEMP_LeafSeasonEntry entry = new AI_TEMP_LeafSeasonEntry();
            entry.name = system;
            entry.spring = LoadSheet(system + SeasonSuffix[0] + ".png");
            entry.summer = LoadSheet(system + SeasonSuffix[1] + ".png");
            entry.autumn = LoadSheet(system + SeasonSuffix[2] + ".png");
            entry.winter = LoadSheet(system + SeasonSuffix[3] + ".png");

            config.entries.Add(entry);

            Debug.Log("[AI TEMP-003] " + system + "：春 " + entry.spring.Length +
                      " / 夏 " + entry.summer.Length +
                      " / 秋 " + entry.autumn.Length +
                      " / 冬 " + entry.winter.Length + " 张。");
        }

        // 场景装饰用的大树：春/夏/秋/冬 各一张
        config.treeSpring = LoadNamed(TreePath, "大树_春");
        config.treeSummer = LoadNamed(TreePath, "大树_夏");
        config.treeAutumn = LoadNamed(TreePath, "大树_秋");
        config.treeWinter = LoadNamed(TreePath, "大树_冬");

        Debug.Log("[AI TEMP-003] 大树：春 " + (config.treeSpring != null) +
                  " / 夏 " + (config.treeSummer != null) +
                  " / 秋 " + (config.treeAutumn != null) +
                  " / 冬 " + (config.treeWinter != null));

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

        Debug.Log("[AI TEMP-003] 四季树叶配置已生成：" + ConfigPath);
    }

    // 把一张切图表里的 Sprite 按名字最后的编号排成数组
    private static Sprite[] LoadSheet(string fileName)
    {
        string path = Folder + "/" + fileName;

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) == null)
        {
            Debug.LogWarning("[AI TEMP-003] 找不到贴图：" + path);
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

    // 从一张切图里按精确名字取 Sprite
    private static Sprite LoadNamed(string path, string spriteName)
    {
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) == null)
        {
            Debug.LogWarning("[AI TEMP-003] 找不到贴图：" + path);
            return null;
        }

        Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int i = 0; i < all.Length; i++)
        {
            Sprite sp = all[i] as Sprite;
            if (sp != null && sp.name == spriteName) return sp;
        }

        Debug.LogWarning("[AI TEMP-003] 在 " + path + " 里没找到 Sprite：" + spriteName);
        return null;
    }

    // "双层树叶_12" / "双层树叶_夏_12" 都取最后一段数字
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
