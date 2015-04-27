using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UnitManager : MonoBehaviour
{

	public ComboBox TeamDropdown;
	
	Toggle AirToggle;
	Toggle SurfaceToggle;
	Toggle SubSurfaceToggle;
	Toggle MarineToggle;
	
	public static string AIR = "Air";
	public static string SURFACE = "Surface";
	public static string SUBSURFACE = "SubSurface";
	public static string MARINE = "Marine";
	
	List<LocalizedItem> nameList;
	
	DAOFactory Factory;
	ObjectFactory objectFactory;
	
	ComboBox units;
	
	//GUI components for displaying unit info
	GameObject UnitName;
	GameObject RangeValue;
	GameObject HealthValue;
	GameObject AirSensorValue;
	GameObject SurfaceSensorValue;
	GameObject SubSensorValue;

	GameObject WeaponCountValue;
	GameObject ContainerValue;
	GameObject ContainerLabel;



	//Currently selected unit gameobject
	GameObject currentObject;
	Dictionary<string, System.Object> currentObjectDictionary;
	public bool realObjectSelected = false;

	public bool canLand = false;

    public Button StoreButton;
    public Button EditButton;
	
	
	// Use this for initialization
	void Start ()
	{

		//Fill team dropdown
		TeamDropdown.ClearItems ();
		TeamDropdown.AddItems (teamListToStringArray (Team.GetTeams ()));

		//Get instances of toggles
		SurfaceToggle = transform.FindChild ("SurfaceToggle").gameObject.GetComponent<Toggle> ();
		AirToggle = transform.FindChild ("AirToggle").gameObject.GetComponent<Toggle> ();
		SubSurfaceToggle = transform.FindChild ("SubSurfaceToggle").gameObject.GetComponent<Toggle> ();
		MarineToggle = transform.FindChild ("MarineToggle").gameObject.GetComponent<Toggle> ();
		
		Factory = DAOFactory.GetFactory ();
		nameList = new List<LocalizedItem> ();
		
		
		//Get instances of Unit info labels
		RangeValue = GameObject.Find ("RangeValue");
		HealthValue = GameObject.Find ("HealthValue");
		WeaponCountValue = GameObject.Find ("WeaponCountValue");
		UnitName = transform.parent.Find ("UnitInfo").Find ("UnitName").gameObject;

		AirSensorValue = GameObject.Find ("AirSensorValue");
		SurfaceSensorValue = GameObject.Find ("SurfaceSensorValue");
		SubSensorValue = GameObject.Find ("SubSensorValue");
		ContainerValue = GameObject.Find ("ContainerValue");
		ContainerLabel = GameObject.Find ("ContainerLabel");
		
		units = GameObject.Find ("UnitComboBox").GetComponent<ComboBox> ();
		units.gameObject.SetActive (false);
		transform.parent.Find ("UnitInfo").gameObject.SetActive (false);
		
		objectFactory = new ObjectFactory ();
		
		units.OnSelectionChanged += (int index) => {
			updateUnitInfo (nameList [index].label); 
		};
		
	}


	public void OnEnable ()
	{
		//Debug.Log ("Team dropdown refreshed");
		TeamController.RefreshTeams (TeamDropdown);
		if (currentObject != null)
		{
			RefreshUI ();
		}

        if (!NetworkManager.NetworkInstance.IsServer)
        {
            EditButton.interactable = false;
            StoreButton.interactable = false;
        }

	}
	
	public void SetAirLabels ()
	{
        if(null == units)
            units = GameObject.Find ("UnitComboBox").GetComponent<ComboBox> ();
        if(null == units)
            return;
		units.gameObject.SetActive (true);
		
		nameList = Factory.GetAirDAO ().GetLocalizedNames ();
		units.ClearItems ();
		units.AddItems (nameList.ToArray ());
		units.SelectedIndex = 0;
		setSelectedToggle (AIR);
		RefreshUI();
	}
	
	public void SetSurfaceLabels ()
	{
        if(null == units)
            units = GameObject.Find ("UnitComboBox").GetComponent<ComboBox> ();
        if(null == units)
            return;
        units.gameObject.SetActive (true);
		nameList = Factory.GetSurfaceDAO ().GetLocalizedNames ();
		
		units.ClearItems ();
		units.AddItems (nameList.ToArray ());
		units.SelectedIndex = 0;
		setSelectedToggle (SURFACE);
		RefreshUI();
	}
	
	public void SetSubSurfaceLabels ()
	{
        if(null == units)
            units = GameObject.Find ("UnitComboBox").GetComponent<ComboBox> ();
        if(null == units)
            return;
        units.gameObject.SetActive (true);
		nameList = Factory.GetSubSurfaceDAO ().GetLocalizedNames ();
		
		units.ClearItems ();
		units.AddItems (nameList.ToArray ());
		units.SelectedIndex = 0;
		setSelectedToggle (SUBSURFACE);
		RefreshUI();
	}
	
	public void SetMarineLabels ()
	{
        if(null == units)
            units = GameObject.Find ("UnitComboBox").GetComponent<ComboBox> ();
        if(null == units)
            return;
        units.gameObject.SetActive (true);
		nameList = Factory.GetMarineDAO ().GetLocalizedNames ();
		
		units.ClearItems ();
		units.AddItems (nameList.ToArray ());
		units.SelectedIndex = 0;
		setSelectedToggle (MARINE);

	}
	
	public void setSelectedToggle (string unittype)
	{
		//Get the selected item
		string unitName = nameList [units.SelectedIndex].label;
		//updateUnitInfo (unitName, unittype);

	}
	
	private void updateUnitInfo (string unitName)
	{

        if (AirToggle.isOn)
        {
            currentObjectDictionary = objectFactory.LoadAirDict (unitName);
            currentObjectDictionary.Add ("file", "Plane");
        }
        else if (SurfaceToggle.isOn)
        {
			currentObjectDictionary = objectFactory.LoadSurfaceDict (unitName);
            currentObjectDictionary.Add ("file", "Ship");
        }
        else if (SubSurfaceToggle.isOn)
        {
			currentObjectDictionary = objectFactory.LoadSubSurfaceDict (unitName);
            currentObjectDictionary.Add ("file", "Submarine");
        }
        else if (MarineToggle.isOn)
        {
			currentObjectDictionary = objectFactory.LoadMarineDict (unitName);
            currentObjectDictionary.Add ("file", "Marine_Unit");
        }
        else
        {
            return;
        }
		
        RefreshUI ();
		
	}
	/*DUPLICATE WITH EXTRA PARAMETER BECAUSE I HATE EVERYTHING*/
	private void updateUnitInfo (string unitName, string unitType)
	{

        if (unitType.Equals (AIR))
        {
            currentObjectDictionary = objectFactory.LoadAirDict (unitName);
        }
        else if (unitType.Equals (SURFACE))
        {
			currentObjectDictionary = objectFactory.LoadSurfaceDict (unitName);
        }
        else if (unitType.Equals (SUBSURFACE))
        {
			currentObjectDictionary = objectFactory.LoadSubSurfaceDict (unitName);
        }
        else if (unitType.Equals (MARINE))
        {
			currentObjectDictionary = objectFactory.LoadMarineDict (unitName);
        }
        else
        {
            return;
        }
		
        RefreshUI ();
		
	}
	
	public void createUnit ()
	{
		if (currentObject != null)
		{
			currentObject.SetActive (true);
		}
	}
	public GameObject GetCurrentObject ()
	{
		return currentObject;
	}
	public void SetCurrentObject (GameObject unit)
	{
		if (currentObject != null)
		{
			GameObject oldselect = currentObject.transform.FindChild ("Selection Sprite").gameObject;
			oldselect.GetComponent<SpriteRenderer> ().enabled = false;
		}
		currentObject = unit;
		//updateUnitInfo(unit.GetComponent<IdentityController>().GetName ());
		GameObject newselect = currentObject.transform.FindChild ("Selection Sprite").gameObject;
		newselect.GetComponent<SpriteRenderer> ().enabled = true;
	}

	private string[] teamListToStringArray (List<Team> teams)
	{
		string[] names = new string[teams.Count];
		for (int i = 0; i < teams.Count; i++)
		{
			names [i] = teams [i].GetTeamName ();
		}
		return names;
	}

	public string GetSelectedUnit ()
	{
		return nameList [units.SelectedIndex].label;
	}

	public void RefreshUI ()
	{
		if (currentObjectDictionary != null)
		{
			transform.parent.Find ("UnitInfo").gameObject.SetActive (true);
			ContainerLabel.SetActive (false);
			ContainerValue.SetActive (false);


		    UnitName.GetComponent<Text> ().text = LanguageManager.instance.getString(currentObjectDictionary["file"].ToString(), ((Dictionary<string,System.Object>)currentObjectDictionary["Identity"])["name"].ToString());
			HealthValue.GetComponentInChildren<Text> ().text = (((Dictionary<string,System.Object>)currentObjectDictionary["Health"])["currenthealth"]).ToString() + " / " + (((Dictionary<string,System.Object>)currentObjectDictionary["Health"])["defense"]).ToString();
			RangeValue.GetComponentInChildren<Text>().text = (((Dictionary<string,System.Object>)currentObjectDictionary["Mover"])["maxmove"]).ToString();

			if(currentObjectDictionary.ContainsKey("Detector") && ((Dictionary<string,System.Object>)currentObjectDictionary["Detector"]).ContainsKey("radar_air")) 
			{
				AirSensorValue.SetActive(true);
				SurfaceSensorValue.SetActive(true);
				SubSensorValue.SetActive(true);
				AirSensorValue.GetComponentInChildren<Text>().text = ((Dictionary<string,System.Object>)currentObjectDictionary["Detector"])["radar_air"].ToString();
				SurfaceSensorValue.GetComponent<Text> ().text = ((Dictionary<string,System.Object>)currentObjectDictionary["Detector"])["radar_surf"].ToString();
				SubSensorValue.GetComponent<Text> ().text = ((Dictionary<string,System.Object>)currentObjectDictionary["Detector"])["radar_sub"].ToString();
			}
			else
			{
				AirSensorValue.SetActive(false);
				SurfaceSensorValue.SetActive(false);
				SubSensorValue.SetActive(false);
			}

			WeaponCountValue.GetComponent<Text> ().text = ((List<System.Object>)((Dictionary<string,System.Object>)currentObjectDictionary["Attack"])["weapons"]).Count.ToString();


			if (currentObjectDictionary.ContainsKey("Embarker") && Convert.ToBoolean(((Dictionary<string,System.Object>)currentObjectDictionary["Embarker"])["state"]))
			{
				ContainerLabel.SetActive (true);
				ContainerValue.SetActive (true);
				ContainerValue.GetComponent<Text> ().text = ((List<System.Object>)((Dictionary<string,System.Object>)currentObjectDictionary["Container"])["guids"]).Count.ToString();
			}                                                          
		}
	}

	public void OnStore(){
		if(currentObject != null)
		{
			string name = currentObject.GetComponent<IdentityController>().GetName();
			if(currentObject.tag.ToUpper() == "AIR")
				objectFactory.SaveAir( currentObject , name );
			else if(currentObject.tag.ToUpper() == "SURFACE")
				objectFactory.SaveSurface( currentObject , name );
			else if(currentObject.tag.ToUpper() == "SUBSURFACE")
				objectFactory.SaveSubSurface( currentObject , name );
			else if(currentObject.tag.ToUpper() == "MARINE")
				objectFactory.SaveMarine( currentObject , name );
		}
		if (AirToggle.isOn) {
			SetAirLabels ();
		} else if (SurfaceToggle.isOn) {
			SetSurfaceLabels();
		} else if (SubSurfaceToggle.isOn) {
			SetSubSurfaceLabels();
		} else if (MarineToggle.isOn) {
			SetMarineLabels();
		}
	}
}
