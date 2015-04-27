using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestScript_JW : MonoBehaviour {

    private ScenarioFactory scenarioFactory;

	// Use this for initialization
	void Start () {
        scenarioFactory = new ScenarioFactory();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject[] ships = GameObject.FindGameObjectsWithTag("Surface");
            //Debug.Log("number of ships is: " + ships.Length);
            //Debug.Log("Saving");
            scenarioFactory.SaveScenario("Test");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            //Debug.Log("Loading");
            scenarioFactory.LoadScenario("Test");
        }
	}
	
	
	
}
