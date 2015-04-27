/*****
 * 
 * Name: EventHandler
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * This interface defines the functionality for event handlers
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-01-30	Initial Commit. 
 * M.Schumacher	2015-02-11	Added register method. 
 * T. Brennan	2015-02-17	Refactored
 */

using UnityEngine;
using System.Collections;

public abstract class EventHandler {

	/**
	 * Registers with the instanced eventmanager
	 */
	public abstract void Register ();
	

	/**
	 * Handles the specified event
	 */
	public abstract void HandleEvent(GEvent e);

	public abstract void SetEventString(GEvent e);

	public void ProcessEvent(GEvent e) {
		HandleEvent(e);
		SetEventString(e);
		EventLog.LogEvent(e);
	}

	public static string GetTeam(string guid) {
        if (GuidList.GetGameObject(guid) == null)
        {
            return "";
        }
		return Team.Teams[GuidList.GetGameObject(guid).GetComponent<IdentityController>().GetTeam()].GetTeamName();
	}
	
	public static string GetTeamColor(string guid) {
		Color c = Team.Teams[GuidList.GetGameObject(guid).GetComponent<IdentityController>().GetTeam()].GetTeamColor();
		return ((int)(c.r)).ToString("X2") + ((int)(c.g)).ToString("X2") + ((int)(c.b)).ToString("X2")+ "FF";
	}

	public static string GetName(string guid) { 
        if (GuidList.GetGameObject (guid) != null) {
            IdentityController current = GuidList.GetGameObject(guid).GetComponent<IdentityController>();
			string localizedname = current.GetFullName();
			//return "<color=#" + GetTeamColor(guid) + ">[[" + GetTeam(guid) + "]" + localizedname + "]</color>";
			return "{(" + GetTeam(guid) + ")" + localizedname + "}";
		} else {
            return "";
		}
	}

}
