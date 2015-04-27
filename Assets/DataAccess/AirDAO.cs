using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class AirDAO : iGameObjectDAO
{

	private string jsonString;
	
	private iFileLoader saveLoader;
	
	private iFileLoader defaultLoader;
	
	private Dictionary<string, System.Object> saveContents;
	
	private Dictionary<string, System.Object> defaultContent;
	
	/*
	 * Simple constructor.
	*/
	public AirDAO (iFileLoader saveLoader, iFileLoader defaultLoader)
	{		
		this.saveLoader = saveLoader;
		this.defaultLoader = defaultLoader;
		string toLoadJson = saveLoader.Read();
		//.Read() will return ERROR if the file does not exist.  If this is the case
		//then we will wait for saveContents to get set in the BuildFilePath() function which
		//is predictably called before any DAO's are used.  In the other case we are using the
		//save files which DO exist and so saveContents will get set here. 
		if(!toLoadJson.Equals("ERROR"))
			saveContents = ((Dictionary<string, System.Object>)Json.Deserialize (toLoadJson));
		
		
		string toLoadDef = defaultLoader.ReadDefault();
		if(!toLoadDef.Equals("ERROR"))
			defaultContent = ((Dictionary<string, System.Object>)Json.Deserialize (toLoadDef));
		
	}
	
	public void SaveOne(string name, Dictionary<string, System.Object> toSave){
	}
	
	
	/**
	 * Saves the object specified by the Dictionary parameter.
	 * This function will always 
	 * check if an object with the name exists before it adds it.  If it does
	 * it just won't add the object.  Also will check to make sure there is 
	 * still room (could become outdated functionality).
	 */
	public void SaveAll ()
	{
		
		string toWriteToFile = (string)Json.Serialize (saveContents);
		
		saveLoader.Write (toWriteToFile);
		
		saveContents = ((Dictionary<string, System.Object>)Json.Deserialize (saveLoader.Read()));
		
	}
	
	public void AddToSaveList(string name, Dictionary<string,System.Object> toSave){
		
		if(!saveContents.ContainsKey(name)){
			saveContents.Add(name, toSave);	
		}
		else {
			
			saveContents.Remove(name);
			saveContents.Add(name, toSave);
			
		}
		
	}
	
	public void BuildPath ()
	{
		saveLoader.Write ((string)Json.Serialize (new Dictionary<string, System.Object> ()));
		
		string toLoadJson = saveLoader.Read();
		saveContents = ((Dictionary<string, System.Object>)Json.Deserialize (toLoadJson));
		
		string toLoadDef = defaultLoader.ReadDefault();
		defaultContent = ((Dictionary<string, System.Object>)Json.Deserialize (toLoadDef));
	}
	
	/**
	 * Iterates through a list of ships and checks for one with a name
	 * that matches the name parameter.  If it does it'll remove the object.
	 *
	 *@param name - the name of the ship that wil be removed
	 *@return true or false, depending if item is removed
	 */
	public bool DeleteOne (string name)
	{
		
		Dictionary<string, System.Object> toDelete = saveContents;
		
		if (toDelete.ContainsKey (name)) {
			
			toDelete.Remove (name);
			
			string jsonString = Json.Serialize (toDelete);
			
			saveLoader.Write (jsonString);
			
			//Debug.Log ("Success!");
			
			return true;
			
		} else {
			
			//Debug.Log ("Object Doesn't Exist!");
			
			return false;
		}
		
		
	}
	
	/**
	 * Loads an item from the prototype files in data/ship/saves
	 * and uses the default file to ensure the object is loaded with
	 * all the necessary values.  Than the object is created through
	 * ObjectFactory.CreateObject().  
	 */
	public Dictionary<string, System.Object> LoadOne (string name)
	{
		
		Dictionary<string, System.Object> item1 = ((Dictionary<string, System.Object>)saveContents[name]);
		
		Dictionary<string, System.Object> comb = (Dictionary<string, System.Object>)Combiner.Combine (item1, defaultContent);
		
		
		return comb;
	}
	
	/**
	 * returns a list of all keys in the prototype list.  User will
	 * access this list and call LoadOne() in order to get the actual object
	 */
	public List<string> GetAllNames ()
	{
		
		List<string> toReturn = new List<string> ();
		
		foreach (string key in saveContents.Keys)
			toReturn.Add (key);
		
		return toReturn;
	}
	
	public List<LocalizedItem> GetLocalizedNames ()
	{
		
		
		List<LocalizedItem> toReturn = new List<LocalizedItem> ();
		
		foreach (string key in saveContents.Keys)
			toReturn.Add (new LocalizedItem ("Plane", key));
		
		return toReturn;
	}
	
	public Dictionary<string, System.Object> LoadDefault ()
	{
		string toCompareJson = defaultLoader.ReadDefault ();
		
		Dictionary<string, System.Object> toCompare = (Dictionary<string, System.Object>)Json.Deserialize (toCompareJson);
		
		return toCompare;
	}

}
