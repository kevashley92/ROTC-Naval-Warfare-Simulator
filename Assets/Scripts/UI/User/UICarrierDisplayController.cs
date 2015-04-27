using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*****
 * 
 * Name: UICarrierDisplayController
 * 
 * Date Created: 2015-03-XX
 * 
 * Original Team: UI
 * 
 * This class holds functionality needed to update a carrier's unit broswing panel
 * 
 */
public class UICarrierDisplayController : MonoBehaviour {

	/**
	 * The carrier being inspected
	 */
	private GameObject Carrier;
	/**
	 * The gameobject that displays the carrier's name
	 */
	public GameObject CarrierNameDisplay;
	/**
	 * The gameobject that displays the sprite of the carrier's currently selected held unit
	 */
	public GameObject UnitPreviewDisplay;
	/**
	 * The gameobject that displays the name of the carrier's currently selected held unit
	 */
	public GameObject UnitNameDisplay;
	/**
	 * The gameobject that displays the current index being viewed of the carrier's held units
	 */
	public GameObject UnitNavigatorDisplay;
	/**
	 * The list of the carrier's held units
	 */
	private List<string> Units = new List<string>();
	/**
	 * The current index being viewed of the carrier's held units
	 */
	private int CurrentUnitIndex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Carrier != null){
			Units = Carrier.GetComponent<ContainerController> ().GetUnits ();
			if(Units.Count > 0){
				UnitNavigatorDisplay.GetComponent<Text>().text = "" + (CurrentUnitIndex + 1) + " of " + Units.Count;
				if(CurrentUnitIndex >= Units.Count || CurrentUnitIndex < 0) {
					CurrentUnitIndex = 0;
				}
				string UnitGUID = Units[CurrentUnitIndex];
				UnitNameDisplay.GetComponent<Text>().text = GuidList.GetGameObject(UnitGUID).GetComponent<IdentityController>().GetName();
				UnitPreviewDisplay.GetComponent<Image>().enabled = true;
				UnitPreviewDisplay.GetComponent<Image>().sprite = GuidList.GetGameObject(UnitGUID).GetComponent<SpriteRenderer>().sprite;
			}
			else{
				UnitNavigatorDisplay.GetComponent<Text>().text = "No Units";
				UnitNameDisplay.GetComponent<Text>().text = "";
				UnitPreviewDisplay.GetComponent<Image>().enabled = false;
			}
		}
	}

	/**
	 * Starts displaying the carrier's units.
	 * 
	 * @param Unit
	 * 		The carrier to display units for.
	 */
	public void Display(GameObject Carrier){
		this.Carrier = Carrier;
		CarrierNameDisplay.GetComponent<Text> ().text = Carrier.GetComponent<IdentityController>().GetName();
		CurrentUnitIndex = 0;
	}
	/**
	 * Stops displaying units for the carrier
	 * 
	 */
	public void StopDisplaying(){
		this.Carrier = null;
		Units.Clear();
		UnitNavigatorDisplay.GetComponent<Text> ().text = "";
		UnitNameDisplay.GetComponent<Text> ().text = "";
		UnitPreviewDisplay.GetComponent<Image> ().sprite = null;
	}

	/**
	 * Displays the next unit if there is one
	 */
	public void NextUnit(){
		CurrentUnitIndex++;
		if(CurrentUnitIndex >= Units.Count){
			CurrentUnitIndex = 0;
		}
	}
	/**
	 * Displays the previous unit if there is one
	 */
	public void PreviousUnit(){
		CurrentUnitIndex--;
		if(CurrentUnitIndex <  0){
			CurrentUnitIndex = Units.Count - 1;
		}
	}
	/**
	 * Deploys the unit currently being viewed if there is one
	 * 
	 */
	public void DeployUnit(){
		if (Units.Count > 0) {
			Carrier.GetComponent<ContainerController>().Launch(Units[CurrentUnitIndex]);
			NextUnit();
		}
	}
}
