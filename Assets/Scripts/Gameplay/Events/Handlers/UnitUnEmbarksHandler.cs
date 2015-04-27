/*****
 * 
 * Name: UnitUnEmbarksHandler
 * 
 * Date Created: 2015-03-16
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle units unembarking.
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

public class UnitUnEmbarksHandler : EventHandler
{

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register ()
	{

		EventManager.Instance.RegisterMe (GEventType.UnitUnEmbarksEvent, this);

	}


	public override void HandleEvent (GEvent e)
	{

		String SmallerUnitGuid =  Convert.ToString(e.Arguments [0]);
        //Debug.Log ("YO IM HERE: " + SmallerUnitGuid);
        Point p = (Point)e.Arguments [1];
        Vector3 pos = new Vector3 (p.x, p.y, p.z);
        string parent = Convert.ToString (e.Arguments [2]);
		EmbarkerController smaller = (EmbarkerController)GuidList.GetGameObject(SmallerUnitGuid).GetComponent<EmbarkerController>();
        
		smaller.LaunchEventHelper(SmallerUnitGuid, pos, parent);
	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments; 
		string unitName = GetName(Convert.ToString(eArguments[0]));
		string superName = GetName(Convert.ToString(eArguments [2]));
		string[] teams = {GetTeam(Convert.ToString (eArguments [0]))};
		
		string message = "Unit " + unitName + " unembarked off of unit " + superName;
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}

}
