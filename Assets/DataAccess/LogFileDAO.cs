using UnityEngine;
using System.Collections;

/**
 * Class which standardizes the location of the event logs.
 * 
 */
public class LogFileDAO {

	private iFileLoader directory;

	public LogFileDAO(LogFileLoader logDirectory){

		directory = logDirectory;

	}

	/**
	 * Save string to file.  Will be appended.
	 */
	public void Save(string line)
	{
		directory.Write(line);

	}

	/*
	 * Load the entire list of logs from log file.
	 */
	public string Load()
	{

		string toReturn = directory.Read();
		return toReturn;
	}

}
