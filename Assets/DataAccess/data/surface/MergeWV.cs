using UnityEngine;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public class MergeWV : MonoBehaviour {

	string assets = null;

	string weapons = null;

	// Use this for initialization
	void Start () {
	
		
		// This WILL NOT WORK
		assets = File.ReadAllText("C:\\Users\\landi_000\\Documents\\outputm.json");

		weapons = File.ReadAllText("C:\\Users\\landi_000\\Documents\\marineweapon.json");

		Dictionary<string, System.Object> asset = (Dictionary<string, System.Object>)Json.Deserialize(assets);

		Dictionary<string, System.Object> weapon = (Dictionary<string, System.Object>)Json.Deserialize(weapons);

		Dictionary<string, System.Object> assetCopy = new Dictionary<string, System.Object>(asset);

		Dictionary<string, System.Object> weaponCopy = new Dictionary<string, System.Object>(weapon);


		foreach(string ship in asset.Keys){

			Dictionary<string, System.Object> obj = (Dictionary<string, System.Object>)assetCopy[ship];

			Dictionary<string, System.Object> objCopy = new Dictionary<string, System.Object>(obj);

			foreach(string controller in obj.Keys){

				if(controller.Equals("casevac") || controller.Equals("special"))
					break;

				Dictionary<string, System.Object> controllerData = (Dictionary<string, System.Object>)objCopy[controller];

				Dictionary<string, System.Object> controllerDataCopy = new Dictionary<string, System.Object>(controllerData);

				if(controller.Equals("Attack")) {

					List<System.Object> weaponsList = (List<System.Object>)controllerData["weapons"];

					List<System.Object> weaponsCopy = new List<System.Object>(weaponsList);

					foreach(System.Object w in weaponsList) {

						Dictionary<string, System.Object> innerW = (Dictionary<string, System.Object>)w;

						string name = "";

						foreach(string key in innerW.Keys){

							name = key;

						}

						int index = weaponsCopy.IndexOf(w);

						int count = int.Parse(innerW[name].ToString());

						Dictionary<string, System.Object> weaponReplace = new Dictionary<string, object>((Dictionary<string, System.Object>)weapon[name]);

						if(!weaponReplace.ContainsKey("count"))
							weaponReplace.Add("count", count);
						else if(weaponReplace.ContainsKey("count"))
							weaponReplace["count"] = count;

						weaponsCopy.RemoveAt(index);

						//Debug.Log(name);
						weaponsCopy.Insert(index, weaponReplace);

					}

					weaponsList = weaponsCopy;

					controllerDataCopy["weapons"] = weaponsList;
				}

				controllerData = controllerDataCopy;

				objCopy[controller] = controllerData;

			}

			obj = objCopy;

			assetCopy[ship] = obj;

		}

		asset = assetCopy;
	
		string json = Json.Serialize(asset);

		//Debug.Log(json);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
