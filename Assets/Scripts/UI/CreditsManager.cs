using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {
	Text content;
	float defaultY = 650;
	// Use this for initialization
	void Start () {
		content = GameObject.Find ("ScenarioText").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		content = GameObject.Find ("ScenarioText").GetComponent<Text> ();
		prefHeight ();
	}

	void prefHeight(){
		float ph = content.preferredHeight;
		if (ph < defaultY)
			ph = defaultY;
		RectTransform rt = GameObject.Find ("ScenarioContent").GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, ph);
	}
}
