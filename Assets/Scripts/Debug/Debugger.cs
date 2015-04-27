using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class Debugger {
    public static bool Enabled { get; set; }

    static Debugger() {
        Enabled = true;
        isEnabled = new Dictionary<Type, bool>();
    }

    #region Private
    private static Dictionary<Type, bool> isEnabled;

    private static StackFrame GetCallerInfo() {
        return new StackFrame(2);
    }

    private static void Log(StackFrame stack, object obj) {
        if (obj is Exception) {
            Log(stack, (Exception)obj);
        } else {
            Log(stack, (string)obj);
        }
    }

    private static void Log(StackFrame stack, Exception exception) {
        Log(stack, "Exception Message: {0}", exception.Message);
        Log(stack, "Exception Stack: {0}", exception.StackTrace);
    }

    private static void Log(StackFrame stack, string message) {
        var method = stack.GetMethod();
        if (!IsEnabled(method.DeclaringType)) {
            return;
        }

        var methodTag = string.Format("[{0}.{1}()]", method.DeclaringType.Name, method.Name);
        var locationTag = "";
        if (stack.GetFileName() != null) {
            locationTag = string.Format("[{0} ({1}:{2})]", stack.GetFileName(), stack.GetFileLineNumber(), stack.GetFileColumnNumber());
        }

        UnityEngine.Debug.Log(string.Format("[DEBUG]{0}{1}: {2}", locationTag, methodTag, message));
    }

    private static void Log(StackFrame stack, string format, params object[] args) {
        Log(stack, string.Format(format, args));
    }

    private static void Error(StackFrame stack, object obj) {
        if (obj is Exception) {
            Error(stack, (Exception)obj);
        } else {
            Error(stack, (string)obj);
        }
    }

    private static void Error(StackFrame stack, Exception exception) {
        Error(stack, "Exception Message: {0}", exception.Message);
        Error(stack, "Exception Stack: {0}", exception.StackTrace);
    }

    private static void Error(StackFrame stack, string message) {
        var method = stack.GetMethod();
        var methodTag = string.Format("[{0}.{1}()]", method.DeclaringType.Name, method.Name);
        var locationTag = "";
        if (stack.GetFileName() != null) {
            locationTag = string.Format("[{0} ({1}:{2})]", stack.GetFileName(), stack.GetFileLineNumber(), stack.GetFileColumnNumber());
        }

        UnityEngine.Debug.LogError(string.Format("[ERROR]{0}{1}: {2}", locationTag, methodTag, message));
    }

    private static void Error(StackFrame stack, string format, params object[] args) {
        Error(stack, string.Format(format, args));
    }

    private static void Warning(StackFrame stack, object obj) {
        if (obj is Exception) {
            Warning(stack, (Exception)obj);
        } else {
            Warning(stack, (string)obj);
        }
    }

    private static void Warning(StackFrame stack, Exception exception) {
        Warning(stack, "Exception Message: {0}", exception.Message);
        Warning(stack, "Exception Stack: {0}", exception.StackTrace);
    }

    private static void Warning(StackFrame stack, string message) {
        var method = stack.GetMethod();
        var methodTag = string.Format("[{0}.{1}()]", method.DeclaringType.Name, method.Name);
        var locationTag = "";
        if (stack.GetFileName() != null) {
            locationTag = string.Format("[{0} ({1}:{2})]", stack.GetFileName(), stack.GetFileLineNumber(), stack.GetFileColumnNumber());
        }

        UnityEngine.Debug.LogWarning(string.Format("[WARN]{0}{1}: {2}", locationTag, methodTag, message));
    }

    private static void Warning(StackFrame stack, string format, params object[] args) {
        Warning(stack, string.Format(format, args));
    }
    #endregion Private

    #region Logging
    public static void Log(object obj) {
        Log(GetCallerInfo(), obj);
    }

    public static void Log(Exception exception) {
        Log(GetCallerInfo(), exception);
    }

    public static void Log(string message) {
        Log(GetCallerInfo(), message);
    }

    public static void Log(string format, params object[] args) {
        Log(GetCallerInfo(), format, args);
    }

    public static void Error(object obj) {
        Error(GetCallerInfo(), obj);
    }

    public static void Error(Exception exception) {
        Error(GetCallerInfo(), exception);
    }

    public static void Error(string message) {
        Error(GetCallerInfo(), message);
    }

    public static void Error(string format, params object[] args) {
        Error(GetCallerInfo(), format, args);
    }

    public static void Warning(object obj) {
        Warning(GetCallerInfo(), obj);
    }

    public static void Warning(Exception exception) {
        Warning(GetCallerInfo(), exception);
    }

    public static void Warning(string message) {
        Warning(GetCallerInfo(), message);
    }

    public static void Warning(string format, params object[] args) {
        Warning(GetCallerInfo(), format, args);
    }
    #endregion Logging

    public static bool IsEnabled(Type type) {
        return (Enabled && (!isEnabled.ContainsKey(type) || isEnabled[type])) || (isEnabled.ContainsKey(type) && isEnabled[type]);
    }

    public static void Disable(Type type) {
        Enable(type, false);
    }

    public static void Enable(Type type) {
        Enable(type, true);
    }

    public static void Enable(Type type, bool enabled) {
        isEnabled[type] = enabled;
    }
}
