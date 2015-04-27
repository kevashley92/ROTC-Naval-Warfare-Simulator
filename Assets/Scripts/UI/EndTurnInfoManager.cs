using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndTurnInfoManager : MonoBehaviour {
	Text content;
	float defaultY = 650;

	// Use this for initialization
	void Start () {
		content = GameObject.Find ("CreditsText").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		content = GameObject.Find ("CreditsText").GetComponent<Text> ();
		prefHeight ();
	}

	void Populate(string s){
		content.text = s;
	}

	void prefHeight(){
		float ph = content.preferredHeight;
		if (ph < defaultY)
			ph = defaultY;
		RectTransform rt = GameObject.Find ("CreditsContent").GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, ph);
	}
}
