using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

public class WeatherDAO {

	private iFileLoader saves;

	public WeatherDAO(iFileLoader saves){

		this.saves = saves;

	}

	public void SaveOne(string name, Dictionary<string, System.Object> toSave){

		string toLoadJson = saves.Read ();
		
		if (toLoadJson.Equals(""))
		{
			Dictionary<string, System.Object> toAddAdditionalTo = new Dictionary<string, object>();
			toAddAdditionalTo.Add(name, toSave);
			saves.Write((string)Json.Serialize(toAddAdditionalTo));
		}
		else
		{
			
			Dictionary<string, System.Object> toAddAdditionalTo = ((Dictionary<string, System.Object>)Json.Deserialize(toLoadJson));
			
			toAddAdditionalTo.Add(name, toSave);
			
			string toWriteToFile = (string)Json.Serialize(toAddAdditionalTo);
			
			saves.Write(toWriteToFile);
		}


	}
	public Dictionary<string, System.Object> LoadOne(string name)
	{		
		string toLoadJson = saves.Read ();
		
		Dictionary<string,System.Object> savedDictionary = (Dictionary<string, System.Object>)Json.Deserialize(toLoadJson);
		
		Dictionary<string,System.Object> toLoad = (Dictionary<string, System.Object>)savedDictionary[name];
	
		return toLoad;
	}
	
	public List<string> GetAllNames()
	{
		List<string> toReturn = new List<string>();
		
		string toLoadJson = saves.Read ();
		
		Dictionary<string,System.Object> toCompare = (Dictionary<string, System.Object>)Json.Deserialize(toLoadJson);
		
		toReturn.AddRange(toCompare.Keys);
		
		return toReturn;
	}

}
