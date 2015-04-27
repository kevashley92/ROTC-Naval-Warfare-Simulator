using UnityEngine;
using System.Collections;
using System.IO;

public class FileLoader : iFileLoader{
	
	string path;
	string folder;
    string fileName;
	
	public FileLoader(string folder, string fileName)
	{
        this.fileName = fileName;
		this.folder = folder;
		this.path = "Assets"  + Path.DirectorySeparatorChar + "DataAccess"  + Path.DirectorySeparatorChar + "" + folder;
	}
	
	public string Read()
	{
		if(File.Exists(path + Path.DirectorySeparatorChar + fileName + ".json"))
        	return File.ReadAllText(path + Path.DirectorySeparatorChar + fileName + ".json");
		else
			return "ERROR";
	}

    public string ReadDefault()
    {
		if(File.Exists(path + Path.DirectorySeparatorChar + "default.json"))
        	return File.ReadAllText(path + Path.DirectorySeparatorChar + "default.json");
		else
			return "ERROR";
    }
	
	public void Write(string json)
	{
        Directory.CreateDirectory(path);
        StreamWriter sw = new StreamWriter(path + Path.DirectorySeparatorChar + fileName + ".json");
        sw.Write(json);
		sw.Close();
	}
}
