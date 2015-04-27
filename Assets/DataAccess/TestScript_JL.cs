using UnityEngine;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using System.Runtime.Serialization.Formatters.Binary;

public class TestScript_JL : MonoBehaviour {
	SurfaceDAO shipDAO = DAOFactory.GetFactory().GetSurfaceDAO();

	AirDAO airDAO = DAOFactory.GetFactory().GetAirDAO();

	string name =  "NULL";
	string type = "NULL";
	int x_pos = 0;
	int y_pos = 0;
	int maxmove = 0;
	int surf = 0;
	int air =  0;
	int es = 0;
	int sonar = 0;
	string weapon = "NULL";
	int defense = 0;
	GameObject o;
	// Use this for initialization
	void Start () {
	
//
	}
	// Update is called once per frame
	void Update () {
		
//
	}



	void OnGUI() {
		GUI.Label(new Rect(325, 100, Screen.width, Screen.height), "name: " + name + "\n" + "type: " + type + "\n" + 
		          "x_pos: " + x_pos + "\n" + "y_pos: " + y_pos + "\n" + "maxmove: " + maxmove + "\n" + "surf detector: " +
		          surf + "\n" + "air detector: " + air + "\n" + "es detector: " + es + "\n" + "sonar detector: " + sonar + "\n" +
		          "weapon: " + weapon + "\n" + "defense: " + defense + "\n");
	}

}
