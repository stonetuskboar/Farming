// =====================================================================
// 【AI TEMP-007】白桦（临时）作弊种植 + 四季换图
// 搜索关键字：AI TEMP-007
//
// 用法（Play 模式）：
//   把鼠标指在地面上，按 B 键 → 在鼠标位置种一棵成年的白桦。
//   白桦会跟着季节自动换图。
//
// 说明：这棵白桦是运行时临时造的，id = 900，不写进 CropDataList，
//       不会被存进存档，也不会影响正式数据。
// 删除方法：删掉 AI_TEMP_Birch*.cs 三个脚本 + Resources/AI_TEMP/ 即可。
// =====================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AI_TEMP_BirchCheat : MonoBehaviour
{
    private const string ConfigPath = "AI_TEMP/BirchConfig";
    private const int BirchId = 900;
    private const int BirchItemId = 901;

    private AI_TEMP_BirchConfig config;
    private FarmLandManager farm;
    private TreeData birchData;
    private bool warned;
    private bool hotbarAdded;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_BirchCheat>() != null) return;

        GameObject go = new GameObject("[AI TEMP-007] BirchCheat");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_BirchCheat>();
    }

    private void Awake()
    {
        TryLoad();
    }

    private void TryLoad()
    {
        config = Resources.Load<AI_TEMP_BirchConfig>(ConfigPath);
        if (config == null)
        {
            if (!warned)
            {
                warned = true;
                Debug.LogWarning("[AI TEMP-007] 没有找到白桦配置：Assets/Resources/AI_TEMP/BirchConfig.asset\n" +
                                 "请先切好「白桦_春/夏/秋/冬」，再点菜单 AI TEMP > 重新生成白桦配置。");
            }
            return;
        }

        BuildData();
    }

    private void BuildData()
    {
        if (config.spring == null) return;

        birchData = new TreeData();
        birchData.Name = "白桦(临时)";
        birchData.id = BirchId;
        birchData.matureTurns = 0;
        birchData.CanGrowSeasons = new List<Season>();
        birchData.size = new Vector2Int(2, 2);
        birchData.stumpId = 115;
        birchData.DropedItemId = 0;
        birchData.DropedAmountMin = 0;
        birchData.DropedAmountMax = 0;
        birchData.growthStages = new List<GrowthStage>();

        GrowthStage stage = new GrowthStage();
        stage.GrowthProgress = 0;
        stage.GrowthSprite = config.spring;
        stage.风力 = WindType.微动;
        birchData.growthStages.Add(stage);

        Debug.Log("[AI TEMP-007] 白桦作弊已就绪：Play 里把鼠标指地面，按 B 种树。");
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            TryPlant();
        }

        // F9：重新发放一次白桦物品（热栏被重置时用）
        if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame)
        {
            hotbarAdded = false;
        }
    }

    private void LateUpdate()
    {
        if (config == null) return;

        if (farm == null)
        {
            farm = FindObjectOfType<FarmLandManager>();
            if (farm == null) return;
        }

        GameManager gm = farm.gameManager;
        if (gm == null) return;

        Sprite want = config.Get(Mathf.Clamp((int)gm.season, 0, 3));
        if (want == null) return;

        EnsureHotbarItem();

        foreach (BasicCrop crop in farm.crops)
        {
            if (crop == null || crop.cropData == null) continue;
            if (crop.cropData.id != BirchId) continue;
            if (crop.cropSr == null || crop.cropSr.sprite == want) continue;

            crop.cropSr.sprite = want;
        }
    }

    private void EnsureHotbarItem()
    {
        if (hotbarAdded || birchData == null || config == null || config.spring == null) return;

        GameManager gm = farm != null ? farm.gameManager : null;
        if (gm == null || gm.hotbarController == null) return;

        HotBarController hotbar = gm.hotbarController;
        if (hotbar.ButtonList == null || hotbar.ButtonList.Count == 0) return;
        if (hotbar.ItemList == null || hotbar.ItemList.Count == 0) return; // 等 Start 把道具发完
        if (hotbar.NowTool == null) return;

        if (hotbar.FindItemIndexById(BirchItemId) >= 0)
        {
            hotbarAdded = true;
            return;
        }

        AI_TEMP_BirchSeed seed = new AI_TEMP_BirchSeed(BirchItemId, birchData, config.spring);
        hotbar.AddItem(seed, 999);

        // 跳到白桦所在的那一页，避免停在空页
        hotbar.NowPage = Mathf.Max(0, (hotbar.ItemList.Count - 1) / hotbar.ButtonList.Count);
        hotbar.FreshButton();

        hotbarAdded = true;

        Debug.Log("[AI TEMP-007] 已在作弊栏加入「白桦（成年）」×999。");
    }

    private void TryPlant()
    {
        if (config == null) TryLoad();
        if (birchData == null)
        {
            Debug.LogWarning("[AI TEMP-007] 白桦配置还没生成，先点菜单 AI TEMP > 重新生成白桦配置。");
            return;
        }

        if (farm == null) farm = FindObjectOfType<FarmLandManager>();
        if (farm == null) return;

        Camera cam = Camera.main;
        if (cam == null || Mouse.current == null) return;

        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        world.z = 0f;

        List<Vector3Int> cells = GridCursorController.GetNearestCells(farm.grid, world, birchData.size);
        for (int i = 0; i < cells.Count; i++)
        {
            if (!farm.CanFarmHere(cells[i]))
            {
                Debug.Log("[AI TEMP-007] 这里不能种（不在可耕种区域）。");
                return;
            }
        }

        farm.Plant(cells, birchData);
        Debug.Log("[AI TEMP-007] 种下了一棵白桦（临时）。");
    }
}

// 作弊栏里的「白桦（成年）」物品：种下即成年，跟着季节换图
public class AI_TEMP_BirchSeed : UsableItem
{
    private CropData birchData;
    private Sprite icon;

    public AI_TEMP_BirchSeed(int id, CropData data, Sprite icon)
    {
        Id = id;
        birchData = data;
        this.icon = icon;
    }

    public override void Init(GameManager manager)
    {
        GameManager = manager;

        itemData = new ItemData();
        itemData.Id = Id;
        itemData.Name = "白桦（成年）";
        itemData.Icon = icon;
        itemData.Price = 0;
    }

    public override bool CanUse(List<Vector3Int> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (!CanUseThisCell(cells[i])) return false;
        }
        return true;
    }

    public override bool CanUseThisCell(Vector3Int cell)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        if (farm == null || Amount <= 0) return false;
        if (!farm.CanFarmHere(cell)) return false;
        if (farm.GetFarmTileData(cell).cropObject != null) return false;
        return true;
    }

    public override void Use(List<Vector3Int> cells)
    {
        FarmLandManager farm = GameManager.FarmLandManager;
        BasicCrop crop = farm.Plant(cells, birchData);
        crop.growthProgress = birchData.matureTurns;
        crop.SetByGrowthProgress();
        GameManager.hotbarController.DecreaseItem(Id, 1);
    }
}
