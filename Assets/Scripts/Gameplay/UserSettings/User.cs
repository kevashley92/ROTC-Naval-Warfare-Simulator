/*****
 * 
 * Name: Team
 * 
 * Date Created: 2015-03-16s
 * 
 * Original Team: Gameplay
 * 
 * This class will represent a user and its associated attributes.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	-------------------------------------------------------------------------------------------	
 * T. Brennan	2015-03-16	Initial Creation. Added Username, TeamNumber,
 * 							SubPlayer, SubUnit, GetUsername, SetUSername,
 * 							GetUnitsVisibleToMe
 * T. Brennan	2015-03-17	Added MyPermissionLevel, MyBranch. Updated
 * 							related constructors and visible to me
 * 							Added omnipresent and invisible object checking for
 * 							GetVisibleToMes
 * T. Brennan	2015-03-18	Made SetUsername use new static UsernameInUser
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class User {

	/**
	 * The name of the user
	 */
	public string Username;

	/**
	 * The number associated with the team this user is on.
	 */
	public int TeamNumber;

	/**
	 * The permission level of this user
	 */
	public PermissionLevel MyPermissionLevel;

	/**
	 * The branch that this user is playing as
	 */
	public MilitaryBranch MyBranch;

	/**
	 * True 	if the user controls a sub.
	 * False 	if the user is a normal player
	 */
	public bool SubPlayer;

	/**
	 * The GUID or NetworkID of the sub this user controls, or null if they
	 * are not a sub player.
	 */
	public string SubUnit;

	/**
	 * A Null user that is used to avoid null reference exceptions. 
	 */
	public static User NullUser;


	/**
	 * Basic constructor for User
	 * 
	 * @param permissionLevel
	 * 		The permission level of the user
	 * @param branch
	 * 		The branch of the military this user is playing for
	 * @param team
	 * 		The team that this user logs onto
	 * @param username
	 * 		The name of the user
	 * @param unit
	 * 		null for non-sub players
	 * 		The GUID/NetworkID of the sub controlled by this player.
	 */
	public User(PermissionLevel permissionLevel, MilitaryBranch branch, int team, string username, string unit) {

		// Set the permission level
		MyPermissionLevel = permissionLevel;

		// Set the military branch
		MyBranch = branch;

		// Set the team
		TeamNumber = team;

		// Set the username
		Username = username;

		// Check if there is not an associated sub
		if (unit == null) {
			// If there is not

			// Set sub player to false
			SubPlayer = false;

		}
		else {
			// If there is an associated sub

			// Set sub player to true
			SubPlayer = true;

			// Record the GUID/NetworkID of the Sub
			SubUnit = unit;

		}

	}


	/**
	 * Determines what objects are visible to this user
	 * 
	 * @returns
	 * 		A list of all game objects visible to this user
	 */
	public List<GameObject> GetUnitsVisibleToMe() {

		// A set of all objects that the user should be able to see
		HashSet<GameObject> visibleObjects = new HashSet<GameObject>();

		// Check the permission level of the user
		if (MyPermissionLevel == PermissionLevel.Admin || MyPermissionLevel == PermissionLevel.Spectator) {
			// If the user is an Admin or a Spectator

			// Add all objects to their visible objects list
			visibleObjects.UnionWith(GuidList.GetAllObjects());

		}
		else if (MyPermissionLevel == PermissionLevel.User) {
			// If the user is a User

			// Check if the user is a sub player and sub players currently
			// only see what their sub can see.
			if (SubPlayer && !GlobalSettings.GetSubmarineSensorLinkState()) {
				// If this user only sees what the sub sees

				// Add the results of the sub's Ping
				visibleObjects.UnionWith(GuidList.GetGameObject(SubUnit).GetComponent<DetectorController>().Ping());

				// Add the sub itself.
				visibleObjects.Add(GuidList.GetGameObject(SubUnit));

				// Add all omnipresent objects
				visibleObjects.UnionWith(GuidList.GetOmnipresentObjects());

				// Remove all invisible objects
				visibleObjects.ExceptWith(GuidList.GetInvisibleObjects());

			}
			else {
				// If the user sees everything the team sees

				// Get a list of everything the team sees.
				visibleObjects.UnionWith(Team.GetTeam(TeamNumber).GetVisibleToTeam());

			}



		}

		return visibleObjects.ToList<GameObject>();

	}


	/**
	 * Gets the username for this user.
	 * 
	 * @reutrn
	 * 		The username of this user.
	 */
	public string GetUsername() {

		return Username;

	}

	/**
	 * Sets the username of the user. Makes sure the username is available.
	 * 
	 * @param username
	 * 		The new desired username
	 * 
	 * @return
	 * 		True	If the username was set
	 * 		False	If the username was not available
	 */
	public bool SetUsername(string username) {
		
		// Check if the username is used by that team
		if (Team.UsernameInUse(username)) {
			// If the username is already in use
			
			// Return false because the user cannot be added.
			return false;
			
		}

		Username = username;

		return true;

	}


	/**
	 * Gets if the user is a SubPlayer
	 * 
	 * @return
	 * 		True	The player is a sub player
	 * 		False	The player is not a sub player
	 */
	public bool GetSubPlayer() {

		return SubPlayer;

	}


	/**
	 * Gets the string form of the GUID for this players sub.
	 * 
	 * @return
	 * 		The string form of the GUID for this player sub.
	 */
	public string GetSubUnit() {

		return SubUnit;

	}

	public static User getCurrentUser() {
		if (NetworkManager.NetworkInstance.IsServer) {
			User admin = Team.GetUser ("Admin", 0);
			if (admin != null)
				return admin;
		} else {
			UIMainController obj = Object.FindObjectOfType<UIMainController>();
			if (obj.GetUser() != null) return obj.GetUser();
		}
		if (NullUser != null) return NullUser;
		NullUser = new User (PermissionLevel.Spectator, MilitaryBranch.Navy, 0, "Null", null);
		return NullUser;
	}


}
