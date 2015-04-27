/*****
 * 
 * Name: UIUnitStateController
 * 
 * Date Created: 2015-01-XX
 * 
 * Original Team: UI
 * 
 * This class holds functionality needed to activate/deactivate unit specific UI visual effects, must be attached to every unit game object
 * 
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIUnitStateController : MonoBehaviour {

	/**
	 * Whether or not the unit is currently selected
	 */
	private bool Selected = false;
	/**
	 * Whether or not the unit is currently targeted
	 */
	private bool Targeted = false;
	/**
	 * The game object that renders the mouse over sprite effect
	 */
	private GameObject MouseOver;
	/**
	 * The game object that renders the selection sprite effect
	 */
	private GameObject Select;
	/**
	 * The game object that renders the attack pulse animation
	 */
	private GameObject AttackPulse;

	// Use this for initialization
	void Start () {
		MouseOver = gameObject.transform.FindChild ("Hover Sprite").gameObject;
		Select = gameObject.transform.FindChild ("Selection Sprite").gameObject;
		AttackPulse = gameObject.transform.FindChild ("Attack Pulse").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * Toggles the attack pulse animation on or off
	 */
	public void ToggleTargeted(){
		if (AttackPulse.GetComponent<SpriteRenderer> ().enabled) {
			AttackPulse.GetComponent<SpriteRenderer> ().enabled = false;
		} 
		else {
			AttackPulse.GetComponent<SpriteRenderer>().enabled = true;
		}
	}

	/**
	 * Checks if the unit is currently selected
	 * @return
	 * 		true if it is selected, false if it isn't
	 */
	public bool IsSelected(){
		return Selected;
	}

	/**
	 * Sets the unit as being slected or not
	 * @param selected
	 * 		whether or not the unit is selected
	 */
	public void SetSelected(bool Selected){
		this.Selected = Selected;
	}

	/**
	 * Gets the game object that renders the mouse over sprite effect
	 * @return
	 * 		the game object that renders the mouse over sprite effect
	 */
	public GameObject GetMouseOver(){
		return MouseOver;
	}

	/**
	 * Gets the game object that renders the selection sprite effect
	 * @return
	 * 		the game object that renders the selection sprite effect
	 */
	public GameObject GetSelect(){
		return Select;
	}

	/**
	 * Gets the game object that renders the attack pulse animation
	 * @return
	 * 		the game object that renders the attack pulse animation
	 */
	public GameObject GetAttackPulse(){
		return AttackPulse;
	}
}
