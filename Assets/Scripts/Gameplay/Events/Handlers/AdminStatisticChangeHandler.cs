/*****
 * 
 * Name: AdminStatisticChangeHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle admins statistic change events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit. 
 * T. Brennan	2015-02-17	Refactored
 */

using UnityEngine;
using System.Collections;

public class AdminStatisticChangeHandler : EventHandler{

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.AdminStatisticChangeEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		object[] args = e.Arguments;

		GameObject go = GuidList.GetGameObject((string) args[0]) as GameObject;
		Controller c = go.GetComponent((string) args[1]) as Controller;

		c.SetValue((string) args[2],args[3]);

	}


	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;

		string unitName = GetName((string)eArguments [0]) + " " + eArguments [0];
		string[] teams = {GetTeam((string)eArguments [0] )};

		string message = "Admin has modified statistics of unit " + unitName;
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);

	}

}
