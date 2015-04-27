/*****
 * 
 * Name: MoveHandler
 * 
 * Date Created: 2015-02-09
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle move events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-09	Initial Commit.
 * T. Brennan	2015-02-17	Refactored
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class MoveHandler : EventHandler
{

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register ()
	{

		EventManager.Instance.RegisterMe (GEventType.MoveEvent, this);

	}


	public override void HandleEvent (GEvent e)
	{

		MoverController[] obj = GameObject.FindObjectsOfType (typeof(MoverController)) as MoverController[];

		foreach (MoverController m in obj)
		{

			GameObject o = m.gameObject;

			if ((o.gameObject.GetComponent<IdentityController> ()).GetGuid ().Equals((string)e.Arguments [0]))
			{

				m.MoveAsPlanned (new Vector3 ((float)e.Arguments [1], (float)e.Arguments [2], (float)e.Arguments [3]));

				break;

			}
		}

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string unitName = GetName((string)eArguments [0]);
		string[] teams = {GetTeam((string)eArguments [0])};
		
		string message = "Unit " + unitName + " has moved to position: " + eArguments [1] + " " + eArguments[2];
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}


}
