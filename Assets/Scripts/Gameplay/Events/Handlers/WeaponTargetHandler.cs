/*****
 * 
 * Name: WeaponTargetHandler
 * 
 * Date Created: 2015-02-12
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle weapon target events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-02-12	Initial Commit. 
 * M.Schumacher	2015-02-13	Fixed bugs and updated to work with guid system. 
 * T. Brennan	2015-02-17 	Refactored
 * M.Schumacher	2015-03-12	Prevented damaged ships from targeting
 */

using UnityEngine;
using System.Collections;
using System;

public class WeaponTargetHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.WeaponTargetEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		string firerer = (string)e.Arguments [0];
		string target = (string) e.Arguments [1];
		string firing = (string) e.Arguments [2];
		int shots = (int) e.Arguments [3];

        //Debug.Log(firerer + " " + target + " " + firing + " " + shots);

		float PercentHealth = ((HealthController)GuidList.GetGameObject (firerer).GetComponent ("HealthController")).GetCurrentPercentHealth();
		
		if(PercentHealth < GlobalSettings.GetHealthThresholdForNoWeapons()){
			//Debug.Log ("Unable to target weapon since unit is very damaged");
		}
		else{

			Weapon w = ((AttackController)GuidList.GetGameObject (firerer).GetComponent <AttackController>()).GetWeapon(firing);

			w.SetTarget (target, shots);

			//Debug.Log ("Weapon " + firing + " Targeted with " + shots +" shots");
		}

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string unitName = GetName((string)eArguments [0]);
		string enemyName = GetName((string)eArguments [1]);
		string rawName = ((AttackController)GuidList.GetGameObject ((string)e.Arguments [0]).GetComponent <AttackController>()).GetWeapon((string) e.Arguments [2]).GetWeaponType();
		rawName = rawName.Substring(0, rawName.Length - 4) + "name";
		
		string weaponName = LanguageManager.instance.getString("Weapons", rawName);
		
		string[] teams = {GetTeam((string)eArguments [0])};
		
		string message = "Unit " + unitName + " targeted unit " + enemyName + " with " + weaponName + " and " + e.Arguments[3] + " shots.";

		string firerer = (string) eArguments [0];
		string firing = (string) eArguments [2];
		int shots = (int) eArguments [3];
		Weapon w = ((AttackController)GuidList.GetGameObject (firerer).GetComponent <AttackController>()).GetWeapon(firing);
		if(w.GetCurAmmo() != 0){
			e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		} else {
			e.String = "Unit " + unitName + " tried targeting unit " + enemyName + " with " + weaponName + " and " + e.Arguments[3] + " shots and failed.";
			e.String = EventLog.FormatEventString(teams,e.Timestamp,e.String);
		}
	}

}
