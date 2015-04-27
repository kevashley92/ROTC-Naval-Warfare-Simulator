using UnityEngine;

public abstract class MapInterfacePanel : MapDebuggableBehaviour {
    public static readonly Terrain DEFAULT_PAINT_TERRAIN = Terrain.Get("LAND");
    public static readonly Terrain DEFAULT_CLEAR_TERRAIN = Terrain.Get("WATER");

    public GameObject uiMapEditor;
    public GameObject uiTerrainEditor;
}
