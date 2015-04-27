/*****
 * 
 * Name: Controller
 * 
 * Date Created: 2015-02-09
 * 
 * Original Team: Gameplay
 * 
 * This class serves as a superclass for all other controllers.  It provides access 
 * to an event that will be passed to the EventManager at the end of the turn.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------
 * B. Croft		2015-02-09	Initial commit.
 * B. Croft		2015-02-11	Add EventHandler implementation.
 * B. Croft		2015-02-11	Removed EventHandler implementation.
 * T. Brennan	2015-02-17	Refactored
 * B. Croft		2015-03-20	Add SetValue Method
 * 
 */

using UnityEngine;
using System.Collections;

public abstract class Controller : MonoBehaviour, ISetGetValues
{


	/**
	 * The event held by this controller
	 */
	protected GEvent MyGEvent;


	/**
	 * Use this for initialization
	 */
	void Start ()
	{

		MyGEvent = null;

	}

	/**
	 * TODO
	 */
	public abstract void SetValuesNavy (IDictionary values);

	/**
	 * TODO
	 */
	public abstract void SetValuesMarines (IDictionary values);

	/* Sets an individual value by an admin action
	 * 
	 * @param paramName
	 * 		The parameter to be set.
	 * @param value
	 * 		The value for the parameter to be set to.
	 */
	public abstract void SetValue (string paramName, object value);

    public abstract void SetValues(IDictionary values);

    public abstract IDictionary GetValues();

	/**
	 * Sets the event for this controller
	 * 
	 * @param e
	 * 		The event for the controller.
	 */
	protected void SetEvent (GEvent e)
	{

		MyGEvent = e;

	}


	/**
	 * Returns the event held by this controller.
	 */
	public GEvent GetEvent ()
	{

		return MyGEvent;

	}


	/**
	 * Deletes the event held by this controller.
	 */
	public void DeleteEvent ()
	{

		MyGEvent = null;

	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		SyncStats ss = gameObject.GetComponent<SyncStats> ();
		if (ss != null)
		{
			ss.sync (stream);
		}
	}

}
