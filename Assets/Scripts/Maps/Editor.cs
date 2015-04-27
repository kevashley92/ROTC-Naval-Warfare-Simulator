/* ****************************************************************
 * 
 * Name: Editor
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Maps
 * 
 * Description: Provides API for editing maps.
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * D. York          2015-02-23  Created
 */

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Maps;

public class Editor : MapInterfacePanel, ISave {
    private static KeyBinding keyPickWater;
    private static KeyBinding keyPickLand;
    private static KeyBinding keyPickMountain;
    private static KeyBinding keyPickUndefined;
    private static KeyBinding keyPickWooded;
    private static KeyBinding keyPickHeavyBrush;
    private static KeyBinding keySwapTerrain;
    private static KeyBinding keyDefaultTerrain;
    private static KeyBinding keyUndo;
    private static KeyBinding keyRedo;

    public static KeyBinding KeyPickWater { get { return keyPickWater; } }
    public static KeyBinding KeyPickLand { get { return keyPickLand; } }
    public static KeyBinding KeyPickMountain { get { return keyPickMountain; } }
    public static KeyBinding KeyPickUndefined { get { return keyPickUndefined; } }
    public static KeyBinding KeyPickWooded { get { return keyPickWooded; } }
    public static KeyBinding KeyPickHeavyBrush { get { return keyPickHeavyBrush; } }
    public static KeyBinding KeySwapTerrain { get { return keySwapTerrain; } }
    public static KeyBinding KeyDefaultTerrain { get { return keyDefaultTerrain; } }
    public static KeyBinding KeyUndo { get { return keyUndo; } }
    public static KeyBinding KeyRedo { get { return keyRedo; } }

    static Editor() {
        keyPickWater = new KeyBinding(KeyCode.Z);
        keyPickLand = new KeyBinding(KeyCode.X);
        keyPickMountain = new KeyBinding(KeyCode.C);
        keyPickUndefined = new KeyBinding(KeyCode.V);
        keyPickWooded = new KeyBinding(KeyCode.A);
        keyPickHeavyBrush = new KeyBinding(KeyCode.F);
        keySwapTerrain = new KeyBinding(KeyCode.S);
        keyDefaultTerrain = new KeyBinding(KeyCode.D);
        keyUndo = new KeyBinding(new KeyCombo(KeyCode.LeftControl, KeyCode.Z), new KeyCombo(KeyCode.RightControl, KeyCode.Z));
        keyRedo = new KeyBinding(new KeyCombo(KeyCode.LeftControl, KeyCode.Y), new KeyCombo(KeyCode.RightControl, KeyCode.Y));
    }

    private const float MAX_BRUSH_SIZE = 30;

    private OperationManager operationManager;
    public float brushSize; //radius of brush
    public ToggleGroup tools;
    private Point startPoint;
    private enum ToolType { FILL, BRUSH, LINE, SHAPE };
    private enum BrushType { CIRCLE, SQUARE };
    private ToolType currentTool;
    private BrushType currentBrush;
    private Point previous;
    private static byte[,] tiles;
    public LoadWindowController fileBrowser;
    public SaveWindowController saveDialog;
    public ComboBox terrainSelect;

    private Terrain paintCurrent;
    private Terrain paintPrevious;

    private static Editor EditorInstance;

    private bool mouseRightDrag;
    private bool mouseRightDown;

    public TemplateLoader templateLoader;
    public MapLoader mapLoader;

    public static string mapName;

    private int SelectedIndex { get { return terrainSelect.SelectedIndex; } }
    private Terrain SelectedTerrain { get {int i = terrainSelect.Items[SelectedIndex].LocalItem.label.LastIndexOf("p_"); return Terrain.Get(terrainSelect.Items[SelectedIndex].LocalItem.label.Substring(i+2)); } }

    void Awake() {
        EditorInstance = this;
		mapName = "";
		paintCurrent = DEFAULT_PAINT_TERRAIN;
		paintPrevious = DEFAULT_CLEAR_TERRAIN;
		
		tiles = null;
		operationManager = new OperationManager();
		brushSize = 15;
		currentBrush = BrushType.CIRCLE;
		currentTool = ToolType.BRUSH;
		
		RefreshTerrainList();
		
		terrainSelect.SelectedIndex = 2;
		terrainSelect.OnSelectionChanged += new Action<int>(ChangeTerrainSelected);
		
		templateLoader = new TemplateLoader(this);
		mapLoader = new MapLoader(this);
		mapName = "Default_Map";
	}
	
	void Start() {
		
	}

    void OnEnable() {
        RefreshTerrainList();
    }

	public void ChangeTerrainSelected(int index) {

        Debug("Interface selection changed.");
        var terrain = SelectedTerrain;
        ChangeTerrainSelectedByID(terrain.ID);
    }

    public void ChangeTerrainSelectedByID(int id) {
        Pick(Terrain.Get(id));
        TerrainEditor.ChangeSelected(id);
    }

    public void RefreshTerrainList() {
        Debug("Refreshing interface terrain list.");

        terrainSelect.ClearItems();
        terrainSelect.AddItems(new string[] { "" });
        
        LocalizedItem[] itemList = new LocalizedItem[Terrain.Names.Length];
        for(int i = 0; i < Terrain.Names.Length; i++)
            itemList[i] = new LocalizedItem("UI_Admin", "Text_Map_"+Terrain.Names[i]);
        terrainSelect.AddItems(itemList);

        UpdateSelection();
    }

    public void UpdateSelection() {
        Debug("Updating interface terrain selection.");
        terrainSelect.SelectedIndex = Terrain.NamesList.IndexOf(paintCurrent.Name) + 1;
    }

    public void SwapPick() {
        Debug("Swapping pick.");
        Pick(paintPrevious);
    }

    public void Pick(byte id) {
        Pick(Terrain.Get(id));
    }

    public void Pick(string name) {
        if (name != null) {
            Pick(Terrain.GetID(name));
        }
    }

    public void Pick(Terrain terrain) {
        if (terrain != null) {
            if (paintCurrent.ID != terrain.ID) {
                paintPrevious = paintCurrent;
                paintCurrent = terrain;
                Debug("Picked: " + paintCurrent);

                /* Update the interface selection. */
                UpdateSelection();
            } else {
                Debug("Picked same terrain.", false);
            }
        } else {
            Debug("Picked null terrain.", false);
        }
    }

    void Update() {
        if (UIMenuController.IsActiveMenu(uiMapEditor)) {
            if (KeyUndo.Released) {
                Undo();
            } else if (KeyRedo.Released) {
                Redo();
            } else {
                if (KeyDefaultTerrain.Pressed) {
                    Pick(DEFAULT_CLEAR_TERRAIN);
                    Pick(DEFAULT_PAINT_TERRAIN);
                } else if (KeySwapTerrain.Pressed) {
                    SwapPick();
                } else if (KeyPickWater.Pressed) {
                    Pick("WATER");
                } else if (KeyPickLand.Pressed) {
                    Pick("LAND");
                } else if (KeyPickUndefined.Pressed) {
                    Pick("UNDEFINED");
                } else if (KeyPickMountain.Pressed) {
                    Pick("MOUNTAIN");
                } else if (KeyPickWooded.Pressed) {
                    Pick("WOODED");
                } else if (KeyPickHeavyBrush.Pressed) {
                    Pick("HEAVY_BRUSH");
                }
            }
        }
    }

    // TODO: Look into right mouse
    /*void OnMouseOver() {
        if (MapMenu.activeSelf) {
            //Debug.Log(Input.GetMouseButton(1) + ", " + Input.GetMouseButtonDown(1) + ", " + Input.GetMouseButtonUp(1));
        }
    }*/

    public void OnMouseDown() {
        if (tiles == null) {
            tiles = World.Instance.Tiles;
        }

        if (UIMenuController.IsActiveMenu(uiMapEditor)) {
            Debug("Left: " + Input.GetMouseButton(0) + "  Right: " + Input.GetMouseButton(1));
            Debug("Should be doing something.");
            operationManager.BeginDraw();
            var gridPos = MouseToGridAPI.GetGridCoordinate(Input.mousePosition);
            Point point = new Point((int)gridPos.x, (int)gridPos.y);
            switch (currentTool) {
                case ToolType.FILL:
                    Fill(point, paintCurrent.ID, tiles[Convert.ToInt32(point.x), Convert.ToInt32(point.y)]);
                    MapDisplayAPI.FinishOperation();
                    break;
                case ToolType.LINE:
                case ToolType.SHAPE:
                    startPoint = point;
                    break;
                case ToolType.BRUSH:
                    previous = point;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnMouseDrag() {
        if (UIMenuController.IsActiveMenu(uiMapEditor)) {
            var gridPos = MouseToGridAPI.GetGridCoordinate(Input.mousePosition);
            Point point = new Point((int)gridPos.x, (int)gridPos.y);
            switch (currentTool) {
                case ToolType.BRUSH:
                    DrawLine(previous, point, paintCurrent.ID);
                    previous = point;
                    MapDisplayAPI.FinishOperation();
                    break;
                default:
                    break;
            }
        }
    }

    public void OnMouseUp() {
        if (UIMenuController.IsActiveMenu(uiMapEditor)) {
            var gridPos = MouseToGridAPI.GetGridCoordinate(Input.mousePosition);
            Point point = new Point((int)gridPos.x, (int)gridPos.y);
            switch (currentTool) {
                case ToolType.LINE:
                    DrawLine(startPoint, point, paintCurrent.ID);
                    MapDisplayAPI.FinishOperation();
                    break;
                case ToolType.SHAPE:
                    int distance = (int)(Math.Sqrt(Math.Pow(startPoint.x - point.x, 2) + Math.Pow(startPoint.y - point.y, 2)));
                    int deltaX = Convert.ToInt32(startPoint.x) - Convert.ToInt32(point.x);
                    int deltaY = Convert.ToInt32(startPoint.y) - Convert.ToInt32(point.y);
                    Point center = new Point(startPoint.x - (deltaX / 2), startPoint.y - (deltaY / 2));
                    DrawWithBrush(center, distance / 2, paintCurrent.ID);
                    MapDisplayAPI.FinishOperation();
                    break;
                case ToolType.BRUSH:
                    previous = null;
                    break;
                default:
                    break;
            }
            operationManager.EndDraw();
        }
    }

    public void SetTool(string tool) {
        switch (tool) {
            case "BRUSH":
                currentTool = ToolType.BRUSH;
                break;
            case "FILL":
                currentTool = ToolType.FILL;
                break;
            default:
                break;
        }
    }

    public void SetBrush(string brush) {
        switch (brush) {
            case "CIRCLE":
                currentBrush = BrushType.CIRCLE;
                break;
            case "SQUARE":
                currentBrush = BrushType.SQUARE;
                break;
            default:
                break;
        }
    }

    private void DrawLine(Point start, Point end, byte fill) {
        int startX = Convert.ToInt32(start.x);
        int startY = Convert.ToInt32(start.y);
        int endX = Convert.ToInt32(end.x);
        int endY = Convert.ToInt32(end.y);

        int deltaX = endX - startX;
        int deltaY = endY - startY;

        if (deltaX == 0 && deltaY == 0) {
            DrawWithBrush(start, fill);
        } else if (Math.Abs(deltaX) >= Math.Abs(deltaY)) {
            int change = 1;
            if (deltaX < 0)
                change = -1;

            for (int x = startX; x != endX; x += change) {
                int y = startY + (deltaY * (x - startX) / deltaX);
                DrawWithBrush(new Point(x, y), fill);
            }
        } else {
            int change = 1;
            if (deltaY < 0)
                change = -1;

            for (int y = startY; y != endY; y += change) {
                int x = startX + (deltaX * (y - startY) / deltaY);
                DrawWithBrush(new Point(x, y), fill);
            }
        }
    }

    private void DrawWithBrush(Point point, int radius, byte fill) {
        switch (currentBrush) {
            case BrushType.CIRCLE:
                CircleBrush(point, radius, fill);
                break;
            case BrushType.SQUARE:
                SquareBrush(point, radius, fill);
                break;
            default:
                break;
        }
    }

    private void DrawWithBrush(Point point, byte fill) {
        DrawWithBrush(point, (int)brushSize, fill);
    }

    public void SetBrushSize(float brushSize) {
        this.brushSize = brushSize - 1;
    }

    private void CircleBrush(Point point, int radius, byte fill) {
        int x0 = Convert.ToInt32(point.x);
        int y0 = Convert.ToInt32(point.y);

        int x = radius;
        int y = 0;
        int radiusError = 1 - x;

        ////Debug.Log("Paint color: " + terrainToPaint);

        while (x >= y) {
            for (int i = x0 - x; i <= x0 + x; i++) {
                if (InBounds(i, y0 + y))
                    operationManager.changeTile(i, y0 + y, fill);
                if (InBounds(i, y0 - y))
                    operationManager.changeTile(i, y0 - y, fill);
            }
            for (int i = x0 - y; i <= x0 + y; i++) {
                if (InBounds(i, y0 + x))
                    operationManager.changeTile(i, y0 + x, fill);
                if (InBounds(i, y0 - x))
                    operationManager.changeTile(i, y0 - x, fill);
            }

            y++;

            if (radiusError < 0) {
                radiusError += 2 * y + 1;
            } else {
                x--;
                radiusError += 2 * (y - x) + 1;
            }
        }
    }

    private void Fill(Point point, byte fill, byte replace) {
        if (fill == replace) {
            Debug("Tried to fill same terrain type.", false);
            return;
        }

        bool left, right, xGreaterThanZero, xLessThanWidthMinusOne;

        int w = tiles.GetLength(0);
        int h = tiles.GetLength(1);

        int x, y, y1, x2, x3;

        Stack<Point> pointStack = new Stack<Point>();
        pointStack.Push(point);
        Point currentPoint;

        while (pointStack.Count != 0) {
            currentPoint = pointStack.Pop();
            x = Convert.ToInt32(currentPoint.x);
            y1 = y = Convert.ToInt32(currentPoint.y);
            // Move all the way to bottom.
            while (y1 >= 0 && tiles[x, y1] == replace)
                y1--;
            y1++;

            left = right = false;
            // Move up until we run out of tiles to replace
            x2 = x - 1;
            x3 = x + 1;
            xGreaterThanZero = x > 0;
            xLessThanWidthMinusOne = x < w - 1;
            while (y1 < h && tiles[x, y1] == replace) {
                operationManager.changeTile(x, y1, fill);

                if (xGreaterThanZero) {
                    if (!left && tiles[x2, y1] == replace) {
                        pointStack.Push(new Point(x2, y1));
                        left = true;
                    } else if (left && x > 0 && tiles[x2, y1] != replace) {
                        left = false;
                    }
                }

                if (xLessThanWidthMinusOne) {
                    x3 = x + 1;
                    if (!right && tiles[x3, y1] == replace) {
                        pointStack.Push(new Point(x3, y1));
                        right = true;
                    } else if (right && tiles[x3, y1] != replace) {
                        right = false;
                    }
                }

                y1++;
            }
        }
    }

    public void SquareBrush(Point point, int length, byte fill) {
        int x = Convert.ToInt32(point.x);
        int y = Convert.ToInt32(point.y);

        for (int i = x - length; i <= x + length; i++) {
            for (int j = y - length; j <= y + length; j++) {
                operationManager.changeTile(i, j, fill);
            }
        }
    }

    public bool InBounds(int x, int y) {
        if (x >= 0 && x < tiles.GetLength(0)
            && y >= 0 && y < tiles.GetLength(1)) {
            return true;
        }
        return false;
    }

    public bool InBounds(Point point) {
        if (point.x >= 0 && point.x < tiles.GetLength(0)
            && point.y >= 0 && point.y < tiles.GetLength(1)) {
            return true;
        }
        return false;
    }

    public void Save(string fileName) {
        World.Save(fileName);
        mapName = fileName;
    }

    public void DisplaySavePanel() {
        saveDialog.Activate(mapName, this);
    }

    public void DisplayTemplateLoadPanel() {
        fileBrowser.Activate(World.ListTemplateFiles(), templateLoader);
    }

    public void DisplayMapLoadPanel() {
        fileBrowser.Activate(World.ListFilesIn(World.DEFAULT_DIRECTORY), mapLoader);
    }

    public class TemplateLoader : ILoad {
        private Editor editor;

        public TemplateLoader(Editor editor) {
            this.editor = editor;
        }

        public void Load(string filename) {
            World.Load(World.ValidateFilename(filename, null, World.TEMPLATE_DIRECTORY));
            editor.Reset(filename);
        }
    }

    public class MapLoader : ILoad {
        private Editor editor;

        public MapLoader(Editor editor) {
            this.editor = editor;
        }

        public void Load(string filename) {
            World.Load(World.ValidateFilename(filename, null, World.DEFAULT_DIRECTORY));
            editor.Reset(filename);
        }
    }

    public void Reset(string filename) {
        tiles = World.Instance.Tiles;
        MapDisplayAPI.ApplyOnceEditor();
        operationManager.Reset();
        if (filename != null) {
            mapName = filename;
        }
    }

    public void Undo() {
        operationManager.Undo();
        MapDisplayAPI.FinishOperation();
    }

    public void Redo() {
        operationManager.Redo();
        MapDisplayAPI.FinishOperation();
    }

    public void ResetMapToDefault() {
        Reset("");
    }

    /*public void AddMarineMap() {
        var map = World.MarineMaps[World.AvailableMarineMaps[marineAddMapBox.SelectedIndex]];
        World.Load(World.ValidateFilename(map.File, World.DEFAULT_EXTENSION, World.TEMPLATE_DIRECTORY));
        World.OverlayName = map.Overlay;
        Reset(map.Name);
    }*/

    public class TerrainPanel : ISave {
        public void Save(string filename) {
            if (filename == null) {
                filename = "";
            }

            filename = filename.Trim();
            filename = filename.ToUpper();

            var terrain = Terrain.Create(filename);
            EditorInstance.RefreshTerrainList();
            EditorInstance.ChangeTerrainSelectedByID(terrain.ID);
        }
    }

    public void DisplayNewTerrainPanel() {
        saveDialog.Activate("", new TerrainPanel());
    }

    public void DeleteSelectedTerrain() {
        Terrain.Remove(SelectedTerrain);
        Debugger.Log("Still exists: " + Terrain.Exists(SelectedTerrain.ID));
        ChangeTerrainSelectedByID(1);
        RefreshTerrainList();
        MapDisplayAPI.ApplyOnceEditor();
    }

	
	public void ResizeMap(){
		//GameObject startX = GameObject.Find ("MapStartXInput");
		//GameObject startY = GameObject.Find ("MapStartYInput");
		//GameObject scale = GameObject.Find ("MapScaleInput");
		GameObject width = GameObject.Find ("MapWidthInput");
		GameObject height = GameObject.Find ("MapHeightInput");

		
		//double sX = startX.GetComponent<Text> ().text;
		//double sY = startY.GetComponent<Text> ().text;
		//double s = scale.GetComponent<Text> ().text;
		int w = int.Parse( width.GetComponentInChildren<Text> ().text );
		int h = int.Parse( height.GetComponentInChildren<Text> ().text );

		
		World.Instance.Resize (w, h);
		MapDisplayAPI.ApplyOnceEditor ();
		
	}

    public void OnOpen() {

    }

    public void OnClose() {

    }
}
