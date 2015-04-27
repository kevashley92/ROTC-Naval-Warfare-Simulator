/*****
 * 
 * Name: IdentityController
 * 
 * Date Created: 2015-01-31
 * 
 * Original Team: Gameplay
 * 
 * This class will store data relevant to a unit's identity.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * D.Durand  	2015-01-31  Creation
 * M.Schumacher	2015-02-13	Fixed bugs and updated to work with guid system. 
 * B. Croft		2015-02-14	Added SetValues method
 * T. Brennan	2015-02-17	Refactored and added setters for MyName,
 * 							MyType, and Symbol
 * D.Durand  	2015-02-23  Added a public member Team
 * B. Croft		2015-03-05	Changed Guid to type String
 * T. Brennan	2015-03-16	Added GetTeam, SetTeam, Changed Team to TeamNumber
 * T. Brennan	2015-03-17	Added visibility and related getter and setter
 * J. Woods     2015-03-13  Added the GetValues method for DataIntegration.
 * D.Durand     2015-03-20  Updated GetValues and SetValues
 * A. Smith     2015-04-11  Added isObjective attribute and getters/setters
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.Maps;

[System.Serializable]
public class IdentityController : Controller {

    //sticking this at the top to not touch anything
    private static Dictionary<string, int> unitCounts;
    private int count = -1;
    private bool thisbooleanshouldntexist = false;
    private string lastName;

	/**
	 * The unique name of this unit.
	 */
	public LocalizedItem MyName;

	/**
	 * The type of this object, as in type of ship or other gameobject
	 */
	public string MyType;

	/**
	 * TODO Define this better
	 * Potentially the file path of the image related to the obj
	 */
	public string Symbol;
     
	/**
	 * The Globally Unique Identifier for this object
	 */
	public string MyGuid = null;

	/**
	 * An itentifier that marks this unit as being on a specific team
	 */
	public int TeamNumber = -1;

	/**
	 * The visibility information about this unit
	 */
	public VisibilityType Visibility;

	/**
	 * 
	 */
	public string Map = null;

    public bool init = true;

	public float startTime = Time.time;
	public float timeNow = Time.time;
	private bool badPos = false;

    /**
     *  Is the unit a mission objective
     */
    public bool IsObjective = false;
	/**
	 * Initializes the Guid and sets initial visibility.
	 */
	public void StartIdent(){
        if (init)
        {
            init = false;
            if (gameObject.networkView.viewID == NetworkViewID.unassigned)
            {
                gameObject.networkView.viewID = Network.AllocateViewID();
            }
            //Debug.Log("MyGuid Before: " + MyGuid);
            if (MyGuid == null || "guid_string".Equals(MyGuid) || "".Equals(MyGuid))
            {
                MyGuid = gameObject.networkView.viewID.ToString();
            }
            //Debug.Log("MyGuid After: " + MyGuid);
            //Debug.Log(MyGuid + "");
            GuidList.AddObject(MyGuid, this.gameObject);
            Visibility = VisibilityType.Normal;
            World.AddUnitToWorld(gameObject);
            startTime = Time.time;
            timeNow = Time.time;
        }
	}

	/** 
	 * Set up a new controller
	 * 
	 * @param values
	 * 		A json friendly dictionary containing the parameters for
	 * 		this controller
	 */
	public override void SetValues(IDictionary values) {
        if (!"guid_string".Equals((string)values["guid"]))
        {
            MyGuid = (string)values["guid"];
        }
		this.StartIdent();
		MyName = new LocalizedItem((string)values["file"], (string)values["name"]);
        lastName = MyName.label;
		MyType = (string)values["type"];
		TeamNumber = Convert.ToInt32(values["team"]);
        if(values.Contains("count")){
            if(null == unitCounts)
                unitCounts = new Dictionary<string, int>();
            if(!unitCounts.ContainsKey((string)values["name"]+TeamNumber))
                unitCounts.Add((string)values["name"]+TeamNumber, Convert.ToInt32(values["count"]));
            else if(Convert.ToInt32(values["count"]) > unitCounts[(string)values["name"]+TeamNumber])
                unitCounts[(string)values["name"]+TeamNumber] = Convert.ToInt32(values["count"]);
            count = Convert.ToInt32(values["count"]);

        }

		TeamNumber = Convert.ToInt32(values["team"]);
		
        //GuidList.AddObject(MyGuid, this.gameObject);
		Visibility = (VisibilityType) Convert.ToInt32(values ["visibility"]);
		Map = (string)values["map"];
        IsObjective = Convert.ToBoolean (values ["objective"]);
	}

    public void Awake(){
        if(null == unitCounts)
            unitCounts = new Dictionary<string, int>();
    }

    void Update()
    {
        
        if(null == unitCounts)
            unitCounts = new Dictionary<string, int>();
        if((null != lastName && null != MyName && !MyName.label.Equals(lastName)) || (-1 != TeamNumber && !thisbooleanshouldntexist)){
            if(null != lastName && null != MyName)
                lastName = MyName.label;
            if(null == unitCounts)
                unitCounts = new Dictionary<string, int>();
            if(null != MyName){
                thisbooleanshouldntexist = true;
                if(!unitCounts.ContainsKey(MyName.label+TeamNumber)){
                    unitCounts.Add(MyName.label+TeamNumber, 1);
                }
                count = unitCounts[MyName.label+TeamNumber]++;
            }
        }
        StartIdent();
        World.SetTeamColor(gameObject);
        World.UpdateObjectiveSprite(gameObject);

        if (badPos) {
            timeNow = Time.time;
        }
        if (gameObject.transform.localPosition.y == 35 
            && gameObject.transform.localPosition.x == 0 
            && gameObject.transform.localPosition.z == 0 
            && MyName == null) {
            badPos = true;
        } else {
            badPos = false;
            startTime = Time.time;
        }
        float number = Convert.ToSingle (Math.Floor((float) GuidList.GetAllObjects().Count));;
        if (badPos  && ((timeNow - startTime) > (number))) {

            GameObject.Destroy (gameObject);
        }

    }

	/**
     *Gets the values of the controller.
     *
     * @return
     *   A dictionary containing the json friendly values of the controller.
     */
	public override IDictionary GetValues() {
		Dictionary<string, object> toReturn = new Dictionary<string, System.Object>();
		toReturn["name"] = MyName.label;
        toReturn["file"] = MyName.file;
        toReturn["count"] = count;
		toReturn["type"] = MyType;
		toReturn ["team"] = TeamNumber;
		toReturn ["guid"] = MyGuid;
		toReturn ["visibility"] = (int)Visibility;
		toReturn ["map"] = Map;
        toReturn ["objective"] = IsObjective;
		return toReturn;
	}
	public override void SetValuesNavy(IDictionary values) {
		SetValues(values);
	}
	public override void SetValuesMarines(IDictionary values) {
		SetValues(values);
	}

	/** Sets an individual value after admin change
	 */
	public override void SetValue(string paramName,object value) {
		// Possible behavior is setting object name.  paramName is "name" and value is desired name
		if(paramName.ToLower().Equals("name")) {
            if(value is string)
                MyName.label = (string)value;
            else
			    SetName((LocalizedItem) value);
		}
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowNameChangeEvent(string value){
		object[] arguments = new object[4];
		arguments[0] = MyGuid;
		arguments[1] = "IdentityController";
		arguments[2] = "name";
		arguments[3] = value;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}

	/**
	 * Sets additional values in the identidy controller.
	 * 
	 * TODO Figure out why this AND SetValues exist and are different.
	 * 
	 * @param name
	 * 		The new name of the unit
	 * @param type
	 * 		The new type of the unit
	 * @param symbol
	 * 		The new symbol for the unit
	 */
	public void SetIdentity (LocalizedItem name, string type, string symbol) {

		this.MyName = name;
		this.MyType = type;
		this.Symbol = symbol;

	}


	/**
	 * Gets the unique name of the object.
	 * 
	 * @return
	 * 		The unique name of the object/
	 */
	public string GetName() {
		return MyName.label;
	}

    public string GetLocalizedName(){

        return LanguageManager.instance.getString(MyName);

    }

    public string GetFullName(){
        string designationInput = MyName.label.Replace("_name", "")+"_designation";
        string designation = LanguageManager.instance.getString(MyName.file, designationInput);
        return LanguageManager.instance.getString(MyName)+" ("+ (designation.Equals(designationInput) ? "" : designation+"-") +count+")";
    }


	/**
	 * Sets the name of the object.
	 * 
	 * @param newName
	 * 		The new unique name for the unit
	 */
	public void SetName(LocalizedItem newName) {

		MyName = newName;

	}


	/**
	 * Gets the type of the object.
	 * 
	 * @return
	 * 		The type of the object
	 */
	public string GetObjectType() {

		return MyType;

	}


	/**
	 * Sets the type of the object
	 * 
	 * @param newType
	 * 		The new type of the object
	 */
	public void SetObjectType(string newType) {

		MyType = newType;

	}

	/**
	 * Gets the symbol associated with this object
	 * 
	 * @return
	 * 		The string representing the symbol associated with this
	 * 		object
	 */
	public string GetSymbol() {

		return Symbol;

	}


	/**
	 * Sets the string representing the symbol assoicated with this obejct
	 * 
	 * @param newSymbol
	 * 		The string representing the symbol assoicated with this
	 * 		object
	 */
	public void SetSymbol(string newSymbol) {

		Symbol = newSymbol;

	}


	/**
	 * Gets the Globally Unique IDentifier of this object.
	 * 
	 * @return
	 * 		The Guid of this object.
	 */
	public string GetGuid(){

		return MyGuid;

	}


	/**
	 * Gets the team this object is associated with.
	 */
	public int GetTeam() {

		return TeamNumber;

	}


	/**
	 * Sets the team this object is associated with
	 */
	public void SetTeam(int team) {

		if (Team.HasTeam(team)) {

			TeamNumber = team;

		}
       

	}


	/**
	 * Gets the visibility of this object
	 * 
	 * @return
	 * 		The visibility of the object
	 */
	public VisibilityType GetVisibility() {

		return Visibility;

	}


	/**
	 * Sets the visibility of this object
	 * 
	 * @param visibility
	 * 		The new visibility of the obejct
	 */
	public void SetVisibility(VisibilityType visibility) {

		this.Visibility = visibility;

	}

    /**
     * Sets if unit is an objective
     */
    public void SetIsObjective(bool value){
        this.IsObjective = value;
    }

    /**
     * Gets a boolean for if the unit is an objective
     */
    public bool GetIsObjective(){
        return IsObjective;
    }
}

