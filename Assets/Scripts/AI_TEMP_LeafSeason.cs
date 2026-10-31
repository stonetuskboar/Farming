// =====================================================================
// 【AI TEMP-003】树叶 / 双层树叶 四季换图（临时补丁·运行部分）
// 搜索关键字：AI TEMP-003
//
// 作用：换季时把「双层树叶」「树叶顶」两个 DualGrid 图层的 RuleTile
//       贴图换成对应季节的版本，并刷新图块，不需要手动改场景。
//
// 原理：这两套图的 16 张切图编号一一对应（_0 ~ _15），
//       所以只替换 RuleTile 里每条规则引用的 Sprite，邻居/形状规则不动。
//       修改只发生在运行时内存里，退出 Play 会自动还原，不会存盘。
//
// 依赖：Assets/Resources/AI_TEMP/LeafSeasonConfig.asset（编辑器自动生成）
// 删除方法：删掉 AI_TEMP_LeafSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using System.Collections.Generic;
using skner.DualGrid;
using UnityEngine;

public class AI_TEMP_LeafSeason : MonoBehaviour
{
    private const string ConfigPath = "AI_TEMP/LeafSeasonConfig";

    private AI_TEMP_LeafSeasonConfig config;
    private FarmLandManager farm;
    private float timer;
    private bool warned;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_LeafSeason>() != null) return;

        GameObject go = new GameObject("[AI TEMP-003] LeafSeason");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_LeafSeason>();
    }

    private void Awake()
    {
        TryLoadConfig();
    }

    private void TryLoadConfig()
    {
        config = Resources.Load<AI_TEMP_LeafSeasonConfig>(ConfigPath);
        if (config == null && !warned)
        {
            warned = true;
            Debug.LogWarning("[AI TEMP-003] 没有找到树叶四季配置：Assets/Resources/AI_TEMP/LeafSeasonConfig.asset\n" +
                             "请回到 Unity 等它编译完成（会自动生成），或点菜单 AI TEMP > 重新生成四季树叶配置。");
        }
    }

    private void LateUpdate()
    {
        // 0.5 秒检查一次，避免每帧扫描场景
        timer -= Time.unscaledDeltaTime;
        if (timer > 0f) return;
        timer = 0.5f;

        if (config == null)
        {
            TryLoadConfig();
            if (config == null) return;
        }

        if (farm == null)
        {
            farm = FindObjectOfType<FarmLandManager>();
            if (farm == null) return;
        }

        GameManager gm = farm.gameManager;
        if (gm == null) return;

        int season = Mathf.Clamp((int)gm.season, 0, 3); // 春0 夏1 秋2 冬3

        DualGridTilemapModule[] modules = FindObjectsOfType<DualGridTilemapModule>(true);
        for (int i = 0; i < modules.Length; i++)
        {
            DualGridTilemapModule module = modules[i];
            if (module == null || module.RenderTile == null) continue;

            AI_TEMP_LeafSeasonEntry entry = FindEntry(module.RenderTile.name);
            if (entry == null) continue;

            Sprite[] sprites = entry.Get(season);
            if (sprites == null || sprites.Length == 0) continue;

            if (IsApplied(module.RenderTile, sprites)) continue;

            Apply(module.RenderTile, sprites);
            module.RefreshRenderTilemap();

            Debug.Log("[AI TEMP-003] " + entry.name + " 已切换到 " + SeasonText(season) + "。");
        }

        // 场景里的大树：单张 SpriteRenderer，按名字前缀 "大树_" 识别
        Sprite treeSprite = config.GetTreeSprite(season);
        if (treeSprite != null)
        {
            SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>(true);
            int swapped = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer sr = renderers[i];
                if (sr == null || sr.sprite == null) continue;

                // 只处理名叫「大树」的对象（以及已经换过季的图），避免误伤其它用同图的装饰
                string goName = sr.gameObject.name;
                string n = sr.sprite.name;
                bool isBigTree = goName == "大树" || goName.StartsWith("大树 (") || n.StartsWith("大树_");
                if (!isBigTree) continue;
                if (sr.sprite == treeSprite) continue;

                // 新旧图的轴心不同，换图时补偿位置，保证树的落脚点不跳
                Vector3 before = GroundOffset(sr.sprite);
                Vector3 after = GroundOffset(treeSprite);
                sr.sprite = treeSprite;
                sr.transform.localPosition += before - after;
                swapped++;
            }

            if (swapped > 0)
            {
                Debug.Log("[AI TEMP-003] 大树 已切换到 " + SeasonText(season) + "（" + swapped + " 处）。");
            }
        }
    }

    private AI_TEMP_LeafSeasonEntry FindEntry(string tileName)
    {
        if (string.IsNullOrEmpty(tileName)) return null;

        for (int i = 0; i < config.entries.Count; i++)
        {
            AI_TEMP_LeafSeasonEntry entry = config.entries[i];
            if (entry != null && !string.IsNullOrEmpty(entry.name) && tileName.StartsWith(entry.name))
            {
                return entry;
            }
        }
        return null;
    }

    // 用第一条规则判断当前是不是已经是目标季节（便宜且够用）
    private static bool IsApplied(DualGridRuleTile tile, Sprite[] sprites)
    {
        if (tile.m_TilingRules == null || tile.m_TilingRules.Count == 0) return true;

        RuleTile.TilingRule rule = tile.m_TilingRules[0];
        if (rule == null || rule.m_Sprites == null || rule.m_Sprites.Length == 0) return true;

        Sprite current = rule.m_Sprites[0];
        if (current == null) return true;

        int index = IndexSuffix(current.name);
        if (index < 0 || index >= sprites.Length) return true;

        return sprites[index] == null || sprites[index] == current;
    }

    private static void Apply(DualGridRuleTile tile, Sprite[] sprites)
    {
        if (tile.m_TilingRules != null)
        {
            for (int r = 0; r < tile.m_TilingRules.Count; r++)
            {
                RuleTile.TilingRule rule = tile.m_TilingRules[r];
                if (rule == null || rule.m_Sprites == null) continue;

                for (int k = 0; k < rule.m_Sprites.Length; k++)
                {
                    Sprite current = rule.m_Sprites[k];
                    if (current == null) continue;

                    int index = IndexSuffix(current.name);
                    if (index >= 0 && index < sprites.Length && sprites[index] != null)
                    {
                        rule.m_Sprites[k] = sprites[index];
                    }
                }
            }
        }

        if (tile.m_DefaultSprite != null)
        {
            int index = IndexSuffix(tile.m_DefaultSprite.name);
            if (index >= 0 && index < sprites.Length && sprites[index] != null)
            {
                tile.m_DefaultSprite = sprites[index];
            }
        }
    }

    // 以 Sprite 左下角为参照算“底部中心点”的本地偏移，换图时用它保持落脚点不动
    private static Vector3 GroundOffset(Sprite sprite)
    {
        if (sprite == null || sprite.rect.width <= 0f || sprite.rect.height <= 0f) return Vector3.zero;

        float px = sprite.pivot.x / sprite.rect.width;
        float py = sprite.pivot.y / sprite.rect.height;

        return new Vector3(
            (0.5f - px) * sprite.bounds.size.x,
            (0f - py) * sprite.bounds.size.y,
            0f);
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

    private static string SeasonText(int season)
    {
        if (season == 0) return "春";
        if (season == 1) return "夏";
        if (season == 2) return "秋";
        return "冬";
    }
}
