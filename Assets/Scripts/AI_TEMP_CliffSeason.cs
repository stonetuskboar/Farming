// =====================================================================
// 【AI TEMP-005】悬崖 / 悬崖底部 四季换图（临时补丁·运行部分）
// 搜索关键字：AI TEMP-005
//
// 作用：换季时自动切换「悬崖」和「悬崖底部」的贴图。
//   - 悬崖：直接换 DualGrid 模块的 RenderTile（四张 RuleTile 已有）
//   - 悬崖底部：普通 Tilemap，按切图编号替换 Tile 的 Sprite
//
// 特点：不改原有脚本、不改场景，自动挂载；修改只在内存，退出 Play 还原。
// 删除方法：删掉 AI_TEMP_CliffSeason*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using System.Collections.Generic;
using skner.DualGrid;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AI_TEMP_CliffSeason : MonoBehaviour
{
    private const string ConfigPath = "AI_TEMP/CliffSeasonConfig";
    private const string BottomName = "悬崖底部";

    private AI_TEMP_CliffSeasonConfig config;
    private FarmLandManager farm;
    private float timer;
    private bool warned;

    private readonly List<TileBase> usedTiles = new List<TileBase>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_CliffSeason>() != null) return;

        GameObject go = new GameObject("[AI TEMP-005] CliffSeason");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_CliffSeason>();
    }

    private void Awake()
    {
        TryLoadConfig();
    }

    private void TryLoadConfig()
    {
        config = Resources.Load<AI_TEMP_CliffSeasonConfig>(ConfigPath);
        if (config == null && !warned)
        {
            warned = true;
            Debug.LogWarning("[AI TEMP-005] 没有找到悬崖四季配置：Assets/Resources/AI_TEMP/CliffSeasonConfig.asset\n" +
                             "请回到 Unity 等编译完成，或点菜单 AI TEMP > 重新生成悬崖四季配置。");
        }
    }

    private void LateUpdate()
    {
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

        int season = Mathf.Clamp((int)gm.season, 0, 3);
        ApplyCliff(season);
        ApplyBottom(season);
    }

    private void ApplyCliff(int season)
    {
        DualGridRuleTile target = config.GetCliff(season);
        if (target == null) return;

        DualGridTilemapModule[] modules = FindObjectsOfType<DualGridTilemapModule>(true);
        for (int i = 0; i < modules.Length; i++)
        {
            DualGridTilemapModule module = modules[i];
            if (module == null || module.RenderTile == null) continue;
            if (!module.RenderTile.name.StartsWith("悬崖_")) continue;
            if (module.RenderTile == target) continue;

            module.RenderTile = target;
            module.RefreshRenderTilemap();
            Debug.Log("[AI TEMP-005] 悬崖已切换到 " + SeasonText(season) + "。");
        }
    }

    private void ApplyBottom(int season)
    {
        Sprite[] sprites = config.GetBottom(season);
        if (sprites == null || sprites.Length == 0) return;

        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>(true);
        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tm = tilemaps[i];
            if (tm == null || tm.gameObject.name != BottomName) continue;

            usedTiles.Clear();
            BoundsInt bounds = tm.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase tb = tm.GetTile(pos);
                if (tb == null) continue;
                if (!usedTiles.Contains(tb)) usedTiles.Add(tb);
            }

            bool changed = false;
            for (int t = 0; t < usedTiles.Count; t++)
            {
                Tile tile = usedTiles[t] as Tile;
                if (tile == null || tile.sprite == null) continue;

                int index = IndexSuffix(tile.sprite.name);
                if (index < 0 || index >= sprites.Length || sprites[index] == null) continue;
                if (tile.sprite == sprites[index]) continue;

                tile.sprite = sprites[index];
                changed = true;
            }

            if (changed)
            {
                tm.RefreshAllTiles();
                Debug.Log("[AI TEMP-005] 悬崖底部已切换到 " + SeasonText(season) + "。");
            }
        }
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

    private static string SeasonText(int season)
    {
        if (season == 0) return "春";
        if (season == 1) return "夏";
        if (season == 2) return "秋";
        return "冬";
    }
}
