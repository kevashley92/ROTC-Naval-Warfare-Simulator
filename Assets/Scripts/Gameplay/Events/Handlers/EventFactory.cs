/*****
 * 
 * Name: EventFactory
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * This class will handle the creation of events
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-01-30	Initial Commit. Created initial fields and 
 * 							ThrowExampleEvent function
 * B. Croft		2015-02-11	Change ThrowEvent to actually push to event manager
 * 							while CreateEvent just creates and returns
 * M.Schumacher	2015-02-12	Added a few debug messages and changed attackevent slightly
 * B. Croft		2015-02-15	Change MoveEvent to have Vector3 as parameter
 * T. Brennan	2015-02-17 	Refactored
 * T. Brennan	2015-03-18	Added more correct line to PlayerJoinEvent
 */


using UnityEngine;
using System.Collections;
using System;

public class EventFactory
{
	
	/* This is a list of events and their arguments order
	* unless listed otherwise argument 0 is always the originating unit id (ex the moving unit, the firing unit)
	
		AdminSpawnEvent: 1: latitude 2: longitude
		
		AdminStatisticChangeEvent: 1: Controller name as a string 2: Parameter name as a string 3: new value
		AttackEvent: 1: target guid 2: weapon 3: damage //note damage of 0 indicates a miss
		
		WeaponTargetEvent: 1: target unit id 2: weapon name 3: amount of shots
	
		MoveEvent: 1: Vector3 of new position
		
		DieEvent: no other arguments
		
		EndOfTurnEvent: 0: the turn number
		
		BackfireEvent: 1: the backfire damage

		RemoveTargetEvent: 1: target guid 2: weapon name
		
		UnitEmbarksEvent: 1: the smaller unit guid 2: the super unit guid
		
		UnitUnEmbarksEvent: 1: the smaller unit guid 2: the super unit guid
		
		WeatherChangeEvent: 0: the index of the new weather

		ExplosionEvent: 0: The explosion instance

		PlanMoveEvent: 0: Unit guid 1: user name 2: unit name 3: vector3 position 4: vector3 destination
	
	*/


	/**
	 * Creates and returns an event with default priority of 0
	 */
	public static GEvent CreateEvent (GEventType eType, object[] eArguments) {

		return EventFactory.CreateEvent(eType, eArguments, 0);

	}
	
	/**
	 * Creates and returns an event
	 */
	public static GEvent CreateEvent (GEventType eType, object[] eArguments, int ePriority)
	{

		GEvent e = new GEvent ();

		e.Arguments = eArguments;

		e.Timestamp = Time.time;

		e.EventType = eType;

		e.Turn = TurnManager.GetCurrentTurn();

		e.Priority = ePriority;

		return e;

	}


	/**
	 * Creates an event and throws it to the event manager.
	 */
	public static void ThrowEvent (GEventType eType, object[] eArguments) {
		EventFactory.ThrowEvent (eType, eArguments, 0);
	}

	/**
	 * Creates an event and throws it to the event manager.
	 */
	public static void ThrowEvent (GEventType eType, object[] eArguments, int priority) {
		EventManager.Instance.AddEvent (CreateEvent (eType, eArguments, priority));
	}

	
}
