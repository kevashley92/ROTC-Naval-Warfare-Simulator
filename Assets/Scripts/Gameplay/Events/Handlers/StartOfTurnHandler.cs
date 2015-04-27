/*****
 * 
 * Name: StartOfTurnHandler
 * 
 * Date Created: 2015-02-16
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle start of turn events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Brien Croft	2015-02-16	Inital Commit.  Reset weapon targets
 * T. Brennan	2015-02-17	Refactored
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartOfTurnHandler : EventHandler{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.StartOfTurnEvent, this);

	}


	public override void HandleEvent( GEvent e ){

		////Debug.Log("Handling Start of Turn Event");
		
		//Get text for everything that happened this turn
		
		List<Team> teams = Team.GetTeams();
		
		Dictionary<string, string> teamResults = new Dictionary<string, string>();
		
		for(int i = 0; i < teams.Count; i++){
			string teamName = Team.Teams[i].GetTeamName();
			
			teamResults.Add(teamName, EventLog.ReadLogFile(teamName));
			////Debug.Log(EventLog.ReadLogFile( teamName ));
		}
		
		Dictionary<string, string> teamResultsFull = new Dictionary<string, string>();
		
		for(int i = 0; i < teams.Count; i++){
			string teamName = Team.Teams[i].GetTeamName();
			
			teamResultsFull.Add(teamName, EventLog.ReadLogFileFull(teamName));
			////Debug.Log(EventLog.ReadLogFile( teamName ));
		}
		
		NetworkManager.NetworkInstance.SendLogs(teamResults);
		
		NetworkManager.NetworkInstance.SendFullLog(teamResultsFull);
		
		//Debug.Log(teamResults["Admin"].ToString());
		
		EventLog.StartNewTurn();

		//Reset targeting on weapons on each ship that have been aimed
		AttackController[] fire = GameObject.FindObjectsOfType(typeof(AttackController)) as AttackController[];

		foreach( AttackController t in fire){

			t.ClearTargets();

		}

        NetworkManager.NetworkInstance.SendRefresh();
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string[] teams = Team.GetTeamNames().ToArray();
		
		string message = "Turn " + eArguments [0] + " has started";
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}

}
