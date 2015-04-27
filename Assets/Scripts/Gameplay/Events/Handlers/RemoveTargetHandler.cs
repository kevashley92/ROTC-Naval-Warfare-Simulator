/*****
 * 
 * Name: RemoveTargetHandler
 * 
 * Date Created: 2015-02-25
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle removing targets.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * T. Dill		2015-02-25	Initial Commit.
 * T. Dill		2015-02-25	Adding Comments.
 * T. Brennan	2015-03-07	Removed Unused class level gevent variable
 */

using UnityEngine;
using System.Collections;
using System;

public class RemoveTargetHandler : EventHandler {
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {
		
		EventManager.Instance.RegisterMe(GEventType.RemoveTargetEvent, this);
		
	}
	
	
	public override void HandleEvent(GEvent e) {

		string attackerGuid = (string)e.Arguments.GetValue(0);
		string targetGuid = (string)e.Arguments.GetValue(1); 
		string weapon = (string)e.Arguments.GetValue(2);

		GameObject obj = GuidList.GetGameObject(attackerGuid);
		AttackController ac = (AttackController) obj.GetComponent ("AttackController");
		Weapon wep = ac.GetWeapon (weapon);
		wep.RemoveTarget (targetGuid);
			

		
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string unitName = GetName((string)eArguments [0]);
		string enemyName = GetName((string)eArguments [1]);
		string weaponName = (string)eArguments[2];
		string[] teams = {GetTeam((string)eArguments [0])};
		
		string message = "Unit " + unitName + " stopped targeted unit " + enemyName + " with " + weaponName;
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}
	

	
}