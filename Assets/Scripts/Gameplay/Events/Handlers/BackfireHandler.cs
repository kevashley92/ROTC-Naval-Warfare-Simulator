/*****
 * 
 * Name: BackfireHandler
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle backfire events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-02-23	Initial Commit.
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class BackfireHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.BackfireEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		
		HealthController Unit = (HealthController)GuidList.GetGameObject ((string)e.Arguments [0]).GetComponent ("HealthController");
		
		Unit.DamageUnit((float)e.Arguments[1], GetTeam((string)e.Arguments [0]));
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string unitName = GetName((string)eArguments [0]);
		string[] teams = {GetTeam((string)eArguments [0])};
		
		string message = "Unit " + unitName + " got hit by retaliation for " + eArguments [1] + " damage";
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}



}
