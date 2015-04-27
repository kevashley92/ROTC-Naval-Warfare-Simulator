/*****
 * 
 * Name: SubmarineMover
 * 
 * Date Created: 2015-03-06
 * 
 * Original Team: Gameplay
 * 
 * This class holds functionality needed specifically for submarine
 * movement functionality.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * M. Schumacher2015-03-06  Inital commit
 */ 
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class SubmarineMover : NavyMover {
	
	//whether the sub is surfaced or not
	private bool Surfaced;
	
	public override void SetValues(IDictionary values) {

		Surfaced = (bool)values ["Surfaced"];
		base.SetValues (values);
	}

	/** Sets an individual value after admin change
	 */
	public override void SetValue(string paramName,object value) {
		// Possible behaviors are setting Surfaced.
		// For Surfaced, paramName is "surfaced" and value is the desired bool.
		base.SetValue(paramName,value);

		if(paramName.ToLower().Equals("surfaced")) {
			Surfaced = (bool) value;
		}
	}
	
	public override void SetValuesMarines(IDictionary values) {
		SetValues (values);
	}
	
	public override void SetValuesNavy(IDictionary values) {
		SetValues (values);
	}

	public override IDictionary GetValues() {
		Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();
		
		toReturn ["surfaced"] = Surfaced;
		
		return toReturn;
	}
	
	public bool GetSurfaced(){
		return Surfaced;
	}
	
	public void ToggleSurfaced(){
		if(Surfaced == true){
			Surfaced = false;
		}
		else{
			Surfaced = true;
		}
		
		//TODO anything else that is needed to change gameplay between the two states
		
	}
	
	
}

