/*****
 * 
 * Name: EmbarkerController	
 * 
 * Date Created: 2015-03-16
 * 
 * Original Team: Gameplay
 * 
 * A Controller to implement the ability to embark embarkable units.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * T. Dill	    2015-03-16	Creation
 * D. Durand    2015-03-17  Made Launch Virtual so children can override it. Also refueled planes on Embark()
 * D. Durand    2015-03-17  Made SetValues() and GetValues
 * D. Durand    2015-03-24  Overloaded Launch and threw the Embarks events.
 */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EmbarkerController : Controller {
	/**
	 * Weather or not this component is in use
	 */
	private bool InUse = false;

	/*
	 * State of unit.
	 */
	public bool IsInSomething = false;

	/*
	 * Guid of the container that contains this unit, if there is one, else null.
	 */ 
	private string Parent = "";

	/** Sets an individual value after admin change; should be unused in this controller
	 */
	public override void SetValue(string paramName,object value) {

	}

	/*
	 * Sets the container to contain this unit, and changes state, if the unit is within range of the target container.
	 * 
	 * @param parent
	 * 		The container that will contain this unit.
	 */
	public void Embark(String parent) {
        //Debug.Log ("HERE:" + parent);

        GameObject g = GuidList.GetGameObject (parent);
        if (g.GetComponent<ContainerController> ().GetInUse ()) {
            if (Vector3.Distance (this.transform.position, GuidList.GetGameObject (parent).transform.position) < this.gameObject.GetComponent<MoverController> ().MoveRange) {
                // throw the UnitEmbarksEvent event
                object[] arguments = new object[2];
                arguments [0] = this.gameObject.GetComponent<IdentityController> ().GetGuid ();
                arguments [1] = GuidList.GetGameObject (parent).GetComponent<IdentityController> ().GetGuid ();
                Parent = parent;
                //Debug.Log ("HEREERE: "  + Parent);
                EventManager.Instance.AddEvent (EventFactory.CreateEvent (GEventType.UnitEmbarksEvent, arguments, 1));
            }
            gameObject.GetComponent<CircleCollider2D> ().enabled = false;
            gameObject.GetComponent<SpriteRenderer> ().enabled = false;
        }
	}

	/*
	 * Releases the unit from its container at the parent units position.
	 * 
	 */
	public void LaunchEventHelper(string embarker, Vector3 pos, string parent) {
		IsInSomething = false;
		// Offset unit so it can be selected, values based on current unit size in editor
		int XRandom = UnityEngine.Random.Range (0, 2);
		int YRandom = UnityEngine.Random.Range (0, 2);
		int XOffset, YOffset;
        Parent = Parent;
		if (gameObject.GetComponent<MarineMover> () != null) {
			if(XRandom == 0){
				XOffset = -1;
			}
			else{
				XOffset = 1;
			}
			if(YRandom == 0){
				YOffset = -1;
			}
			else{
				YOffset = 1;
			}
			gameObject.transform.position = GuidList.GetGameObject(Parent).transform.position + new Vector3(XOffset, YOffset, 0);
		}
		else {
			if(XRandom == 0){
				XOffset = -5;
			}
			else{
				XOffset = 5;
			}
			if(YRandom == 0){
				YOffset = -5;
			}
			else{
				YOffset = 5;
			}
            float f = gameObject.transform.position.z;
            if(GuidList.GetGameObject(Parent) != null){
			    gameObject.transform.position = GuidList.GetGameObject(Parent).transform.position + new Vector3(XOffset, YOffset, 0);
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, f);
            }
		}
		gameObject.GetComponent<CircleCollider2D> ().enabled = true;
		gameObject.GetComponent<SpriteRenderer> ().enabled = true;
        if (GuidList.GetGameObject (Parent) != null) {
            GuidList.GetGameObject (Parent).GetComponent<ContainerController> ().Remove (embarker);
        }
		Parent = "";
	}

	/**
	 * Takes all the logical actions of Embarking a unit.
	 * This method should only be called from the appropiate EventHandler.
	 * 
	 * @param container
	 * 		The container that this unit has been Embarked into.
	 * 
	 * @param embarker
	 * 		The Guid of this unit. Used as a shortcut to getting the Guid from this unit's IdentityController. 
	 */
	public void EmbarkEventHelper(string container, string embarker) {
		IsInSomething = true;
		Parent = container;
		AirCraftMover acm;
		// If the Embarker is an Air Craft we need to 'refuel it' by resetting the appropriate variables
		if ((acm = (AirCraftMover)this.gameObject.GetComponent<AirCraftMover> ()) != null) {
			acm.Land ();
		}
		gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		GuidList.GetGameObject (container).GetComponent<ContainerController> ().Add (embarker);
	}


	/**
	 * Returns the containment for this unit.
	 * @return
	 * 		The state of containment for this unit.
	 */
	public bool getIsInSomething(){
		return IsInSomething;
	}

	/** 
	 * Set up a new controller
	 * 
	 * @param values
	 * 		A json friendly dictionary containing the parameters for
	 * 		this controller
	 */
	public override void SetValues(IDictionary values) {
        IsInSomething = Convert.ToBoolean (values ["state"]);
        InUse = Convert.ToBoolean(values ["inuse"]);

		Parent = values["parent"] as String;
	}
	
	/**
     *Gets the values of the controller.
     *
     * @return
     *   A dictionary containing the json friendly values of the controller.
     */
	public override IDictionary GetValues() {
			Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();
            toReturn ["state"] = IsInSomething;

            toReturn ["inuse"] = InUse;

			toReturn ["parent"] = Parent;
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

	// TODO
	public override void SetValuesMarines(IDictionary values) {}
	public override void SetValuesNavy(IDictionary values) {}
	
}
