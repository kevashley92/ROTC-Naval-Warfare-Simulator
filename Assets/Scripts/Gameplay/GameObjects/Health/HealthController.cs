/*****
 * 
 * Name: HealthController
 * 
 * Date Created: 2015-01-28
 * 
 * Original Team: Gameplay
 * 
 * This class will manage all actions associated with a unit taking damage.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-01-28	Initial Commit. Added Health and taking
 * 							damage. Outline unit dying.
 * T. Brennan   2015-01-31  Added call to Destroy in killUnit method.
 * 							Changed Datatype of health to float. Changed
 * 							curHealth to protected so that childern can
 * 							access it.
 * T. Brennan	2015-02-02	Refactored dealDamage to damageUnit. Added
 * 							healUnit and setHealth
 * T. Brennan	2015-02-03	Changed killUnit to be virtual so that child
 * 							classes can overwrite it without raising a
 * 							warning
 * M.Schumacher	2015-02-09	Added Event throwing 
 * B. Croft		2015-01-14	Added SetValues method
 * T. Brennan	2015-02-17	Refactored. Deleted unused Update method.
 * 							Added getters and setters for CurrentHealth
 * 							and MaxHealth
 * B. Croft		2015-03-05	Added Dying field and implementation
 * M.Schumacher	2015-03-12	Added method to get current percent health
 * James Woods	2015-03-13	Added GetValues method for DataIntegration.
 * T. Brennan	2015-03-19	Added SetValuesNavy and SetValuesMarines
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class HealthController : Controller
{

	
	/**
	 * The base value for the health that comes from the DI Team
	 */
	public float MaxHealth;

	/**
	 * The current level of health for this particular unit
	 */
	public float CurrentHealth;

	/* Set to true after the unit has created an event to die so it doesn't try to destroy itself multiple times. */
	private bool Dying;

	/**
	 * Use this for initialization
	 */
	void Start ()
	{

		// Set the initial value for health to be the default fromt the data
		// Integration Team's API
		CurrentHealth = MaxHealth;
		Dying = false;

	}


	/**
	 * Sets the values in a new controller.
	 * 
	 * @param values
	 * 		A json friendly dictionary of parameters for this controller
	 */
	public override void SetValues (IDictionary values)
	{
		CurrentHealth = Convert.ToSingle (values ["currenthealth"]);
		MaxHealth = Convert.ToSingle (values ["defense"]);
	}


	// TODO
	public override void SetValuesNavy (IDictionary values)
	{
		SetValues (values);
	}
	public override void SetValuesMarines (IDictionary values)
	{
		SetValues (values);
	}

	/**
     *Gets the values in teh controller.
     *
     * @return
     *      A json friendly dictionary.
     */
	public override IDictionary GetValues ()
	{
		Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object> ();
		toReturn ["currenthealth"] = CurrentHealth;
		toReturn ["defense"] = MaxHealth;
		return toReturn;
	}

	/** Sets an individual value after admin change
	 */
	public override void SetValue (string paramName, object value)
	{
		// Possible behaviors are set max and current health
		// paramName is "maxhealth" and "currenthealth" respectfully and value is desired value

		if (paramName.ToLower ().Equals ("maxhealth"))
		{
			SetMaxHealth ((float)value);
		}
		else if (paramName.ToLower ().Equals ("currenthealth"))
		{
			SetCurrentHealth ((float)value);
		}
	}

	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowMaxHealthChangeEvent (float value)
	{
		object[] arguments = new object[4];
		arguments [0] = this.gameObject.GetComponent<IdentityController> ().GetGuid ();
		arguments [1] = "HealthController";
		arguments [2] = "maxhealth";
		arguments [3] = value;
		
		EventFactory.ThrowEvent (GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowCurrentHealthChangeEvent (float value)
	{
		object[] arguments = new object[4];
		arguments [0] = this.gameObject.GetComponent<IdentityController> ().GetGuid ();
		arguments [1] = "HealthController";
		arguments [2] = "currenthealth";
		arguments [3] = value;
		
		EventFactory.ThrowEvent (GEventType.AdminStatisticChangeEvent, arguments);
	}



	/**
	 * Allows the unit to take damage. Checks if the unit should die after
	 * damage is dealt.
	 * 
	 * @param damage
	 * 		The amount of damage the unit will take
	 */
	public void DamageUnit (float damage, string causingTeam)
	{

		// Deal damage to the unit
		SetCurrentHealth (GetCurrentHealth () - damage);

		// Check if the unit shoud die
		if (GetCurrentHealth () <= 0 && !Dying)
		{
			// If the unit should die

			// throws the event for death
			object[] arguments = new object[5];
			arguments [0] = (this.gameObject.GetComponent<IdentityController> ()).GetGuid ();
			arguments [1] = causingTeam;
			arguments [2] = this.gameObject.GetComponent<IdentityController> ().GetName ();
			arguments [3] = Team.Teams[this.gameObject.GetComponent<IdentityController> ().GetTeam ()].GetTeamName() ;
			arguments [4] = this.gameObject.GetComponent<IdentityController> ().GetFullName();
				
			EventManager.Instance.AddEvent (EventFactory.CreateEvent (GEventType.DieEvent, arguments));
			Dying = true;

		}

	}


	/**
	 * Allows the unit to be healed up to its starting health
	 * 
	 * @param restore	The amount of health that is to be restored
	 */
	public void HealUnit (float restore)
	{

		//Add health back to the unit
		SetCurrentHealth (GetCurrentHealth () + restore);

		// Check if the current health exceeds the default health
		if (GetCurrentHealth () > GetMaxHealth ())
		{
			// If the health does exceed the max

			// Set it to the max
			SetCurrentHealth (GetMaxHealth ());

		}

	}


	/**
	 * Governs what happens when the unit dies. 
	 * 
	 * This will call the destroy method of the parent class,
	 * but throretically we could make it a special unit like a base that
	 * drops something when it is destroyed.
	 */
	public virtual void KillUnit ()
	{
		GameObject Canvas = GameObject.Find ("Canvas");
		UIUnitInspectorController Inspector = Canvas.GetComponent<UIUnitInspectorController> ();
		if (Inspector != null)
		{
			Inspector.StopInspecting ();
		}

		if (NetworkManager.NetworkInstance.IsServer)
		{
			NetworkManager.NetworkInstance.DestroyUnitOnNetwork (gameObject);
		}
		
	}


	/**
	 * Gets the current health of the object.
	 * 
	 * @return
	 * 		The current health of the object
	 */
	public float GetCurrentHealth ()
	{

		return CurrentHealth;

	}


	/**
	 * Set the health of the unit to a provided value.
	 * 
	 * NOTE: This is not restricted by base health
	 * 
	 * @param newHealth
	 * 		What the units health should be set to
	 */
	public void SetCurrentHealth (float newCurrentHealth)
	{

		CurrentHealth = newCurrentHealth;

	}


	/**
	 * Gets the max health of the object
	 * 
	 * @return
	 * 		The max health of the object.
	 */
	public float GetMaxHealth ()
	{

		return MaxHealth;

	}


	/**
	 * Sets the max health of the object
	 * 
	 * @param newMaxHealth
	 * 		The new max health of the object
	 */
	public void SetMaxHealth (float newMaxHealth)
	{

		MaxHealth = newMaxHealth;

	}
	
	/**
	 * Gets the current percent health of the object
	 * 
	 * @return currentPercentHealth
	 * 		the current percent health of the object
	 */
	public float GetCurrentPercentHealth ()
	{
		return 100 * CurrentHealth / MaxHealth;
	}
}
