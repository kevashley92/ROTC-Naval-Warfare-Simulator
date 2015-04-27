/*****
 * 
 * Name: TurnManager
 * 
 * Date Created: 2015-02-02
 * 
 * Original Team: Gameplay
 * 
 * This class will manage the turn system for the game.
 * The turns are handled with a countdown timer.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * B. Croft		2015-02-02	Initial Commit.
 * B. Croft		2015-02-04	Add support for updating a Text GUI component.
 * B. Croft		2015-02-06	Add pause function.
 * B. Croft		2015-02-10  At the end of turn, throw a EndOfTurnEvent to EventManager
 * B. Croft		2015-02-11	Add turn counter.
 * M.Schumaher	2015-02-11	Commented out unity Debug call
 * B. Croft		2015-02-17	Added ThrowStartOfTurnEvent function.
 * T. Brennan	2015-02-17	Refactored
 */


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class TurnManager : MonoBehaviour
{

	/**
	 * The timer counting down the turn.
	 */
	public float TurnTimer;

	/**
	 * The max value of the timer.
	 */
	public float TurnTimerMax;

	/**
	 * If true, turns will complete automatically when the timer runs out.
	 * If false, turns will require an administrator to end a turn.
	 */
	public bool AutoTurnComplete;

	/**
	 * Whether or not the timer is actually running 
	 */
	public bool Paused;

	/**
	 * The text field that the turn timer text is placed in.
	 */
	public Text TimerText;

	/** 
	 * The current turn number (ie turn 1, turn 2)
	 */
	public static int TurnCount;

	private Animation EndOfTurnAnimation;

	private RectTransform TimerHandTransform;

	/** 
	 * The game object containing the end of turn animation
	 */
	private GameObject EndOfTurnDisplay;

    /**
     * This should be all the buttons avalible for the user/admin
     */
    private List<GameObject> _toDisable = null;
    private List<Button> _toDisableButton = null;
    private List<ComboBox> _toDisableComboBox = null;
    private List<EventTrigger> _toDisableEventTrigger = null;
    private readonly string[] _adminWhitelist = 
    {
        "SETTINGS", "PauseButton", "EndTurnButton", "Sound On Button", "Sound Off Button", "ComboButton(Clone)", 
        "ComboButton", "Timer Text", "ChatButton1", "ChatButton2", "ChatButton4", "ScenarioButton", "LOG",
        "CloseDisconnectedPopupButton"
    };
    private readonly string[] _userWhitelist = 
    {
        "Log Out Button", "Settings Button", "Previous Unit Button", "Next Unit Button", 
        "Previous Weapon Button", "Next Weapon Button", "Next Target Button", "Previous Target Button",
        "Team Dropdown", "Start Button", "ComboButton", "ScenarioButton", "ComboButton(Clone)",
        "Sound On Button", "Sound Off Button", "ChatButton1", "ChatButton2", "Button", "Language Dropdown",
        "ChatButton4", "CloseDisconnectedPopupButton"
    };

	/**
	 * Use this for initialization
	 */
	void Start ()
	{

		TurnTimer = TurnTimerMax;
		Paused = true;
		TurnCount = 1;
		TimerText = GameObject.Find ("Timer Text").GetComponent<Text> ();
		TimerHandTransform = GameObject.Find ("Timer Hand").GetComponent<RectTransform> ();
		EndOfTurnAnimation = GameObject.Find ("End Of Turn Display").GetComponent<Animation> ();
		EndOfTurnDisplay = GameObject.Find ("End Of Turn Display");
		EndOfTurnDisplay.SetActive (false);
		gameObject.tag = "TurnManager";

        GenerateDisableLists();
        if (NetworkManager.NetworkInstance.IsServer)
        {
            NetworkManager.NetworkInstance.Pause(Paused);
        }
	}


	/**
	 * Runs through all the actions to be done upon completing a turn and
	 * starts another turn.
	 */
	public void CompleteTurn ()
	{

		// Only throw the end of turn event if you are on the server.
		if (NetworkManager.NetworkInstance.IsServer)
		{

			EventFactory.ThrowEvent (GEventType.EndOfTurnEvent, new object[] { TurnCount });
			ActivateEndTurnSpinner();
			NetworkManager.NetworkInstance.ActivateEndTurnSpinner();

		}

		TurnTimer = TurnTimerMax;

	}

	public void ActivateEndTurnSpinner() {

		////Debug.Log("Turn Completed");
		UIMainController UserUI = GameObject.Find ("Canvas").GetComponent<UIMainController> ();
		if (UserUI != null) {
			UserUI.ShowLog();
		}

		EndOfTurnDisplay.SetActive (true);
		EndOfTurnAnimation.Play ();

	}

	public static void ActivateEndTurnSpinnerStatic() {

		TurnManager curTurnManager = GetTurnManager();

		if (curTurnManager == null) {

			//Debug.Log("Could not find turn manager instance");
			return;

		}

		curTurnManager.ActivateEndTurnSpinner();
		
	}


	/**
	 * Returns the current turn.
	 * 
	 * @return
	 * 		The current turn.
	 */
	public static int GetCurrentTurn ()
	{

		return TurnCount;
		
	}


	/**
	 * Increments the turn count by 1
	 */
	public static void MoveToNextTurn ()
	{

		TurnCount++;

	}


	/**
	 * Waits for the end of the frame, then throws a startofturn event.
	 */
	public static void ThrowStartOfTurnEvent ()
	{

		//yield return new WaitForEndOfFrame();

		object[] args = {TurnCount};

		EventManager.Instance.AddEvent (EventFactory.CreateEvent (GEventType.StartOfTurnEvent, args));

	}

	/**
	 * Allows an admin to end a turn regardless of the timer state.
	 */
	public void AdminEndTurn ()
	{

		CompleteTurn();

	}


	/**
	 * Allows an admin to pause the timer
	 */
	public void Pause ()
	{
        //Toggle the pause
		Paused = !Paused;
        NetworkManager.NetworkInstance.Pause (Paused);
	}

    private void GenerateDisableLists()
    {
        GameObject canvas = GameObject.Find("Canvas");
        List<Transform> list = new List<Transform>(canvas.GetComponentsInChildren<Transform>(true));
        List<string> whitelist = new List<string>();

        if ("UserViewIke".Equals(Application.loadedLevelName))
        {
            whitelist.AddRange(_userWhitelist);
        }
        else
        {
            whitelist.AddRange(_adminWhitelist);
        }

        int i = 0;
        //Filter out all non EventTriggers or Buttons or ComboBoxes
        while (i < list.Count)
        {
            GameObject go = list[i].gameObject;
            if (go.GetComponent<Button>() == null &&
                go.GetComponent<ComboBox>() == null &&
                go.GetComponent<EventTrigger>() == null)
            {
                list.RemoveAt(i);
            }
            //Filter out all whitelisted items
            else if (whitelist.Contains(go.name))
            {
                list.RemoveAt(i);
            }
            else 
            {
                i++;
            }
        }
        _toDisable = new List<GameObject>();
        foreach (Transform t in list)
        {
            _toDisable.Add(t.gameObject);
        }

        _toDisableButton = new List<Button>();
        _toDisableComboBox = new List<ComboBox>();
        _toDisableEventTrigger = new List<EventTrigger>();
        foreach (GameObject go in _toDisable)
        {
            _toDisableButton.AddRange(go.GetComponents<Button>());
            _toDisableComboBox.AddRange(go.GetComponents<ComboBox>());
            _toDisableEventTrigger.AddRange(go.GetComponents<EventTrigger>());
        }
        
    }

    public void SetObjectsForPause(bool value)
    {
        //Debug.Log("Setting all objects to: " + value);
        foreach (Button b in _toDisableButton)
        {
            b.enabled = value;
            b.interactable = value;
        }
        foreach (ComboBox cb in _toDisableComboBox)
        {
            cb.Interactable = value;
        }
        foreach (EventTrigger et in _toDisableEventTrigger)
        {
            et.enabled = value;
        }
        if ("AdminView_AJ_Kenny".Equals(Application.loadedLevelName) && !NetworkManager.NetworkInstance.IsServer)
        {
            GameObject.Find("EditUnitButton").GetComponent<Button>().enabled = false;
            GameObject.Find("EditUnitButton").GetComponent<Button>().interactable = false;
            GameObject.Find("StoreUnitButton").GetComponent<Button>().enabled = false;
            GameObject.Find("StoreUnitButton").GetComponent<Button>().interactable = false;
        }
    }

	/**
	 * Update is called once per frame
	 */
	void Update ()
	{

		TimerHandTransform.rotation = Quaternion.Euler (0, 0, 360 - (360 * ((TurnTimerMax - TurnTimer) / TurnTimerMax)));
		if (!Paused && Network.isServer)
		{

			TurnTimer = Mathf.Max (0.0f, TurnTimer - Time.deltaTime);

		}

		if (TimerText != null)
		{

			TimerText.text = Paused ? "| |" : (Mathf.Floor (TurnTimer / 60.0f) + ":" + (Mathf.Floor (TurnTimer % 60.0f) < 10 ? "0" : "") + Mathf.Floor (TurnTimer % 60.0f));

		}

		if (TurnTimer <= 0.0f && AutoTurnComplete)
		{

			CompleteTurn ();

		}

		if (EndOfTurnAnimation.isPlaying)
		{
			EndOfTurnDisplay.SetActive (true);
		}
		else
		{
			EndOfTurnDisplay.SetActive (false);
		}
	}

	private Dictionary<string, object> GetValues ()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("TurnTimer", TurnTimer);
		values.Add ("TurnTimerMax", TurnTimerMax);
		values.Add ("AutoTurnComplete", AutoTurnComplete);
		values.Add ("Paused", Paused);
		values.Add ("TimerText.text", TimerText.text);
		values.Add ("TurnCount", TurnCount);
		return values;
	}

	private void SetValues (Dictionary<string, object> values)
	{
		object turnTimer = null;
		values.TryGetValue ("TurnTimer", out turnTimer);
		if (turnTimer != null)
		{
			TurnTimer = Convert.ToSingle (turnTimer);
		}

		object turnTimerMax = null;
		values.TryGetValue ("TurnTimerMax", out turnTimerMax);
		if (turnTimerMax != null)
		{
			TurnTimerMax = Convert.ToSingle (turnTimerMax);
		}

		object autoTurnComplete = null;
		values.TryGetValue ("AutoTurnComplete", out autoTurnComplete);
		if (autoTurnComplete != null)
		{
			AutoTurnComplete = Convert.ToBoolean (autoTurnComplete);
		}

		object paused = null;
		values.TryGetValue ("Paused", out paused);
		if (paused != null)
		{
			Paused = Convert.ToBoolean (paused);
		}

		object timerTextdottext = null;
		values.TryGetValue ("TimerText.text", out timerTextdottext);
		if (timerTextdottext != null)
		{
			//TimerText.text = Convert.ToString (timerTextdottext);
		}

		object turnCount = null;
		values.TryGetValue ("TurnCount", out turnCount);
		if (turnCount != null)
		{
			TurnCount = Convert.ToInt32 (turnCount);
		}
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			string variablesSerializedString = MiniJSON.Json.Serialize (GetValues ());
			
			char[] serializedChars = variablesSerializedString.ToCharArray ();
			
			char end = '\0';
			
			for (int i = 0; i < serializedChars.Length; i++)
			{
				stream.Serialize (ref serializedChars [i]);
			}
			
			stream.Serialize (ref end);
		}
		else
		{
			List<char> serializedCharList = new List<char> ();
			
			char c = ' ';
			stream.Serialize (ref c);
			while (c != '\0')
			{
				serializedCharList.Add (c);
				stream.Serialize (ref c);
			}
			
			char[] serializedCharArray = serializedCharList.ToArray ();
			
			string variablesSerialized = new string (serializedCharArray);
			
			Dictionary<string, object> variablesDeserialized = MiniJSON.Json.Deserialize (variablesSerialized) as Dictionary<string, object>;

			SetValues (variablesDeserialized);
		}

	}


	public static TurnManager GetTurnManager() {

		//Debug.Log("TurnManager.GetTurnManager() entering");
		GameObject TurnManagerGO = GameObject.FindWithTag("TurnManager");

		if (TurnManagerGO == null) {

			//Debug.Log("TurnManager.GetTurnManager(): retrieved null object, returning null");
			return null;

		}

		TurnManager ret = TurnManagerGO.GetComponent<TurnManager>();
		//Debug.Log("TurnManager.GetTurnManager(): exiting with " + ret.ToString());
		return ret;

	}

}
