using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class NetworkManager : MonoBehaviour
{
	public static NetworkManager NetworkInstance;
	private List<NetworkPlayer> NetworkPlayers;
	private List<bool> NetWorkPlayersDoneSending;
	private Master Master;
	private Chat Chat;
	private static bool isServer = false;
	public static string pass;
	public static string ip;
	public static int teamcode;
	public static HostData hostData;
	public static string gameName;
	public bool isLan;
	private bool loadedWorld = false;
	private int playerCountForDelete;
	private int playersThatResponded;
    public GameObject disconnectedPopup;
    public UnityEngine.UI.Text pauseButton;

	// private Team teams = new Team.Teams();
    
	public Master MASTER
	{
		get{ return this.Master;}
	}
    
	public Chat CHAT
	{
		get{ return this.Chat;}
	}
    
	public bool IsServer
	{
		get{ return NetworkManager.isServer;}
		set{ NetworkManager.isServer = value;}
	}
	public void setLan (bool Lan)
	{
		isLan = Lan;
	}

    public void OnDisconnectedFromServer(NetworkDisconnection info){
        if(null != disconnectedPopup)
            disconnectedPopup.SetActive(true);
    }

    public void ApplicationClose(){
        Application.Quit();
    }
    
    #region Network Instance
	void Awake ()
	{   
		if (NetworkInstance == null)
		{
			NetworkInstance = this;
			NetworkPlayers = new List<NetworkPlayer> ();
			NetWorkPlayersDoneSending = new List<bool> ();
			Master = new Master ();
			Chat = new Chat ();
            
		}
	}
    #endregion
    
    #region Server
	public void StartServer (int connectionCount, int portNumber, bool useNAT)
	{
		LoginHandler.LoginHandlerInstance.connectionCode = 1;
		isServer = true;
        Network.InitializeServer (connectionCount, portNumber, true);
		
        
	}
    public void Register(){
        Master.Register (gameName);
    }
	public void SetGameName (String Name)
	{
		gameName = Name;
	}
    
    
	public void SetServerPassword (string password)
	{
		Network.incomingPassword = password;
	}
    #endregion
    
    #region Client
	public void ConnectToLocalServer (string ipAddress, int portNumber, string password)
	{
		isServer = false;
		Master = new Master ();
		if (ip.Equals ("-1"))
		{
			ConnectToServer (hostData, password);
		}
		else
		{
			Network.Connect (ipAddress, portNumber, password);
		}

    }
    public void ConnectToServer (HostData hostdata, string password)
    {
        isServer = false;

        try{
            Master.Connect (hostdata, password);
        }catch(Exception e){
            Application.LoadLevel("Login");
        }
        
	}
    public void SendRefresh()
    {
        if (isServer)
        {
            networkView.RPC("RefreshWeapons", RPCMode.Others);
        }
    }

    [RPC]
    public void RefreshWeapons()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            GameObject selectedUnit = canvas.GetComponent<UIMainController>().GetSelectedUnit();
            if (selectedUnit != null)
            {
                canvas.GetComponent<UIMainController>().DeselectUnit();
                if (canvas.GetComponent<UIUnitInspectorController>() != null)
                {
                    canvas.GetComponent<UIUnitInspectorController>().StopInspecting();
                }
            }
        }
    }
    #endregion
    
    #region Player Lists
	public List<NetworkPlayer> GetPlayers ()
	{
		return NetworkPlayers;
	}
    #endregion
    
    #region Network Messages
	void OnPlayerConnected (NetworkPlayer player)
	{   
		if (!NetworkPlayers.Contains (player))
		{
			NetworkPlayers.Add (player);
            
			NetWorkPlayersDoneSending.Add (false);
		}
		SendWorld (World.Instance);
		SendTeams ();

        if (isServer)
        {
            networkView.RPC("Pausing", RPCMode.All, TurnManager.GetTurnManager().Paused);
        }
		/*List<GameObject> lists = GuidList.GetAllObjects ();
        foreach (GameObject go in lists)
        {
            //Debug.Log (lists.Count);
            SendObject (go);
        }*/
    }
    
    void OnPlayerDisconnected (NetworkPlayer player)
    {
        Network.RemoveRPCs (player);
        Network.DestroyPlayerObjects (player);
        for (int i = 0; i < NetworkPlayers.Count; i++)
        {
            if (player == NetworkInstance.NetworkPlayers [i])
            {
                NetWorkPlayersDoneSending.RemoveAt (i);
                break;
            }
        }
        NetworkPlayers.Remove (player);
    }
    
    void OnConnectedToServer ()
    {
        if (!isServer)
        {
            LoginHandler.LoginHandlerInstance.connectionCode = 1;
        }
    }
    
    void OnFailedToConnect ()
    {
        if (!isServer) {
            LoginHandler.LoginHandlerInstance.connectionCode = 0;
            //Debug.Log ("Failed To Connect");
            Application.Quit();

        } 
    }

    #endregion
    
    #region Event Handling
	public void SendEvent (GEvent e)
	{
		MemoryStream m = new MemoryStream ();
		BinaryFormatter b = new BinaryFormatter ();
		b.Serialize (m, e);
		string test = Convert.ToBase64String (m.GetBuffer ());

		networkView.RPC ("GetEvent", RPCMode.Server, test); 	
		//Debug.Log ("Sent event of type " + e.EventType.ToString () + " with timestamp " + e.Timestamp.ToString () + " and " + e.Arguments.Length + " argument(s)");
	}
    
	private void ResetDoneSending ()
	{
		for (int i = 0; i < NetworkInstance.NetWorkPlayersDoneSending.Count; i++)
		{
			NetWorkPlayersDoneSending [i] = false;
		}
	}
    
	private bool AllDoneSending ()
	{
		for (int i = 0; i < NetworkInstance.NetWorkPlayersDoneSending.Count; i++)
		{
			if (!NetWorkPlayersDoneSending [i])
			{
				return false;
			}
		}
		return true;
	}
    
	[RPC]
	void GetEvent (string e)
	{
		MemoryStream m = new MemoryStream (Convert.FromBase64String (e));
		BinaryFormatter b = new BinaryFormatter ();
		GEvent test = (GEvent)b.Deserialize (m);
		EventManager.Instance.AddEvent (test);
        
	}
    
	[RPC]
	void DoneSending (NetworkPlayer p)
	{
		for (int i = 0; i < NetworkInstance.NetworkPlayers.Count; i++)
		{
			if (p == NetworkInstance.NetworkPlayers [i])
			{
				NetWorkPlayersDoneSending [i] = true;
			}
		}
	}
    #endregion
    
    #region Chat
	public void SendMessage (Chat.ChatMessage c)
	{
		MemoryStream m = new MemoryStream ();
		BinaryFormatter b = new BinaryFormatter ();
		b.Serialize (m, c);
		string test = Convert.ToBase64String (m.GetBuffer ());
		networkView.RPC ("GetMessage", RPCMode.All, test);  
	}
    
	//      public void SendMessage (User u, string m, int c)
	//      {
	//              Chat.ChatMessage ch = new Chat.ChatMessage (c, m, u);
	//      }
    
	[RPC]
	void GetMessage (string s)
	{
		MemoryStream m = new MemoryStream (Convert.FromBase64String (s));
		BinaryFormatter b = new BinaryFormatter ();
		Chat.ChatMessage test = (Chat.ChatMessage)b.Deserialize (m);
		Chat.AddMessage (test);
	}
    
	public List<Chat.ChatMessage> GetMessages ()
	{
		return Chat.GetMessages ();
	}
    #endregion
    
    #region World
	public void SendWorld (World w)
	{
		MapDebuggable.Debug ("Sending world to client.");
		var b = World.ConvertToBytes (w);
		networkView.RPC ("GetWorld", RPCMode.Others, b);
	}
    
	[RPC]
	void GetWorld (byte[] b)
	{
		if (!loadedWorld)
		{

			MapDebuggable.Debug ("World received from server.");
			var w = World.ConvertFromNetworkBuffer (b);
			World.Instance = w;
			loadedWorld = true;
		}
	}

    #endregion
    
    #region Unit Destroying
	// Initial call
	public void DestroyUnitOnNetwork (GameObject go)
	{
		IdentityController ic = go.GetComponent<IdentityController> ();
		if (ic == null)
		{
			return;
		}
		
		if (isServer && NetworkPlayers.Count <= 0)
		{
			GuidList.RemoveGameObject (ic.MyGuid);
			networkView.RPC ("RemoveObjectFromGuidList", RPCMode.OthersBuffered, ic.MyGuid);
			Network.Destroy (go);
		}
		else
		{
			networkView.RPC ("NetworkDestroyUnit", RPCMode.Server, ic.MyGuid);
		}
	}
    
	// Server initial RPC
	[RPC]
	void NetworkDestroyUnit (string guid)
	{
		playerCountForDelete = NetworkPlayers.Count;
		playersThatResponded = 0;

		//Remove the object from all of the GUID lists
		networkView.RPC ("RemoveObjectFromGuidList", RPCMode.OthersBuffered, guid);
	}
    
	// Client initial RPC
	[RPC]
	void RemoveObjectFromGuidList (string guid)
	{
		GuidList.RemoveGameObject (guid);
        
		networkView.RPC ("ClientRemovedGuid", RPCMode.Server, guid);
	}
    
	// Server client response
	[RPC]
	void ClientRemovedGuid (string guid)
	{
		playersThatResponded++;
        
		if (playersThatResponded >= playerCountForDelete)
		{
			GameObject go = GuidList.GetGameObject (guid);
			GuidList.RemoveGameObject (guid);
			Network.Destroy (go);
		}
	}
    #endregion
    
    #region SerializationTestForObjects
	public Controller ConvertByteArrayToObject (byte[] data)
	{
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		Controller test = (Controller)b.Deserialize (m);
		return test;
	}
    
	public byte[] ConvertObjectToByteArray (object obj)
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		MemoryStream stream = new MemoryStream ();
		formatter.Serialize (stream, obj);
		byte[] message = stream.ToArray ();
		return message;
	}
    
	private GameObject curObj = null;
    
	public GameObject InstancePrefabOnNetwork (string prefab)
	{
		return Network.Instantiate (Resources.Load (prefab), new Vector3 (0, 35, 0), new Quaternion (), 0) as GameObject;
		networkView.RPC ("StartObject", RPCMode.Server, prefab);
	}
    #endregion
    
    #region Teams

	public void SendTeams ()
	{
		byte[] b = ConvertObjectToByteArray (Team.Teams);
		networkView.RPC ("GetTeams", RPCMode.Others, b);
	}
	[RPC]
	void GetTeams (byte[] data)
	{
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		Dictionary<int, Team> d = (Dictionary<int,Team>)b.Deserialize (m);
		//Debug.Log ("RPC: Got " + d.Count + " teams");
		Team.Teams = d;
        
	}
    #endregion
    
    #region GlobalSettings
	public void SendGlobalSettings ()
	{
		byte[] b = ConvertObjectToByteArray (GlobalSettings.Instance ().GetValues ());
		networkView.RPC ("GetGlobalSettings", RPCMode.Others, b);

	}
	[RPC]
	void GetGlobalSettings (byte[] data)
	{
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		Dictionary<string, float> d = (Dictionary<string,float>)b.Deserialize (m);
		//Debug.Log ("RPC: Got Global Settings");
		GlobalSettings.Instance ().SetValues (d);
        
	}
    #endregion
    
    #region SendToServerCode
	public void StartObject (string selected, Point p, string team, string objType)
	{
		byte[] b = ConvertObjectToByteArray (p);
		networkView.RPC ("GetServerAdd", RPCMode.Server, selected, b, team, objType);
	}
	[RPC]
	void GetServerAdd (string selected, byte[] data, string team, string typeObj)
	{
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		ScenarioEditor temp = new ScenarioEditor ();
		Point point = (Point)b.Deserialize (m);
		ObjectFactory unit = new ObjectFactory ();
		int type = Convert.ToInt32 (typeObj);
		GameObject gameObjectToAdd;
		if (type == 0)
		{
			gameObjectToAdd = unit.LoadAir (selected);
		}
		else if (type == 1)
		{
			gameObjectToAdd = unit.LoadMarine (selected);
		}
		else if (type == 2)
		{
			gameObjectToAdd = unit.LoadSubSurface (selected);
		}
		else
		{
			gameObjectToAdd = unit.LoadSurface (selected);
		}
		gameObjectToAdd.GetComponent<IdentityController> ().TeamNumber = Convert.ToInt32 (team);
		gameObjectToAdd.transform.localPosition = new Vector3 (point.x, point.y);
	}
	
    private GameObject selectedObj = null;
    public void setObject(string b){
        networkView.RPC ("SettingObject", RPCMode.All, b);

    }
    [RPC]
    void SettingObject(string b){
        selectedObj = GuidList.GetGameObject (b);

    }
    public void MoveObject(string name, Point p)
    {
        byte[] b = ConvertObjectToByteArray(p);
        networkView.RPC("MovingObject", RPCMode.Server, name, b);
    }
    [RPC]
    void MovingObject(string name, byte[] data)
    {
        MemoryStream m = new MemoryStream(data);
        BinaryFormatter b = new BinaryFormatter();
        Point point = (Point)b.Deserialize(m);
        if (selectedObj != null) {
            selectedObj.transform.localPosition = new Vector3 (point.x, point.y, selectedObj.transform.localPosition.z);
        }
    }

	public void StartObject(string selected, Point p, string team){
		byte[] b = ConvertObjectToByteArray (p);
		networkView.RPC ("GetServerAdd", RPCMode.Server, selected, b, team);
	}
	[RPC]
	void GetServerAdd(string selected, byte[] data, string team){
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		ScenarioEditor temp = new ScenarioEditor ();
		Point point = (Point)b.Deserialize (m);
		ObjectFactory unit = new ObjectFactory();
		GameObject gameObjectToAdd = unit.LoadSurface(selected);
		gameObjectToAdd.GetComponent<IdentityController>().TeamNumber = Convert.ToInt32(team);
		gameObjectToAdd.transform.localPosition = new Vector3 (point.x, point.y);
	}

	public void SendLogs(Dictionary<string, string> input){
		byte[] b = ConvertObjectToByteArray (input);
		networkView.RPC ("GetLogs", RPCMode.Others, b);
	}
	[RPC]
	void GetLogs(byte[] data){
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		Dictionary<string, string> p = (Dictionary<string, string>)b.Deserialize (m);
		
		int currentTeamNumber = User.getCurrentUser().TeamNumber;
		
		string teamName = Team.Teams[currentTeamNumber].GetTeamName();

		GameObject.Find ("LogText").GetComponent<UIUpdateLog> ().SetLogText (p [teamName].ToString ());
		////Debug.Log(p[teamName].ToString());
	}
	
	public void SendFullLog(Dictionary<string, string> input){
		byte[] b = ConvertObjectToByteArray (input);
		networkView.RPC ("GetFullLog", RPCMode.Others, b);
	}
	[RPC]
	void GetFullLog(byte[] data){
		MemoryStream m = new MemoryStream (data);
		BinaryFormatter b = new BinaryFormatter ();
		Dictionary<string, string> p = (Dictionary<string, string>)b.Deserialize (m);
		
		int currentTeamNumber = User.getCurrentUser().TeamNumber;
		
		string teamName = Team.Teams[currentTeamNumber].GetTeamName();

		GameObject.Find ("LogText").GetComponent<UIUpdateLog> ().SetFullLogText (p [teamName].ToString ());
		////Debug.Log(p[teamName].ToString());
	}
	
	#endregion

	#region SceneInitialization
	void OnLevelWasLoaded (int level)
	{
        
		if (Application.loadedLevelName != "Login")
		{
            
			if (isServer)
			{
                
				StaticSceneInitializer.InitializeGenericServerScene ();
                
			}
			else
			{
                
				StaticSceneInitializer.InitializeGenericClientScene ();
                
			}   
            
		}
        
	}
    #endregion
    
    #region EndOfTurnSpinner
    
	public void ActivateEndTurnSpinner ()
	{
		//Debug.Log("On Server: Sending TurnSpinner RPC");
		networkView.RPC ("RPCActivateEndTurnSpinner", RPCMode.Others);
        
	}
    
	[RPC]
	void RPCActivateEndTurnSpinner ()
	{
		//Debug.Log("On Client: Received TurnSpinner RPC");
		TurnManager.ActivateEndTurnSpinnerStatic ();	
	}
    
    #endregion

    #region Turn Timer
    public void Pause(bool paused){
        if (isServer)
        {
            networkView.RPC("Pausing", RPCMode.All, paused);
        }
    }
    [RPC]
    void Pausing(bool paused){
        if ("UserViewIke".Equals(Application.loadedLevelName))
        {
            TurnManager.GetTurnManager().SetObjectsForPause(!paused);
        }
        else
        {
            TurnManager.GetTurnManager().SetObjectsForPause(paused);
        }
        if(pauseButton != null)
            pauseButton.text = paused ? LanguageManager.instance.getString("UI_Admin", "Text_Unpause_Label") : LanguageManager.instance.getString("UI_Admin", "Text_Pause_Label");
    }
    #endregion
}
