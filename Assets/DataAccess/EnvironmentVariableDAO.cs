using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

public class EnvironmentVariableDAO {
	private iFileLoader saves;
	private iFileLoader defaults;
	
	public EnvironmentVariableDAO(iFileLoader saves, iFileLoader defaults) {
		this.saves = saves;
		this.defaults = defaults;
	}
	
	public void SaveOne(Dictionary<string,System.Object> toSave)
	{

		string toWriteToFile = (string)Json.Serialize(toSave);
		
		saves.Write (toWriteToFile);
	}
	
	public Dictionary<string, System.Object> LoadOne()
	{		
		string toLoadJson = saves.Read ();
		
		Dictionary<string,System.Object> savedDictionary = (Dictionary<string, System.Object>)Json.Deserialize(toLoadJson);
		
		string toCompareJson = defaults.ReadDefault();
		
		Dictionary<string,System.Object> toCompare = (Dictionary<string, System.Object>)Json.Deserialize(toCompareJson);
		
		return savedDictionary;
	}

	public void BuildPath()
	{
		saves.Write((string)Json.Serialize(new Dictionary<string, System.Object>()));
	}
	
	public List<string> GetAllNames()
	{
		List<string> toReturn = new List<string>();
		
		string toLoadJson = saves.Read ();
		
		Dictionary<string,System.Object> toCompare = (Dictionary<string, System.Object>)Json.Deserialize(toLoadJson);
		
		toReturn.AddRange(toCompare.Keys);
		
		return toReturn;
	}

	public Dictionary<string, System.Object> LoadDefault()
	{
		string toCompareJson = defaults.ReadDefault();
		Dictionary<string, System.Object> toCompare = (Dictionary<string, System.Object>)Json.Deserialize(toCompareJson);
		
		return toCompare;
	}
	
}
