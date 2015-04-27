/* ****************************************************************
 * 
 * Name: MapDisplayAPI
 * 
 * Date Created: 2015-02-20
 * 
 * Original Team: Maps
 * 
 * Description: Display API
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * K. Huang         2015-02-20  Created
 * K. Huang         2015-03-02  Implemented Texture blocking
 */

using System;
using UnityEngine;

public class MapDisplayAPI : MapDebuggable {
    public const int BLOCK_SIZE = 1024;
    public const int SIZE_GRID = 256;
    public const int DEPTH_GRID = 5;
    public static readonly Color COLOR_CLEAR = new Color(0, 0, 0, 0);
    public static readonly Color COLOR_GRID = new Color(0, 0, 0, 50);
    public static readonly Color[] BLOCK_CLEAR;

    private static GameObject mapObject;
    private static GameObject gridObject;

    private static GameObject overlayObject;
    private static Texture2D overlayTexture;

    private static GameObject[] gridLevels;
    private static Texture2D gridTexture;

    private static GameObject[,] editorObjects;
    private static Texture2D[,] editorTextures;
    private static bool[,] editorDirty;

    /* Read-only overlay game object accessor. */
    public static GameObject OverlayObject { get { return overlayObject; } }

    /* Read-only overlay texture accessor. */
    public static Texture2D OverlayTexture { get { return overlayTexture; } }

    public static int Width { get { return World.Instance.Width; } set { World.Instance.Width = value; } }
    public static int Height { get { return World.Instance.Height; } set { World.Instance.Height = value; } }

    static MapDisplayAPI() {
        BLOCK_CLEAR = new Color[BLOCK_SIZE * BLOCK_SIZE];

        for (int i = 0; i < BLOCK_CLEAR.Length; i++) {
            BLOCK_CLEAR[i] = COLOR_CLEAR;
        }
    }

    public static void Init() {
        mapObject = GameObject.Find("MapObject");
        if (mapObject.GetComponent<SpriteRenderer>() == null) {
            mapObject.AddComponent<SpriteRenderer>();
        }

        gridObject = GameObject.Find("GridObject");

        InitializeOverlay();

        BuildTextures();
    }

    #region Overlay
    /* Loads the overlay texture from World.OverlayName.
     * Returns false if the overlay object isn't initialized or the overlay name is null/zero-length.
     */
    public static bool LoadOverlayTexture() {
        InitializeOverlay();

        if (!UnloadOverlayTexture()) {
            Debug("Failed to unload overlay.");
            return false;
        }

        if (World.OverlayName == null || World.OverlayName.Trim().Length < 1) {
            RefreshTransparency();
            Debug("No overlay to load.");
            return false;
        }

        var filename = OVERLAYS_DIRECTORY + World.OverlayName;
        overlayTexture = Resources.Load<Texture2D>(filename);

        if (overlayTexture == null) {
            Debug("Failed to load overlay. Check to see if it exists at '" + filename + "'.");
            return false;
        }

        overlayTexture.filterMode = FilterMode.Point;
        overlayObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(overlayTexture, new Rect(0, 0, overlayTexture.width, overlayTexture.height), new Vector2(0, 0), 1);
        overlayObject.transform.localScale = new Vector2(World.Instance.Width / (float)overlayTexture.width, World.Instance.Height / (float)overlayTexture.height);

        RefreshTransparency();

        return true;
    }

    /* Nullifies the overlay texture, returns true. */
    public static bool UnloadOverlayTexture() {
        overlayTexture = null;
        overlayObject.GetComponent<SpriteRenderer>().sprite = null;

        return true;
    }

    /* Parameterless version of InitializeDisplay(GameObject parent), calls InitializeDisplay(null). */
    public static bool InitializeOverlay() {
        return InitializeOverlay(null);
    }

    /* Refer to InitializeDisplay(GameObject parent, bool dieOnError), calls InitializeDisplay(parent, null). */
    public static bool InitializeOverlay(GameObject parent) {
        return InitializeOverlay(parent, false);
    }

    /* Initializes the display for the overlay.
     * If parent is null, attempts to initialize from GameObject.Find("MapObject").
     * If dieOnError is specified and the parent is null, returns false.
     * If dieOnError is specified and any errors are encountered, returns false.
     * If the overlay object has already been initialized or initializes, returns true.
     */
    public static bool InitializeOverlay(GameObject parent, bool dieOnError) {
        if (parent == null) {
            if (dieOnError) { return false; }

            return InitializeOverlay(GameObject.Find("MapObject"));
        }

        if (overlayObject == null) {
            overlayObject = new GameObject();
            overlayObject.transform.parent = parent.transform;
            overlayObject.transform.localPosition = new Vector2(0, 0);
            overlayObject.AddComponent<SpriteRenderer>();
            overlayObject.GetComponent<SpriteRenderer>().sortingOrder = -2;
            overlayObject.name = "MAP_OVERLAY";
        }

        return true;
    }
    #endregion Overlay

    public static void RefreshTransparency() {
        if (editorObjects == null) {
            BuildTextures();
        }

        var blocksX = (int)Math.Ceiling((float)Width / BLOCK_SIZE);
        var blocksY = (int)Math.Ceiling((float)Height / BLOCK_SIZE);

        var materialColor = new Color(1, 1, 1, overlayTexture == null ? 1 : 0.3f);

        for (int x = 0; x < blocksX; x++) {
            for (int y = 0; y < blocksY; y++) {
                editorObjects[x, y].GetComponent<SpriteRenderer>().renderer.material.color = materialColor;
            }
        }
    }

    public static void BuildTextures() {
        BuildEditorMap();
        BuildGrid();
    }

    public static void BuildEditorMap() {
        var mapCamera = mapObject.transform.FindChild("MapCamera");

        Vector3 position = new Vector3(Width / 2, Height / 2, -Height / 2);
        mapCamera.transform.localPosition = position;
        mapCamera.camera.orthographicSize = -position.z;

        var blocksX = (int)Math.Ceiling((float)Width / BLOCK_SIZE);
        var blocksY = (int)Math.Ceiling((float)Height / BLOCK_SIZE);

        var tmpEditorTextures = editorTextures;
        var tmpEditorObjects = editorObjects;
        var tmpEditorDirty = editorDirty;

        var currentX = (tmpEditorTextures == null ? 0 : tmpEditorTextures.GetLength(0));
        var currentY = (tmpEditorTextures == null ? 0 : tmpEditorTextures.GetLength(1));

        blocksX = Math.Max(blocksX, currentX);
        blocksY = Math.Max(blocksY, currentY);

        editorTextures = new Texture2D[blocksX, blocksY];
        editorObjects = new GameObject[blocksX, blocksY];
        editorDirty = new bool[blocksX, blocksY];

        Debug("Refreshing Blocks: (" + blocksX + ", " + blocksY + ")");
        for (var x = 0; x < blocksX; x++) {
            var existsX = x < currentX;
            for (var y = 0; y < blocksY; y++) {
                if (existsX && y < currentY) {
                    editorTextures[x, y] = tmpEditorTextures[x, y];
                    editorObjects[x, y] = tmpEditorObjects[x, y];
                    editorDirty[x, y] = tmpEditorDirty[x, y];
                } else {
                    editorTextures[x, y] = new Texture2D(BLOCK_SIZE, BLOCK_SIZE);
                    editorObjects[x, y] = new GameObject();
                    editorDirty[x, y] = false;
                }

                var t = editorTextures[x, y];
                t.filterMode = FilterMode.Point;
                t.SetPixels(BLOCK_CLEAR);
                t.Apply();

                var g = editorObjects[x, y];
                g.name = "EDITOR_BLOCK[" + x + ", " + y + "]";
                
                g.transform.parent = mapObject.transform;
                g.transform.localPosition = new Vector2(x * BLOCK_SIZE, y * BLOCK_SIZE);

                var spriteComponent = g.GetComponent<SpriteRenderer>();
                if (spriteComponent == null) {
                    spriteComponent = g.AddComponent<SpriteRenderer>();
                }

                spriteComponent.sortingOrder = -1;
                spriteComponent.sprite = Sprite.Create(t, new Rect(0, 0, BLOCK_SIZE, BLOCK_SIZE), new Vector2(0f, 0f), 1f);
            }
        }
    }

    public static void ApplyOnceEditor() {
        BuildTextures();

        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                ApplySinglePixel(x, y);
            }
        }

        FinishOperation();

        Vector3 t = new Vector3();
        t.y = Height;
        mapObject.transform.FindChild("UnitContainer").localPosition = t;
    }

    public static void ApplySinglePixel(int x, int y) {
        ApplySinglePixel(x, y, World.Instance.Tiles[x, y]);
    }

    public static void ApplySinglePixel(int x, int y, byte terrain) {
        ApplySinglePixel(x, y, Terrain.Get(terrain));
    }

    public static void ApplySinglePixel(int x, int y, Terrain terrain) {
        var yMod = Height - (y + 1);
        var blockX = x / BLOCK_SIZE;
        var blockY = yMod / BLOCK_SIZE;

        editorTextures[blockX, blockY].SetPixel(x % BLOCK_SIZE, yMod % BLOCK_SIZE, terrain.Color);
        editorDirty[blockX, blockY] = true;
    }

    public static void FinishOperation() {
        for (var x = 0; x < editorTextures.GetLength(0); x++) {
            for (var y = 0; y < editorTextures.GetLength(1); y++) {
                if (editorDirty[x, y]) {
                    editorTextures[x, y].Apply();
                    editorDirty[x, y] = false;
                }
            }
        }
    }

    public static void BuildGrid() {
        if (gridTexture == null) {
            gridTexture = new Texture2D(SIZE_GRID, SIZE_GRID);
        }

        gridTexture.wrapMode = TextureWrapMode.Repeat;
        gridTexture.filterMode = FilterMode.Bilinear;
        gridTexture.SetPixels(BLOCK_CLEAR);

        for (var x = 0; x < SIZE_GRID; x++) {
            for (var y = 0; y < SIZE_GRID; y++) {
                gridTexture.SetPixel(0, y, COLOR_GRID);
            }

            gridTexture.SetPixel(x, 0, COLOR_GRID);
        }

        gridTexture.Apply();

        ApplyGrid();
    }

    public static void ApplyGrid() {
        if (gridLevels == null) {
            gridLevels = new GameObject[DEPTH_GRID];
        }

        var sX = World.Instance.StartX;
        var sY = World.Instance.StartY;

        for (var level = 0; level < DEPTH_GRID; level++) {
            var scale = (float)Math.Pow(10, level);
            var gridLevel = gridLevels[level];

            if (gridLevel == null) {
                gridLevel = new GameObject();
                gridLevel.name = "GRID_LEVEL_" + level;
                gridLevel.transform.parent = gridObject.transform;
                gridLevel.transform.localPosition = new Vector3(0, 0, -scale / 5f);
                gridLevel.AddComponent<SpriteRenderer>();
            }

            float px = 0.5f, py = 0.5f;
            px += SIZE_GRID * (1 - (sX / scale));
            py += SIZE_GRID * (1 - ((Height - sY) / scale));

            gridLevel.GetComponent<SpriteRenderer>().sortingOrder = 1;
            gridLevel.GetComponent<SpriteRenderer>().sprite = Sprite.Create(gridTexture,
                new Rect(px, py, Width * SIZE_GRID / scale, Height * SIZE_GRID / scale),
                new Vector2(), SIZE_GRID / scale);

            gridLevels[level] = gridLevel;
        }
    }
}
