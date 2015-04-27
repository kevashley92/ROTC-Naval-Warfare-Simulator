using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
public class ChatManager : MonoBehaviour {
	//Chat class from networking
	Chat chat;
	//Network manager class from networking
	NetworkManager nm;
	//List of messages that will be recieved from networking
	List<Chat.ChatMessage> mess;
	//Content of chat
	Text content;
	//Input field that user fills for chat
	InputField inputF;
	//User that is currently active (don't know how this will work with admin)
	User user;
	//Counter not currently being used
	int num = -1;
	//Scenario window
	GameObject Scenario;
	//Log window
	GameObject LogWindow;
	//Channel of chat
	int channel = 0;
	//Button text
	Text channelText;
	//List of team names
	List<string> teamNames;
	//Active players team


	//Default chat size
	float defaultChatY = 200f;

	// Use this for initialization
	void Start () {
		//Get the network manager
		nm = NetworkManager.NetworkInstance;
		//Get chat from network manager
		chat = nm.CHAT;
		//Get UI controller to get access to the active user
		getUser ();

		//Get Default chat height
		defaultChatY = GameObject.Find ("ChatContent").GetComponent<RectTransform> ().sizeDelta.y;

		//Get Channel text
		channelText = GameObject.Find ("ChannelText").GetComponent<Text> ();

		//Geta all teams
		getTeams ();

        if(teamNames == null)
            return;
		channelText.text = teamNames [0];

		channel = user.TeamNumber;
		nextTeam ();

	}

	void Update(){
		//if (user.TeamNumber != 0) {
		//	channel = user.TeamNumber;
		//}
		// if (num < nm.GetMessages ().GetRange ().Count) {
		getUser ();
		getTeams ();
		mess = nm.GetMessages ();
		////Debug.LogError("PREFFERRED SIZE: " + GameObject.Find ("ChatText").GetComponent<Text> ().preferredHeight);
		////Debug.LogError ("WORK");
		populateChat ();
		prefHeight ();
		// num = nm.GetMessages().GetRange ().Count;
		// }

		
	}

	//Sends the message and outputs it
	public void sendChat(){
		//Having trouble getting user once, so do it every time
		getUser ();

		//Get input from user
		inputF = GameObject.Find ("ChatInput").GetComponent<InputField> ();
		//displays the scenrio window if you type the command
		if( inputF.text.Equals("showScenario")){
			Scenario.SetActive(true);
			return;
		}
		//displays the log window if you type the command
		if( inputF.text.Equals("showLog")){
			LogWindow.SetActive(true);
			return;
		}
		//Create new chat message
		String message = inputF.text;
		if (user.TeamNumber == 0)
			message = "<color=red>" + message + "</color>";
		Chat.ChatMessage cm = new Chat.ChatMessage (channel, message, user);
		//set message to input text
		clearChat ();
		clearInput ();

		//NEED TO SET USER AND TEAM OF MESSAGE
		//send the message
		nm.SendMessage (cm);
//		//Debug.Log ("SIZE: " + mess.Count);
		//display the message
		addChatLine (createChatLine (cm));
	}

	//Populates the chat box with messages
	//Will probably need to sort by channel at some point
	void populateChat(){
//		//Debug.Log ("POPULATING CHAT YALL");
		//populate the chat
		////Debug.Log ("STUFF:" + nm.GetMessages().Count);
		clearChat ();
		foreach (Chat.ChatMessage m in mess) {
//			//Debug.Log ("POPULATING CHAT YALL2");
//			//Debug.Log ("User channel: " + user.TeamNumber);
//			//Debug.Log ("Message channel: " + m.channel);
//			//Debug.Log ("Channel: " + channel);
			if( m.channel == channel)
				addChatLine( createChatLine(m));
		}
	}

	//Helper method that clears chat
	void clearChat(){
		content = GameObject.Find ("ChatText").GetComponent<Text> ();
		content.text = "";
	}

	void clearInput(){
		inputF = GameObject.Find ("ChatInput").GetComponent<InputField> ();
		inputF.text = "";
	}

	//Helper method that populates chat content with line of text
	void addChatLine(String input){
		content = GameObject.Find ("ChatText").GetComponent<Text> ();
		content.text = content.text + input + "\n";
	}

	//Helper method that creates a line for chat based on ChatMessage object
	string createChatLine(Chat.ChatMessage cm){
		return cm.user.GetUsername() + ": " + cm.message;
	}

	void getUser(){

		if (GameObject.Find ("Canvas").GetComponent<UIMainController> () != null) {
			user = GameObject.Find ("Canvas").GetComponent<UIMainController> ().GetUser ();
		} else {
			PermissionLevel pl = PermissionLevel.User;
			MilitaryBranch mb = MilitaryBranch.Navy;
			user = new User (pl, mb, 0, "admin", null);
		}

	}

	void prefHeight(){
		content = GameObject.Find ("ChatText").GetComponent<Text> ();
		float ph = content.preferredHeight;
		if (ph < defaultChatY)
			ph = defaultChatY;
		RectTransform rt = GameObject.Find ("ChatContent").GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, ph+ 40);

	}

	void getTeams(){
		teamNames = Team.GetTeamNames ();
//		if (user.TeamNumber == 0) {
//			teamNames.Add ("Global");
//		} else {
//			for( int i = 0; i < teamNames.Count; i++){
//				if( user.TeamNumber != i || i != 0){
//					teamNames.Remove (teamNames[i]);
//					//Debug.Log("DELETE THE CHANNEL");
//				}
//			}
//			teamNames.Add ("Admins");
//		}
	}

	public void nextTeam(){
		if (user.TeamNumber == 0) {
			if (channel < teamNames.Count - 1) {
				channel++;
			} else {
				channel = 0;
			}
		} else {
			channel = user.TeamNumber;
		}

		channelText.text = teamNames [channel];
//		//Debug.Log ("COUNT: " + teamNames.Count);
//		//Debug.Log ("COUNT: " + teamNames[0]);
//		//Debug.Log ("COUNT: " + teamNames[1]);
//		//Debug.Log ("COUNT: " + teamNames[2]);
	}

	public void prevTeam(){
		if (user.TeamNumber == 0) {
			if (channel > 0) {
				channel--;
			} else {
				channel = teamNames.Count - 1;
			}
		} else {
			channel = user.TeamNumber;
		}

		////Debug.Log ("PRESSED PREV GOSHDARNIT");

		channelText.text = teamNames [channel];
	}
}

