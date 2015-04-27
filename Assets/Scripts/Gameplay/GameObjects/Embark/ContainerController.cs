/*****
 * 
 * Name: ContainerController	
 * 
 * Date Created: 2015-03-16
 * 
 * Original Team: Gameplay
 * 
 * A Controller to implement the embarking and containment of other units with a unit.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * T. Dill	    2015-03-16	Creation
 * D. Durand    2015-03-17  Made Embarkers Protected and Recieve virtual for child classes
 * D. Durand    2015-03-20  Created SetValues() and GetValues();
 * A. Smith     2015-04-07  Added GetCount()
 */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ContainerController : Controller {
	/**
	 * Weather or not this component is in use
	 */
	private bool InUse = false;

	/**
	 * The units currently contained within the container.
	 */
    protected List<String> Embarkers;
	
	/**
	 * Initializes the list of Embarked units in this container.
	 */
	void Start() {
        Embarkers = new List<String>();
	}
	
	public List<String> GetUnits(){
		return Embarkers;
	}

	/** Sets an individual value after admin change; should be unused in this controller
	 */
	public override void SetValue(string paramName,object value) {

	}
	
	/**
	 * Adds a unit to this container if it is within range to do so.
	 * @param unit
	 * 		The unit to be added.
	 */
	public virtual void Receive(String unit) {
		GuidList.GetGameObject (unit).GetComponent<EmbarkerController> ().Embark(this.gameObject.GetComponent<IdentityController>().MyGuid);
	}

	/**
	 * Adds a unit to Embarkers
	 * 
	 * @param unit
	 * 		The unit to be added.
	 */
	public void Add(string unit) {
        if (InUse){
            if (!Embarkers.Contains (unit)) {
                this.Embarkers.Add (unit);
            }
        }

	}

	/**
	 * Removes a unit from Embarkers
	 * 
	 * @param unit
	 * 		The unit to be removed.
	 */
	public void Remove(string unit) {
		this.Embarkers.Remove (unit);
	}
	
	/**
	 * Removes a unit from this Container, changes state of unit to unembarked, and moves it to the container's position.
	 * @param unit
	 * 		The unit to be removed
	 */
	public void Launch(String unit) {
		Launch(unit, this.transform.position);		
	}

	/**
	 * Removes a unit from this Container, changes state of unit to unembarked, and moves it to the given position.
	 * @param unit
	 * 		The unit to be removed
	 */
	public void Launch(String unit, Vector3 pos) {
		float PercentHealth = ((HealthController)this.gameObject.GetComponent ("HealthController")).GetCurrentPercentHealth();
		//TODO: determine if we want the health restriction for just aircraft or all contained units
		if(PercentHealth >= GlobalSettings.GetHealthThresholdForNoDetectors()){
			GameObject u = GuidList.GetGameObject(unit);
			if(Vector3.Distance(pos, this.transform.position) < u.GetComponent<MoverController>().MoveRange) {
				// throw the UnitUnEmbarksEvent event
				object[] arguments = new object[3];
				arguments[0] = unit;
				arguments[1] = new Point((int)pos.x, (int)pos.y, (int)pos.z);
                arguments[2] = this.gameObject.GetComponent<IdentityController>().GetGuid();
				
				EventManager.Instance.AddEvent(EventFactory.CreateEvent(GEventType.UnitUnEmbarksEvent, arguments, 1));
			}
		}
	}


	/** 
	 * Set up a new controller
	 * 
	 * @param values
	 * 		A json friendly dictionary containing the parameters for
	 * 		this controller
	 */
	public override void SetValues(IDictionary values) {
        int size = Convert.ToInt32(values["embarkers-count"]);
        Embarkers = new List<String>();
        
        for (int i = 0; i < size; i++)
        {
            Embarkers.Add(Convert.ToString(values["embarkers-" + i]));
            //Debug.Log("Embarker added: " + Embarkers[i]);
        }
        InUse = Convert.ToBoolean(values["inuse"]);
        //Debug.Log("Embarker size " + gameObject.name + " " + gameObject.GetComponent<IdentityController>().GetGuid() + " : " + Embarkers.Count);
	}

	/**
     *Gets the values of the controller.
     *
     * @return
     *   A dictionary containing the json friendly values of the controller.
     */
	public override IDictionary GetValues() {
		Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();
		toReturn ["embarkers-count"] = Embarkers.Count;
        for (int i = 0; i < Embarkers.Count; i++)
        {
            toReturn["embarkers-" + i] = Embarkers[i];
        }
        toReturn["inuse"] = InUse;
		return toReturn;
	}

	/**
	 * Changes the variable that knows weather or not this component is InUse
	 */
	public void ToggleInUse() {
		InUse = !InUse;
	}

	/**
	 * Get weather or not this component is in use
	 */
	public bool GetInUse() {
		return InUse;
	}

    /**
     * Gets the number of units on the container
     * 
     * @return
     *      The number of units contained
     */
    public int GetCount() {
        if (Embarkers != null)
            return Embarkers.Count;
        else
            return 0;
    }

	// TODO
	public override void SetValuesMarines(IDictionary values) {}
	public override void SetValuesNavy(IDictionary values) {}
	
	
}
