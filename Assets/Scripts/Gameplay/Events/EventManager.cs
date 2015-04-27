/*****
 * 
 * Name: EventManager
 * 
 * Date Created: 2015-01-29
 * 
 * Original Team: Gameplay
 * 
 * This class will handle the collection and execution of events
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-01-29	Initial Commit. Created initial fields and 
 *                          UpdateEndOfTurnEvents and AddEvent function
 * B. Croft		2015-01-30  Fixed error in UpdateEndOfTurnEvents
 * 							(no parentheses in declaration)
 * M.Schumacher	2015-01-30	Changed to use a Sorted list sorted by
 * 							priority then timestamp
 * M.Schumacher	2015-01-30	Added basic functionality for registering
 * 							eventhandlers
 * M.Schumacher 2015-02-02	Implemented update method
 * B. Croft		2015-02-04	Minor cleanup
 * B.Croft		2015-02-11	Add EventHandler interface, add singleton
 * 							pattern
 * M.Schumacher	2015-02-12	Fixed a few bugs and initialized logger
 * M.Schumacher	2015-02-12	Added initialization for event handlers
 * T. Brennan	2015-02-17	Refactored
 * T. Brennan	2015-03-05	Changed to be compatible with networked
 * 							Events and prioritization
 * T. Brennan	2015-03-23	Registered most of the handlers
 * J. Balik		2015-03-23  Added ExplosionHandler register
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;// needed for ArgumentOutOfRangeException

public class EventManager : MonoBehaviour
{

	/** 
	 * Singleton reference.  Singleton pattern obtained from:
	 * 
	 * 		http://unitypatterns.com/singletons/
	 */
	private static EventManager _Instance;


	/**
	 * This is the public reference that other classes will use
	 */
	public static EventManager Instance {

		get {

			/**
			 * If _instance hasn't been set yet, we grab it from the scene
			 * This will only happen the first time this reference is used.
			 */
			if (_Instance == null) {
				_Instance = (EventManager) GameObject.FindGameObjectWithTag ("EventManager").GetComponent<EventManager>();

			}

			return _Instance;

		}

	}
	
	/**
	 * The sorter for the list of events. Sorted by priority then by timestamp
	 */
	private IComparer EventSorter;
	
	/**
	 * The sorted list of events. The key is the event itself, the value is null
	 * for all events
	 */
	private Dictionary<int,List<GEvent>> Events;
	
	/**
	 * The list of event handlers
	 * This is a Dictionary that links the event type to a linked list of
	 * interested handlers
	 */
	private Dictionary<GEventType, List<EventHandler>> EventHandlers;


	/**
	 * Use this for initialization
	 */
	public void Awake () {

		Events = new Dictionary<int, List<GEvent>>();
		Events.Add(0, new List<GEvent>()); // Immediate Events
		Events.Add(1, new List<GEvent>()); // Movement Events
		Events.Add(2, new List<GEvent>()); // Attack Events
		EventHandlers = new Dictionary<GEventType, List<EventHandler>> ();
		EventLog.Init ("DebugLog.txt");
		
		
		StartCoroutine(InitHandlers());

	}
	
	
	public IEnumerator InitHandlers() {
		yield return new WaitForFixedUpdate();
		//create and add eventhandlers

		// Administrative Handlers
        new AdminStatisticChangeHandler().Register();
		new EndOfTurnHandler ().Register ();
		new PlayerJoinHandler ().Register();
		new PlayerLeaveHandler().Register();
		new StartOfTurnHandler ().Register ();
		new WeatherChangeHandler ().Register ();
		
		// Attacking Handlers
		new AttackHandler ().Register ();
		new BackfireHandler ().Register ();
		new RemoveTargetHandler ().Register ();
		new WeaponTargetHandler ().Register ();
		new ExplosionHandler () .Register ();
		
		// Death Handlers
		new DeathHandler ().Register ();

		// Movement Handlers
		new MoveHandler ().Register ();
		new PlanMoveHandler ().Register();
		new UnitEmbarksHandler ().Register ();
		new UnitUnEmbarksHandler ().Register ();
		
	}


	/**
	 * Update is called once per frame
	 */
	void Update () {

		HandleEvents(0);

	}
	 

	/**
	 * Add an event to the event list
	 * If this is the server, add the event to the event list
	 * If this is a client, send the event to the server
	 * 
	 * @param e
	 * 		The event to be added to the list.
	 */
	public void AddEvent (GEvent e) {
	
		e.Timestamp = Time.time;

		// Check if this is the server
		if (NetworkManager.NetworkInstance.IsServer) {
			// If this is the server

			////Debug.Log("On Server: Got event of type " + e.EventType.ToString () + " with timestamp " + e.Timestamp.ToString () + " and " + e.Arguments.Length + " argument(s)");

			// Add event as key and value as null
			Events[e.Priority].Add (e);

		}
		else {
			// If this is not the server

			////Debug.Log("On Client: Got event of type " + e.EventType.ToString () + " with timestamp " + e.Timestamp.ToString () + " and " + e.Arguments.Length + " argument(s)");

			// Send the event to the server
			NetworkManager.NetworkInstance.SendEvent (e);

		}

	}


	/**
	 * Handles all events that should be handled at this turn of the given
	 * priority.
	 * 
	 * @param priority
	 * 		The priority of the events that should be handled.
	 */
	public void HandleEvents(int priority) {

		// record the current turn
		int curTurn = TurnManager.GetCurrentTurn();

		List<GEvent> current = Events[priority];
		
		if(current == null){
			return;
		}

		current.Sort();
		
		// Iterate over all the events of the priority
		while(Events[priority].Count > 0 && Events[priority][0].Turn <= curTurn) {

			GEvent e = Events[priority][0];

			// Removes the event that was just handled.
			Events[priority].RemoveAt(0);

			////Debug.Log("Handling Event of type: " + e.GetType().ToString());

			// Iterate over every interested handler
			foreach (EventHandler h in EventHandlers[e.GetEventType()]) {

				// Send the event to be handled by the handler
				h.ProcessEvent(e);

			}

		}
	}



	/**
	 * Function that a handler calls to register itself for events
	 */
	public void RegisterMe (GEventType t, EventHandler eh) {

		//add the linked list to the key "t" if it doesnt exist yet
		if (!EventHandlers.ContainsKey (t)) {

			EventHandlers.Add (t, new List<EventHandler> ());

		}
		
		//add event to the linked list
		EventHandlers [t].Add (eh);
	}

	/**
	 * Send events to server for handling.
	 */
	public void SendEventToServer (GEvent e) {
		NetworkManager.NetworkInstance.SendEvent (e);
	}
}
