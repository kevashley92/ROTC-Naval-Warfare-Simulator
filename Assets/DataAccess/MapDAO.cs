using System.IO;

public class MapDAO {
    public string path;
    public string ending;
    public string scenarioPath;

    public MapDAO(string folder) {
        this.scenarioPath = "Assets" + Path.DirectorySeparatorChar + "DataAccess" + Path.DirectorySeparatorChar + folder + Path.DirectorySeparatorChar;
        this.path = this.scenarioPath + "map" + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(this.path);
        this.ending = ".map";
    }

    private void RecursiveDirectoryRemoval(string currentPath) {
        DirectoryInfo dir = new DirectoryInfo(currentPath);

        foreach (FileInfo file in dir.GetFiles()) {
            file.Delete();
        }

        foreach (DirectoryInfo di in dir.GetDirectories()) {
            RecursiveDirectoryRemoval(di.FullName);
            di.Delete();
        }
    }

    public void RemoveDirectory() {
        RecursiveDirectoryRemoval(scenarioPath);
        DirectoryInfo dir = new DirectoryInfo(scenarioPath);
        dir.Delete();
    }

    public void Save() {
        World.Save(path + Editor.mapName + ending);
    }

    public string Load() {
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] files = dirInfo.GetFiles();

        foreach (FileInfo file in files) {
            if (file.Extension.Equals(".map")) {
                World.Load(file.FullName);
                return file.Name;
            }
        }

        return "";
    }
}
