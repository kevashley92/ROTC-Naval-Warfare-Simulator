/*****
 * 
 * Name: ObjectFactory
 * 
 * Date Created: 2015-02-13
 * 
 * Original Team: Gameplay
 * 
 * This class will generate an object given a full set of parameters.
 * What parameters are searched for are determined by the parameters in the defaults.json file.
 * Each parameter in the file should have the parameters "value" and "controller"
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * B.Croft		2015-02-13	Initial Commit
 * T. Brennan	2015-02-17	Refactored
 * S. Lang		2015-03-15	Make sure object exists before calling SetValues
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using System;
using System.Threading;

public class ObjectFactory {

    private DAOFactory factory;

	/**DAO that access prototype surface assets*/
	private SurfaceDAO surfaceDAO;
	/**DAO that accesses prototype subsurface assets*/
	private SubSurfaceDAO subsurfaceDAO;
	/**DAO that access prototype air assets*/
	private AirDAO airDAO;
	/**DAO that access prototype marine assets*/
	private MarineDAO marineDAO;
	/**DAO that access prototype weapon assets*/
	private WeaponDAO weaponDAO;
	/**DAO that accesses surface assets for a scenario*/
	private SurfaceDAO surfaceScenarioDAO;
	/**DAO that accesses subsurface assets for a scenario*/
	private SubSurfaceDAO subsurfaceScenarioDAO;
	/**DAO that accesses air assets for a scenario*/
	private AirDAO airScenarioDAO;
	/**DAO that accesses marine assets for a scenario*/
	private MarineDAO marineScenarioDAO;
	/**DAO that access environmentVariables for a scenario*/
	private EnvironmentVariableDAO environmentVariableDAO;

    public ObjectFactory()
    {
        factory = DAOFactory.GetFactory();
		surfaceDAO = factory.GetSurfaceDAO();
		subsurfaceDAO = factory.GetSubSurfaceDAO();
		airDAO = factory.GetAirDAO();
		marineDAO = factory.GetMarineDAO();
		weaponDAO = factory.GetWeaponDAO();
		environmentVariableDAO = factory.GetEvironmentVariableDAO();
    }

	/**
	 * Optional constructor which takes the scenario for which
	 * this ObjectFactory will be used as a parameter
	 *
	 */
	public ObjectFactory(string scenarioName)
    {
		factory = DAOFactory.GetFactory();
		surfaceDAO = factory.GetSurfaceDAO();
		subsurfaceDAO = factory.GetSubSurfaceDAO();
		airDAO = factory.GetAirDAO();
		marineDAO = factory.GetMarineDAO();
		weaponDAO = factory.GetWeaponDAO();
		surfaceScenarioDAO = factory.GetSurfaceScenarioDAO(scenarioName);
		subsurfaceScenarioDAO = factory.GetSubSurfaceScenarioDAO(scenarioName);
		airScenarioDAO = factory.GetAirScenarioDAO(scenarioName);
		marineScenarioDAO = factory.GetMarineScenarioDAO(scenarioName);
		environmentVariableDAO = factory.GetEvironmentVariableScenarioDAO(scenarioName);
	}

	public Dictionary<string, System.Object> LoadSurfaceDict(string name) {
		SurfaceDAO surface = factory.GetSurfaceDAO();
		return surface.LoadOne(name);
	}

	public Dictionary<string, System.Object> LoadAirDict(string name) {
		AirDAO air = factory.GetAirDAO();
		return air.LoadOne(name);
	}

	public Dictionary<string, System.Object> LoadSubSurfaceDict(string name) {
		SubSurfaceDAO subSurface = factory.GetSubSurfaceDAO();
		return subSurface.LoadOne(name);
	}

	public Dictionary<string, System.Object> LoadMarineDict(string name) {
		MarineDAO marine = factory.GetMarineDAO();
		return marine.LoadOne(name);
	}

    public GameObject LoadSurface(string name)
    {
		surfaceDAO = factory.GetSurfaceDAO();
        GameObject freshSurface = LoadFromDAO(surfaceDAO, name, "Surface");
        return freshSurface;
    }

    public void SaveSurface(GameObject toSave, string name)
    {
        SaveWithDAO(surfaceDAO, toSave, name);
        ////Debug.LogError("Thread start: " + Time.realtimeSinceStartup);
        //Thread t = new Thread(new ThreadStart(surfaceDAO.SaveAll));
        //t.Start();
        ////Debug.LogError("Back to main thread: " + Time.realtimeSinceStartup);
		surfaceDAO.SaveAll();
    }

    public GameObject LoadAir(string name)
    {
		airDAO = factory.GetAirDAO();
        GameObject freshAir = LoadFromDAO(airDAO, name, "Air");
        return freshAir;
    }

    public void SaveAir(GameObject toSave, string name)
    {
        SaveWithDAO(airDAO, toSave, name);
		airDAO.SaveAll();

    }

    public GameObject LoadSubSurface(string name)
    {
		subsurfaceDAO = factory.GetSubSurfaceDAO();
        GameObject freshSubSurface = LoadFromDAO(subsurfaceDAO, name, "Subsurface");
        return freshSubSurface;
    }

    public void SaveSubSurface(GameObject toSave, string name)
    {
        SaveWithDAO(subsurfaceDAO, toSave, name);
        //Thread t = new Thread(new ThreadStart(subsurfaceDAO.SaveAll));
		subsurfaceDAO.SaveAll();

        //t.Start();
    }

    public GameObject LoadWeapon(string name)
    {
		weaponDAO = factory.GetWeaponDAO();
        GameObject freshWeapon = LoadFromDAO(weaponDAO, name, "Weapon");
        return freshWeapon;
    }

    public void SaveWeapon(GameObject toSave, string name)
    {
        SaveWithDAO(weaponDAO, toSave, name);

    }

    public GameObject LoadMarine(string name)
    {
		marineDAO = factory.GetMarineDAO ();
        GameObject freshMarine = LoadFromDAO(marineDAO, name, "Marine");
        return freshMarine;
    }

    public void SaveMarine(GameObject toSave, string name)
    {
        SaveWithDAO(marineDAO, toSave, name);
		marineDAO.SaveAll();

    }

    public void BuildFilePath()
    {

        surfaceScenarioDAO.BuildPath();
        subsurfaceScenarioDAO.BuildPath();
        airScenarioDAO.BuildPath();
        marineScenarioDAO.BuildPath();
		environmentVariableDAO.BuildPath();
        
    }

    private GameObject LoadFromDAO(iGameObjectDAO dao, string name, string tag)
    {
        IDictionary toLoad = dao.LoadOne(name);
        GameObject fresh = this.CreateObject(toLoad, tag);
        fresh.tag = tag;
        return fresh;
    }

    private void SaveWithDAO(iGameObjectDAO dao, GameObject go, string name)
    {
        IDictionary toSave = DeconstructObject(go, dao);

        dao.AddToSaveList(name, (Dictionary<string, System.Object>)toSave);
    }

    private void SaveWithDAOMultiThreaded(iGameObjectDAO dao, string key, string name)
    {
        IDictionary toSave = DeconstructObjectMultiThreaded(key, dao);

        dao.AddToSaveList(name, ((Dictionary<string, System.Object>)toSave));
    }

    public List<GameObject> LoadAllSurface()
    {
        List<string> names = surfaceScenarioDAO.GetAllNames();
        List<GameObject> list = new List<GameObject>();

        foreach(string name in names)
        {
            list.Add(LoadFromDAO(surfaceScenarioDAO, name, "Surface"));
        }

        return list;
    }

    public void SaveSurfaceToScenario(GameObject toSave, string name)
    {
        SaveWithDAO(surfaceScenarioDAO, toSave, name);
    }

    public void SaveSurfaceToScenarioMultiThreaded(string toSave, string name) {
        SaveWithDAOMultiThreaded(surfaceScenarioDAO, toSave, name);   
    }

    public List<GameObject> LoadAllAir()
    {
        List<string> names = airScenarioDAO.GetAllNames();
        List<GameObject> list = new List<GameObject>();

        foreach (string name in names)
        {
            list.Add(LoadFromDAO(airScenarioDAO, name, "Air"));
        }

        return list;
    }

    public void SaveAirToScenario(GameObject toSave, string name)
    {
        SaveWithDAO(airScenarioDAO, toSave, name);
    }

    public void SaveAirToScenarioMultiThreaded(string toSave, string name)
    {
        SaveWithDAOMultiThreaded(airScenarioDAO, toSave, name);
    }

    public List<GameObject> LoadAllSubSurface()
    {
        List<string> names = subsurfaceScenarioDAO.GetAllNames();
        List<GameObject> list = new List<GameObject>();

        foreach (string name in names)
        {
            list.Add(LoadFromDAO(subsurfaceScenarioDAO, name, "Subsurface"));
        }

        return list;
    }

    public void SaveSubSurfaceToScenario(GameObject toSave, string name)
    {
        SaveWithDAO(subsurfaceScenarioDAO, toSave, name);
    }

    public void SaveSubSurfaceToScenarioMultiThreaded(string toSave, string name)
    {
        SaveWithDAOMultiThreaded(subsurfaceScenarioDAO, toSave, name);
    }

    public List<GameObject> LoadAllMarines()
    {
        List<string> names = marineScenarioDAO.GetAllNames();
        List<GameObject> list = new List<GameObject>();

        foreach (string name in names)
        {
            list.Add(LoadFromDAO(marineScenarioDAO, name, "Marine"));
        }

        return list;
    }

    public void SaveMarineToScenario(GameObject toSave, string name)
    {
		SaveWithDAO(marineScenarioDAO, toSave, name);
    }

    public void SaveMarineToScenarioMultiThreaded(string toSave, string name)
    {
        SaveWithDAOMultiThreaded(marineScenarioDAO, toSave, name);
    }

	/**
	 * After everything has been added to the dictionaries for the scenario,
	 * we make a consolidated call the write all these changes to file
	 *
	 */
	public void SaveToFile()
	{
        //Thread t = new Thread(new ThreadStart(surfaceScenarioDAO.SaveAll));
        //t.Start();
        surfaceScenarioDAO.SaveAll();
		subsurfaceScenarioDAO.SaveAll();
		airScenarioDAO.SaveAll();
		marineScenarioDAO.SaveAll();


	}

	public void LoadEnvironmentVariables() {
		

		Dictionary<string, System.Object> variables = environmentVariableDAO.LoadOne();
		
		Dictionary<string, System.Object> globalSettings = (Dictionary<string, System.Object>)variables["GlobalSettings"];
		Dictionary<string, System.Object> turnManager = (Dictionary<string, System.Object>)variables["TurnManager"];
		Dictionary<string, System.Object> weather = (Dictionary<string, System.Object>)variables["Weather"];
		List<System.Object> teams = (List<System.Object>)variables["Teams"];
		Dictionary<string, System.Object> objectives = (Dictionary<string, System.Object>)variables["Objective"];

		GlobalSettings.SetHealthThresholdForHalfMovement(Convert.ToSingle(globalSettings["half_movement"]));
		GlobalSettings.SetHealthThresholdForNoAircraftLaunching(Convert.ToSingle(globalSettings["no_aircraft"]));
		GlobalSettings.SetHealthThresholdForNoDetectors(Convert.ToSingle(globalSettings["no_detector"]));
		GlobalSettings.SetHealthThresholdForNoMovement(Convert.ToSingle(globalSettings["no_movement"]));
		GlobalSettings.SetHealthThresholdForNoWeapons(Convert.ToSingle(globalSettings["no_weapons"]));
		GlobalSettings.SetRetaliationPercentChance(Convert.ToSingle(globalSettings["retaliation_p"]));
		GlobalSettings.SetRetaliationDamage(Convert.ToSingle(globalSettings["retaliation_d"]));
		GlobalSettings.SetCurrentWeatherIndex(Convert.ToInt32(globalSettings["w_index"]));
		
		TurnManager.TurnCount = Convert.ToInt32(turnManager["count"]);
		
		foreach(string key in weather.Keys) {
			//Debug.Log(key);
			Dictionary<string, System.Object> singleItem = (Dictionary<string, System.Object>)weather[key];
			
			int index = Convert.ToInt32(singleItem["index"]);
			
			string type = key;

			float movement;
			float visibility;
			try{
				movement = Convert.ToSingle(singleItem["movement"]);
			}
			catch(InvalidCastException e){

				//Debug.Log(singleItem["movement"]);
				int moveInt = Convert.ToInt32(singleItem["movement"]);
				movement = (float)moveInt;
			}

			try{
				visibility = Convert.ToSingle(singleItem["visibility"]);
			}
			catch(InvalidCastException e) {

				int visInt = Convert.ToInt32(singleItem["visibility"]);
				visibility = (float)visInt;
			}
			
			//if(Weather.WeatherTypes == null) {
				Weather.WeatherTypes = new Dictionary<int, Weather>();
			//}
			
			Weather.WeatherTypes.Add(index, new Weather(type, movement, visibility));

		}

        Team.ClearTeams();
		foreach(System.Object team in teams){

			Dictionary<string, System.Object> singleItem = (Dictionary<string, System.Object>)team;

            try
            {
                Team toAdd = new Team((string)singleItem["team_name"],Convert.ToInt32(singleItem["index"]));
                if (!toAdd.GetTeamName().Equals("default"))
                {
                    Color teamColor = new Color((float)Convert.ToDecimal(singleItem["red"]), 
                                                (float)Convert.ToDecimal(singleItem["green"]), 
                                                (float)Convert.ToDecimal(singleItem["blue"]));
                    toAdd.SetTeamColor(teamColor);
                    Team.addTeamAtIndex(toAdd, Convert.ToInt32(singleItem["index"]));
                }
            }
            catch (ArgumentException){}
		}

		Objective.GetInstance().SetObjective((string)objectives["text"]);

        List<System.Object> toTranslate = (List<System.Object>)objectives["ind"];
        List<string> toSet = new List<string>();

        foreach (System.Object toAdd in toTranslate)
        {
			Dictionary<string, System.Object> item = (Dictionary<string, System.Object>)toAdd;
            toSet.Add((string)item["value"]);
        }


		Objective.GetInstance().SetIndividualObjectives(toSet);
        ObjectiveController.GetInstance().UpdateDropDown();
		
	}

	public void SaveEnvironmentVariables() {
		
		Dictionary<string, System.Object> toSave = environmentVariableDAO.LoadDefault();

		//below block will set all environment variables that go to globalSettings
		Dictionary<string, System.Object> globalSettings = (Dictionary<string, System.Object>)toSave["GlobalSettings"];
		globalSettings["half_movement"] = GlobalSettings.GetHealthThresholdForHalfMovement();
		globalSettings["no_aircraft"] = GlobalSettings.GetHealthThresholdForNoAircraftLaunching();
		globalSettings["no_detector"] = GlobalSettings.GetHealthThresholdForNoDetectors();
		globalSettings["no_movement"] = GlobalSettings.GetHealthThresholdForNoMovement();
		globalSettings["no_weapons"] = GlobalSettings.GetHealthThresholdForNoWeapons();
		globalSettings["retaliation_p"] = GlobalSettings.GetRetaliationPercentChance();
		globalSettings["retaliation_d"] = GlobalSettings.GetRetaliationDamage();
		globalSettings["w_index"] = GlobalSettings.GetCurrentWeatherIndex();
		
		Dictionary<string, System.Object> turnManager = (Dictionary<string, System.Object>)toSave["TurnManager"];
		//records the current turn
		turnManager["count"] = TurnManager.TurnCount;
		
		Dictionary<string, System.Object> weather = new Dictionary<string, System.Object>();

		//if there were some weatherTypes to be saved, than save them appropriately
		if(Weather.WeatherTypes != null) {
			foreach(int key in Weather.WeatherTypes.Keys) {
				
				string newKey = Weather.WeatherTypes[key].WeatherType;
				
				weather[newKey] = new Dictionary<string, System.Object>();
				
				((Dictionary<string, System.Object>)weather[newKey])["index"] = key;
				((Dictionary<string, System.Object>)weather[newKey])["movement"] = Weather.WeatherTypes[key].MovementModifier;
				((Dictionary<string, System.Object>)weather[newKey])["visibility"] = Weather.WeatherTypes[key].VisionModifier;
				
				
			}
		}

		//save team data in the form of a dictionary who's name is the team name and a
		//parameter called index which holds the key in the Teams list it corresponds to.
		//This is because gameplay stores teams in a <int, Teams> dictionary.
		List<System.Object> teamData = (List<System.Object>)toSave["Teams"];

		Dictionary<int, Team> inGameTeams = Team.GetAllTeams();
		
		if(inGameTeams != null) {

			foreach(int key in inGameTeams.Keys){
				Team currentTeam = ((Team)inGameTeams[key]);
                string teamName = currentTeam.GetTeamName();
                Color currentTeamColor = currentTeam.GetTeamColor();

				Dictionary<string, System.Object> putTeam = new Dictionary<string, System.Object>();

				putTeam.Add("index", key);
				putTeam.Add ("team_name", teamName);
                putTeam.Add("red", currentTeamColor.r);
                putTeam.Add("green", currentTeamColor.g);
                putTeam.Add("blue", currentTeamColor.b);

//				if(teamData.ContainsKey(key)){
//					teamData.Remove(key);
				teamData.Add(putTeam);
//				}

			}
		}

		Dictionary<string, System.Object> objective = (Dictionary<string, System.Object>)toSave["Objective"];

		objective["text"] = Objective.GetInstance().GetObjective();

		List<System.Object> individualsObjects = (List<System.Object>)objective["ind"];
		individualsObjects.Clear();

		foreach (string ob in Objective.GetInstance().GetIndividualObjectives())
        {
			Dictionary<string, System.Object> item = new Dictionary<string, object>();
			item.Add("value", ob);
			individualsObjects.Add(item);
        }

		objective["ind"] = individualsObjects;

		toSave["Teams"] = teamData;
		toSave["Weather"] = weather;
		toSave["TurnManager"] = turnManager;
		toSave["GlobalSettings"] = globalSettings;
		toSave["Objective"] = objective;
		
		environmentVariableDAO.SaveOne(toSave);
	}

    public void SaveEnvironmentVariablesMultiThreaded() {
        IDictionary environmentalSettings = EnvironmentSettingsShell.EnvironmentalSettings;

        Dictionary<string, System.Object> toSave = environmentVariableDAO.LoadDefault();
        toSave["GlobalSettings"] = environmentalSettings["GlobalSettings"];
        toSave["TurnManager"] = environmentalSettings["TurnManager"];
        toSave["Weather"] = environmentalSettings["Weather"];
        toSave["Teams"] = environmentalSettings["Teams"];
        toSave["Objective"] = environmentalSettings["Objective"];

        environmentVariableDAO.SaveOne(toSave);
    }


    /**
    * TODO Fill this in
    * 
    * @param gO
    * 		TODO Fill this in
    * @param controllerName
    * 		TODO Fill this in
    * @param value
    * 		TODO Fill this in
    */
    private void PassToController(GameObject gO, string controllerName, IDictionary value)
    {

		// create new component if not existant
		if (gO.GetComponent(controllerName) == null) {
			//Debug.Log("ObjectFactory: controller name " + controllerName + " does not exist, creating now.");
			gO.AddComponent(controllerName);
			if (gO.GetComponent(controllerName) == null) {
				//Debug.Log("ObjectFactory: Controller with name " + controllerName + " failed to be created!!! Null Reference Exception Immenent!!!!");
			}

		}

		//Debug.Log("ObjectFactory: GetComponent("+controllerName+"): " + (gO.GetComponent(controllerName) as Controller).ToString());
		// call the subclass's SetValues() method to handle from here
		if(!value.Contains ("file"))
        	value.Add("file", gO.tag.Equals("Air") ? "Plane" : gO.tag.Equals("Surface") ? "Ship" : gO.tag.Equals("Subsurface") ? "Submarine" : "Marine_Unit");

		(gO.GetComponent(controllerName) as Controller).SetValues(value);

    }

	/**
	 * Creates an object from the supplied hash table
	 * 
	 * @param newParams
	 * 		TODO Fill this in
	 */
	private GameObject CreateObject(IDictionary Params, string Tag) {

		string prefab = ObjectFactoryHelper.DeterminePrefab(Params, Tag);
		//Debug.Log("ObjectFactory: prefab: " + prefab);
		GameObject BaseObject = Resources.Load(prefab) as GameObject;
		//Debug.Log("Objectfactory: BaseObject: " + BaseObject);
		//Debug.Log("ObjectFactory: BaseObject transform: " + BaseObject.transform.position);

		//GameObject BaseObject = Resources.Load(prefab) as GameObject;
		////Debug.Log("Objectfactory: BaseObject: " + BaseObject);
		GameObject GO = NetworkManager.NetworkInstance.InstancePrefabOnNetwork (prefab);
		// Iterate through all possible parameters for the game object.
		// If one of the parameters has not been supplied, use the default value.
		foreach(string ParamName in Params.Keys) {
			IDictionary Value = Params[ParamName] as IDictionary;
			
			// Determines the controller that this parameter will be sent to.
			//string Ctrl = ParamName + "Controller";
			string Ctrl = ObjectFactoryHelper.DetermineControllerCreate(ParamName, prefab);
			// Send the value of the parameter to the controller

			PassToController(GO,Ctrl,Value);
		}
		
		//Debug.Log(System.String.Format("ObjectFactory: GO.transform at end: {0}", GO.transform.position));
		return GO;
	}

    private IDictionary GetFromController(GameObject toDeconstruct, string controllerName)
    {
        return (IDictionary)(toDeconstruct.GetComponent(controllerName) as ISetGetValues).GetValues();
    }

    private IDictionary DeconstructObject(GameObject toDeconstruct, iGameObjectDAO dao)
    {
 
        Dictionary<string,System.Object> defaultDict = dao.LoadDefault();
        List<string> keys = new List<string>();
        keys.AddRange(defaultDict.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            //string Ctrl = keys[i] + "Controller";
			string Ctrl = ObjectFactoryHelper.DetermineControllerDestroy(keys[i], toDeconstruct);

            defaultDict[keys[i]] = GetFromController(toDeconstruct, Ctrl);

            //Debug.Log(defaultDict[keys[i]]);
            
        }
        return defaultDict;
    }

    private IDictionary DeconstructObjectMultiThreaded(string toDeconstruct, iGameObjectDAO dao)
    {
        Dictionary<string, System.Object> defaultDict = dao.LoadDefault();
        List<string> keys = new List<string>();
        keys.AddRange(defaultDict.Keys);

        IDictionary shell = ControllerShell.Container[toDeconstruct] as IDictionary;
        for (int i = 0; i < keys.Count; i++)
        {
            string tag = toDeconstruct.Split('-')[0];
            string Ctrl = ObjectFactoryHelper.DetermineControllerDestroyMultiThreaded(keys[i], tag);

            defaultDict[keys[i]] = (shell[Ctrl] as ISetGetValues).GetValues();
        }
        return defaultDict;
    }
}
