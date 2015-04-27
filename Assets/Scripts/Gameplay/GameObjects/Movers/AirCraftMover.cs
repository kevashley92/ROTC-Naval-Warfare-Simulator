/*****
 * 
 * Name: AirCraftMover
 * 
 * Date Created: 2015-02-27
 * 
 * Original Team: Gameplay
 * 
 * This class holds functionality needed specifically for air craft
 * movement functionality.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * D. Durand	2015-02-27	Creation
 * T. Brennan	2015-03-15	Changed GUID to string
 * D. Durand    2015-03-17  Created A method for 'refueling the air craft'. Also made it die when airborne too long
 */ 
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class AirCraftMover : MoverController {
	/**
	 * The range of this unit as of this turn.
	 * If the unit is at it's home this should equal MoverController.MoveRange
	 * If not, then this should be MoverController.MoveRange - DistanceTraveled
	 */
	private float CurrentRange;
	
	/**
	 * The number of turns in the air, for fuel purposes
	*/
	private int TurnsNotLanded;
	
	/**
	 * The max of turns in the air, for fuel purposes
	*/
	public int MaxTurnsAirborne;
	

	/**
	 * True if this aircraft is at on runway 
	 */
	//private bool IsLanded;

	/**
	 * The default state of an Air Craft is that it is landed.
	 * If this is not the case TakeOff() should be called.
	 */
	//void Start() {
		//this.IsLanded = true;
	//}

	/** 
	 * Set up a new controller
	 * 
	 * @param values
	 * 		A json friendly dictionary containing the parameters for
	 * 		this controller
	 */
	public override void SetValues(IDictionary values) {
		CurrentRange = Convert.ToSingle(values["maxmove"]);
		if (values.Contains("fuel_limit")) {
			MaxTurnsAirborne = Convert.ToInt32(values["fuel_limit"]);
		}
		else {
			MaxTurnsAirborne = 5;
		}

		TurnsNotLanded = 0;
		base.SetValues (values);
	}

	public override void SetValuesMarines(IDictionary values) {
		SetValues(values);
	}
	public override void SetValuesNavy(IDictionary values) {
		SetValues(values);
	}

	public override IDictionary GetValues() {
		Dictionary<string, System.Object> toReturn = (Dictionary<string, System.Object>)base.GetValues();

		toReturn["maxmove"] = CurrentRange;
		toReturn["fuel_limit"] = MaxTurnsAirborne;
		TurnsNotLanded = 0;
	
		return toReturn;
	}

	/** Sets an individual value after admin change
	 */
	public override void SetValue(string paramName,object value) {
		// Possible behaviors are setting turns since landing and max turns airborne
		// For turns since landing, paramName is "turns" and value is desired number of turns
		// For max turns airborne, paramName is "maxturns" and value is desireed number of turns
		base.SetValue(paramName,value);

		if(paramName.ToLower().Equals("turns")) {
			TurnsNotLanded = (int) value;
		}
		else if(paramName.ToLower().Equals("maxturns")) {
			MaxTurnsAirborne = (int) value;
		}


	}

	public override void PlanMove (Vector3 newPos) {
		// Create the MoveEvent Event
		base.PlanMove (newPos);
	}

	/**
	 * TODO will be called every turn regardless of weather it is moved or not in order to count turns in air
	 */
	public override void MoveAsPlanned(Vector3 newPos) {
		
		TurnsNotLanded++;
		if(TurnsNotLanded >= MaxTurnsAirborne){
			// Create a DieEvent
			object[] arguments = new object[1];
			arguments[0] = (this.gameObject.GetComponent<IdentityController>()).GetGuid();
			EventManager.Instance.AddEvent(EventFactory.CreateEvent(GEventType.DieEvent, arguments));
		}
		else{
		
			// Modify the CurrentRange by the distance traveled
			CurrentRange -= Vector3.Distance(newPos,this.gameObject.transform.position);

			// If the unit has moved outside of its current range
			if (CurrentRange < 0) {
				// Create a DieEvent
				object[] arguments = new object[1];
				arguments[0] = (this.gameObject.GetComponent<IdentityController>()).GetGuid();
				EventManager.Instance.AddEvent(EventFactory.CreateEvent(GEventType.DieEvent, arguments));
			} else {
				base.MoveAsPlanned(newPos);
			}
		}
	}

	/**
	 * Gets a Guid list of Runways that are in the range of this air craft
	 * Return: List<Guid>; A list of Runways that are in range of this air craft.
	 */
	public List<string> RunwaysInRange() {
		return Runway.RunwaysInRange (this.transform.position, CurrentRange);
	}

	/**
	 * Resets variables that keep track of fuel status
	 */
	public void Land() {
		this.CurrentRange = this.GetMoveRange ();

		this.TurnsNotLanded = 0;
	}

	/**
	 * Changes the state of this air craft to not be landed.
	 * If you are trying to make an aircraft take off you should not call this method.
	 * Use Runway.LaunchAirCraft(Guid airCraft) instead.
	public void TakeOff(Vector3 pos) {
		IsLanded = false;
		PlanMove (pos);
	}

	/**
	 * Lands the aircraft on a particullar Runway and updates the Runways List of air crafts
	 * Param: Guid runway; The Runway to land on.
	 *
		// Verify that the given guid is actually a runway
		Runway run;
		if ( (run = GuidList.GetGameObject (runway).GetComponent<Runway> ()) != null) {
			// Since the air craft has landed we can reset it's CurrentRange.
			CurrentRange = MoveRange;
			IsLanded = true;
			// Get the Runway component of the given object and add this air craft to it
			run.ReceiveUnit (this.gameObject.GetComponent<IdentityController> ().GetGuid ());
		}
	}

	*/

	public float GetCurrentRange(){
		return CurrentRange;
	}
	/**
	 * TODO Fill this in
	 */
	public void DrawRange() {
		transform.FindChild ("Move Range Sprite").transform.localScale = new Vector3( (float)(.0033 * CurrentRange), (float)(.0033 * CurrentRange), 1);
		//Debug.Log ("Drawing move range of " + CurrentRange);
		transform.FindChild ("Move Range Sprite").gameObject.SetActive (true);
		
	}
	
	/**
	 * TODO Fill this in
	 */
	public void DeleteCircle() {
		transform.FindChild ("Move Range Sprite").gameObject.SetActive (false);
	}
}

