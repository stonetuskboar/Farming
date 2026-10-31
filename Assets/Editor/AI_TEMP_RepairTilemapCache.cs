// =====================================================================
// 【AI TEMP-006】修复四季贴图缓存（临时工具·仅编辑器）
// 搜索关键字：AI TEMP-006
//
// 作用：把场景里所有 DualGrid 图层的渲染重建一遍，
//       清掉之前运行时换季留下的 Winter/Summer Sprite 缓存。
//
// 用法：退出 Play → 菜单 AI TEMP → 修复四季贴图缓存 → Ctrl+S 保存场景。
// 删除方法：删掉本文件即可。
// =====================================================================
#if UNITY_EDITOR
using skner.DualGrid;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AI_TEMP_RepairTilemapCache
{
    [MenuItem("AI TEMP/修复四季贴图缓存")]
    public static void Repair()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("AI TEMP-006", "请先退出 Play 模式，再执行修复。", "好");
            return;
        }

        DualGridTilemapModule[] modules = Object.FindObjectsOfType<DualGridTilemapModule>(true);
        int count = 0;

        for (int i = 0; i < modules.Length; i++)
        {
            DualGridTilemapModule module = modules[i];
            if (module == null || module.RenderTile == null) continue;

            module.RefreshRenderTilemap();
            count++;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        AssetDatabase.SaveAssets();

        Debug.Log("[AI TEMP-006] 已重建 " + count + " 个 DualGrid 图层的显示缓存。请按 Ctrl+S 保存场景。");
    }
}
#endif
