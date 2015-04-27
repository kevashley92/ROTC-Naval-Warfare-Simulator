using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditor : MapInterfacePanel {
    public class TerrainCache {
        public string name;
        public int id;
        public float colorR;
        public float colorG;
        public float colorB;
        public float colorA;
        public float modMovement;
        public float modVisual;
        public int modBlocking;
        public bool enableAlpha;

        public TerrainCache() {
            id = -1;
        }

        public static TerrainCache Create(Terrain terrain) {
            return new TerrainCache().Reset(terrain);
        }

        public TerrainCache Reset(Terrain terrain) {
            name = terrain.Name;
            id = terrain.ID;

            colorR = terrain.Color.r;
            colorG = terrain.Color.g;
            colorB = terrain.Color.b;
            colorA = terrain.Color.a;

            modMovement = terrain.ModifierMovement;
            modVisual = terrain.ModifierVisual;
            modBlocking = terrain.ModifierBlocking;

            enableAlpha = terrain.Color.a != 1;

            return this;
        }

        public TerrainCache Reset() {
            if (256 > id && id > -1) {
                return Reset(Terrain.Get(id));
            }

            return null;
        }

        public bool Commit() {
            return Commit(Terrain.Get(id));
        }

        public bool Commit(Terrain terrain) {
            var dirtyTexture = (terrain.Color.a != (enableAlpha ? colorA : 1));
            dirtyTexture |= (terrain.Color.r != colorR);
            dirtyTexture |= (terrain.Color.g != colorG);
            dirtyTexture |= (terrain.Color.b != colorB);

            terrain.SetColor(new Color(colorR, colorG, colorB, enableAlpha ? colorA : 1));
            terrain.SetModifierMovement(modMovement);
            terrain.SetModifierVisual(modVisual);
            terrain.SetModifierBlocking(modBlocking);

            return dirtyTexture;
        }

        public override string ToString() {
            return "[TerrainCache { id: " + id + ", name: " + name + ", color: (" + colorR + ", " + colorG + ", " + colorB + ", " + colorA + "), movement: " + modMovement + ", visual: " + modVisual + ", blocking: " + modBlocking + " ]";
        }
    }

    private static TerrainEditor _this = null;

    public InputField ifID = null;
    public InputField ifName = null;
    public InputField ifColorR = null;
    public InputField ifColorG = null;
    public InputField ifColorB = null;
    public InputField ifColorA = null;
    public InputField ifModMovement = null;
    public InputField ifModVisual = null;
    public Toggle tBlocking = null;
    public Toggle tEnabledAlpha = null;

    private static TerrainCache SelectedTerrainCache = TerrainCache.Create(Terrain.Get(1));
    private string[] terrainNames = null;
    private byte[] terrainIds = null;

    public static void ChangeSelected(int id) {
        Debug("Changing selected: " + id);
        SelectedTerrainCache.Reset(Terrain.Get(id));
    }

    private int GetIndex(int id) {
        for (var i = 0; i < terrainIds.Length; i++) {
            if (terrainIds[i] == id) {
                return i + 1;
            }
        }

        return 1;
    }

    void Start() {
        _this = this;

        ifID.enabled = false;
        ifName.enabled = false;

        ifID.text = "TEST";
        ifName.text = "TEST";

        ifColorR.text = "TEST";
        ifColorG.text = "TEST";
        ifColorB.text = "TEST";
        ifColorA.text = "TEST";

        ifModMovement.text = "TEST";
        ifModVisual.text = "TEST";

        UpdateDisplay();

        ifColorR.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifColorR_OnChanged));
        ifColorG.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifColorG_OnChanged));
        ifColorB.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifColorB_OnChanged));
        ifColorA.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifColorA_OnChanged));

        ifModMovement.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifModMovement_OnChanged));
        ifModVisual.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ifModVisual_OnChanged));

        tEnabledAlpha.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(tEnabledAlpha_OnChanged));
        tBlocking.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(tBlocking_OnChanged));
    }

    void OnEnable() {
        UpdateDisplay();
    }

    private void ifColorR_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.colorR);
    }

    private void ifColorG_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.colorG);
    }

    private void ifColorB_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.colorB);
    }

    private void ifColorA_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.colorA);
    }

    private void ifModMovement_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.modMovement);
    }

    private void ifModVisual_OnChanged(string value) {
        float.TryParse(value, out SelectedTerrainCache.modVisual);
    }

    private void tEnabledAlpha_OnChanged(bool value) {
        SelectedTerrainCache.enableAlpha = value;
    }

    private void tBlocking_OnChanged(bool value) {
        SelectedTerrainCache.modBlocking = (value ? 1 : 0);
    }

    private void UpdateDisplay() {
        var cache = SelectedTerrainCache;
        cache.Reset();

        ifName.text = cache.name.ToString();
        ifID.text = cache.id.ToString();
        ifColorR.text = cache.colorR.ToString();
        ifColorG.text = cache.colorG.ToString();
        ifColorB.text = cache.colorB.ToString();
        ifColorA.text = cache.colorA.ToString();

        ifModMovement.text = cache.modMovement.ToString();
        ifModVisual.text = cache.modVisual.ToString();

        tEnabledAlpha.isOn = cache.enableAlpha;
        tBlocking.isOn = cache.modBlocking > 0;
    }

    private void UpdateCache() {
        ifColorR_OnChanged(ifColorR.text);
        ifColorG_OnChanged(ifColorG.text);
        ifColorB_OnChanged(ifColorB.text);
        ifColorA_OnChanged(ifColorA.text);

        ifModMovement_OnChanged(ifModMovement.text);
        ifModVisual_OnChanged(ifModVisual.text);

        tBlocking_OnChanged(tBlocking.isOn);
        tEnabledAlpha_OnChanged(tEnabledAlpha.isOn);
    }

    public void Commit() {
        Debugger.Log("Committing Color: {0}", new Color(SelectedTerrainCache.colorR, SelectedTerrainCache.colorG, SelectedTerrainCache.colorB, SelectedTerrainCache.enableAlpha ? SelectedTerrainCache.colorA : 1));

        if (SelectedTerrainCache.Commit()) {
            Debugger.Log("Color was dirty.");
            MapDisplayAPI.ApplyOnceEditor();
        }

        Debugger.Log("Committed Color: {0}", Terrain.Get(SelectedTerrainCache.id).Color);
    }

    public void Reset() {
        SelectedTerrainCache.Reset();
    }

    public void OnSave() {
        Commit();
    }

    public void OnReset() {
        UpdateDisplay();
    }

    public void OnCancel() {
        Reset();
    }
}