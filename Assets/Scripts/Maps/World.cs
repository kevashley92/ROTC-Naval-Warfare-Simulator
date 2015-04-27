using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using MiniJSON;

[Serializable]
public class World : MapDebuggable {
    #region Constants
    public static string DEFAULT_EXTENSION { get { return ".map"; } }
    #endregion Constants

    #region World Instance Class Definition
    private byte[,] tiles;
    //private HashSet<GameObject> gameObjects;
    private Coordinate startCoordinate;
    private MarineGridCoordinate startMarineCoordinate;
    private int startX, startY;
    private ushort width;
    private ushort height;
    private float navyScale;

    public World(int width, int height)
        : this(width, height, new Coordinate(), 1.0f) {

    }

    public World(int width, int height, Coordinate startCoordinate, float navyScale) {
        this.width = 0;
        this.height = 0;

        this.startCoordinate = startCoordinate;
        this.navyScale = navyScale;

        this.tiles = new byte[width, height];
        //this.gameObjects = new HashSet<GameObject>();

        Resize(width, height);
    }

    public byte[,] Tiles {
        set { tiles = value; }
        get { return tiles; }
    }

    public MarineGridCoordinate StartMarineGridCoordinate {
        get { return startMarineCoordinate; }
        set { startMarineCoordinate = value; }
    }

    public int StartX {
        get { return startX; }
        set { startX = value; }
    }

    public int StartY {
        get { return startY; }
        set { startY = value; }
    }

    public Terrain TerrainAt(Vector2 mouseCoordinate) {
        return TerrainAt(new Vector3(mouseCoordinate.x, mouseCoordinate.y, 0));
    }

    public Terrain TerrainAt(Vector3 mouseCoordinate) {
        return Terrain.Get(TerrainIDAt(mouseCoordinate));
    }

    public byte TerrainIDAt(Vector2 mouseCoordinate) {
        return TerrainIDAt(new Vector3(mouseCoordinate.x, mouseCoordinate.y, 0));
    }

    public byte TerrainIDAt(Vector3 mouseCoordinate) {
        var position = mouseCoordinate;
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height) {
            return 0xFF;
        }
        return tiles[(int)position.x, height - (int)position.y];
    }

    public bool TerrainDirty(Vector2 mouseCoordinate) {
        return TerrainDirty(new Vector3(mouseCoordinate.x, mouseCoordinate.y, 0));
    }

    public bool TerrainDirty(Vector3 mouseCoordinate) {
        return !Terrain.Exists(TerrainIDAt(mouseCoordinate));
    }

    public Coordinate StartCoordinate {
        set {
            startCoordinate = value;
        }
        get {
            return startCoordinate;
        }
    }

    public Coordinate EndCoordinate {
        get { return World.AbsoluteToCoordinate(new Vector2(), navyScale) + startCoordinate; }
    }

    public Vector2 Size {
        get { return new Vector2(width, height); }
    }

    public int Width {
        get { return width; }
        set {
            Resize(value, height);
        }
    }

    public int Height {
        get { return height; }
        set {
            Resize(width, value);
        }
    }

    private void Resize() {
        this.width = (ushort)Mathf.Max(0, this.width);
        this.height = (ushort)Mathf.Max(0, this.height);

        int oldWidth = this.tiles.GetLength(0), oldHeight = this.tiles.GetLength(1);
        var oldTiles = this.tiles;
        this.tiles = new byte[this.width, this.height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                this.tiles[i, j] = Terrain.Get("WATER").ID;

                if (i < oldWidth && j < oldHeight) {
                    this.tiles[i, j] = oldTiles[i, j];
                }
            }
        }
    }

    public void Resize(int width, int height) {
        this.width = (ushort)width;
        this.height = (ushort)height;

        Resize();
    }

    public int Area {
        get { return ((int)Width) * ((int)Height); }
    }

    public float NavyScale {
        get { return navyScale; }
    }
    #endregion World Instance Class Definition

    #region Coordinate Conversion Methods
    public static Coordinate AbsoluteToCoordinate(Vector2 vector, float scale) {
        return new Coordinate(vector.x * scale, vector.y * scale);
    }

    public static MarineGridCoordinate AbsoluteToMGC(Vector2 vector) {
        return new MarineGridCoordinate((ushort)vector.x, (ushort)vector.y);
    }

    public static Vector2 CoordinateToAbsolute(Coordinate coord, float scale) {
        return new Vector2((float)(coord.Latitude / scale), (float)(coord.Longitude / scale));
    }

    public static Vector2 MGCToAbsolute(MarineGridCoordinate coord) {
        return new Vector2(coord.X, coord.Y);
    }
    #endregion Coordinate Conversion Methods

    #region Static Fields and Accessors
    private static World globalInstance;
    private static GameObject mapObject;
    private static string overlayName;

    private static Dictionary<string, MarineMap> marineMaps;

    private static int encoderPreference;
    private static Dictionary<int, WorldEncoder> encoders;

    public static World Instance {
        get { return globalInstance; }
        set {
            /* If the value is not the global instance, don't update. */
            if (globalInstance != value) {
                /* Intialize display API. */
                MapDisplayAPI.Init();

                /* Set the global instance. */
                globalInstance = value;

                /* Refresh the display. */
                MapDisplayAPI.ApplyOnceEditor();
            }
        }
    }

    public static bool IsMarine { get { return NetworkManager.teamcode == LoginHandler.MARINES_CODE; } } // TODO: FIX
    public static bool IsNavy { get { return NetworkManager.teamcode == LoginHandler.MARINES_CODE; } } // TODO: FIX

    /* Accessor for the preferred encoder. */
    public static int EncoderPreference { get { return encoderPreference; } set { encoderPreference = value; } }

    public static string OverlayName {
        get { return overlayName; }
        set {
            /* If the value is not the overlay name, don't update. */
            if (overlayName != value) {
                overlayName = value;

                MapDisplayAPI.LoadOverlayTexture();
            }
        }
    }
    #endregion Static Fields and Accessors

    #region Marine Maps
    public static Dictionary<string, MarineMap> MarineMaps {
        get {
            if (marineMaps == null) {
                marineMaps = new Dictionary<string, MarineMap>();

                var mmBlob = Json.Deserialize(File.ReadAllText(TEMPLATE_DIRECTORY + "marinemaps.json")) as Dictionary<string, object>;
                foreach (var key in mmBlob.Keys) {
                    marineMaps[key] = new MarineMap(key, mmBlob[key]);
                }
            }

            return marineMaps;
        }
    }

    public static List<string> AvailableMarineMaps {
        get { return new List<string>(MarineMaps.Keys); }
    }

    public static List<string> AvailableMarineMapNames {
        get {
            var list = new List<string>();

            foreach (var map in MarineMaps.Values) {
                list.Add(map.Name);
            }

            return list;
        }
    }

    public struct MarineMap {
        public const string FIELD_NAME = "name";
        public const string FIELD_OVERLAY = "overlay";

        private string file;
        private string name;
        private string overlay;

        public string File {
            get { return file; }
            set {
                if (value == null || value.Trim().Length < 1) {
                    throw new ArgumentNullException("Failed to set MarineMap.File. Value is null or zero-length.");
                }

                file = value.Trim();
            }
        }

        public string Name {
            get { return name; }
            set {
                name = value;

                if (value == null || value.Trim().Length < 1) {
                    name = file;
                }

                name = name.Trim();
            }
        }

        public string Overlay {
            get { return overlay; }
            set {
                overlay = value;

                if (overlay == null) {
                    overlay = "";
                }

                overlay = overlay.Trim();
            }
        }

        public MarineMap(string file, string name, string overlay) {
            this.file = null;
            this.name = null;
            this.overlay = null;

            File = file;
            Name = name;
            Overlay = overlay;
        }

        public MarineMap(string file, object blob) {
            this.file = null;
            this.name = null;
            this.overlay = null;

            File = file;

            var name = blob.ToString();
            var overlay = "";

            if (blob is Dictionary<string, object>) {
                var dictionary = blob as Dictionary<string, object>;

                if (dictionary.ContainsKey(FIELD_NAME)) { name = (string)dictionary[FIELD_NAME]; }
                if (dictionary.ContainsKey(FIELD_OVERLAY)) { overlay = (string)dictionary[FIELD_OVERLAY]; }
            }

            Name = name;
            Overlay = overlay;
        }
    }
    #endregion Marine Maps

    #region Initialization and Clear
    /* Static initializer. */
    static World() {
        encoders = new Dictionary<int, WorldEncoder>();

        /* Register the default world encoder. */
        RegisterEncoder(new SimpleWorldEncoder());

        /* Create a default global instance. */
        globalInstance = new World(0, 0);

        RestoreDefault();

        mapObject = GameObject.Find("MapObject").gameObject;
    }

    /* Restore default map. */
    public static void RestoreDefault() {
        Clear();

        /* Restore default terrain. */
        Terrain.Load();

        /* Create the default world. */
        Instance = new World(256, 256);
    }
	
    public static void Clear() {
        var gameObjects = GuidList.GetAllObjects();
        foreach (GameObject unit in gameObjects) {
            // Clear code
        }
    }
    #endregion Initialization and Clear

    #region World Encoders
    /* Register world encoder. */
    public static bool RegisterEncoder(WorldEncoder encoder) {
        if (encoder == null) {
            return false;
        }

        if (!encoders.ContainsKey(encoder.VersionCode)) {
            try {
                encoders.Add(encoder.VersionCode, encoder);

                return true;
            } catch (Exception e) {
                Error(e);
            }
        }

        return false;
    }

    /* Deregister world encoder by version code. */
    public static bool DeregisterEncoder(int versionCode) {
        return encoders.Remove(versionCode);
    }

    /* Deregister world encoder by reference. */
    public static bool DeregisterEncoder(WorldEncoder encoder) {
        if (encoder == null) {
            return false;
        }

        return DeregisterEncoder(encoder.VersionCode);
    }

    /* Get preferred world encoder. Calls GetEncoder(encoderPreference). */
    public static WorldEncoder GetEncoder() {
        return GetEncoder(encoderPreference);
    }

    /* Gets the encoder registered as versionCode, attempts to get any if not found. */
    public static WorldEncoder GetEncoder(int versionCode) {
        if (encoders.ContainsKey(versionCode)) {
            return encoders[versionCode];
        }

        foreach (var pair in encoders) {
            if (pair.Value != null) {
                return pair.Value;
            }
        }

        return null;
    }
    #endregion World Encoders

    #region Saving - Global
    /* Parameterless version of Save(string filename), calls Save(null). */
    public static bool Save() {
        return Save((string)null);
    }

    /* Saves the global instance to the provided filename, calls Save(string filename, World world). */
    public static bool Save(string filename) {
        var headerBlob = new Dictionary<string, object>();

        headerBlob["overlay"] = OverlayName;
        headerBlob["terrain"] = ValidateFilename(filename) + ".terrain.json";

        try {
            File.WriteAllText(ValidateFilename(filename, "json"), Json.Serialize(headerBlob));
            Terrain.Save(headerBlob["terrain"].ToString());
        } catch (Exception e) {
            Debug(e);
        }

        return SaveWorld(filename, globalInstance);
    }
    #endregion Saving - Global

    #region Saving - Instance
    /* Saves the provided world to the provided filename.
     * Calls Save(Stream stream, World world, bool closeStream = true).
     */
    public static bool SaveWorld(string filename, World world) {
        filename = ValidateFilename(filename);

        Debug("Saving World to '" + filename + "'.");
        return SaveWorld(File.OpenWrite(filename), world, true);
    }

    /* Saves the provided world to the provided stream.
     * Calls Save(Stream stream, Wolrd world, bool closeStream = false).
     */
    public static bool SaveWorld(Stream stream, World world) {
        return SaveWorld(stream, world, false);
    }

    /* Saves the provided world to the provided stream, optionally closing the stream. */
    public static bool SaveWorld(Stream stream, World world, bool closeStream) {
        try {
            var data = ConvertToBytes(world);

            Debug("Writing data to stream...");
            stream.Write(data, 0, data.Length);
            Debug("Done writing to stream.");

            if (closeStream) {
                Debug("Closing stream based on preferences.");
                stream.Close();
                Debug("Closed stream based on preferences.");
            }

            return true;
        } catch (Exception e) {
            Debug(e);
        }

        return false;
    }
    #endregion Saving - Instance

    #region Loading - Global
    /* Parameterless version of Load(string filename), calls Load(null). */
    public static bool Load() {
        return Load((string)null);
    }

    /* Loads the world from the provided filename, calls Load(Stream stream, bool closeStream = true). */
    public static bool Load(string filename) {
        var mapFilename = ValidateFilename(filename);
        var headerFilename = ValidateFilename(filename, "json");

        Debug("Loading World from '" + mapFilename + "'.");
        var result = Load(File.OpenRead(mapFilename), true);

        Debug("Trying to load header from '" + headerFilename + "'.");
        try {
            var headerBlob = Json.Deserialize(File.ReadAllText(headerFilename)) as Dictionary<string, object>;
            if (headerBlob.ContainsKey("terrain")) { Terrain.Load(headerBlob["terrain"].ToString()); }

            var overlay = (string)null;
            if (headerBlob.ContainsKey("overlay")) { overlay = (string)headerBlob["overlay"]; }
            if (overlay == null) { overlay = ""; }
            OverlayName = overlay.Trim();
        } catch (Exception e) {
            Debug(e);
        }

        return result;
    }

    /* Loads the world from the provided stream, calls Load(Stream stream, bool closeStream = false). */
    private static bool Load(Stream stream) {
        return Load(stream, false);
    }

    /* Loads the world from the provided stream, optionally closing the stream.
     * Calls LoadWorld(Stream stream, bool closeStream).
     */
    private static bool Load(Stream stream, bool closeStream) {
        Debug("Clearing the World state.");
        Clear();

        Instance = LoadWorld(stream, closeStream);

        return (Instance == null);
    }
    #endregion Loading - Global

    #region Loading - Instance
    /* Loads a world from the provided filename, returning the world.
     * Calls LoadWorld(Stream stream, bool closeStream = true).
     */
    public static World LoadWorld(string filename) {
        filename = ValidateFilename(filename);
        Debug("Reading world from '" + filename + "'.");
        return LoadWorld(File.OpenRead(filename), true);
    }

    /* Loads a world from the provided stream, return the world.
     * Calls LoadWorld(Stream stream, bool closeStream = false).
     */
    public static World LoadWorld(Stream stream) {
        return LoadWorld(stream, false);
    }

    /* Loads a world from the provided stream returning the world, optionally closing the stream. */
    public static World LoadWorld(Stream stream, bool closeStream) {
        try {
            var lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            var length = BitConverter.ToInt32(lengthBytes, 0);
            Debug("Reading world (" + WorldEncoder.DataSizeString(length) + ").");

            var data = new byte[length];
            stream.Read(data, 0, length);

            if (closeStream) {
                Debug("Closing stream based on preferences.");
                stream.Close();
                Debug("Closed stream based on preferences.");
            }

            return ConvertFromBytes(data);
        } catch (Exception e) {
            Debug(e);
        }

        return null;
    }
    #endregion Loading - Instance

    #region World-Byte Conversion
    /* Convert the provided byte array to a world instance, not ignoring length. */
    public static World ConvertFromBytes(byte[] data) {
        return ConvertFromBytes(data, false);
    }

    /* Convert the provided byte array to a world instance, ignoring length from network buffer. */
    public static World ConvertFromNetworkBuffer(byte[] data) {
        return ConvertFromBytes(data, true);
    }

    /* Convert the provided byte array to a world instance, optionally ignoring length. */
    public static World ConvertFromBytes(byte[] data, bool ignoreLength) {
        var dataInterface = new DataInterface(ref data);

        if (ignoreLength) {
            dataInterface.ReadInt();
        }

        var version = dataInterface.ReadInt();
        var encoder = GetEncoder(version);

        Debug("Decoding world.");
        return encoder.Decode(dataInterface);
    }

    /* Convert the global instance world to bytes. */
    public static byte[] ConvertToBytes() {
        return ConvertToBytes(globalInstance);
    }

    /* Convert the provided world to bytes. */
    public static byte[] ConvertToBytes(World world) {
        var encoder = GetEncoder();

        Debug("Encoding world (" + WorldEncoder.DataSizeString(encoder.CalculateSize(world)) + ").");

        var data = new byte[encoder.CalculateSize(world)];
        var dataInterface = new DataInterface(ref data);

        dataInterface.Write(data.Length);
        dataInterface.Write(encoder.VersionCode);
        encoder.Encode(world, dataInterface);

        Debug("Finished encoding the world.");

        return data;
    }
    #endregion World-Byte Conversion

    #region File Listing
    /* List all files in the templates directory with the default extension. */
    public static List<string> ListTemplateFiles() {
        return ListTemplateFiles(World.DEFAULT_EXTENSION);
    }

    /* List all files in the templates directory with the provided extension. */
    public static List<string> ListTemplateFiles(string extension) {
        extension = ValidateExtension(extension);

        return ListFilesIn(TEMPLATE_DIRECTORY, extension);
    }

    /* List all files in the provided directory with the default extension. */
    public static List<string> ListFilesIn(string directory) {
        return ListFilesIn(directory, World.DEFAULT_EXTENSION);
    }

    /* List all files in the provided directory with the provided extension. */
    public static List<string> ListFilesIn(string directory, string extension) {
        directory = ValidateDirectory(directory);
        extension = ValidateExtension(extension);

        DirectoryInfo info = new DirectoryInfo(Path.GetFullPath(directory));
        if (!info.Exists) {
            return null;
        }

        List<string> files = new List<string>();
        foreach (var file in info.GetFiles()) {
            if (file.Extension.Equals(extension)) {
                files.Add(file.Name.Remove(file.Name.IndexOf(extension)));
            }
        }

        return files;
    }
    #endregion File Listing

    #region Validation
    /* Calls ValidateFilename(string filename, string extension) with the parameters 'filename' and 'null'. */
    public static string ValidateFilename(string filename) {
        return ValidateFilename(filename, DEFAULT_EXTENSION);
    }


    /* Validates the extension, returning a validated extension, starting with a period.
     * If extension is null, zero-length or only a period this will return ValidateExtension(World.DEFAULT_EXTENSION).
     */
    public static string ValidateExtension(string extension) {
        /* If the extension is null or zero-length, try again with the default extension. */
        if (extension == null || extension.Trim().Length < 1 || extension.Trim().Equals(".")) {
            return ValidateExtension(World.DEFAULT_EXTENSION);
        }

        /* Strip whitespace from the extension. */
        extension = extension.Trim();

        /* Ensure the extension starts with a period. */
        if (!extension.StartsWith(".")) {
            extension = "." + extension;
        }

        return extension;
    }

    /* Validates the directory, returning a validated directory, ending in a path separator but not creating the directory.
     * If directory is null or zero-length this will return ValidateDirectory(World.DEFAULT_DIRECTORY).
     */
    public static string ValidateDirectory(string directory) {
        /* If the directory is null or zero-length, try again with the default directory. */
        if (directory == null || directory.Trim().Length < 1) {
            return ValidateDirectory(World.DEFAULT_DIRECTORY);
        }

        /* Strip whitespace from the directory. */
        directory = directory.Trim();

        /* Ensure the directory ends with a platform-specific separator. */
        if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString())) {
            directory += Path.DirectorySeparatorChar;
        }

        return directory;
    }
    #endregion Validation

    #region Units
    /* Clear unit game objects. 
    public static void ClearUnits() {
        if (Instance == null) {
            return;
        }

        var gameObjects = Instance.gameObjects;

        foreach (GameObject unit in gameObjects) {
            GameObject.DestroyImmediate(unit);
        }

        gameObjects.Clear();
    }
     */

    /* Adds unit at its position. */
    public static void AddUnitToWorld(GameObject unit) {
        var position = unit.transform.position - mapObject.transform.position;

        unit.transform.SetParent(mapObject.transform, false);

        unit.transform.localPosition = new Vector3(position.x, position.y, position.z);

    }

    public static void SetTeamColor(GameObject unit) {
        List<Team> teams = Team.GetTeams();
        if(null == teams)
            return;
        var teamIndex = unit.GetComponent<IdentityController>().TeamNumber;
        unit.GetComponent<SpriteRenderer>().color = Team.GetTeams()[teamIndex].GetTeamColor();
    }

    public static void UpdateObjectiveSprite(GameObject unit)
    {
        unit.transform.FindChild("Objective").gameObject.SetActive(unit.GetComponent<IdentityController>().GetIsObjective());

    }
    #endregion Units
}
