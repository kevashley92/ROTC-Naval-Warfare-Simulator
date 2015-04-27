/*****
 * 
 * Name: Carrier
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * An interface for units that can carry other units
 *
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * D. Durand	2015-03-14	Initial commit.
  * T. Brennan	2015-03-15	Changed GUID to string
*/
using System;
using UnityEngine;
using System.Collections.Generic;

public interface Carrier {

	/**
	 * Adds a unit to this Carrier.
	 * @param unit
	 * 		The unit to be added.
	 */
	void ReceiveUnit(string unit);

	/**
	 * Removes a unit from this Carrier
	 * @param unit
	 * 		The unit to be removed
	 */
	void LaunchUnit(string unit);
}


