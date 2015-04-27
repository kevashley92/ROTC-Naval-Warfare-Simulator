/*****
 * 
 * Name: ClientEventManager
 * 
 * Date Created: 2015-02-25
 * 
 * Original Team: Gameplay
 * 
 * This class will handle sending events to the server for managing.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * T.Dill		2015-25-2	Initial Commit.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;// needed for ArgumentOutOfRangeException

public class ClientEventManager : MonoBehaviour
{
	
	/**
	 * The comparer for the list of events. Sorted by priority then by timestamp
	 */
	private class PriorityTimestampComparer: IComparer
	{
		
		int IComparer.Compare (object first, object second)
		{
			
			GEvent x = (GEvent)first;
			
			GEvent y = (GEvent)second;
			
			if (x.Priority == y.Priority)
			{
				
				if (x.Timestamp < y.Timestamp)
				{
					
					return -1;
					
				}
				
				return 1;
				
			}
			
			if (x.Priority < y.Priority)
			{
				
				return -1;
				
			}
			
			return 1;
			
		}
		
	}
	
	
	/** 
	 * Singleton reference.  Singleton pattern obtained from:
	 * 
	 * 		http://unitypatterns.com/singletons/
	 */
	private static ClientEventManager _Instance;
	
	
	/**
	 * This is the public reference that other classes will use
	 */
	public static ClientEventManager Instance
	{
		
		get
		{
			
			/**
			 * If _instance hasn't been set yet, we grab it from the scene
			 * This will only happen the first time this reference is used.
			 */
			if (_Instance == null)
			{
				
				_Instance = (ClientEventManager) GameObject.FindGameObjectWithTag("EventManager").GetComponent <ClientEventManager> ();
				
			}
			
			return _Instance;
			
		}
		
	}
	
	/**
		* The sorter for the list of events. Sorted by priority then by timestamp
	*/
	private IComparer EventSorter;
	
	/**
		* The sorted list of events. The key is the event itself, the value is null for all events
	*/
	private SortedList EventList;
	
	/**
		* The event log used for all events
	*/
	private EventLog EventLog;
	
	/**
	 * The list of event handlers
	 * This is a Dictionary that links the event type to a linked list of interested handlers
	 */
	private Dictionary<GEventType, LinkedList<EventHandler>> EventHandlers;
	
	/**
	 * Use this for initialization
	 */
	public void Start ()
	{
		
		EventSorter = new PriorityTimestampComparer ();
		EventList = new SortedList (EventSorter);
		EventHandlers = new Dictionary<GEventType, LinkedList<EventHandler>> ();
		EventLog.Init ("DebugLog.txt");
		
	}
	
	
	/**
	 * Update is called once per frame
	 */
	void Update ()
	{
		//check for events here if we need to, either from our game or network events
		
		//run events and add them to the EventLogDeveloper
		
		float now = Time.time;
		
		int currentEventId = 0;
		
		while (currentEventId < EventList.Count)
		{
			
			// Get the event
			GEvent currentEvent = (GEvent)EventList.GetKey (currentEventId);
			
			// Check if it is time for the event
			if (currentEvent.Timestamp <= now)
			{

				EventList.RemoveAt (currentEventId);
				LinkedList<EventHandler> InterestedHandlers= new LinkedList<EventHandler>();
				// Get the interested handlers for thi event type
				InterestedHandlers = EventHandlers[currentEvent.EventType];
				
				
				// Iterate over each element in the list
				foreach (EventHandler eh in InterestedHandlers){
					
					eh.HandleEvent(currentEvent);
					
				}
				
				SendEventToServer (currentEvent);
				
				EventLog.LogEvent (currentEvent);
				
			}
			else
			{
				// If it is not time for the event to execute
				
				//increment currentEventID so we don't just keep trying the same one
				currentEventId++;
				
			}
			
		}
		
	}
	
	
	/**
	 * Add an event to the event list
	 */
	public void AddEvent (GEvent e)
	{
		
		//add event as key and value as null
		EventList.Add (e, null);
		
	}
	
	
	/**
	 * Function that a handler calls to register itself for events
	 */
	public void RegisterMe (GEventType t, EventHandler eh)
	{
		
		//add the linked list to the key "t" if it doesnt exist yet
		if (!EventHandlers.ContainsKey (t))
		{
			
			EventHandlers.Add (t, new LinkedList<EventHandler> ());
			
		}
		
		//add event to the linked list
		EventHandlers [t].AddLast (eh);
	}
	/**
	 * Send events to server for handling.
	 */
	public void SendEventToServer (GEvent e)
	{
		NetworkManager.NetworkInstance.SendEvent (e);
	}
	
}
