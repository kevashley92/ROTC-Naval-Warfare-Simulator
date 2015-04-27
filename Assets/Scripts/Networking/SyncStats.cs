using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SyncStats : MonoBehaviour
{
	GameObject SyncGameObject;
	
	void Awake ()
	{
		SyncGameObject = gameObject;
	}
	
	private void AddNewtorkViews ()
	{
		Controller[] controllers = SyncGameObject.GetComponents<Controller> ();
		
		foreach (Controller controller in controllers)
		{
			NetworkView controllerNetworkView = SyncGameObject.AddComponent<NetworkView> ();
			controllerNetworkView.observed = controller;
			controllerNetworkView.viewID = Network.AllocateViewID ();
		}
	}
	
	public void sync (BitStream stream)
	{
		
		if (stream.isWriting)
		{
			write (stream);
		}
		else
		{
			read (stream);
		}
	}
	
	private void write (BitStream stream)
	{
		Dictionary<string, object> controllersSerialized = new Dictionary<string, object> ();
		Controller[] controllers = SyncGameObject.GetComponents<Controller> ();
		
		foreach (Controller controller in controllers)
		{
			Dictionary<string, object> tempValues = (Dictionary<string, object>)controller.GetValues ();
			string controllerValues = MiniJSON.Json.Serialize (tempValues);
			string controllerKey = controller.GetType ().Name;
			controllersSerialized.Add (controllerKey, controllerValues);
		}
		
		string controllersSerializedString = MiniJSON.Json.Serialize (controllersSerialized);
		
		char[] serializedChars = controllersSerializedString.ToCharArray ();
		
		char end = '\0';
		
		for (int i = 0; i < serializedChars.Length; i++)
		{
			stream.Serialize (ref serializedChars [i]);
		}
		
		stream.Serialize (ref end);
	}
	
	private void read (BitStream stream)
	{
		List<char> serializedCharList = new List<char> ();
		
		char c = ' ';
		stream.Serialize (ref c);
		while (c != '\0')
		{
			serializedCharList.Add (c);
			stream.Serialize (ref c);
		}
		
		char[] serializedCharArray = serializedCharList.ToArray ();
		
		string controllersSerialized = new string (serializedCharArray);
		
		Dictionary<string, object> controllersDeserialized = MiniJSON.Json.Deserialize (controllersSerialized) as Dictionary<string, object>;
		
		List<string> keys = new List<string> (controllersDeserialized.Keys);
		
		foreach (string key in keys)
		{
			object controlerDeserializedObject = null;
			controllersDeserialized.TryGetValue (key, out controlerDeserializedObject);
			string controlerDeserializedString = controlerDeserializedObject as string;
			Dictionary<string, object> controlerDeserialized = MiniJSON.Json.Deserialize (controlerDeserializedString) as Dictionary<string, object>;
			Controller controller = SyncGameObject.GetComponent (key) as Controller;
			if (controller != null)
			{
				controller.SetValues (controlerDeserialized);
			}
		}
	}
}
