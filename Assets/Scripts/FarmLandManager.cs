using skner.DualGrid;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
//管理农田的数据以及相关操作
public class FarmLandManager : MonoBehaviour
{
    public GameManager gameManager;
    [Header("Tilemaps")]
    public Grid grid;
    [SerializeField] private Tilemap canFarmTileMap;
    //前景那个绿色的地，初始没有，玩家耕种了才有
    [SerializeField] private DualGridTilemapModule grassTilemap;
    [SerializeField] private DualGridTilemapModule dirtTilemap;
    [SerializeField] private DualGridTilemapModule tilledFieldTilemap;
    [SerializeField] private DualGridTilemapModule wateredFieldTilemap;
    [Header("Tile")]
    [SerializeField] private DualGridRuleTile springTile;
    [SerializeField] private DualGridRuleTile summerTile;
    [SerializeField] private DualGridRuleTile fallTile;
    [SerializeField] private DualGridRuleTile winterTile;
    [Header("Crop")]
    public int WiltPlantId;
    public GameObject cropPrefab;
    public Transform cropParent;
    public HashSet<BasicCrop> crops = new();
    public float PositionRandomRange = 0.2f;

    public Material CloudMaterial;
    [System.NonSerialized]
    public Dictionary<int, int> PlantDic = new();
    [System.NonSerialized]
    public Dictionary<int, int> HarvestDic = new();
    [System.NonSerialized]
    public Dictionary<int, int> WiltDic = new();
    [SerializeField] private Dictionary<Vector3Int, FarmTileData> farmTiles = new();

    public void Start()
    {
        foreach (Transform child in cropParent)
        {
            BasicCrop crop = child.GetComponent<BasicCrop>();
            if (crop == null)
            {
                continue;
            }
            CropData data = GameDataManager.Instance.cropDataList.GetTreeDataById(crop.cropData.id);
            if (data != null)
            {
                crop.cropData = data;
            }
            else
            {
                crop.cropData = GameDataManager.Instance.cropDataList.GetCropDataById(crop.cropData.id);
            }
            crop.SetFarmLand(this);
            crop.SetByGrowthProgress();
            crops.Add(crop);
            for (int i = 0; i < crop.TileCells.Count; i++)
            {
                GetFarmTileData(crop.TileCells[i]).cropObject = crop;
            }
        }
        SetWindRandom(gameManager.season);
    }

    public bool CanFarmHere(Vector3Int cell)
    {
        return canFarmTileMap.HasTile(cell);
    }

    public void ClearStatData()
    {
        PlantDic.Clear();
        HarvestDic.Clear();
        WiltDic.Clear();
    }
    public void OnNextTurn(int year, Season season, int month)
    {
        SetWindRandom(season);
        HashSet<BasicCrop> cropCopy = new(crops);
        foreach (BasicCrop crop in cropCopy)
        {
            crop.OnNextTurn(year, season, month);
        }
        if(month % 3 == 1)
        {
            if (season == Season.春)
            {
                grassTilemap.RenderTile = springTile;
            }
            else if (season == Season.夏)
            {
                grassTilemap.RenderTile = summerTile;
            }
            else if (season == Season.秋)
            {
                grassTilemap.RenderTile = fallTile;
            }
            else if (season == Season.冬)
            {
                grassTilemap.RenderTile = winterTile;
            }
            grassTilemap.RefreshRenderTilemap();
            foreach (Vector3Int pos in canFarmTileMap.cellBounds.allPositionsWithin)
            {
                int rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    CropData cropData;
                    int i = Random.Range(0, 10);
                    if (i < 8)
                    {
                        cropData = GameDataManager.Instance.cropDataList.GetCropDataById(Random.Range(31, 42));
                    }
                    else
                    {
                        cropData = GameDataManager.Instance.cropDataList.GetTreeDataById(Random.Range(15, 18));
                    }
                    List<Vector3Int> NearPos = GetNearestCells(pos, cropData.size);
                    if (CanPlantWeedTheseCell(NearPos) == true)
                    {
                        Plant(NearPos, cropData, IsRandom: true);
                    }
                }
            }
        }
        foreach (KeyValuePair<Vector3Int, FarmTileData> pair in farmTiles)
        {
            Vector3Int position = pair.Key;
            FarmTileData tileData = pair.Value;

            if (tileData.soilState == SoilState.Watered)
            {
                tileData.soilState = SoilState.Tilled;
                wateredFieldTilemap.ClearDataTile(position);
                tilledFieldTilemap.SetDataTile(position);
            }
            else if (tileData.cropObject == null)
            {
                if (tileData.soilState == SoilState.Tilled)
                {
                    int rand = Random.Range(0, 3);
                    if (rand == 0)
                    {
                        tileData.soilState = SoilState.Dirt;
                        tilledFieldTilemap.ClearDataTile(position);
                        dirtTilemap.SetDataTile(position);
                    }
                }
                else if (tileData.soilState == SoilState.Dirt)
                {
                    int rand = Random.Range(0, 3);
                    if (rand == 0)
                    {
                        tileData.soilState = SoilState.Grass;
                        dirtTilemap.ClearDataTile(position);
                        grassTilemap.SetDataTile(position);
                    }
                }
            }
        }
    }

    public FarmTileData GetFarmTileData(Vector3Int cell)
    {
        if (!farmTiles.TryGetValue(cell, out FarmTileData data))
        {
            data = new FarmTileData();
            farmTiles.Add(cell, data);
        }
        return data;
    }

    public void Till(Vector3Int cell)
    {
        FarmTileData data = GetFarmTileData(cell);
        data.soilState = SoilState.Tilled;

        tilledFieldTilemap.SetDataTile(cell);
        grassTilemap.ClearDataTile(cell);
        dirtTilemap.ClearDataTile(cell);
    }

    public void Water(Vector3Int cell)
    {
        FarmTileData data = GetFarmTileData(cell);
        data.soilState = SoilState.Watered;

        wateredFieldTilemap.SetDataTile(cell);
        tilledFieldTilemap.ClearDataTile(cell);
    }

    public BasicCrop Plant(Vector3Int cell, CropData cropData, bool IsRandom = false)
    {
        List<Vector3Int> cells = new();
        cells.Add(cell);
        return Plant(cells, cropData,IsRandom);
    }
    public BasicCrop Plant(List<Vector3Int> cells, CropData cropData, bool IsRandom = false)
    {
        Vector3 sum = Vector3.zero;

        foreach (var cell in cells)
        {
            sum += grid.GetCellCenterWorld(cell);
        }
        Vector3 center = sum / cells.Count;
        if(IsRandom == true)
        {
            Vector3 randomPos = Random.insideUnitCircle * PositionRandomRange * Mathf.Sqrt(cells.Count);
            center = center + randomPos;
        }
        return Plant(cells,cropData, center);
    }
    public BasicCrop Plant(List<Vector3Int> cells, CropData cropData, Vector3 Position)
    {
        GameObject obj = Instantiate(
            cropPrefab,
            Position,
            Quaternion.identity,
            cropParent
        );
        BasicCrop crop = obj.GetComponent<BasicCrop>();
        crops.Add(crop);
        foreach (Vector3Int cell in cells)
        {
            FarmTileData data = GetFarmTileData(cell);
            data.cropObject = crop;
        }
        crop.Init(this, cropData, cells);
        return crop;
    }
    public void ReplaceToWiltPlant(BasicCrop crop)
    {
        foreach (Vector3Int cell in crop.TileCells)
        {
            FarmTileData data = GetFarmTileData(cell);
            data.cropObject = null;
        }
        crops.Remove(crop);
        Destroy(crop.gameObject);
        CropData WiltPlantData = GameDataManager.Instance.cropDataList.GetCropDataById(WiltPlantId);
        Plant(crop.TileCells, WiltPlantData);
        Debug.Log(crop.cropData.Name);
        if (WiltDic.TryAdd(crop.cropData.id, 1) == false)
        {
            WiltDic[crop.cropData.id] += 1;
        }
    }
    public void Harvest(BasicCrop crop)
    {
        if (crop == null)
            return;
        // 收获作物
        // 移除作物对象
        foreach (Vector3Int cell in crop.TileCells)
        {
            FarmTileData data = GetFarmTileData(cell);
            data.cropObject = null;
        }
        crops.Remove(crop);
        if(crop.IsMature() == true)
        {
            int amount = Random.Range(crop.cropData.DropedAmountMin, crop.cropData.DropedAmountMax +1);
            gameManager.HarvestEffectManager.PlayHarvestEffect(crop.transform.position, crop.cropData.DropedItemId, amount);
            if (HarvestDic.TryAdd(crop.cropData.id, 1) == false)
            {
                HarvestDic[crop.cropData.id] += 1;
            }
        }
        else
        {
            gameManager.HarvestEffectManager.PlayParticle(crop.transform.position);
        }

        Destroy(crop.gameObject);
    }
    public bool CanPlantWeedTheseCell(List<Vector3Int> cells)
    {
        foreach (Vector3Int cell in cells)
        {
            if (CanPlantWeedThisCell(cell) == false)
            {
                return false;
            }
        }
        return true;
    }


    public bool CanPlantWeedThisCell(Vector3Int cell)
    {
        if (CanFarmHere(cell) == false)
        {
            return false;
        }
        else
        {
            FarmTileData data = GetFarmTileData(cell);

            if (data.soilState != SoilState.Grass)
            {
                return false;
            }
            else if (data.cropObject != null)
            {
                return false;
            }
        }
        return true;
    }

    public List<Vector3Int> GetNearestCells(Vector3Int cellPos, Vector2Int size)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        int m = size.x;
        int n = size.y;

        // 计算区域左下角的偏移量
        int startX = cellPos.x -(m / 2);
        int startY = cellPos.y - (n / 2);

        for (int x = 0; x < m; x++)
        {
            for (int y = 0; y < n; y++)
            {
                cells.Add(new Vector3Int(startX + x, startY + y, cellPos.z));
            }
        }

        return cells;
    }

    public void SetWindRandom(Season season)
    {
        float angle;

        if (Random.value < 0.5f)
        {
            angle = Random.Range(-30f, 30f);
        }
        else
        {
            angle = Random.Range(150f, 210f);
        }
        float rad = angle * Mathf.Deg2Rad;

        Vector2 direction = new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        );

        Shader.SetGlobalVector("_WindDirection", direction);
        CloudMaterial.SetVector("_CloudDirection", Random.insideUnitCircle.normalized);
        Shader.SetGlobalFloat("_SeasonStrength", Random.Range(0.7f,1.3f));
        if (season == Season.春)
        {
            CloudMaterial.SetFloat("_ShadowStrength", Random.Range(0.4f, 0.8f));
            CloudMaterial.SetFloat("_CloudScale", Random.Range(0.12f, 0.2f));
            CloudMaterial.SetFloat("_CloudContrast", Random.Range(0.3f, 1.5f));
            CloudMaterial.SetFloat("_CloudThreshold", 0.5f);
            CloudMaterial.SetFloat("_Speed", Random.Range(0.05f, 0.2f));
            CloudMaterial.SetFloat("_CloudSoftness", 0.05f);

        }
        else if(season == Season.夏)
        {
            CloudMaterial.SetFloat("_ShadowStrength", Random.Range(0.3f, 0.6f));
            CloudMaterial.SetFloat("_CloudScale", Random.Range(0.2f, 0.3f));
            CloudMaterial.SetFloat("_CloudContrast", Random.Range(0.1f, 0.4f));
            CloudMaterial.SetFloat("_CloudThreshold", 0.5f);
            CloudMaterial.SetFloat("_Speed", Random.Range(0.1f, 0.3f));
            CloudMaterial.SetFloat("_CloudSoftness", 0.05f);
        }
        else if (season == Season.秋)
        {
            CloudMaterial.SetFloat("_ShadowStrength", Random.Range(0.5f, 0.7f));
            CloudMaterial.SetFloat("_CloudScale", Random.Range(0.2f, 0.4f));
            CloudMaterial.SetFloat("_CloudContrast", Random.Range(0.2f, 0.5f));
            CloudMaterial.SetFloat("_CloudThreshold", 0.5f);
            CloudMaterial.SetFloat("_Speed", Random.Range(0.05f, 0.15f));
            CloudMaterial.SetFloat("_CloudSoftness", 0.05f);
        }

    }
}
