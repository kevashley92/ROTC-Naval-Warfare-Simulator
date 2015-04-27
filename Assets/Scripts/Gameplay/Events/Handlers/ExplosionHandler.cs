/*****
 * 
 * Name: ExplosionHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle explosion events
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * B. Croft		2015-03-19	Initial Commit.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExplosionHandler : EventHandler {
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {
		
		EventManager.Instance.RegisterMe(GEventType.ExplosionEvent, this);
		
	}

	private Dictionary<string, string> hitUnits = null;
	
	
	public override void HandleEvent(GEvent e) {

		hitUnits = new Dictionary<string,string>();
		Vector3 position = (Vector3)e.Arguments [0];
		Explosion explosion = new Explosion((int)position.x, (int)position.y, (bool)e.Arguments[1], (int)e.Arguments[2], (int)e.Arguments[3]);
		foreach(GameObject o in GuidList.GetAllObjects()) {
			if (explosion.inRange(o))
			{
				hitUnits.Add(o.GetComponent<IdentityController>().GetGuid().ToString(), null);
				if (explosion.getIsDamaging())
				{
					o.GetComponent<HealthController>().DamageUnit(explosion.getDamage(), "Admin");
				}
				else
				{
					o.GetComponent<HealthController>().HealUnit(explosion.getDamage());
				}
			}
		}
		
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;

		string[] teams = new string[hitUnits.Count];
		hitUnits.Keys.CopyTo(teams,0);
		
		Vector3 position = (Vector3)e.Arguments [0];

		string message = "Bomb dropped at (" + position.x + "," + position.y + ")";
		e.String = message;
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}


	
}