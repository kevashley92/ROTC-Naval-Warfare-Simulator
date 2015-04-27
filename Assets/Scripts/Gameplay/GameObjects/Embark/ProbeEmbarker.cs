/*****
 * 
 * Name: Helicopter
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * An Identity Exension that describes probes which cannot move, have no weapons, but have sonar.
 *
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * D. Durand	2015-03-14	Initial commit.
 * D. Durand    2015-03-17  Extended EmbarkerController and refactored as appropriate.
 */
using System;
using UnityEngine;

public class ProbeEmbarker : EmbarkerController {
	/**
	 * Drop this probe at a given location.
	 * Activate it's sonar if the postion is over water.
	 * @param pos
	 * 		The position to be dropped at
	 *
	public override void Launch() {
		base.Launch ();
		Vector3 pos = this.transform.position;
		if(World.Instance.TerrainAt(new Vector2(pos.x, pos.y)).Name == "WATER") {
			object[] arguments = new object[1];
			arguments[0] = (this.gameObject.GetComponent<IdentityController>()).GetGuid();
			EventManager.Instance.AddEvent(EventFactory.CreateEvent(GEventType.DieEvent, arguments));
		}
	}
	*/
}