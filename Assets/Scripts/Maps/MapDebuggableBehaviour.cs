using UnityEngine;

public abstract class MapDebuggableBehaviour : MonoBehaviour {
    public static bool ENABLE_DEBUG {
        get { return MapDebuggable.ENABLE_DEBUG; }
        set { MapDebuggable.ENABLE_DEBUG = value; }
    }

    public static void Debug(object obj) {
        MapDebuggable.Debug(obj);
    }

    public static void Debug(string message) {
        MapDebuggable.Debug(message);
    }

    public static void Debug(string message, bool action) {
        MapDebuggable.Debug(message, action);
    }

    public static void Error(object obj) {
        MapDebuggable.Error(obj);
    }

    public static void Error(string message) {
        MapDebuggable.Error(message);
    }

    public static void Error(string message, bool action) {
        MapDebuggable.Error(message, action);
    }
}
