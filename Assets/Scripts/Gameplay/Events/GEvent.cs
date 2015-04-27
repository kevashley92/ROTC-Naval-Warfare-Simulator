/*****
 * 
 * Name: GEvent
 * 
 * Date Created: 2015-01-29
 * 
 * Original Team: Gameplay
 * 
 * This class will represent a single, generic event of any type or functionality
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-01-29	Initial Commit. Created initial fields
 * B. Croft		2015-01-30	Renamed to GEvent to prevent Unity builtin Event 
 * 							class interference (feel free to re-rename to something better)
 * 							Added ToString method
 * B. Croft		2015-02-11	Changed to not be a MonoBehaviour
 * T. Brennan	2015-02-17	Refactored, Deleted unused Update method
 * T. Brennan	2015-03-05	Added Turn, Changed Type to EventType, Added
 * 							GetEventType, made comparable
 */

using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class GEvent : IComparable {

	/**
	 * The time this event was created
	 */
	public float Timestamp;

	/**
	 * The type of the event
	 */
	public GEventType EventType;

	/**
	 * The priority of the event. Priority determines when event groups are
	 * executed.
	 */
	public int Priority;

	/**
	 * The turn that this event should be handled on.
	 */
	public int Turn;

	/**
	 * This will be the array of Parameters for the event
	 */
	public object[] Arguments;

	/**
	 * This is the string representation of the GEvent.
	 */
	public string String;

	public GEvent() {
		// set parameters

	}

	/**
	 * Allows this object to be compared with other objects
	 * 
	 * @return
	 * 		Less than zero - this precedes obj
	 * 		0 - this is same position as obj
	 * 		Greater than zero - obj precedes this
	 */		
	public int CompareTo(object obj) {

		if (obj == null) {

			return 1;

		}

		GEvent otherGEvent = obj as GEvent;

		if (otherGEvent != null) {

			if (this.Turn == otherGEvent.Turn) {
				
				if (this.Timestamp < otherGEvent.Timestamp) {
					
					return -1;
					
				}
				
				return 1;
				
			} 
			else if (this.Turn < otherGEvent.Turn) {
				
				return -1;
				
			}
			else {
				
				return 1;
				
			}

		}
		else {

			throw new ArgumentException("Object is not a GEvent");

		}
		

	}
	


	/**
	 * Gets the type of the current event
	 * 
	 * @return
	 * 		The type of the event
	 */
	public GEventType GetEventType() {

		return EventType;

	}


	/**
	 * Returns a string representation of the event.  This will be used for
	 * logging purposes.
	 */
	public override string ToString() {

		return String;

	}

}
