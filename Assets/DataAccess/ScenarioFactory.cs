using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MiniJSON;
using Assets.Scripts.Maps;

public class ScenarioFactory {
    private static float _saveStatus = 0f;

    public string LoadScenario(string scenarioName)
	{
        //Debug.Log("loading scenario: " + scenarioName);
		
		var gameObjects = GuidList.GetAllObjects();
		foreach(GameObject gameObject in gameObjects)
		{
			NetworkManager.NetworkInstance.DestroyUnitOnNetwork(gameObject);
		}
		GuidList.Clear();

        string toReturn = Load(scenarioName);
        //Get the factories.
        ObjectFactory objectFactory = new ObjectFactory(scenarioName);
		objectFactory.LoadAllSurface();
		objectFactory.LoadAllSubSurface();
		objectFactory.LoadAllAir();
		objectFactory.LoadAllMarines();
		objectFactory.LoadEnvironmentVariables();

        return toReturn;
	}

	private void SaveObjects(string scenarioName)
	{
        _saveStatus = 0f;
        
        GuidList.CleanNullObjects();

        ControllerShell.DumpGuidList();
        _saveStatus += .25f;

        EnvironmentSettingsShell.DumpAll();
        _saveStatus += .25f;

        Thread t = new Thread(() => MultiThreadedSave(scenarioName));
        t.Start();
	}

    private void MultiThreadedSave(string scenarioName)
    {
        //Get the factory.
        ObjectFactory objectFactory = new ObjectFactory(scenarioName);
        objectFactory.BuildFilePath();

        int i = 0;
        int size = ControllerShell.Container.Count;

        //Debug.Log("ControllerShell Count: " + size);
        //Debug.Log("GUIDLIST Count: " + GuidList.GetAllObjects().Count);

        foreach (string toSave in ControllerShell.Keys)
        {
            switch (toSave.Split('-')[0])
            {
                case "Surface":
                    objectFactory.SaveSurfaceToScenarioMultiThreaded(toSave, (i++).ToString());
                    break;
                case "Air":
                    objectFactory.SaveAirToScenarioMultiThreaded(toSave, (i++).ToString());
                    break;
                case "Subsurface":
                    objectFactory.SaveSubSurfaceToScenarioMultiThreaded(toSave, (i++).ToString());
                    break;
                case "Marine":
                    objectFactory.SaveMarineToScenarioMultiThreaded(toSave, (i++).ToString());
                    break;
                default:
                    break;
            }
            
            _saveStatus += .25f / size;
        }


        objectFactory.SaveEnvironmentVariablesMultiThreaded();
        _saveStatus += .125f;

        objectFactory.SaveToFile();
        _saveStatus = 1f;
    }

    private string Load(string currentScenarioName)
    {
        //Get the DAOs.
        DAOFactory factory = DAOFactory.GetFactory();
        MapDAO map = factory.GetMapScenarioDAO(currentScenarioName);

        //Load the map.
        return map.Load();
    }

	public void SaveScenario(string scenarioName)
	{
		SaveMap(scenarioName);
		SaveObjects(scenarioName);		
	}

    void SaveMap(string currentScenarioName)
    {
        WipeOutDirectory(currentScenarioName);
        DAOFactory factory = DAOFactory.GetFactory();
        MapDAO map = factory.GetMapScenarioDAO(currentScenarioName);

        //Each of these can be threaded at some point.
        //Save the world.
        map.Save();
    }

    public void WipeOutDirectory(string scenarioName)
    {
        DAOFactory factory = DAOFactory.GetFactory();
        MapDAO map = factory.GetMapScenarioDAO(scenarioName);
        map.RemoveDirectory();
    }

    public static String SaveStatus {
        get {
            if (_saveStatus.Equals(1f)) {
                return "Save Done";
            } else if (_saveStatus.Equals(0f)) {
                return "Save Ready";
            } else {
                return String.Format("{0:F2}%", _saveStatus);
            }
        }
        private set { }
    }
}