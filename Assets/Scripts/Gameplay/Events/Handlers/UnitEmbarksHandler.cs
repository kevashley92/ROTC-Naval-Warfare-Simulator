/*****
 * 
 * Name: UnitEmbarksHandler
 * 
 * Date Created: 2015-03-16
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle units embarking.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-03-16	Initial Commit.
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class UnitEmbarksHandler : EventHandler
{

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register ()
	{

		EventManager.Instance.RegisterMe (GEventType.UnitEmbarksEvent, this);

	}


	public override void HandleEvent (GEvent e)
	{
		//Call EmbarkerController.EmbarkEventHelper(string TargetGuid, string EmbarkerGuid);
		String SmallerUnitGuid = (string)e.Arguments [0];
		String SuperUnitGuid = (string)e.Arguments [1];
		
		EmbarkerController smaller = GuidList.GetGameObject(SmallerUnitGuid).GetComponent<EmbarkerController>();

		smaller.EmbarkEventHelper(SuperUnitGuid, SmallerUnitGuid);
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string unitName = GetName((string)eArguments [0]);
		string superName = GetName((string)eArguments [1]);
		string[] teams = {GetTeam((string)eArguments [0])};
		
		string message = "Unit " + unitName + " embarked on unit " + superName;
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}

}
