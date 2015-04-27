using System;
using System.IO;
using UnityEngine;
[Serializable]
public abstract class MapDebuggable {
    #region Debug
    public static bool ENABLE_DEBUG = false;

    public static void Debug(object obj) {
        if (obj != null) {
            if (obj is Exception) {
                Debug(obj as Exception);
            } else {
                Debug(obj.ToString());
            }
        } else {
            Debug("null");
        }
    }

    public static void Debug(Exception e) {
        if (e == null) {
            Debug((object)null);
        }

        Debug(e.Message);
        UnityEngine.Debug.LogException(e);
    }

    public static void Debug(string message) {
        Debug(message, true);
    }

    public static void Debug(string message, bool action) {
        if (ENABLE_DEBUG) {
            UnityEngine.Debug.Log("[DEBUG-MAPS]" + (action ? "" : "[NOACTION]") + " " + message);
        }
    }
    #endregion Debug

    #region Error
    public static void Error(object obj) {
        if (obj != null) {
            if (obj is Exception) {
                Error(obj as Exception);
            } else {
                Error(obj.ToString());
            }
        } else {
            Error("null");
        }
    }

    public static void Error(Exception e) {
        if (e == null) {
            Error((object)null);
        }

        Error(e.Message);
        UnityEngine.Debug.LogException(e);
    }

    public static void Error(string message) {
        Error(message, true);
    }

    public static void Error(string message, bool action) {
        UnityEngine.Debug.LogError("[ERROR-MAPS]" + (action ? "" : "[NOACTION]") + " " + message);
    }
    #endregion Error

    #region Constants
    public static string TEMPLATE_DIRECTORY { get { return "Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar; } }
    public static string OVERLAYS_DIRECTORY { get { return "Maps" + Path.AltDirectorySeparatorChar + "Overlays" + Path.AltDirectorySeparatorChar; } }
    public static string DEFAULT_DIRECTORY { get { return TEMPLATE_DIRECTORY + "saves" + Path.DirectorySeparatorChar; } }
    public static string DEFAULT_FILENAME { get { return "newworld"; } }
    public static string DEFAULT_EXTENSION { get { return ""; } }
    #endregion Constants

    #region Validation
    /* Calls ValidateFilename(string filename, string extension, string directory). */
    public static string ValidateFilename(string filename, string extension) {
        /* If the filename is null or zero-length, try again with the default filename. */
        if (filename == null || filename.Trim().Length < 1) {
            return ValidateFilename(DEFAULT_FILENAME, extension);
        }

        /* Strip whitespace from the filename. */
        filename = filename.Trim();

        /* Pull the directory from the file name. */
        var directory = Path.GetDirectoryName(filename);

        /* Validate the filename with the directory. */
        return ValidateFilename(filename, extension, directory);
    }

    /* Validates the filename, returning a validated filename.
     * If filename is null or zero-length this will return ValidateFilename(World.DEFAULT_FILENAME).
     * If the extension is null or zero-length this will return ValidateFilename(filename, World.DEFAULT_EXTENSION).
     * If the filename does not end in the parameter 'extension', 'extension' is appended.
     * If the directory is null or zero-length this will assume it was supposed to be saved in World.DEFAULT_DIRECTORY.
     */
    public static string ValidateFilename(string filename, string extension, string directory) {
        /* If the filename is null or zero-length, try again with the default filename. */
        if (filename == null || filename.Trim().Length < 1) {
            return ValidateFilename(DEFAULT_FILENAME, extension);
        }

        /* Strip whitespace from the filename. */
        filename = filename.Trim();

        /* Validate the extension. */
        extension = ValidateExtension(extension);

        /* Validate the directory. */
        directory = ValidateDirectory(directory);

        /* Ensure the directory exists. */
        Directory.CreateDirectory(directory);

        /* Ensure the filename ends with the extension. */
        if (!filename.EndsWith(extension)) {
            filename += extension;
        }

        /* Split everything into their respective path-parts to assemble the validated filename. */
        extension = Path.GetExtension(filename);
        filename = Path.GetFileNameWithoutExtension(filename);

        /* Ensure extension is not null by revalidating. */
        extension = ValidateExtension(extension);

        return directory + filename + extension;
    }

    /* Calls ValidateExtension(string extension, string defaultExtension). */
    public static string ValidateExtension(string extension) {
        return ValidateExtension(extension, DEFAULT_EXTENSION);
    }

    /* Validates the extension, returning a validated extension, starting with a period.
     * If extension is null, zero-length or only a period this will return ValidateExtension(World.DEFAULT_EXTENSION).
     */
    public static string ValidateExtension(string extension, string defaultExtension) {
        /* Ensure the default extension is not null. */
        if (defaultExtension == null) {
            defaultExtension = "";
        }

        /* Strip whitespace from the default extension. */
        defaultExtension = defaultExtension.Trim();

        /* If the extension is null or zero-length, use the default extension */
        if (extension == null || extension.Trim().Length < 1 || extension.Trim().Equals(".")) {
            extension = defaultExtension;
        }

        /* Strip whitespace from the extension. */
        extension = extension.Trim();

        /* Ensure the extension starts with a period unless it is zero-length. */
        if (extension.Length > 0 && !extension.StartsWith(".")) {
            extension = "." + extension;
        }

        return extension;
    }

    /* Calls ValidateDirectory(directory, DEFAULT_DIRECTORY). */
    public static string ValidateDirectory(string directory) {
        return ValidateDirectory(directory, DEFAULT_DIRECTORY);
    }

    /* Validates the directory, returning a validated directory, ending in a path separator but not creating the directory.
     * If directory is null or zero-length this will return ValidateDirectory(World.DEFAULT_DIRECTORY).
     */
    public static string ValidateDirectory(string directory, string defaultDirectory) {
        /* If the defaultDirectory is null or zero-length, try again with DEFAULT_DIRECTORY. */
        if (defaultDirectory == null || defaultDirectory.Trim().Length < 1) {
            return ValidateDirectory(directory, DEFAULT_DIRECTORY);
        }

        /* If the directory is null or zero-length, use the default directory. */
        if (directory == null || directory.Trim().Length < 1) {
            directory = defaultDirectory;
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
}
