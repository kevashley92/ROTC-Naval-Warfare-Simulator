using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseCoordinateController : MonoBehaviour {

	public Text xPos;
	public Text yPos;
	
	// Update is called once per frame
	void Update () {
		xPos.text = MouseToGridAPI.GetGridCoordinate (Input.mousePosition).x.ToString ();
		yPos.text = MouseToGridAPI.GetGridCoordinate (Input.mousePosition).y.ToString ();
	}
}
