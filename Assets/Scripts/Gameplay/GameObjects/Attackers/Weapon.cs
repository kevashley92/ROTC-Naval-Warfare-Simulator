/*****
 * 
 * Name: Weapon
 * 
 * Date Created: 2015-01-31
 * 
 * Original Team: Gameplay
 * 
 * This class will represent a weapon that a unit can have.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-01-31	Initial Commit. Added target, range, damage,
 * 							hitChance, setTarget, and clearTarget
 * T. Brennan 	2015-02-02	Added getTarget, setDamage, getDamage,
 * 							setRange, getRange, setHitChance,
 * 							getHitChance, getWeaponType. Changed scope
 * 							of target, range, damage, and hitChance.
 * Mike Schumacher			Started to change this to work with events
 * B.Croft		2015-02-11	Changed event target setting to throw mid-turn
 * M. Schumacher2015-02-12  Changed damage to an int
 * M.Schumacher	2015-02-13	Fixed bugs and updated to work with guid system. Index system changed
 * B. Croft		2015-02-16	Merge event system and unit model finalizations
 * T. Brennan	2015-02-17	Refactored.
 * I. Johnston	2015-02-23	Added EnoughSpareRounds, RemoveTarget, GetMaxShots, getMaxAmmo, modified ClearTarget, SetTarget
 * T. Brennan	2015-02-25	Removed duplicate methods after merge. Put methods and variable in alphabetical order
 */ 
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Weapon {

	/**
	 * Current amount of ammunition remaining.
	 */
	protected int CurAmmo;

	/**
	 * Current number of shots already target.
	 */
	protected int CurShotsTargeted;
	
	/**
	 * The damage that the weapon will do to the target if it hits
	 */
	protected float Damage;

	/**
	 * The probability that the shot will successfully hit the target
	 * Range: 0 - 100
	 */
	protected float HitChance;
	
	/**
	 * The index of the weapon
	 * Used in case there is more than one of the same type
	 */
	protected string Key;

	/**
	 * Max amount of ammunition for the weapon.
	 */ 
	protected int MaxAmmo;

	/**
	 * Number of shots allowed per turn.
	 */
	protected int MaxShots;

	/**
	 * The unit that owns this weapon. 
	 */
	protected string Owner;

	/**
	 * The distance from the unit that this weapon can potentially hit
	 */
	protected float Range;
	
	/**
	 * The target the weapon will try to fire at during the next turn.
	 */
	protected Dictionary<string, int> Targets;
	
	/**
	 * The type of the weapon
	 */
	protected string WeaponType;

	public string Name;
	

	/**
	 * Basic constructor for a weapon
	 *
	 * @param weaponType
	 * 		The type of the weapon
	 * @param range
	 * 		The base range of the weapon
	 * @param damage
	 * 		The base damage of the weapon
	 * @param hitChange
	 * 		The base hit chance of the weapon
	 */

	public Weapon (string weaponType, float range, float damage, float hitChance, int maxAmmo, int curammo, int maxShots, Dictionary<string,int> targets) {

		this.WeaponType = weaponType;
		this.Range = range;
		this.Damage = damage;
		this.HitChance = hitChance;
		this.MaxAmmo = maxAmmo;
		this.MaxShots = maxShots;
		this.CurAmmo = curammo;
		this.CurShotsTargeted = 0;
		Targets = targets;
	}

	public Weapon (string weaponType, float range, float damage, float hitChance, int maxAmmo, int curammo, int maxShots) :
		this(weaponType,range,damage,hitChance,maxAmmo,curammo,maxShots,new Dictionary<string, int> ())
	{

	}


	/**
	 * Deletes the current target and thereby cancels the planned attack
	 */
	public void ClearTarget() {
		
		////Debug.Log ("Someone cleared the targets.");
		
		foreach (string targetToRemove in Targets.Keys) {

			GameObject tmp = GuidList.GetGameObject (targetToRemove);

			// Check if the target has been destroyed
			if (tmp != null) {
				// If it wasn't

				// Turn its targeted pulse off since the attack targeting it
				// has been canceled or executed.
				tmp.transform.FindChild ("Attack Pulse").gameObject.SetActive (false);

			}

		}
		this.Targets.Clear();
		CurShotsTargeted = 0;

        UIUnitInspectorController UIUnit = GameObject.Find("Canvas").GetComponent<UIUnitInspectorController>() as UIUnitInspectorController;

		if(UIUnit != null)
            UIUnit.ClearTargets ();	
	}


	/**
	 * Returns whether or not the specified amount of rounds can be fired this turn
	 * 
	 * @return
	 * 		Whether or not the specified amount of rounds can be fired this turn
	 */
	public bool EnoughSpareRounds(int roundsToFire){
		if (CurShotsTargeted + roundsToFire > MaxShots || CurShotsTargeted + roundsToFire > CurAmmo) {
			return false;
		}
		return true;
	}


	/**
	 * Reduces the ammo count by the amount
	 * 
	 * @param amount the number of shots
	 */
	public void FireShots(int amount) {
		
		CurAmmo -= amount;
		
	}


	/**
	 * Gets the current ammo
	 * 
	 * @return
	 * 		The current ammo
	 */
	public int GetCurAmmo() {
		
		return CurAmmo;
		
	}


	/**
	 * Gets current amount of targeted shots
	 * 
	 * @return
	 * 		Current amount of targeted shots
	 */
	public int GetCurShotsTargeted() {
		
		return CurShotsTargeted;
		
	}


	/**
	 * Gets the damage of the weapon
	 * 
	 * @return	
	 * 		The damage of the weapon
	 */
	public float GetDamage() {
		
		return Damage;
		
	}


	/**
	 * Get the hit chance of the weapon
	 * 
	 * @return
	 * 		The hit chance of the weapon.
	 */
	public float GetHitChance() {
		
		return HitChance;
		
	}


	/**
	 * Gets the max ammo
	 * 
	 * @return
	 * 		The max ammo
	 */
	public int GetMaxAmmo() {
		
		return MaxAmmo;
		
	}


	/**
	 * Gets max shots
	 * 
	 * @return
	 * 		Max shots
	 */
	public int GetMaxShots() {
		
		return MaxShots;
		
	}


	/**
	 * Gets the name of a weapon
	 * 
	 * @return
	 * 		The formatted name
	 */
	public string GetName() {
		
		return Key;
		
	}


	/**
	 * Gets the owner of this weapon.
	 * 
	 * @return
	 * 		The owner of this weapon.
	 */
	public string GetOwner(){
		return Owner;
	}
	

	/**
	 * Gets the range of the weapon
	 * 
	 * @return
	 * 		The range of the weapon
	 */
	public float GetRange() {
		
		return Range;
		
	}


	/**
	 * Gets the shots currently targeted at the target.
	 * 
	 * @param
	 * 		The guid of the target to get number of shot at.
	 * @return
	 * 		The shots targeted at the target..
	 */ 
	public int GetShotsAtTarget(string guid){
		return Targets[guid];
	}


	/**
	 * Gets the remaining shots available to be targeted.
	 * 
	 * @return
	 * 		The remaining shots available to be targeted.
	 */
	public int GetShotsRemaining(){
		return MaxShots - CurShotsTargeted;
	}


	/**
	 * Gets the target of the weapon
	 * 
	 * @return
	 * 		The target of this weapon
	 */
	public Dictionary<String, int> GetTargets()
	{

		return Targets;

	}


	/**
	 * Gets the type of the weapon
	 * 
	 * @return
	 * 		The type of the weapon
	 */
	public string GetWeaponType() {
		
		return WeaponType;
		
	}
	
	public void SetWeaponType(string type) {
		
		WeaponType = type;
		
	}


	/**
	 * Removes the specified target
	 */
	public void RemoveTarget(string targetToRemove) {
		GuidList.GetGameObject (targetToRemove).transform.FindChild ("Attack Pulse").gameObject.SetActive (false);
		int canceledShots = 0;
		int GuidIndex = 0;
		foreach (string targetGuid in Targets.Keys){
			GuidIndex++;
			if(targetGuid.Equals(targetToRemove)){
				break;
			}
		}
		int shotIndex = 0;
		foreach (int targetShots in Targets.Values){
			shotIndex++;
			if(shotIndex == GuidIndex){
				canceledShots = targetShots;
				break;
			}
		}
		Targets.Remove(targetToRemove);
		CurShotsTargeted -= canceledShots;
	}
	

	/**
	 * Sets current ammo to parameter value.
	 * 
	 * @param
	 * 		The amount of current ammo to be set.
	 */
	public void SetCurAmmo(int ammo){	
		CurAmmo = ammo;
	}
	

	/**
	 * Sets the damage of the weapon.
	 * 
	 * @param newDamage
	 * 		The new damage the weapon will do
	 */
	public void SetDamage(float newDamage) {
		
		Damage = newDamage;
		
	}


	/**
	 * Sets the hit chance of the weapon
	 * 
	 * Valid Range: 0.0 - 100.0, other values will be ignored
	 * 
	 * @param newHitChance
	 * 		The hit percentage of the weapon
	 * 
	 * @return
	 * 		True 	if the new hit chance was in range
	 * 		False 	if it was not in range
	 */
	public bool SetHitChance(float newHitChance)
	{
		
		// Check if the hit chance is in range
		if (newHitChance > 100.0F || newHitChance < 0.0F) {
			// If the chance is out of range
			
			// Don't update the hit chance and return false
			return false;
		} 
		else {
			// If the chance is in range
			
			// Change it
			HitChance = newHitChance;
			
			// And return true
			return true;
			
		}
		
	}


	/**
	 * Sets max ammo to parameter value.
	 * 
	 * @param
	 * 		The amount of max ammo to be set.
	 */
	public void SetMaxAmmo(int ammo){	
		MaxAmmo = ammo;
	}
	

	/**
	 * Sets max shots to parameter value.
	 * 
	 * @param
	 * 		The amount of max shots to be set.
	 */
	public void SetMaxShots(int shots){	
		MaxShots = shots;
	}
	

	/**
	 * Records who the weapon is registered to and under what key
	 * 
	 * @param owner
	 * 		The attacker that owns this weapon
	 * @param key
	 * 		The key used to reference to this weapon in the attacker's
	 * 		weapons dictionary
	 */
	public void SetOwner( string owner, string key){
		
		this.Owner = owner;
		this.Key = key;
		
	}


	/**
	 * Sets the range of the weapon
	 * 
	 * @param newRange
	 * 		The value the weapons range should be set to
	 */
	public void SetRange(float newRange) {
		
		Range = newRange;
		
	}


	/**
	 * Used to set the target that this weapon will try to attack at the
	 * end of the turn. Called by the event handler
	 * 
	 * @param target
	 * 		The new target for the weapon
	 */
	public void SetTarget(string target, int amount) {
		
		if ((CurShotsTargeted + amount <= MaxShots) && (CurShotsTargeted + amount <= CurAmmo)) {

			if(Targets.ContainsKey(target)){
				int currentShots = 0;
				int GuidIndex = 0;
				foreach (string targetGuid in Targets.Keys){
					GuidIndex++;
					if(targetGuid.Equals(target)){
						break;
					}
				}
				int shotIndex = 0;
				foreach (int targetShots in Targets.Values){
					shotIndex++;
					if(shotIndex == GuidIndex){
						currentShots = targetShots;
						break;
					}
				}
				Targets.Remove(target);
				Targets.Add(target, currentShots + amount);
			}
			else{
				Targets.Add (target, amount);
			}
			
			CurShotsTargeted+=amount;
		}
		
	}

	
	/**
	 * Raises a create target event
	 * 
	 * @param target
	 * 		The object to be targeted.
	 */
	public void SetTargetEvent(string target, int amount){

		object[] arguments = new object[4];

		arguments [0] = Owner;
		arguments [1] = target;
		arguments [2] = GetName ();
		arguments [3] = amount;
		
		int MaxShotsAllowedToTarget = MaxShots - CurShotsTargeted;
		
		if(Targets.ContainsKey(target)){
			int currentShotsTargetedAtOne = Targets[target];
			MaxShotsAllowedToTarget += currentShotsTargetedAtOne;
		}

		if (!(target.Equals(Owner)) && (amount <= MaxShotsAllowedToTarget) && (CurShotsTargeted+amount <= CurAmmo)) {  //Check we are not firing at ourselves

			EventFactory.ThrowEvent (GEventType.WeaponTargetEvent, arguments);

		}

	}
	

	/**
	 * This method determines if the target is in range. 
	 * 
	 * If an attack needs to factor in conditions such as weather and
	 * terrain, that should be done in a class that inherits from this
	 * base class.
	 * 
	 * @return
	 * 		A boolean of if the the target is in range
	 */
	public virtual bool TargetInRange( string Target) {
		
		// Check if the target is in range
		GameObject o = GuidList.GetGameObject(Owner);
		GameObject ta = GuidList.GetGameObject(Target);
		if (Vector3.Distance (o.transform.position, ta.transform.position) < Range) {
			// If the target is in range
			if (World.IsMarine) {
				Vector3 position = GuidList.GetGameObject(Owner).transform.position;
				Vector3 target = GuidList.GetGameObject(Target).transform.position;
				List<Terrain> tiles = getInterSectingTerrains((int)position.x, (int)position.y, (int)target.x, (int)target.y);
				foreach (Terrain t in tiles) {
					if (t.ModifierVisual == 1)
					{
						return false;
					}
				}
			}
			return true;
			
		} 
		else {
			// If the target is not in range
			
			return false;
			
		}
		
	}

	protected List<Terrain> getInterSectingTerrains(int x0, int y0, int x1, int y1)
	{
		// Setup
		World world = World.Instance;
		List<Terrain> ret = new List<Terrain>();
		// swap p1 and p2 now to make sure later for-loop doesn't exit prematurely
		if (x1 < x0) {
			int tx = x0;
			int ty = y0;
			x0 = x1;
			y0 = y1;
			x1 = tx;
			y1 = ty;
		}
		
		// start actual work of Bresenham's algorithm
		int dx = x1 - x0;
		int dy = y1 - y0;
		// decision variable
		int d = 2*dy - dx;
		ret.Add(world.TerrainAt(new Vector2(x0+0.0f, y0+0.0f)));
		int y = y0;
		
		for (int x = x0 + 1; x < x1; x++) {
			if (d > 0) {
				y++;
				ret.Add(world.TerrainAt(new Vector2(x+0.0f, y+0.0f)));
				d = d + (2*dy - 2*dx);
			} else {
				ret.Add(world.TerrainAt(new Vector2(x+0.0f, y+0.0f)));
				d = d + (2*dy);
			}
		}
		
		return ret;
	}

}
