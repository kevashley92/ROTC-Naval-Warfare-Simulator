using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITimeController : MonoBehaviour {

	private Text CurrMinPH;
	private Text CurrSecPH;
	private Text MaxMinPH;
	private Text MaxSecPH;

	public Button PauseButton;
	public Button EndTurnButton;

	private TurnManager turnManager;

	// Use this for initialization
	void Start () {
		turnManager = GameObject.FindObjectOfType<TurnManager> ();
		
		CurrMinPH = transform.Find ("CurrMinInput").GetComponentsInChildren<Text> ()[0];
		CurrSecPH = transform.Find ("CurrSecInput").GetComponentsInChildren<Text> ()[0];
		MaxMinPH = transform.Find ("MaxMinInput").GetComponentsInChildren<Text> ()[0];
		MaxSecPH = transform.Find ("MaxSecInput").GetComponentsInChildren<Text> ()[0];
		
		MaxMinPH.text = Mathf.FloorToInt (turnManager.TurnTimerMax / 60).ToString ();
		MaxSecPH.text = ((int)(turnManager.TurnTimerMax % 60)).ToString ();

		if(!NetworkManager.NetworkInstance.IsServer)
		{
			PauseButton.interactable = false;
			EndTurnButton.interactable = false;
		}
	}
	
	void Update(){
		CurrMinPH.text = Mathf.FloorToInt(  turnManager.TurnTimer / 60 ).ToString();
		CurrSecPH.text = ((int)(turnManager.TurnTimer % 60)).ToString ();
	}

	public void pauseGame(){
		turnManager.Pause ();
	}
	
	public void endTurn(){
		turnManager.AdminEndTurn ();
	}
	
	/**
	 *  Sets the max time and the current time to the inputs
	 */
	public void setMaxTimer(){
		Text minInput = transform.Find ("MaxMinInput").GetComponentsInChildren<Text> ()[1];
		Text secInput = transform.Find ("MaxSecInput").GetComponentsInChildren<Text> ()[1];
		
		int m = 0;
		int s = 0;
		
		int.TryParse (minInput.text, out m);
		int.TryParse (secInput.text, out s);
		
		//Set turn time
		int newTime = (m * 60) + s;
		
		turnManager.TurnTimerMax = newTime;
		turnManager.TurnTimer = newTime;
		
	}
	
	/**
	 *  Sets the current time to the time in the inputs
	 */
	public void setCurrentTime(){
		Text minInput = transform.Find ("CurrMinInput").GetComponentsInChildren<Text> ()[1];
		Text secInput = transform.Find ("CurrSecInput").GetComponentsInChildren<Text> ()[1];
		
		int m = 0;
		int s = 0;
		
		int.TryParse (minInput.text, out m);
		int.TryParse (secInput.text, out s);
		
		//Set turn time
		int newTime = (m * 60) + s;
		
		turnManager.TurnTimer = newTime;
	}
}
