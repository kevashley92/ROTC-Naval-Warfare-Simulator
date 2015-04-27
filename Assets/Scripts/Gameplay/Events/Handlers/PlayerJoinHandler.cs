/*****
 * 
 * Name: PlayerJoinHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle player join events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit. 
 * T. Brennan	2015-02-17	Refactored
 * T. Brennan	2015-03-18	Added actual addition of a user.
 * 
 */

using UnityEngine;
using System.Collections;

public class PlayerJoinHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.PlayerJoinEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		//Player joining code

		////Debug.Log ("User Join event handled");
		User NewUser = new User ((PermissionLevel)e.Arguments [0], (MilitaryBranch)e.Arguments [1], (int)e.Arguments [2], (string)e.Arguments [3], (string)e.Arguments [4]);
		if(NewUser == null){
			//Debug.Log("User is null.");
		}
		else{
			//Debug.Log("Attempting to add user " + NewUser.GetUsername() + " to team " + NewUser.TeamNumber);
		}
		Team team = Team.GetTeam ((int)e.Arguments [2]);
		if (team == null) {
			//Debug.Log("Team is null.");
		}
		else{
			//Debug.Log(team.GetTeamName());
			team.AddUser(NewUser);
		}

		////Debug.Log (Team.GetTeam ((int)e.Arguments [2]).GetUsers ().ToArray().ToString());
		////Debug.Log (Team.GetUser((string)e.Arguments [3], (int)e.Arguments [2]).GetUsername());
		////Debug.Log (Team.GetTeam ((int)e.Arguments [2]).GetUsers ().Count);
		////Debug.Log (team.GetUsers ().ToArray().ToString());
		////Debug.Log (team.GetUsers ().Count);

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string[] teams = Team.GetTeamNames().ToArray();

		string message = string.Format("{3} joined the game as a(n) {0}", eArguments);

		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);

	}
}
