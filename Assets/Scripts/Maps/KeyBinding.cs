using System.Collections.Generic;
using UnityEngine;

public class KeyBinding : MapDebuggable {
    #region Fields/Properties/Accessors
    private List<object> keys;
    public List<object> Keys { get { return keys; } }

    public bool Pressed { get { return CheckAny(keys, Input.GetKeyDown, Input.GetKeyDown); } }
    public bool Held { get { return CheckAny(keys, Input.GetKey, Input.GetKey); } }
    public bool Released { get { return CheckAny(keys, Input.GetKeyUp, Input.GetKeyUp); } }
    #endregion

    #region Constructors
    public KeyBinding() {
        this.keys = new List<object>();
    }

    public KeyBinding(params object[] keys)
        : this() {
        this.keys.AddRange(keys);
    }
    #endregion Constructors

    #region Utility Methods
    public bool AddKey(object key) {
        if (key != null && !keys.Contains(key)) {
            keys.Add(key);

            return true;
        }

        return false;
    }

    public bool RemoveKey(object key) {
        return keys.Remove(key);
    }

    public void SetKey(object key) {
        keys.Clear();

        if (key != null) {
            AddKey(key);
        }
    }

    public List<object> GetKeys() {
        return keys;
    }
    #endregion Utility Methods

    #region Static Code
    public static bool Check(object param, KeyStringMethod byString, KeyCodeMethod byCode) {
        return (param is string && byString((string)param)) || (param is KeyCode && byCode((KeyCode)param)) || (param is KeyCombo && ((KeyCombo)param).Check(byString, byCode));
    }

    public static bool CheckAny(List<object> list, KeyStringMethod byString, KeyCodeMethod byCode) {
        foreach (var param in list) {
            if (Check(param, byString, byCode)) {
                return true;
            }
        }

        return false;
    }

    public static bool CheckAll(List<object> list, KeyStringMethod byString, KeyCodeMethod byCode) {
        foreach (var param in list) {
            if (!Check(param, byString, byCode)) {
                return false;
            }
        }

        return true;
    }
    #endregion Static Code
}

#region Delegates
public delegate bool KeyStringMethod(string key);
public delegate bool KeyCodeMethod(KeyCode key);
#endregion Delegates

#region Key Combo
public struct KeyCombo {
    private List<object> keys;

    public object Primary { get { return (keys != null && keys.Count > 0 ? keys[0] : null); } set { keys[0] = value; } }
    public List<object> Keys { get { return keys; } }

    public KeyCombo(params object[] keys) {
        this.keys = new List<object>();

        foreach (var k in keys) {
            this.keys.Add(k);
        }
    }

    public bool Check(KeyStringMethod byString, KeyCodeMethod byCode) {
        var lim = (keys.Count > 1 ? keys.Count - 1 : keys.Count);
        if (!KeyBinding.CheckAll(keys.GetRange(0, lim), Input.GetKey, Input.GetKey)) {
            return false;
        }

        if (lim < keys.Count) {
            return KeyBinding.Check(keys[keys.Count - 1], byString, byCode);
        }

        return false;
    }
}
#endregion Key Combo
