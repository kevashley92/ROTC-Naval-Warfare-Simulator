/*****
 * 
 * Name: GuidList
 * 
 * Date Created: TODO Fill this in
 * 
 * Original Team: Gameplay
 * 
 * TODO Fill this in
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * TODO Fill this in		Initial Commit
 * T. Brennan	2015-02-17	Refactored
 * T. Brennan	2015-03-16	Added GetAllObjects, GetObjectsOnTeam
 * T. Brennan	2015-03-17	Added GetOmnipresentObjects and 
 * 							GetInvisibleObjects
 * T. Brennan	2015-03-22	Added GetNonSubsOnTeam
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GuidList
{

	/**
	 * A dictionary containing all registered game objects indexed by their
	 * Guids
	 */
	private static Dictionary<String,GameObject> MyDictionary = new Dictionary<String,GameObject> ();


	/**
	 * Adds an object to the static collection of objects
	 * 
	 * @param objGuid
	 * 		The Guid of the object to add
	 * @param obj
	 * 		The object to be stored
	 */
	public static void AddObject (string objGuid, GameObject obj)
	{   
		if (!MyDictionary.ContainsKey (objGuid))
		{
			MyDictionary.Add (objGuid, obj);
		}
	}

	public static void Clear()
	{
		MyDictionary.Clear();
	}


	/**
	 * Gets the object associated with the Guid provided
	 * 
	 * @param objGuid
	 * 		The Guid of the object wanted.
	 */
	public static GameObject GetGameObject (string objGuid)
	{
		GameObject ret = null;
		try
		{
			ret = MyDictionary [objGuid];
		}
		catch (Exception)
		{
            
		}
		return ret;
	}
	
	/**
	 * Removes the object associated with the Guid provided
	 *
	 * true if the element is successfully found and removed; otherwise, false. 
	 * This method returns false if key is not found in the Dictionary<TKey, TValue>.
	 * 
	 * @param objGuid
	 * 		The Guid of the object removed.
	 */
	public static bool RemoveGameObject (string objGuid)
	{

		return MyDictionary.Remove (objGuid);

	}


	/**
	 * Gets all of the objects in the game.
	 * 
	 * @return
	 * 		All the objects in the game
	 */
	public static List<GameObject> GetAllObjects ()
	{

		return MyDictionary.Values.ToList ();

	}


	/**
	 * Gets all the objects in the game that are associate with that team.
	 * 
	 * @param team
	 * 		The team to get the objects of
	 * 
	 * @return
	 * 		All the objects on the team.
	 */
	public static List<GameObject> GetObjectsOnTeam (int team)
	{

		List<GameObject> objectsOnTeam = new List<GameObject> ();

		foreach (GameObject obj in MyDictionary.Values)
		{

			if (obj != null && obj.GetComponent<IdentityController> ().GetTeam () == team)
			{

				objectsOnTeam.Add (obj);

			}

		}

		return objectsOnTeam;

	}


	/**
	 * Gets all the objects in the game that are associated with that team,
	 * except for subs.
	 * 
	 * @param team
	 * 		The team to get the objects of
	 * 
	 * @return
	 * 		All the non-sub objects on the team.
	 */
	public static List<GameObject> GetNonSubsOnTeam (int team)
	{

		HashSet<GameObject> objs = new HashSet<GameObject> (GuidList.GetObjectsOnTeam (team));

		objs.ExceptWith (GameObject.FindGameObjectsWithTag ("Subsurface"));

		return objs.ToList ();

	}


	/**
	 * Gets all objects that are omnipresent
	 * 
	 * @return
	 * 		A list of all omnipresent objects
	 */
	public static List<GameObject> GetOmnipresentObjects ()
	{

		// Initialize a list of objects
		List<GameObject> objs = new List<GameObject> ();

		// Iterate over every object
		foreach (GameObject obj in MyDictionary.Values)
		{

			// Check if the object is omnipresent
			if (obj != null && obj.GetComponent<IdentityController> ().GetVisibility () == VisibilityType.Omnipresent)
			{
				// If the object is omnipresent

				// Add it to the list of objects to return
				objs.Add (obj);
				
			}

		}

		// Return the list of omnipresent objects
		return objs;

	}


	/**
	 * Gets all objects that are invisible
	 * 
	 * @return
	 * 		A list of all invisible objects
	 */
	public static List<GameObject> GetInvisibleObjects ()
	{
		
		// Initialize a list of objects
		List<GameObject> objs = new List<GameObject> ();
		
		// Iterate over every object
		foreach (GameObject obj in MyDictionary.Values)
		{
			
			// Check if the object is invisible
			if (obj != null && obj.GetComponent<IdentityController> ().GetVisibility () == VisibilityType.Invisible)
			{
				// If the object is invisble
				
				// Add it to the list of objects to return
				objs.Add (obj);
				
			}
			
		}
		
		// Return the list of invisible objects
		return objs;
		
	}


	public static void CleanNullObjects ()
	{
		List<string> keys = new List<string> (MyDictionary.Keys);
		for (int i = 0; i < keys.Count; i++)
		{
			string key = keys [i];
			GameObject goOut = null;
			MyDictionary.TryGetValue (key, out goOut);
			if (goOut == null)
			{
				MyDictionary.Remove (key);
			}
		}
	}

}
