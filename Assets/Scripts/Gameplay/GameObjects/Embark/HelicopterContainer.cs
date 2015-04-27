/*****
 * 
 * Name: Helicopter
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * An Air Craft with the ability to Carry probes.
 *
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * D. Durand	2015-03-14	Initial commit.
 * T. Brennan	2015-03-15	Changed GUID to string
 * D. Durand    2015-03-17  Extended ContainerController and refactored as appropriate.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

public class HelicopterContainer : ContainerController {
	/**
	 * Adds a Probe to this Helicopter.
	 * @param probe
	 * 		The probe to be added.
	 */
	public override void Receive(String probe) {
		if(!Embarkers.Contains(probe)) {
			try{
				((ProbeEmbarker) GuidList.GetGameObject(probe).GetComponent<ProbeEmbarker>()).Embark(((IdentityController) this.GetComponent<IdentityController>()).GetGuid());
				this.Embarkers.Add (probe);
			} catch (KeyNotFoundException) {
				//Do nothing
			}
		}
	}
}