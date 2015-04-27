/*****
 * 
 * Name: AdminActionController
 * 
 * Date Created: 2015-02-11
 * 
 * Original Team: Gameplay
 * 
 * This class will store behavior related to admin abilities
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * M.Schumacher	2015-02-11	Initial Creation
 * B. Croft		2015-02-14	Add empty SetValues method
 * t. Brennan	2015-02-17	Refactored
 * S. Lang		2015-03-16	Refactored to include new abstract methods
 */ 
using UnityEngine;
using System.Collections;
using System;


public class AdminActionController : Controller {

	/**
	 * TODO Fill this in
	 * 
	 * @param values
	 * 		A json friendly dictionary with parameters for this
	 * 		controller
	 */
	public override void SetValues(IDictionary values) {

	}

	// TODO
	public override void SetValuesNavy(IDictionary values) {}
	public override void SetValuesMarines(IDictionary values) {}
    public override IDictionary GetValues()
    {
        throw new NotImplementedException();
    }

	/** lol
	 */
	public override void SetValue(string paramName,object value) { }


	/**
	 * TODO Fill this in
	 * 
	 * @param newUnitId
	 * 		TODO Fill this in
	 * @param xpos
	 * 		TODO Fill this in
	 * @param ypos
	 * 		TODO Fill this in
	 */
	public void ThrowUnitSpawnEvent(String newUnitID, float xpos, float ypos){

		//TODO should we actually create the new object here or somewhere else?
		
		object[] arguments = new object[3];
		arguments[0] = newUnitID;
		arguments[1] = xpos;
		arguments[2] = ypos;
			
		EventFactory.ThrowEvent(GEventType.AdminSpawnEvent, arguments);
		
	}


	/**
	 * TODO Fill this in
	 * 
	 * @param unitID
	 * 		TODO Fill this in
	 * @param field
	 * 		TODO Fill this in
	 * @param newValue
	 * 		TODO Fill this in
	 */
	public void ThrowStatisticChangeEvent(string unitGUID, string controllerName, string paramName, object value){

		object[] arguments = new object[4];
		arguments[0] = unitGUID;
		arguments[1] = controllerName;
		arguments[2] = paramName;
		arguments[3] = value;
			
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
		
	}
	
}

