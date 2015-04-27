/*****
 * 
 * Name: AttackController
 * 
 * Date Created: 2015-01-31
 * 
 * Original Team: Gameplay
 * 
 * This class will manage all actions associated with a unit attacking.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-01-31	Initial Commit. Added Weapon Dictionary.
 * 							Added fireWeapons
 * T. Brennan 	2015-02-02	Added getWeapon, getWeapons, addWeapon, and
 * 							formatKey. Fixed commenting.
 * T. Brennan	2015-02-03	Fixed the infinite loop in addWeapon. Made
 * 							successfulHit and targetInRange virtual.
 * M.Schumacher				Started integrating with events
 * M.Schumacher	2015-02-12	Added method to clear targets
 * M.Schumacher	2015-02-13	Fixed bugs and updated to work with guid system. Index system changed
 * B. Croft		2015-02-14	Add SetValues method
 * B. Croft		2015-02-16	Merge event and unit system finalization.
 * T. Brennan	2015-03-07	Added priority to event call
 * T. Brennan 	2015-03-19	Updated GetValues and SetValues
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;

[System.Serializable]
public class AttackController : Controller {
	
	/**
	 * The map of all weapons that are available for use
	 */
	public Dictionary<string,Weapon> Weapons = new Dictionary<string, Weapon>();

	/**
	 * Sets the values of a new controller.
	 * Adds weapons from the list provided.
	 * 
	 * @param values
	 * 		A json friendly version of a dictionary with the default
	 * 		values for the attack controller
	 */
	public override void SetValues(IDictionary values) {

		IList WeaponList = (IList) values["weapons"];

		Weapons.Clear();
		foreach(IDictionary IWeapon in WeaponList) {

			string name = (string)IWeapon["name"];
			string wepType = (string)IWeapon["type"];
			string Key = GenerateKey (name);
			//TODO add correct amounts of maxshots and maxshots per turn
			float range = Convert.ToSingle(IWeapon["range"]);
			float lethality = Convert.ToSingle(IWeapon["lethality"]);
			float probHit = Convert.ToSingle(IWeapon["prob_of_hit"]);
			// probably string
			int count = Convert.ToInt32(IWeapon["count"]);
            int curammo = (IWeapon.Contains("curammo") ? Convert.ToInt32(IWeapon["curammo"]) : count);
			int maxRate = Convert.ToInt32(IWeapon["max_rate"]);

			Weapon NewWeapon;

			if(IWeapon.Contains("targets")) {
				IDictionary idict = (IDictionary)IWeapon["targets"];
				Dictionary<string, int> temp = new Dictionary<string, int>();
				foreach (object key in idict.Keys)
				{
					temp.Add(key.ToString(), Convert.ToInt32(idict[key]));
				}
				Dictionary<string,int> targets = temp;
                NewWeapon = new Weapon(wepType, range, lethality, probHit, count, curammo, maxRate, targets);
			}
			else {
                NewWeapon = new Weapon(wepType, range, lethality, probHit, count, curammo, maxRate);
			}

			NewWeapon.Name = name;
			Weapons.Add(Key,NewWeapon);

			// Set the owner for the new weapon
			NewWeapon.SetOwner ((this.gameObject.GetComponent<IdentityController>()).GetGuid(), Key);

		}
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowAddWeaponChangeEvent(string name, Weapon newWeapon){
		object[] arguments = new object[5];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = "add";
		
		object[] subarguments = new object[2];
		subarguments[0] = name;
		subarguments[1] = newWeapon;
		
		arguments[3] = subarguments;
		//arguments[4] is set during handling
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowRemoveWeaponChangeEvent(string weaponkey){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = "delete";
		arguments[3] = weaponkey;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowCurrentAmmoChangeEvent(string weaponname, int amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".curammo");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowMaxAmmoChangeEvent(string weaponname, int amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".maxammo");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowMaxShotsChangeEvent(string weaponname, int amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".maxshots");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowDamageChangeEvent(string weaponname, float amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".damage");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowHitChanceChangeEvent(string weaponname, float amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".hitchance");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowRangeChangeEvent(string weaponname, float amount){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".range");
		arguments[3] = amount;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	/**
	 * Code for gui to call to throw event
	 */
	public void ThrowTypeChangeEvent(string weaponname, string type){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "AttackController";
		arguments[2] = ((string)weaponname + ".type");
		arguments[3] = type;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}

	/** Sets an individual value after admin change
	 */
	public override void SetValue(string paramName,object value) {
		// Possible behaviors are add, delete, and modify weapon
		// if adding, paramName is "add", value is an object array: {name,weapon object}
		// if deleting, paramName is "delete", value is weapon name
		// if modifying, paramName is weaponName.weaponParamName, value is desired value

		
		if(paramName.ToLower().Equals("add")) {
			object[] arr = (object[]) value;
			Weapon w = (Weapon) arr[1];
			string Key = GenerateKey((string) arr[0]);
			Weapons.Add(Key,w);
			w.SetOwner((this.gameObject.GetComponent<IdentityController>()).GetGuid(),Key);
		}
		else if(paramName.ToLower().Equals("delete")) {
			Weapons.Remove((string)value);
		}
		else {
			string[] paramParts = paramName.Split('.');
			if(Weapons.ContainsKey(paramParts[0])) {
				Weapon w = Weapons[paramParts[0]];
				string s = paramParts[1].ToLower();
				if(s.Equals("curammo")) {
					w.SetCurAmmo((int) value);
				}
				else if(s.Equals("damage")) {
					w.SetDamage((float) value);
				}
				else if(s.Equals("hitchance")) {
					w.SetHitChance((float) value);
				}
				else if(s.Equals("maxammo")) {
					w.SetMaxAmmo((int) value);
				}
				else if(s.Equals("maxshots")) {
					w.SetMaxShots((int) value);
				}
				else if(s.Equals("range")) {
					w.SetRange((float) value);
				}
				else if(s.Equals("type")) {
					w.SetWeaponType((string) value);
				}
			}
		}
	}


    /**
     * Gets all the values of the controller.
     * 
     * @return 
     *      A json friendly dictionary.
     */
    public override IDictionary GetValues()
    {
        Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();
        List<System.Object> toAdd = new List<System.Object>();
		
        foreach (string name in Weapons.Keys)
        {
            Dictionary<string, System.Object> tier = new Dictionary<string, System.Object>();

            tier.Add ("name",Weapons[name].Name);
			tier.Add ("class", Weapons[name].GetWeaponType().ToString());
			tier.Add ("type", Weapons[name].GetWeaponType().ToString());
            tier.Add("range", Weapons[name].GetRange());
			tier.Add ("lethality", Weapons[name].GetDamage());
            tier.Add ("prob_of_hit", Weapons[name].GetHitChance());
			tier.Add ("count", Weapons[name].GetMaxAmmo());
            tier.Add("curammo", Weapons[name].GetCurAmmo());
			tier.Add ("max_rate", Weapons[name].GetMaxShots());
			tier.Add ("targets", Weapons[name].GetTargets());

			toAdd.Add(tier);

        }
        toReturn["weapons"] = toAdd;
        return toReturn;
    }


	/**
	 * Use this for initialization
	 */
	void Awake () {

		// Initiate the weapons dict so that it can be added to
		//Weapons = new Dictionary<string, Weapon> ();

	}


	/**
	 * An event based implementation of weapons fire.
	 * This should be called after weapons are targeted at end of turn.
	 */
	public void FireWeaponsEvent() {

		//ArrayList argumentList = new ArrayList();
		//argumentList.Add(this.gameObject.GetComponent<IdentityController>().GetGuid());
		
		int fired = 0;

		// Iterate over all ship weapons
		foreach (Weapon weapon in Weapons.Values) {
		
			float totalDamage = 0.0f;

			foreach(string g in weapon.GetTargets().Keys){
				
				fired = 1;
				
				int ShotCount = 0;
				
				object[] arguments = new object[5];
				arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
				arguments[1] = g;
				arguments[2] = weapon;
				arguments[4] = 0;

				for(int i = 0; i < weapon.GetTargets ()[g]; i++){
				
					ShotCount++;
					
					// Check if the target gets hit by the shot
					if(SuccessfulHit(weapon)) {
						//throw weapons attack event
						
						totalDamage += weapon.GetDamage();
					}
				}
				arguments[3] = totalDamage;
				arguments[4] = ShotCount;

				EventFactory.ThrowEvent(GEventType.AttackEvent, arguments, 2);
			}
		}
		
		if(fired == 1){
			if(getBackfireChance() >= UnityEngine.Random.Range(0.0F, 100.0f)){
				
				object[] arguments = new object[2];

				arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
				arguments[1] = getBackfireDamage();
				
				EventFactory.ThrowEvent(GEventType.BackfireEvent, arguments, 2);
			}
		}
		
	}
	
	private float getBackfireChance(){
		return GlobalSettings.GetRetaliationPercentChance();
	}
	
	private float getBackfireDamage(){
		return GlobalSettings.GetRetaliationDamage();
	}


	/**
	 * Calls the clear target method of each weapon associated with it.
	 */
	public void ClearTargets() {

		foreach (Weapon entry in Weapons.Values) {

			entry.ClearTarget ();

		}

	}

	
	/**
	 * This method determines if the target gets hit.
	 * 
	 * If an attack needs to factor in conditions such as weather and
	 * terrain, that should be done in a class that inherits from this
	 * base class.
	 * 
	 * @param weapon
	 * 		The weapon we are testing for a successful hit
	 * 
	 * @return
	 * 		Boolean of weather the shot was successful
	 */
	public virtual bool SuccessfulHit(Weapon weapon) {

		// Check if the hit is successful
		if (weapon.GetHitChance() > UnityEngine.Random.Range(0.0F, 100.0f)) {
			// If the hit is a success

			return true;

		}
		else {
			// If the hit is a failure

			return false;

		}

	}


	/**
	 * Get the value dictionary of weapons equipped by this unit
	 * 
	 * @return
	 * 		The dictionary of weapons this units has equipped
	 */
	public Dictionary<string, Weapon> GetWeapons() {

		return Weapons;

	}


	/**
	 * Get a particular weapon equipped by the unit
	 * 
	 * @param key
	 * 		The key associated with the weapon in the dictionary
	 * 
	 * @return
	 * 		The weapon associated with the key
	 */
	public Weapon GetWeapon(string key) {
		return Weapons[key];

	}


	/**
	 * Adds a weapon to the hashmap
	 * 
	 * @param newWeapon
	 * 		The weapon to be added
	 * 
	 * @return
	 * 		The key used to store the weapon
	 */
	public string AddWeapon(Weapon newWeapon) {
	

		// Get an unused key for this weapon
		string key = GenerateKey(newWeapon.GetWeaponType ());

		// Add the weapon to the dictioary with the unused key
		Weapons.Add (key, newWeapon);

		// Inform the weapon who its owner is
		newWeapon.SetOwner (this.gameObject.GetComponent<IdentityController>().GetGuid(), key);

		// Return the key used
		return key;

	}
	
	/**
	 * Removes a weapon from the list of weapons
	 * 
	 * @param type
	 * 		The key used to store the weapon
	 * 
	 */
	public void RemoveWeapon(string key){
		
		Weapons.Remove(key);
		
	}


	/**
	 * Generates a key for a weapon
	 * 
	 * @param type
	 * 		The string representation of the weapon type
	 * 
	 * @return
	 * 		A key for the weapon that is unique within this
	 * 		controller
	 */
	private string GenerateKey(string type) {

		// Initialize an indexing variable
		int i = 0;
		
		// Initialize the first potential name of the key
		string key = FormatKey (type, i);
		
		// Iterate until the key is not currently being used
		while (Weapons.ContainsKey(key)) {
			// If the last key was already in use
			
			// Increment the index
			i++;
			
			// Reformat the key
			key = FormatKey (type, i);
			
		}

		return key;

	}


	/**
	 * Formats the key of a weapon
	 * 
	 * @param type	
	 * 		The type of the weapon
	 * @param index
	 * 		The number of the weapon of the same type
	 * 
	 * @retrun
	 * 		The formatted key
	 */
	private string FormatKey(string type, int index) {
		return type + "-" + index;
	}

	// TODO
	public override void SetValuesNavy(IDictionary values) {
		SetValues(values);
	}
	public override void SetValuesMarines(IDictionary values) {
		SetValues(values);
	}

}
