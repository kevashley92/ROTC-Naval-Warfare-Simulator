/*****
 * 
 * Name: WeatherChangeHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle weather change events.
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

public class WeatherChangeHandler : EventHandler{

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register() {

		EventManager.Instance.RegisterMe(GEventType.WeatherChangeEvent, this);

	}


	public override void HandleEvent( GEvent e ){
		// weather change code

		int weatherIndex = (int)e.Arguments[0];
		
		GlobalSettings.SetCurrentWeatherIndex(weatherIndex);

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string[] teams = Team.GetTeamNames().ToArray();
		
		string message = "Weather changed to " + Weather.GetWeatherType((int)eArguments [0]);
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}
}
