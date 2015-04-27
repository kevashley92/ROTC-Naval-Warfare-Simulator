using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Maps;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScenarioEditor : MonoBehaviour, ILoad, ISave
{
	public LoadWindowController fileBrowser;
	public SaveWindowController saveDialog;
	private ScenarioFactory sceneFactory;
	private string currentFileName;
	private static World currentWorld;
	public GameObject UnitMenu;
	private string selectedUnit = "";
	private enum UnitType
	{
		AIR,
		MARINE,
		SUBSURFACE,
		SURFACE,
		NONE }
	;
	private UnitType selectedUnitType;
	private enum Tools
	{
		PLACE,
		MOVE,
		DELETE,
		EDIT }
	;
	private Tools selectedTool;
	private ObjectFactory unitFactory;
	public Camera mapCamera;
	private GameObject selectedGO;
	private Vector3 origionalPosition;
	public ComboBox unitComboBox;
	private Editor editor;
	public GameObject MapButton;
	public UnitManager unitmanager;
	public GameObject LoadButton;
	public GameObject SaveButton;
	public Sprite surface, subsurface, air, marine;
	public ComboBox TeamComboBox;
	//For displaying unit info
	public GameObject InfoBoxPrefab;
	private GameObject InfoBox;
	private int timeSinceClick;
	// Use this for initialization
	void Start ()
	{
		sceneFactory = new ScenarioFactory ();
		currentWorld = null;
		selectedTool = Tools.EDIT;
		unitFactory = new ObjectFactory ();
		selectedUnitType = UnitType.NONE;
		editor = GameObject.FindObjectOfType<Editor> ();
		InfoBox = Instantiate (InfoBoxPrefab) as GameObject;
		InfoBox.SetActive (false);
		timeSinceClick = 0;
        if (!NetworkManager.NetworkInstance.IsServer)
        {
            GameObject.Destroy(MapButton.GetComponent<EventTrigger>());
            var color = MapButton.GetComponent<Text>().color;
            MapButton.GetComponent<Text>().color = new Color(color.r, color.g, color.b, .5f);
            color = LoadButton.GetComponent<Image>().color;
            LoadButton.GetComponent<Image>().color = new Color(color.r, color.g, color.b, .3f);
            LoadButton.GetComponent<Button>().enabled = false;
            SaveButton.GetComponent<Image>().color = new Color(color.r, color.g, color.b, .3f);
            SaveButton.GetComponent<Button>().enabled = false;
        }
	}
    
	// Update is called once per frame
	void Update ()
	{
		timeSinceClick++;
		currentWorld = World.Instance;
		GameObject toShowInfo = RaycastForObject ();
		if (toShowInfo != null)
		{
			if (toShowInfo.GetComponent<IdentityController> () != null && timeSinceClick > 60)
			{
				InfoBox.SetActive (true);
				InfoBox.GetComponent<UIUnitInfoController> ().init (toShowInfo);
			}

		}
		else
		{
			InfoBox.SetActive (false);
		}
	}
    
	void OnMouseDown ()
	{
		var gridPos = mapCamera.ScreenToWorldPoint (Input.mousePosition);
		timeSinceClick = 0;
		// Record the grid position of where the mouse was released
		////Debug.Log ("OnMouseDown at: [" + gridPos.x + "," + gridPos.y + "]");
		Point point = new Point ((int)(gridPos.x + 0.5), (int)(gridPos.y + 0.5));
		////Debug.Log ("Point at : [" + point.x + "," + point.y + "]");
        
		if (UIMenuController.IsActiveMenu (UnitMenu))
		{
			GameObject gameObjectToAdd = null;
			switch (selectedTool)
			{
			case Tools.PLACE:
				try
				{
					selectedUnit = unitmanager.GetSelectedUnit ();
				}
				catch (Exception)
				{
					//Debug.Log ("Selected Unit not found from combobox!");
					selectedUnit = "";
				}

                if (!selectedUnit.Equals (""))
                {
                    switch (selectedUnitType)
                    {
                    case UnitType.AIR:
                        if (IsValid (Convert.ToInt32(point.x), Convert.ToInt32(point.y)))
                        {
                            point.z = -0.0004f;
                            if (NetworkManager.NetworkInstance.IsServer)
                            {
                                gameObjectToAdd = unitFactory.LoadAir (selectedUnit);
                                AddObject (gameObjectToAdd, point);

								string[] teams = {Team.Teams[TeamComboBox.SelectedIndex].GetTeamName()};
								string message = "Admin has spawned an Air unit on team " + teams[0] + " at latitude: " + point.x + " longitude: " + point.y;
								
								EventLog.LogManual(EventLog.FormatEventString(teams,Time.time,message));
                            }
                            else
                            {
                                int teamIndex = TeamComboBox.SelectedIndex;
                                NetworkManager.NetworkInstance.StartObject (selectedUnit, point, Convert.ToString (teamIndex), "0");
                            }
                        }
                        break;
                    case UnitType.MARINE:
                        if (IsValid (point.x, point.y) && IsLand (point.x, point.y))
                        {
                            if (NetworkManager.NetworkInstance.IsServer)
                            {
                                gameObjectToAdd = unitFactory.LoadMarine (selectedUnit);
                                AddObject (gameObjectToAdd, point);

								string[] teams = {Team.Teams[TeamComboBox.SelectedIndex].GetTeamName()};
								string message = "Admin has spawned a Marine unit on team " + teams[0] + " at latitude: " + point.x + " longitude: " + point.y;
								
								EventLog.LogManual(EventLog.FormatEventString(teams,Time.time,message));
                            }
                            else
                            {
                                int teamIndex = TeamComboBox.SelectedIndex;
                                NetworkManager.NetworkInstance.StartObject (selectedUnit, point, Convert.ToString (teamIndex), "1");
                            }
                        }
                        break;
                    case UnitType.SUBSURFACE:
                        if (IsValid (point.x, point.y) && !IsLand (point.x, point.y))
                        {
                           
                            if (NetworkManager.NetworkInstance.IsServer)
                            {
                                gameObjectToAdd = unitFactory.LoadSubSurface (selectedUnit);
                                gameObjectToAdd.transform.localPosition = new Vector3(point.x, point.y, point.z);
                                AddObject (gameObjectToAdd, point);

								string[] teams = {Team.Teams[TeamComboBox.SelectedIndex].GetTeamName()};
								string message = "Admin has spawned a Subsurface unit on team " + teams[0] + " at latitude: " + point.x + " longitude: " + point.y;
								
								EventLog.LogManual(EventLog.FormatEventString(teams,Time.time,message));
                            }
                            else
                            {
                                int teamIndex = TeamComboBox.SelectedIndex;
                                NetworkManager.NetworkInstance.StartObject (selectedUnit, point, Convert.ToString (teamIndex), "2");
                            }   
                        }
                        break;
                    case UnitType.SURFACE:
                        if (IsValid (point.x, point.y) && !IsLand (point.x, point.y))
                        {
                            point.z = -.0002f;
                            if (NetworkManager.NetworkInstance.IsServer)
                            {
                                gameObjectToAdd = unitFactory.LoadSurface (selectedUnit);
                                AddObject (gameObjectToAdd, point);

								string[] teams = {Team.Teams[TeamComboBox.SelectedIndex].GetTeamName()};
								string message = "Admin has spawned an Surface unit on team " + teams[0] + " at latitude: " + point.x + " longitude: " + point.y;
								
								EventLog.LogManual(EventLog.FormatEventString(teams,Time.time,message));
                            }
                            else
                            {
                                int teamIndex = TeamComboBox.SelectedIndex;
                                NetworkManager.NetworkInstance.StartObject (selectedUnit, point, Convert.ToString (teamIndex), "3");
                            }

                        }
                        break;
                    default:
                        break;
                    }
                }
                break;
            case Tools.MOVE:
                    selectedGO = RaycastForObject ();
                    if (selectedGO != null)
                    {
                        if(NetworkManager.NetworkInstance.IsServer){
                            origionalPosition = selectedGO.transform.localPosition;
                        }else{
                            NetworkManager.NetworkInstance.setObject(selectedGO.GetComponent<IdentityController>().GetGuid());
                        }
                    }
                break;
            case Tools.DELETE:
                GameObject toDelete = RaycastForObject ();
                if (toDelete != null)
                {
                    NetworkManager.NetworkInstance.DestroyUnitOnNetwork (toDelete); //
                }
                break;
            case Tools.EDIT:
                GameObject toEdit = RaycastForObject ();
                if (toEdit != null)
                {
                    if(NetworkManager.NetworkInstance.IsServer){
                        // Send to editor.
                        unitmanager.SetCurrentObject (toEdit);
                        unitmanager.realObjectSelected = true;
                        unitmanager.RefreshUI ();
                    }else{
                        NetworkManager.NetworkInstance.setObject(selectedGO.GetComponent<IdentityController>().GetGuid());
                        unitmanager.SetCurrentObject (toEdit);
                        unitmanager.realObjectSelected = true;
                        unitmanager.RefreshUI ();
                    }
                }
                break;
            default:
                break;
            }
        }
    }
    private void AddObject (GameObject gameObjectToAdd, Point point)
    {
        int teamIndex = TeamComboBox.SelectedIndex;
        gameObjectToAdd.GetComponent<IdentityController> ().TeamNumber = teamIndex;
        gameObjectToAdd.transform.localPosition = new Vector3 (point.x, point.y, point.z);
    }
    
	private GameObject RaycastForObject ()
	{
		RaycastHit2D hit = Physics2D.Raycast (mapCamera.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		if (hit != null && hit.collider != null)
		{
			return hit.transform.gameObject;
		}
		return null;
	}
    
	void OnMouseDrag ()
	{
		if (UIMenuController.IsActiveMenu (UnitMenu))
		{
			var gridPos = mapCamera.ScreenToWorldPoint (Input.mousePosition);
			Point point = new Point ((int)gridPos.x, (int)gridPos.y);
			switch (selectedTool)
			{
                case Tools.MOVE:
                    if (selectedGO != null)
                    {
                        if (NetworkManager.NetworkInstance.IsServer)
                        {
                            selectedGO.transform.localPosition = new Vector3(point.x, point.y, 0);
                        }
                        else
                        {
                            NetworkManager.NetworkInstance.MoveObject(selectedGO.name, point);

                        }
                    }
                    break;
			default:
				break;
			}
		}
	}
	private bool checkForCarrier (GameObject toAdd)
	{
		//Move object out of way to raycast
		Vector3 pos = toAdd.transform.position;
		toAdd.transform.position = new Vector3 (0, 0, 0);
		GameObject carrier = RaycastForObject ();
		toAdd.transform.position = pos;
        
		if (carrier != null && carrier.GetComponent<ContainerController> () != null)
		{
			//Todo: confirm dialogue
			carrier.GetComponent<ContainerController> ().Add (toAdd.GetComponent<IdentityController> ().GetGuid ());
			toAdd.SetActive (false);
			//Debug.Log ("Unit added to carrier");
			return true;
		}
		return false;
	}
	public void SetSelectedUnit (string selectedUnit)
	{
		this.selectedUnit = selectedUnit;
	}
    
	public void SetSelectedUnitType (string selectedUnitType)
	{
		switch (selectedUnitType)
		{
		case "MARINE":
			this.selectedUnitType = UnitType.MARINE;
			break;
		case "SURFACE":
			this.selectedUnitType = UnitType.SURFACE;
			break;
		case "SUBSURFACE":
			this.selectedUnitType = UnitType.SUBSURFACE;
			break;
		case "AIR":
			this.selectedUnitType = UnitType.AIR;
			break;
		default:
			break;
		}
	}
    
	public void SetSelectedTool (string selectedTool)
	{
		switch (selectedTool)
		{
		case "PLACE":
			this.selectedTool = Tools.PLACE;
			break;
		case "MOVE":
			this.selectedTool = Tools.MOVE;
			break;
		case "DELETE":
			this.selectedTool = Tools.DELETE;
			break;
		case "EDIT":
			this.selectedTool = Tools.EDIT;
			break;
		default:
			break;
		}
	}
    
	private bool IsLand (float x, float y)
	{
        byte terrain = currentWorld.TerrainAt (new Vector2 (Convert.ToInt32(x), Convert.ToInt32(y))).ID;
		return (terrain != 0);
	}
    
	private bool IsValid (float x, float y)
	{
        if (InBounds (Convert.ToInt32(x), Convert.ToInt32(y)))
		{
			byte terrain = currentWorld.TerrainAt (new Vector2 (x, y)).ID;
			return (terrain != 0xff);
		}
		return false;
	}
    
	public bool InBounds (int x, int y)
	{
		int width = currentWorld.Tiles.GetLength (0);
		int height = currentWorld.Tiles.GetLength (1);
		return (x < width && x >= 0 && y < height && y >= 0);
	}
    
	public void Load (string fileName)
	{
		editor.Reset (sceneFactory.LoadScenario (fileName));
		currentFileName = fileName;
	}
    
	public void Save (string fileName)
	{
		sceneFactory.SaveScenario (fileName);
		currentFileName = fileName;
	}
    
	public void DisplayLoadPanel ()
	{
		//Add a list of all saves.
		fileBrowser.Activate (GetFileNames (), this);
	}
    
	public void DisplaySavePanel ()
	{
		saveDialog.Activate (currentFileName, this);
	}
    
	private static List<string> GetFileNames ()
	{
		string path = "Assets" + Path.DirectorySeparatorChar + "DataAccess" + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "gameSaves";
        
		List<string> files = new List<string> ();
        
		DirectoryInfo dirInfo = new DirectoryInfo (path);
		DirectoryInfo[] infos = dirInfo.GetDirectories ();
        
		foreach (DirectoryInfo directory in infos)
		{
			files.Add (directory.Name);
		}
        
		return files;
	}
    
    public void TestValidation() {
        if (ValidateScenario()) {
            //Debug.Log("Valid");
            GameObject.Destroy(MapButton.GetComponent<EventTrigger>());
            var color = MapButton.GetComponent<Text>().color;
            MapButton.GetComponent<Text>().color = new Color(color.r, color.g, color.b, .5f);
            color = LoadButton.GetComponent<Image>().color;
            LoadButton.GetComponent<Image>().color = new Color(color.r, color.g, color.b, .5f);
            LoadButton.GetComponent<Button>().enabled = false;
        } else {
            //Debug.Log("Not Valid");
        }
    }


    public bool ValidateScenario() {
        /**var world = World.Instance;
        HashSet<GameObject> units = world.GameObjects;
        foreach (GameObject unit in units) {
            currentWorld = world;
            int x = (int)unit.transform.localPosition.x;
            int y = world.Tiles.GetLength(1) - (int)unit.transform.localPosition.y;
            switch (unit.tag) {
                case "Surface":
                case "Subsurface":
                    if (!IsValid(x, y) || IsLand(x, y)) {
                        currentWorld = World.Instance;
                        return false;
                    }
                    break;
                case "Marine":
                    if (!IsValid(x, y) || !IsLand(x, y)) {
                        currentWorld = World.Instance;
                        return false;
                    }
                    break;
                case "Air":
                    if (!IsValid(x, y)) {
                        currentWorld = World.Instance;
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }**/
        return true;
    }

    void OnMouseUp() {
        var gridPos = mapCamera.ScreenToWorldPoint(Input.mousePosition);
        // Record the grid position of where the mouse was released
        //Debug.Log("OnMouseUp at: [" + gridPos.x + "," + gridPos.y + "]");

        if (UIMenuController.IsActiveMenu(UnitMenu)) {
            Point point = new Point((int)gridPos.x, (int)gridPos.y);
            switch (selectedTool) {
                case Tools.MOVE:
                    if (selectedGO != null) {
                        selectedGO.transform.localPosition = new Vector3(point.x, point.y, 0);
                        if( checkForCarrier( selectedGO ) )
                            return;
                        switch (selectedGO.tag) {
                            case "Surface":
                            case "Subsurface":
                                if (!IsValid(point.x, point.y) || IsLand(point.x, point.y)) {
                                    selectedGO.transform.localPosition = origionalPosition;
                                }
                                break;
                            case "Marine":
                                if (!IsValid(point.x, point.y) || !IsLand(point.x, point.y)) {
                                    selectedGO.transform.localPosition = origionalPosition;
                                }
                                break;
                            case "Air":
                                if (!IsValid(point.x, point.y)) {
                                    selectedGO.transform.localPosition = origionalPosition;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            selectedGO = null;
        }
    }
}
