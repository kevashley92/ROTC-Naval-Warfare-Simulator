/*****
 * 
 * Name: AttackController
 * 
 * Date Created: 2015-03-18
 * 
 * Original Team: Gameplay
 * 
 * This class will manage all actions associated with an explosion.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * J. Balik 	2015-03-18	Initial Commit. Added explosion methods and handlers
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Explosion {
	
	/**
	 * Determines whether the explosion heals or damages units in range.
	 */
	protected bool isDamaging;

	/**
	 * Amount of damage that the explosion heals/deals to units in range.
	 */
	protected int damage;

	/**
	 * The range of the explosion.  Acts as a radius from the explosion position.
	 */
	protected int range;

	/**
	 * The location of the center point of the explosion.
	 */
	protected Vector3 position;

	/**
	 * Constructor class for a new explosion
	 * position passed as separate x and y coordinates to handle Marine input
	 */
	public Explosion(int x, int y, bool isDamaging, int damage, int range) {
		this.position = new Vector3 (x, y, 0);
		this.damage = damage;
		this.range = range;
		this.isDamaging = isDamaging;
	}

	/**
	 * An event based implementation of an explosion occurring.
	 * This should be called after weapons are targeted at end of turn.
	 */
	public void ExplosionEvent() {

		object[] arguments = new object[4];
		arguments [0] = position;
		arguments [1] = isDamaging;
		arguments [2] = damage;
		arguments [3] = range;
		EventFactory.ThrowEvent(GEventType.ExplosionEvent, arguments, 2);

	}
	
	
	/**
	 * This method determines if a target gets hit.
	 *  
	 * @param obj
	 * 		The gameobject that is in danger of being damaged
	 * 
	 * @return
	 * 		Boolean of whether or not the explosion was in range
	 */
	public virtual bool inRange(GameObject obj) {
		if (obj == null) return false;
		Vector3 unitPos = MouseToGridAPI.UnitToWorldCoordinate (obj.transform.position);
		float distance = (float)Math.Sqrt(Math.Pow(unitPos.x - this.position.x, 2) + 
		                                  Math.Pow(unitPos.y - this.position.y, 2));
		return (distance <= this.range);
	}
	
	/**
	 * gets the flag that determines whether the explosion is damaging, or healing
	 * 
	 * @return the flag
	 */
	public bool getIsDamaging()
	{
		return this.isDamaging;
	}
	
	/**
	 * gets the damage of the explosion.
	 * 
	 * @return the damage of the explosion
	 */
	public int getDamage()
	{
		return this.damage;
	}
	
	/**
	 * gets the range of the explosion.
	 * 
	 * @return the range of the explosion
	 */
	public int getRange()
	{
		return this.range;
	}

	/**
	 * gets the position of the explosion.
	 * 
	 * @return the position of the explosion
	 */
	public Vector3 getPosition()
	{
		return this.position;
	}
}
