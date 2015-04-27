/*****
 * 
 * Name: MoverController
 * 
 * Date Created: 2015-02-03
 * 
 * Original Team: Gameplay
 * 
 * This class will store data relevant to a unit's ability to move.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * J.Balik	 	2015-02-03	Initial Creation
 * T. Brennan	2015-02-08	Changed scope of MoveRange to protected
 * 							Made position be a Vector3 to align with
 * 							default transform position type. Added
 * 							getOriginalPosition, getNewPosition,
 * 							setPosition, planMove, and moveAsPlanned
 * M. Schumacher 2015-02-09	Added methods for event interaction
 * T. Brennan	2015-02-11	Made moveAsPlanned a virtual method
 * B. Croft		2015-02-14	Add SetValues method
 * T. Brennan	2015-02-16	Replace originalPosition with the default
 * 							transform position and deleted related
 * 							functions.
 * T. Brennan	2015-02-17	Refactored. Removed Start, Update, and
 * T. Brennan	2015-03-07	Added priority to event creation
 * M. Schumacher2015-03-12	changed the GetMoveRange method to take into 
 *							account weather and damage
 * J. Woods     2015-03-13  Added GetValues method.
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[System.Serializable]

public class MoverController : Controller
{

	/**
	 * The distance that the unit can move.
	 */
	public float MoveRange;

	/**
	 * The future destination of the unit when it's being moved
	 */
	public Point Destination;

    public float x_pos;
    public float y_pos;

    public void Update() {
        x_pos = transform.position.x;
        y_pos = transform.position.y;
    }

	/** 
	 * Set the values in a fresh controller
	 * 
	 * @param values
	 * 		A json friendly dictionary with parameters for initializing
	 * 		this controller
	 */
	public override void SetValues (IDictionary values)
	{

		// yes, the (float) (long) is actually necessary
		float x = Convert.ToSingle(values["x_pos"]);
		float y = Convert.ToSingle(values["y_pos"]);
		//float z = BaseDetectorController.OnSurfaceElevationZ;
		float z = GetZCoordinate(gameObject);
		float dx = Convert.ToSingle (values ["dx"]);
		float dy = Convert.ToSingle (values ["dy"]);
		Vector3 pos = new Vector3(x, y, z);
		this.Destination = new Point (dx, dy, z);
		transform.position = pos;
		MoveRange = Convert.ToSingle(values ["maxmove"]);
	}

	public override void SetValuesNavy(IDictionary values) {
		SetValues(values);
	}
	public override void SetValuesMarines(IDictionary values) {
		SetValues(values);
	}


    /**
     *Gets the values of the controller.
     *
     * @return 
     *      A json friendly dictionary containing the values of the controller.
     */
    public override IDictionary GetValues()
    {
        Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();

        toReturn["x_pos"] = x_pos;
        toReturn["y_pos"] = y_pos;
        toReturn["maxmove"] = MoveRange;
		toReturn["dx"] = Destination.x;
		toReturn["dy"] = Destination.y;

        return toReturn;
    }

	/** Sets an individual value after admin change
	 */
	public override void SetValue(string paramName,object value) {
		// Posible behavior is setting MoveRange.
		// For MoveRange, paramName is "range" and value is desired range
		if(paramName.ToLower().Equals("range")) {
			SetMoveRange((float) value);
		}
	}

	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowRangeChangeEvent(float value){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "MoverController";
		arguments[2] = "range";
		arguments[3] = value;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}


	/**
	 * Creates a move event for the object.
	 * 
	 * @param newPos
	 * 		The position for the object to move to
	 */
	public virtual void PlanMove (Vector3 newPos)
	{
		Destination = new Point (newPos.x, newPos.y, this.gameObject.transform.localPosition.z);
		object[] args = new object[4];
		args [0] = this.gameObject.GetComponent<IdentityController> ().GetGuid ();
		args [1] = newPos.x;
		args [2] = newPos.y;
		args [3] = this.gameObject.transform.localPosition.z;
		EventFactory.ThrowEvent (GEventType.MoveEvent, args, 1);

		object[] args2 = new object[5];
		args2 [0] = this.gameObject.GetComponent<IdentityController> ().GetGuid ();
		args2 [1] = FindObjectOfType<UIMainController> ().GetUser ().GetUsername ();
		args2 [2] = this.gameObject.GetComponent<IdentityController> ().GetName();
		args2 [3] = new Point(this.gameObject.transform.position);
		args2 [4] = new Point(newPos);
		EventFactory.ThrowEvent (GEventType.PlanMoveEvent, args2, 0);
	}


	/**
	 * To be used at the end of a turn to update the position with
	 * information from the event.
	 * 
	 * @param newPos
	 * 		The position from the event
	 */
	public virtual void MoveAsPlanned (Vector3 newPos)
	{

		Vector3 tempPos = newPos;
		tempPos.z = GetZCoordinate(gameObject);
		gameObject.transform.position = tempPos;

		Destination = new Point (0f, 0f, 0f);

        x_pos = tempPos.x;
        y_pos = tempPos.y;
	}

	public static readonly string TagAir = "Air";
	public static readonly string TagSubsurface = "Subsurface";
	public static readonly string TagSurface = "Surface";
	public static readonly string TagMarine = "Marine";
	public static float GetZCoordinate(GameObject obj) {
		if (obj.tag.Equals(TagSubsurface)) {
			return BaseDetectorController.SubsurfaceElevationZ;
		} else if (obj.tag.Equals(TagAir)) {
			return BaseDetectorController.InAirElevationZ;
		}

		return BaseDetectorController.OnSurfaceElevationZ;
	}


	/**
	 * Gets the move range of this object,
	 * this is modified by the weather and current health
	 * 
	 * @return
	 * 		The movement range of this unit
	 */
	public float GetMoveRange ()
	{
		float PercentHealth = ((HealthController)this.gameObject.GetComponent ("HealthController")).GetCurrentPercentHealth();
		
		if(PercentHealth < GlobalSettings.GetHealthThresholdForNoMovement()){
			return 0;
		}
		
		float modifier = 1;
		
		if(PercentHealth < GlobalSettings.GetHealthThresholdForHalfMovement()){
			modifier *= .5f;
		}
		
		modifier *= Weather.GetMovementModifier(GlobalSettings.GetCurrentWeatherIndex());
		
		
		return this.MoveRange * modifier;
		
	}
	public float GetIdealMoveRange(){
		return this.MoveRange;
	}


	/**
	 * Sets the move range of the object
	 * 
	 * @param newRange
	 *		The new range in which the object can move.
	 */
	public virtual void SetMoveRange (float newRange)
	{

		MoveRange = newRange;

	}

}

