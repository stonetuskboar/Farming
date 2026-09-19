using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTileData
{
    public BasicCrop cropObject = null;
    public SoilState soilState;
}
public enum SoilState
{
    Dirt,       // 荒地
    Tilled,     // 已锄地
    Watered,    // 已浇水
}