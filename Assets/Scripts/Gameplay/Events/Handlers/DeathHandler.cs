/*****
 * 
 * Name: DeathHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle death events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit.
 * T. Brennan	2015-02-16	Changed how GetComponent was used
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class DeathHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.DieEvent, this);

	}


	public override void HandleEvent( GEvent e ){

		HealthController[] obj = GameObject.FindObjectsOfType(typeof(HealthController)) as HealthController[];

		foreach( HealthController h in obj){

			GameObject o = h.gameObject;

			if( (o.GetComponent<IdentityController>()).GetGuid().Equals( (string) e.Arguments[0])){

				h.KillUnit();

				break;

			}

		}

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string causingTeam = (string)eArguments[1];
		string unitName = (string)eArguments [4];
		string teamName = eArguments[3].ToString();
		
		unitName = "{(" + teamName + ")" + unitName + "}";
		
		string[] teams = {teamName, causingTeam};
		
		string message = "Unit " + unitName + " has been destroyed";
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}


}
