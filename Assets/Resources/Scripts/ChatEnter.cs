﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatEnter : MonoBehaviour {

	Text textContent;
	Button newB;
	InputField inputF;
	int i = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void Click(){
		//textContent = GetComponent<Text>();
		textContent = GameObject.Find ("ChatText").GetComponent<Text> ();
		inputF = GameObject.Find ("ChatInput").GetComponent<InputField> ();
		textContent.text = textContent.text + inputF.text + "\n";
		inputF.text = "";
		
		//newB = GetComponent<Button> ();
		//newB.interactable = false;
		//Debug.Log ("UGH WORK YOWREJOIEFFJ");
	}

}
