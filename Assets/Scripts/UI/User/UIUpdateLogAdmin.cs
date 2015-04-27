using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUpdateLog : MonoBehaviour {

	private string LogText = "";
	
	private string EntireLogText = "";	

	public Scrollbar Scroll;

	void Update(){
		transform.GetComponent<Text> ().text = LogText; 
	}

	public void SetLogText(string log){
		LogText = log;
	}
	
	public void SetFullLogText(string log){
		EntireLogText = log;
	}

	void OnDisable(){
		Scroll.value = 1f;
		LogText = "";
		EntireLogText = "";
	}
}
