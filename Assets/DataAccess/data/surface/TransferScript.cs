using UnityEngine;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public class TransferScript : MonoBehaviour {

	private string from = null;

	public string to = null;

	// Use this for initialization
	void Start () {
	
		from = "Assets" + Path.DirectorySeparatorChar + "DataAccess" + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "assets.json";

		from = File.ReadAllText(from);
		string temp = to;
		to = File.ReadAllText("Assets" + Path.DirectorySeparatorChar + "DataAccess" + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + to + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + to + ".json");

		Dictionary<string, System.Object> assetFrom = (Dictionary<string, System.Object>)Json.Deserialize(from);
		
		Dictionary<string, System.Object> assetTo = (Dictionary<string, System.Object>)Json.Deserialize(to);

		Dictionary<string, System.Object> assetToCopy = new Dictionary<string, object>(assetTo);


		foreach(string key in assetTo.Keys){

			//Debug.Log(key);
			if(assetToCopy.ContainsKey(key)) {

				assetToCopy[key] = assetFrom[key];
			}
			else if(!assetToCopy.ContainsKey(key)) {

				assetToCopy.Add(key, assetFrom[key]);
			}

		}

		assetTo = assetToCopy;

		string json = Json.Serialize(assetTo);

		//Debug.Log(json);
	}
	
	// Update is called once per frame
	void Update () {
	



	}
}
