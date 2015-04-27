/*****
 * 
 * Name: PlanMoveHandler
 * 
 * Date Created: 2015-04-12
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle plan move events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * J.Balik		2015-04-12	Initial Commit.
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class PlanMoveHandler : EventHandler
{
	
	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register ()
	{
		
		EventManager.Instance.RegisterMe (GEventType.PlanMoveEvent, this);
		
	}
	
	
	public override void HandleEvent (GEvent e)
	{
		
		MoverController[] obj = GameObject.FindObjectsOfType (typeof(MoverController)) as MoverController[];
		
		foreach (MoverController m in obj)
		{
			
			GameObject o = m.gameObject;
			
			if ((o.gameObject.GetComponent<IdentityController> ()).GetGuid ().Equals(Convert.ToString(e.Arguments [0])))
			{
				m.Destination = e.Arguments [4] as Point;
				break;
			}
		}
		
	}
	
	public override void SetEventString(GEvent e) {
		
		object[] eArguments = e.Arguments;
		
		string unitName = GetName(Convert.ToString(eArguments [0]));
		string[] teams = {GetTeam(Convert.ToString(eArguments [0]))};
		
		Point curPos = eArguments [3] as Point;
		Point destination = eArguments [4] as Point;
		
		string message = "User " + eArguments [1] + " has planned for unit " + unitName + " to move from (" + curPos.x + "," + curPos.y + ") to (" + destination.x + "," + destination.y + ")";
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}
}
