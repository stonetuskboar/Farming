using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneCropData", menuName = "Farm/SceneCropData")]
public class SceneCropData : ScriptableObject
{

}
[System.Serializable]

public class SceneCrop
{
    public bool IsTree;
    public int cropDataId;
    public List<Vector3Int> TileCells = new();
    public int growthProgress;
}