/*****
 * 
 * Name: AttackHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle attack events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit. 
 * M.Schumacher	2015-02-12	Changed slightly and added a debug message. 
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class AttackHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.AttackEvent, this);

	}


	public override void HandleEvent( GEvent e ){

		////Debug.Log("Attempting to handle attack event... ");

		HealthController[] obj = GameObject.FindObjectsOfType(typeof(HealthController)) as HealthController[];

		Weapon w = e.Arguments[2] as Weapon;

		foreach( HealthController h in obj){

			GameObject o = h.gameObject;

			////Debug.Log(o.GetComponent<IdentityController>().GetGuid() + "_________" + e.Arguments[1]);

			if(o.GetComponent<IdentityController>().GetGuid().Equals((string)e.Arguments[1])){
			
				w.FireShots((int)e.Arguments[4]);

				if(w.TargetInRange((string)e.Arguments[1])) {

					h.DamageUnit((float)e.Arguments[3], GetTeam((string)e.Arguments [0]));

					//Debug.Log("Weapon " + w.GetName() + " Fired");
					e.Arguments[4] = 0;

				}
				else { 

					e.Arguments[3] = 0.0f;

					//Debug.Log ("Weapon" + w.GetName() + " was out of range.");
					e.Arguments[4] = 1;

				}

				break;

			}

		}

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;

		string unitName = GetName((string)eArguments [0]);
		string enemyName = GetName((string)eArguments [1]);
		
		string rawName = (eArguments [2] as Weapon).GetWeaponType();
		rawName = rawName.Substring(0, rawName.Length - 4) + "name";
		
		string weaponName = LanguageManager.instance.getString("Weapons", rawName);
		string[] teams = {GetTeam((string)eArguments [0]),GetTeam((string)eArguments[1])};
		
		string message = "";
		
		if((int) e.Arguments[4] == 1){
			message = "Unit " + unitName + " attacked unit " + enemyName + " with " + weaponName + ", but missed because it moved out of range";
		}
		else{
			message = "Unit " + unitName + " attacked unit " + enemyName + " with " + weaponName + " doing " + eArguments [3] + " damage";
		}
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);

	}

}
