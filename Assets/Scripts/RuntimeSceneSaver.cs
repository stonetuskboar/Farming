using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public static class RuntimeSceneSaver
{
    static List<SceneCrop> list = new();
    static GameDataManager gameDataManager;
    static RuntimeSceneSaver()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // 即将退出 Play Mode
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            FarmLandManager manager = Object.FindFirstObjectByType<FarmLandManager>();
            gameDataManager = GameDataManager.Instance;
            if (manager != null)
            {
                list.Clear();
                foreach(BasicCrop crop in manager.crops)
                {
                    SceneCrop cropdata = new(crop);
                    list.Add(cropdata);
                }
            }
        }

        // 已经回到 Edit Mode
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (list == null)
                return;
            FarmLandManager manager = Object.FindFirstObjectByType<FarmLandManager>();
            for (int i = manager.cropParent.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(
                    manager.cropParent.GetChild(i).gameObject
                );
            }
            foreach (SceneCrop crop in list)
            {
                GameObject prefab = manager.cropPrefab;
                GameObject obj =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab, manager.cropParent);
                BasicCrop cropObj = obj.GetComponent<BasicCrop>();
                cropObj.transform.position = crop.position;
                cropObj.transform.rotation = crop.rotation;
                cropObj.transform.localScale = crop.scale;
                cropObj.TileCells = crop.TileCells;
                cropObj.cropData = crop.data;
                cropObj.growthProgress = crop.growthProgress;
                cropObj.SetByGrowthProgress();
            }
            EditorSceneManager.MarkSceneDirty(
            manager.gameObject.scene
            );
            list.Clear();
        }
    }
}

#endif

[System.Serializable]

public class SceneCrop
{
    public CropData data;
    public List<Vector3Int> TileCells = new();
    public int growthProgress;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public SceneCrop(BasicCrop crop)
    {
        data = crop.cropData;
        TileCells.Clear();
        TileCells.AddRange(crop.TileCells);
        growthProgress = crop.growthProgress;

        position = crop.transform.position;
        rotation = crop.transform.rotation;
        scale = crop.transform.localScale;
    }
}