/*****
 * 
 * Name: ObjectFactoryHelper
 * 
 * Date Created: 2015-02-13
 * 
 * Original Team: Gameplay
 * 
 * A class that will provide static helper data and methods for the
 * ObjectFactory class.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * S. Lang		2015-03-18	Initial creation and skeletonization.
 */ 

using UnityEngine;
using System;
using System.Collections;

class ObjectFactoryHelper {

	/** Directory that holds all the prefabs */
	public static readonly string PrefabsDir = "Prefabs/GameObjects/BaseObjects/";
	/** Root directory for controllers */
	public static readonly string ControllersDir = "";///Assets/Scripts/Gameplay/GameObjects/";
	public static readonly string SubdirAttack = "";//"Attackers/";
	public static readonly string SubdirDetector = "";//"Detectors/";
	public static readonly string SubdirHealth = "";//"Health/";
	public static readonly string SubdirIdentity = "";//"Identities/";
	public static readonly string SubdirMover = "";//"Movers/";
	public static readonly string SubdirContainer = "";
	public static readonly string SubdirEmbark = "";

	/** 
	 * A list of Prefabs used.
	 * These are used for indexing purposes in multi-dimensional arrays.
	 */
	public static readonly string[] Prefabs = {
		PrefabsDir + "Air", 			// 0
		PrefabsDir + "Surface",
		PrefabsDir + "Subsurface",
		PrefabsDir + "Marine Unit",
		PrefabsDir + "Weapon",			// 4
		PrefabsDir + "Carrier"
	};

	/** A list of tags. Useful for indexing prefabs (maybe) */
	public static readonly string[] Tags = {
		"Air",
		"Surface",
		"Subsurface",
		"Marine",
		"Weapon",
		"Carrier"
	};

	public static readonly string[] ControllerFamilies = {
		"Identity", 	// 0
		"Health",
		"Mover",
		"Detector",
		"Attack",		// 4
		"Container",
		"Embarker"
	};

	/**
	 * An in-place mapping of Prefabs and Families to concrete controllers.
	 * Due to using the same indices for tags as prefabs, it is _currently_ possible
	 * to look things up based on tag or prefab and use that as the first index to
	 * access the correct controller name.
	 * N.B. If ever these two ever come out of sync, it will be necessary to create a
	 * second table for tags to families to controllers. This may end up being a better
	 * system anyways, so it may be done for simplicity later.
	 */
	public static readonly string[,] ControllersByPrefabAndFamily = {
		// 0: Air
		{
			// 0: Identity
			ControllersDir + SubdirIdentity + "IdentityController",
			// 1: Health
			ControllersDir + SubdirHealth + "HealthController",
			// 2: Mover
			ControllersDir + SubdirMover + "AirCraftMover",
			// 3: Detector
			ControllersDir + SubdirDetector + "DetectorController",
			// 4: Attack
			ControllersDir + SubdirAttack + "AttackController",
			// 5: Container
			ControllersDir + SubdirContainer + "ContainerController",
			// 6: Embarker
			ControllersDir + SubdirEmbark + "EmbarkerController"
		},
		// 1: Surface
		{
			// 0: Identity
			ControllersDir + SubdirIdentity + "IdentityController",
			// 1: Health
			ControllersDir + SubdirHealth + "HealthController",
			// 2: Mover
			ControllersDir + SubdirMover + "NavyMover",
			// 3: Detector
			ControllersDir + SubdirDetector + "DetectorController",
			// 4: Attack
			ControllersDir + SubdirAttack + "AttackController",
			// 5: Container
			ControllersDir + SubdirContainer + "ContainerController",
			// 6: Embarker
			ControllersDir + SubdirEmbark + "EmbarkerController"
		},
		// 2: Subsurface
		{
			// 0: Identity
			ControllersDir + SubdirIdentity + "IdentityController",
			// 1: Health
			ControllersDir + SubdirHealth + "HealthController",
			// 2: Mover
			ControllersDir + SubdirMover + "NavyMover",
			// 3: Detector
			ControllersDir + SubdirDetector + "DetectorController",
			// 4: Attack
			ControllersDir + SubdirAttack + "AttackController",
			// 5: Container
			ControllersDir + SubdirContainer + "ContainerController",
			// 6: Embarker
			ControllersDir + SubdirEmbark + "EmbarkerController"
		},
		// 3: Marine
		{
			// 0: Identity
			ControllersDir + SubdirIdentity + "IdentityController",
			// 1: Health
			ControllersDir + SubdirHealth + "HealthController",
			// 2: Mover
			ControllersDir + SubdirMover + "MarineMover",
			// 3: Detector
			ControllersDir + SubdirDetector + "DetectorController",
			// 4: Attack
			ControllersDir + SubdirAttack + "AttackController",
			// 5: Container
			ControllersDir + SubdirContainer + "ContainerController",
			// 6: Embarker
			ControllersDir + SubdirEmbark + "EmbarkerController"
		},
		// TODO: Weapon controllers and prefab
		// 4: Weapon
		{
			// 0: Identity
			"",
			// 1: Health
			"",
			// 2: Mover
			"",
			// 3: Detector
			"",
			// 4: Attack
			"",
			// 5: Container
			"",
			// 6: Embarker
			""
		},
		// 5: Carrier
		{
			// 0: Identity
			ControllersDir + SubdirIdentity + "IdentityController",
			// 1: Health
			ControllersDir + SubdirHealth + "HealthController",
			// 2: Mover
			ControllersDir + SubdirMover + "NavyMover",
			// 3: Detector
			ControllersDir + SubdirDetector + "DetectorController",
			// 4: Attack
			ControllersDir + SubdirAttack + "AttackController",
			// 5: Container
			ControllersDir + SubdirContainer + "ContainerController",
			// 6: Embarker
			ControllersDir + SubdirEmbark + "EmbarkerController"
		},
	};
	public static readonly string[,] ControllersByTagAndFamily = ControllersByPrefabAndFamily;

	/**
	 * Determine which prefab is to be loaded based on the values stored within
	 * a given JSON dictionary.
	 *
	 * @param dict
	 * 		JSON dictionary that has been created from the database tables
	 *
	 * @param tag
	 * 		String tag which, to the best of my understanding, looks to be the
	 * 		name of the database table from which the item was grabbed. Examples
	 * 		include "Subsurface", "Air", "Surface", "Marine"
	 *
	 * @return
	 * 		A string name of a prefab that is to be used as the base object
	 * 		outline for a newly loaded/created object.
	 */
	public static string DeterminePrefab(IDictionary dict, string tag) {
		//return PrefabsDir + tag + ".prefab";
		int index = Array.IndexOf(Tags, tag);
		index = index >= Tags.Length ? 0 : index;
		return Prefabs[index];
	}

	/**
	 * Determine which (more granular) Controller will be held responsible for
	 * the information wihtin certain header in a JSON dictionary.
	 *
	 * @param paramName
	 * 		Name of the inner dictionary header in the JSON file.
	 *
	 * @param prefab
	 * 		Name of the prefab used as a skeleton for the basic-level object 
	 *
	 * @return
	 * 		string name of the controller to instantiate for the given prefab
	 * 		and parameter combination
	 */
	public static string DetermineControllerCreate(string paramName, string prefab) {
		int x = Array.IndexOf(Prefabs, prefab);
		int y = Array.IndexOf(ControllerFamilies, paramName);
		if (x < 0 || y < 0) {
			//Debug.Log("Prefab: " + prefab + "\tController: " + paramName);
			//Debug.Log("PrefabIndex: " + x + "\tControllerIndex: " + y);
			throw new ArgumentException("ObjectFactoryHelper: Could not determine correct controller");
		}
		return ControllersByPrefabAndFamily[x,y];
	}

	/**
	 * Determine the controller name for a specific parameter within the JSON
	 * dictionary while we are trying to decontruct/destroy an object.
	 *
	 * @param paramName
	 * 		name of the general family of controllers to look for.
	 *
	 * @param toDestroy
	 * 		GameObject instance that is to be destroyed. This is used to determine
	 * 		which exact controller to pass information to. TODO: rewrite this comment more sensibly
	 */
	public static string DetermineControllerDestroy(string paramName, GameObject toDestroy) {
		if (toDestroy == null) {
			throw new ArgumentException("ObjectFactoryHelper: cannot determine controller with null assets");
		}
		if (paramName == null || paramName.Equals("")) {
			throw new ArgumentException("ObjectFactoryHelper: no parameter name supplied");
		}
		int x = Array.IndexOf(Tags, toDestroy.tag);
		int y = Array.IndexOf(ControllerFamilies, paramName);
		if (x < 0 || y < 0) {
			throw new ArgumentException("ObjectFactoryHelper: Could not determine correct controller");
		}
		return ControllersByTagAndFamily[x,y];
	}

    /**
	 * Determine the controller name for a specific parameter within the JSON
	 * dictionary while we are trying to decontruct/destroy an object.
	 *
	 * @param paramName
	 * 		name of the general family of controllers to look for.
	 *
	 * @param toDestroy
	 * 		GameObject instance that is to be destroyed. This is used to determine
	 * 		which exact controller to pass information to. TODO: rewrite this comment more sensibly
	 */
    public static string DetermineControllerDestroyMultiThreaded(string paramName, string tagName)
    {
        if (paramName == null || paramName.Equals(""))
        {
            throw new ArgumentException("ObjectFactoryHelper: no parameter name supplied");
        }
        int x = Array.IndexOf(Tags, tagName);
        int y = Array.IndexOf(ControllerFamilies, paramName);
        if (x < 0 || y < 0)
        {
            throw new ArgumentException("ObjectFactoryHelper: Could not determine correct controller");
        }
        return ControllersByTagAndFamily[x, y];
    }

	public static string DetermineControllerLiveObject(string paramName, GameObject liveObject) {
		return DetermineControllerDestroy(paramName, liveObject);
	}

}
