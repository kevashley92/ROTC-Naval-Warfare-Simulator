/*****
 * 
 * Name: PlayerLeaveHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle player leaving events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit. 
 * T. Brennan	2015-02-17	Refactored
 * T. Brennan	2015-03-18	Added actual way of removing a player.
 */

using UnityEngine;
using System.Collections;

public class PlayerLeaveHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.PlayerLeaveEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		//Player leave code

		Team.GetTeam((int) e.Arguments[1]).RemoveUser((string) e.Arguments[0]);

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string[] teams = Team.GetTeamNames().ToArray();
		
		string message = "Player " + eArguments [0] + " has left the game";

		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}
}
