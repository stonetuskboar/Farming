using skner.DualGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private DualGridTilemapModule fieldTilemap;
    [SerializeField] private DualGridTilemapModule tilledFieldTilemap;
    [SerializeField] private DualGridTilemapModule wateredFieldTilemap;


    [Header("Crop")]
    [SerializeField] private GameObject cropPrefab;
    [SerializeField] private Transform cropParent;
    public HashSet<BasicCrop> crops = new();

    public Dictionary<Vector3Int, FarmTileData> farmTiles = new();

    public bool CanFarmHere(Vector3Int cell)
    {
        return canFarmTileMap.HasTile(cell);
    }

    public void OnNextTurn()
    {
        foreach (BasicCrop crop in crops)
        {
            crop.OnNextTurn();
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
    }

    public void Water(Vector3Int cell)
    {
        FarmTileData data = GetFarmTileData(cell);
        data.soilState = SoilState.Watered;

        wateredFieldTilemap.SetDataTile(cell);
        tilledFieldTilemap.ClearDataTile(cell);
    }

    public void Plant(Vector3Int cell, CropData cropData)
    {
        List<Vector3Int> cells = new();
        cells.Add(cell);
        Plant(cells, cropData);
    }
    public void Plant(List<Vector3Int> cells, CropData cropData)
    {
        Vector3 sum = Vector3.zero;

        foreach (var cell in cells)
        {
            sum += grid.GetCellCenterWorld(cell);
        }
        Vector3 center = sum / cells.Count;
        GameObject obj = Instantiate(
            cropPrefab,
            center,
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
    }

    public void Harvest(BasicCrop crop)
    {
        if (crop == null || !crop.IsMature())
            return;
        // 收获作物
        // 移除作物对象
        foreach (Vector3Int cell in crop.TileCells)
        {
            FarmTileData data = GetFarmTileData(cell);
            data.cropObject = null;
            data.soilState = SoilState.Dirt;
            wateredFieldTilemap.ClearDataTile(cell);
            tilledFieldTilemap.ClearDataTile(cell);
            fieldTilemap.ClearDataTile(cell);
            grassTilemap.SetDataTile(cell);
        }
        crops.Remove(crop);
        gameManager.HarvestEffectManager.PlayHarvestEffect(crop.transform.position,crop.cropData.DropedItemId,crop.cropData.DropedAmount);
        Destroy(crop.gameObject);
    }
}
