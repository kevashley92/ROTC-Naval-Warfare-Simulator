/*****
 * 
 * Name: EndOfTurnHandler
 * 
 * Date Created: 2015-02-10
 * 
 * Original Team: Gameplay
 * 
 * This implements EventHandler interface to handle end of turn events.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * Tyler Dill	2015-02-10	Initial Commit. 
 * M.Schumacher	2015-02-11	Added register method
 * M.Schumacher	2015-02-12	Added all attackers firing at end of turn
 * T. Brennan	2015-02-17	Refactored
 * T. Brennan	2015-03-07	Added calls to handle attack and move events.
 */

using UnityEngine;
using System.Collections;

public class EndOfTurnHandler : EventHandler {

	/**
	 * Registers with the instanced eventmanager
	 */
	public override void Register ()
	{
		EventManager.Instance.RegisterMe (GEventType.EndOfTurnEvent, this);
	}


	public override void HandleEvent (GEvent e) {
		//Handle End of Turn

		////Debug.Log("Handling End of Turn Event");

		//Fire weapons on each ship that have been aimed
		AttackController[] fire = GameObject.FindObjectsOfType (typeof(AttackController)) as AttackController[];

		foreach (AttackController t in fire) {

			t.FireWeaponsEvent ();

		}

		Controller[] Controllers = MonoBehaviour.FindObjectsOfType (typeof(Controller)) as Controller[];

		foreach (Controller ctrl in Controllers) {

			GEvent ge = ctrl.GetEvent ();

			if (ge != null) {

				EventManager.Instance.AddEvent (ge);

			}

		}

		UIMainController UserUI = GameObject.Find ("Canvas").GetComponent<UIMainController> ();
		if (UserUI != null) {
			UserUI.ShowLog();
		}
		// Handle all move events
		EventManager.Instance.HandleEvents(1);

		// Handle all attack events
		EventManager.Instance.HandleEvents(2);

		TurnManager.MoveToNextTurn ();

		TurnManager.ThrowStartOfTurnEvent ();

	}

	public override void SetEventString(GEvent e) {
		object[] eArguments = e.Arguments;
		
		string[] teams = Team.GetTeamNames().ToArray();
		
		string message = "Turn " + eArguments [0] + " has ended";
		
		e.String = EventLog.FormatEventString(teams,e.Timestamp,message);
		
	}


}
