using UnityEngine;
using System.Collections;
using System.IO;

public class LogFileLoader : iFileLoader{
		
	string path;
	string folder;
	string fileName;
	
	public LogFileLoader(string folder, string fileName)
	{
		this.fileName = fileName;
		this.folder = folder;
		this.path = "Assets"  + Path.DirectorySeparatorChar + "DataAccess"  + Path.DirectorySeparatorChar + "" + folder;
	}
	
	public string Read()
	{
		return File.ReadAllText(path + Path.DirectorySeparatorChar + fileName + ".txt");
	}
	
	public string ReadDefault()
	{
		return "NULL";
	}
	
	public void Write(string txt)
	{
		Directory.CreateDirectory(path);
		StreamWriter sw =  File.AppendText(path + Path.DirectorySeparatorChar + fileName + ".txt");
		sw.Write(txt);
		sw.Close();
	}
}
