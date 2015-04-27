using UnityEngine;
using System.Collections.Generic;
using MiniJSON;
using System.IO;
using System;

public class Terrain : MapDebuggable {
    #region Constants
    public static string DEFAULT_EXTENSION { get { return ".json"; } }
    public static string DEFAULT_FILENAME { get { return "terrain"; } }
    public static string TEMPLATE_FILE { get { return TEMPLATE_DIRECTORY + DEFAULT_FILENAME + DEFAULT_EXTENSION; } }
    #endregion Constants

    private static Dictionary<byte, Terrain> mapIdTerrain = new Dictionary<byte, Terrain>();
    private static Dictionary<string, byte> mapNameId = new Dictionary<string, byte>();

    private const string FIELD_NAME = "name";
    private const string FIELD_COLOR_R = "colorR";
    private const string FIELD_COLOR_G = "colorG";
    private const string FIELD_COLOR_B = "colorB";
    private const string FIELD_COLOR_A = "colorA";
    private const string FIELD_MODIFIER_VISUAL = "modifierVisual";
    private const string FIELD_MODIFIER_MOVEMENT = "modifierMovement";
    private const string FIELD_MODIFIER_BLOCKING = "modifierBlocking";

    #region Instance Class Definition
    private readonly byte id;
    private readonly string name;
    private bool locked;
    private Color color;
    private float modifierVisual;
    private float modifierMovement;
    private int modifierBlocking;

    public byte ID { get { return id; } }
    public string Name { get { return name; } }
    public bool Locked { get { return locked; } }
    public Color Color { get { return color; } set { color = value; } }
    public float ModifierMovement { get { return modifierMovement; } set { modifierMovement = value; } }
    public float ModifierVisual { get { return modifierVisual; } set { modifierVisual = value; } }
    public int ModifierBlocking { get { return modifierBlocking; } set { modifierBlocking = value; } }

    private Terrain(byte id, string name) {
        this.id = id;
        this.name = ((name == null || name.Trim().Length < 1) ? "TERRAIN_" + id.ToString() : name.ToUpper());

        this.modifierMovement = 1.0f;
        this.modifierVisual = 1.0f;
        this.modifierBlocking = 0;
        this.color = new Color(0, 0, 0);
    }

    private Terrain Lock(bool locked) {
        this.locked = locked;

        return this;
    }

    private Terrain Lock() {
        return Lock(true);
    }

    public Terrain SetModifierMovement(float modifier) {
        this.modifierMovement = modifier;

        return this;
    }

    public Terrain SetModifierVisual(float modifier) {
        this.modifierVisual = modifier;

        return this;
    }

    public Terrain SetModifierBlocking(int modifier) {
        this.modifierBlocking = modifier;

        return this;
    }

    public Terrain SetColor(Color color) {
        this.color = color;

        return this;
    }

    public override string ToString() {
        return ("[Terrain (" + id + ",'" + name + "') { "
            + "modifierMovement: " + modifierMovement
            + ", modifierVisual: " + modifierVisual
            + ", color: " + color
            + ", modifierBlocking: " + modifierBlocking
            + " } ]");
    }

    public Dictionary<string, object> ToBlob() {
        var data = new Dictionary<string, object>();

        data[FIELD_NAME] = name;
        data[FIELD_MODIFIER_MOVEMENT] = modifierMovement;
        data[FIELD_MODIFIER_BLOCKING] = modifierBlocking;
        data[FIELD_MODIFIER_VISUAL] = modifierVisual;
        data[FIELD_COLOR_R] = color.r;
        data[FIELD_COLOR_G] = color.g;
        data[FIELD_COLOR_B] = color.b;
        if (color.a < 1) { data[FIELD_COLOR_A] = Math.Max(0, color.a); }

        return data;
    }

    public Terrain FromBlob(Dictionary<string, object> data) {
        color.a = (data.ContainsKey(FIELD_COLOR_A) ? float.Parse(data[FIELD_COLOR_A].ToString()) : 1);

        if (data.ContainsKey(FIELD_COLOR_R)) { color.r = float.Parse(data[FIELD_COLOR_R].ToString()); }
        if (data.ContainsKey(FIELD_COLOR_G)) { color.g = float.Parse(data[FIELD_COLOR_G].ToString()); }
        if (data.ContainsKey(FIELD_COLOR_B)) { color.b = float.Parse(data[FIELD_COLOR_B].ToString()); }
        if (data.ContainsKey(FIELD_MODIFIER_VISUAL)) { modifierVisual = float.Parse(data[FIELD_MODIFIER_VISUAL].ToString()); }
        if (data.ContainsKey(FIELD_MODIFIER_MOVEMENT)) { modifierMovement = float.Parse(data[FIELD_MODIFIER_MOVEMENT].ToString()); }
        if (data.ContainsKey(FIELD_MODIFIER_BLOCKING)) { modifierBlocking = int.Parse(data[FIELD_MODIFIER_BLOCKING].ToString()); }

        return this;
    }
    #endregion Instance Class Definition

    static Terrain() {
        RestoreDefault();
    }

    private static void Clear() {
        for (var i = 0; i < 256; i++) {
            mapIdTerrain[(byte)i] = null;
        }

        mapNameId.Clear();
    }

    public static void RestoreDefault() {
        Clear();

        Create(0x00, "WATER").SetColor(Color.cyan).Lock();
        Create(0x01, "LAND").SetColor(Color.green).Lock();
        Create(0xFF, "UNDEFINED").SetColor(new Color(1, 0, 1)).SetModifierMovement(999999999).SetModifierVisual(999999999).Lock();
    }

    public static List<string> NamesList { get { return new List<string>(mapNameId.Keys); } }
    public static string[] Names { get { return NamesList.ToArray(); } }

    public static List<byte> IdsList { get { return new List<byte>(mapNameId.Values); } }
    public static byte[] Ids { get { return IdsList.ToArray(); } }

    #region Get/Remove/Create/Exists
    public static int GetNextID() {
        for (var i = 0; i < 256; i++) {
            if (!Exists(i)) {
                return i;
            }
        }

        return 256;
    }

    public static bool Exists(byte id) {
        return mapIdTerrain.ContainsKey(id) && mapIdTerrain[id] != null;
    }

    public static bool Exists(int id) {
        return Exists((byte)id);
    }

    public static bool Exists(string name) {
        if (mapNameId.ContainsKey(name)) {
            return mapIdTerrain[mapNameId[name]] != null;
        }

        return false;
    }

    public static byte GetID(string name) {
        if (mapNameId.ContainsKey(name)) {
            return mapNameId[name];
        }
        else{
            return 1;
        }

        return 1;
    }

    public static Terrain Get(byte id) {
        if (Exists(id)) {
            return mapIdTerrain[id];
        }

        return mapIdTerrain[1];
    }

    public static Terrain Get(int id) {
        return Get((byte)id);
    }

    public static Terrain Get(string name) {
        return Get(GetID(name));
    }

    public static Terrain Create(string name) {
        return Create(GetNextID(), name);
    }

    private static Terrain Create(int id, string name) {
        var terrain = new Terrain((byte)id, name);

        if (terrain.id < 0 || 255 < terrain.id || Exists(terrain.id)) {
            return null;
        }

        mapIdTerrain[terrain.id] = terrain;
        mapNameId[terrain.name] = terrain.id;

        return terrain;
    }

    public static Terrain GetOrCreate(string name) {
        if (Exists(name)) {
            return Get(name);
        }

        return Create(name);
    }

    public static Terrain Remove(byte id) {
        return Remove(Get(id));
    }

    public static Terrain Remove(int id) {
        return Remove(Get(id));
    }

    public static Terrain Remove(string name) {
        return Remove(Get(name));
    }

    public static Terrain Remove(Terrain terrain) {
        Debugger.Log("Terrain: " + terrain);
        if (terrain == null || terrain.locked) {
            Debugger.Log("Null? " + (terrain == null));
            Debugger.Log("Locked? " + (terrain != null && terrain.locked));
            return null;
        }

        Debugger.Log("Attempting removal");
        Debugger.Log("Remove 1: " + mapIdTerrain.Remove(terrain.id));
        Debugger.Log("Remove 2: " + mapNameId.Remove(terrain.name));

        return terrain;
    }
    #endregion Get/Remove/Create/Exists

    public static bool Load() {
        Debug("Loading from template file.");
        return Load(TEMPLATE_FILE);
    }

    public static bool Load(string filename) {
        try {
            var blob = Json.Deserialize(File.ReadAllText(ValidateFilename(filename))) as Dictionary<string, object>;
            Debug("Loaded terrain from '" + ValidateFilename(filename) + "'.");
            
            return LoadFromBlob(blob);
        } catch (Exception e) {
            Debug(e);
        }

        return false;
    }

    public static bool LoadFromBlob(Dictionary<string, object> blob) {
        RestoreDefault();

        Debug("Loading from blob.");

        for (var i = 0; i < 256; i++) {
            if (!((Dictionary<string, object>)blob).ContainsKey(i.ToString())) {
                continue;
            }

            var type = (Dictionary<string, object>)((Dictionary<string, object>)blob)[i.ToString()];
            var terrain = GetOrCreate(type[FIELD_NAME].ToString());
            terrain.FromBlob(type);

            Debug(terrain);
        }

        return true;
    }

    public static bool Save() {
        Debug("Saving to template file.");
        return Save(TEMPLATE_FILE);
    }

    public static bool Save(string filename) {
        try {
            File.WriteAllText(ValidateFilename(filename), Json.Serialize(AsBlob()));
            Debug("Saved terrain to '" + ValidateFilename(filename) + "'.");

            return true;
        } catch (Exception e) {
            Debug(e);
        }

        return false;
    }

    public static bool SaveToBlob(Dictionary<string, object> blob) {
        Debug("Saving to blob.");
        AsBlob(blob);
        return false;
    }

    public static Dictionary<string, object> AsBlob() {
        return AsBlob(new Dictionary<string, object>());
    }

    public static Dictionary<string, object> AsBlob(Dictionary<string, object> blob) {
        if (blob == null) {
            return AsBlob();
        }

        foreach (var terrain in mapIdTerrain.Values) {
            if (terrain == null) {
                continue;
            }

            Debug(terrain);

            blob[terrain.ID.ToString()] = terrain.ToBlob();
        }

        return blob;
    }

    public static string ValidateFilename(string filename) {
        return ValidateFilename(filename, DEFAULT_EXTENSION);
    }

    public static void DebugDump() {
        Debug("Printing Terrain");
        for (int i = 0; i < 256; i++) {
            var terrain = Get(i);
            if (terrain != null) {
                Debug(terrain);
            }
        }
    }
    /*
    public static bool Save(Dictionary<string, object> blob) {
        Debug("Saving to memory blob.");
        return SerializeTypes(blob).Count > 0;
    }

    public static bool Save(string filename) {
        return SaveBlob(filename, SerializeTypes());
    }

    public static bool SaveBlob(string filename, Dictionary<string, object> blob) {
        try {
            File.WriteAllText(BuildFilename(filename), Json.Serialize(blob));

            System.Diagnostics.Debug.WriteLine("Saved terrain to: " + BuildFilename(filename));

            return true;
        } catch (Exception e) {
            Debug(e);
        }

        return false;
    }

    public static bool Load() {
        Debug("Loading terrain from global file.");
        return Load(Terrain.FILE_TEMPLATE);
    }

    public static bool Load(string filename) {
        var blob = LoadBlob(filename);

        if (blob != null) {
            DeserializeTypes(blob);

            Debug("Loaded terrain from: " + BuildFilename(filename));

            return true;
        }

        Debug("Failed to load terrain from blob (null)");

        return false;
    }

    public static void Load(Dictionary<string, object> blob) {
        DeserializeTypes(blob);
    }

    public static Dictionary<string, object> LoadBlob() {
        Debug("Loading terrain blob from global file.");
        return LoadBlob(Terrain.FILE_TEMPLATE);
    }

    public static Dictionary<string, object> LoadBlob(string filename) {
        Debug("Loading terrain blob from: " + BuildFilename(filename));
        try {
            Debug("Loading terrain blob from: " + BuildFilename(filename));
            return Json.Deserialize(File.ReadAllText(BuildFilename(filename))) as Dictionary<string, object>;
        } catch {
            Debug("Failed to load blob from terrain file ('" + filename + "').");
        }

        return null;
    }

    private static void DeserializeTypes(Dictionary<string, object> terrainDic) {
        RestoreDefault();

        for (var i = 0; i < 256; i++)
        {
            if (!((Dictionary<string, object>)terrainDic).ContainsKey(i.ToString()))
            {
                continue;
            }

            var typeDic = (Dictionary<string, object>)((Dictionary<string, object>)terrainDic)[i.ToString()];

            var terrain = GetOrCreate(typeDic[FIELD_NAME].ToString());

            terrain.FromBlob(typeDic);

            Debug(terrain);
        }
    }*/
}
