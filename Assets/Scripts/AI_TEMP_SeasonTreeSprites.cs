// =====================================================================
// 【AI TEMP-002】树的四季换图（临时补丁·运行部分）
// 搜索关键字：AI TEMP-002
//
// 作用：按季节替换 松树 / 柏树 / 橡树 在「2阶段」和「成熟」时的贴图。
//       0阶段（种子）、1阶段（小苗）、树桩保持原样不动。
//
// 特点：不改任何原有脚本、不改场景、不需要手动挂组件。
//       运行时会自动创建一个 DontDestroyOnLoad 的隐藏物体。
//
// 依赖：Assets/Resources/AI_TEMP/SeasonTreeConfig.asset
//       该文件由 Assets/Editor/AI_TEMP_SeasonTreeSetup.cs 自动生成。
//
// 删除方法：删掉 AI_TEMP_*.cs 三个脚本 + Assets/Resources/AI_TEMP/ 文件夹即可。
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

public class AI_TEMP_SeasonTreeSprites : MonoBehaviour
{
    private const string ConfigPath = "AI_TEMP/SeasonTreeConfig";

    private AI_TEMP_SeasonTreeConfig config;
    private FarmLandManager farm;
    private readonly List<BasicCrop> buffer = new List<BasicCrop>();
    private bool warned;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_SeasonTreeSprites>() != null) return;

        GameObject go = new GameObject("[AI TEMP-002] SeasonTreeSprites");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_SeasonTreeSprites>();
    }

    private void Awake()
    {
        TryLoadConfig();
    }

    private void TryLoadConfig()
    {
        config = Resources.Load<AI_TEMP_SeasonTreeConfig>(ConfigPath);
        if (config == null && !warned)
        {
            warned = true;
            Debug.LogWarning("[AI TEMP-002] 没有找到四季树配置：Assets/Resources/AI_TEMP/SeasonTreeConfig.asset\n" +
                             "请回到 Unity 等它编译完成（会自动生成），或点菜单 AI TEMP > 重新生成四季树配置。");
        }
    }

    private void LateUpdate()
    {
        if (config == null)
        {
            TryLoadConfig();
            return;
        }

        if (farm == null)
        {
            farm = FindObjectOfType<FarmLandManager>();
            if (farm == null) return;
        }

        GameManager gm = farm.gameManager;
        if (gm == null) return;

        int season = Mathf.Clamp((int)gm.season, 0, 3); // 春0 夏1 秋2 冬3

        buffer.Clear();
        buffer.AddRange(farm.crops);

        for (int i = 0; i < buffer.Count; i++)
        {
            BasicCrop crop = buffer[i];
            if (crop == null || crop.cropData == null || crop.cropSr == null) continue;

            TreeData tree = crop.cropData as TreeData;
            if (tree == null || tree.IsStump) continue;

            AI_TEMP_SeasonTreeEntry entry = GetEntry(tree.id);
            if (entry == null) continue;

            Sprite want = null;
            if (crop.IsMature())
            {
                if (entry.stage3 != null && entry.stage3.Length > season) want = entry.stage3[season];
            }
            else if (crop.growthProgress == 2)
            {
                if (entry.stage2 != null && entry.stage2.Length > season) want = entry.stage2[season];
            }

            if (want != null && crop.cropSr.sprite != want)
            {
                crop.cropSr.sprite = want;
            }
        }
    }

    private AI_TEMP_SeasonTreeEntry GetEntry(int treeId)
    {
        for (int i = 0; i < config.trees.Count; i++)
        {
            if (config.trees[i] != null && config.trees[i].treeId == treeId)
            {
                return config.trees[i];
            }
        }
        return null;
    }
}
