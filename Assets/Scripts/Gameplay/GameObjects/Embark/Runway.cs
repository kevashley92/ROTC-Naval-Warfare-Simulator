/*****
 * 
 * Name: Runway
 * 
 * Date Created: 2015-02-27
 * 
 * Original Team: Gameplay
 * 
 * An Identity Exension that describes units which are able to Carry aircraft.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * D. Durand	2015-02-27	Creation
 * D. Durand    2015-03-02  Changed GetRunwayAt to RunwaysInRange and added documentation.
 * T. Dill		2015-03-15	Added Health threshold for launching aircraft.
 * T. Brennan	2015-03-15	Changed GUID to string
*/
using System;
using System.Collections.Generic;
using UnityEngine;

public class Runway : ContainerController {
	/**
	 * All of the runways in the game
	 */
	private static List<string> Runways;

	/**
		* All of the air crafts that are docked on this runway
	 */
	//private List<Guid> AirCrafts;

	/**
	 * Instantiates Runways if it doesn't exist.
	 * Adds this Runway to Runways.
	 */
	void Start() {
		if (Runways == null) {
			Runways = new List<string>();
		}
		Runways.Add( ((IdentityController)this.gameObject.GetComponent("IdentityController")).GetGuid() );
	}


	/**
	 * Finds any runways that are in range of a given position
	 * @param pos
	 * 		The position to to check against. Intended to be the position of an air craft
	 * 
	 * @param range
	 * 		The range to campre with. Intended to be the CurrenRange of an AirCraft
	 * 
	 * @return
	 * 		A List of Runways that are in range of the position
	 */
	public static List<string> RunwaysInRange(Vector3 pos, float range) {
		List<string> runways = new List<string>();
		foreach (string guid in Runways) {		
			if (Vector3.Distance(GuidList.GetGameObject(guid).transform.position,pos) <= range) {
				runways.Add(guid);
			}
		}
		return runways;
	}

	/**
	 * Adds an air craft to this Runway.
	 * If you are trying to make an aircraft land you shouldn't use this method.
	 * Use AirCraftMover.Land(Guid runway) instead.
	 * @param airCraft
	 * 		The aircraft to be added.
	 *
	public void ReceiveAirCraft(Guid airCraft) {
		if(!AirCrafts.Contains(airCraft)) {
			this.AirCrafts.Add (airCraft);
		}
	}
	*/

	/**
	 * Removes an air craft from this Runway  and change it's state to not being landed.
	 * @param airCraft
	 * 		The air craft to be removed
	 *
	public void LaunchAirCraft(Guid airCraft) {
		float PercentHealth = ((HealthController)this.gameObject.GetComponent ("HealthController")).GetCurrentPercentHealth();

		if(PercentHealth >= GlobalSettings.Instance().HealthThresholdForNoDetectors){
			this.AirCrafts.Remove (airCraft);
			GuidList.GetGameObject (airCraft).GetComponent<AirCraftMover> ().TakeOff ();
		}


	}
	*/


}