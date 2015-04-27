/*****
 * 
 * Name: Team
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Gameplay
 * 
 * This class will statically store all of the teams that the game is using.
 * Additionally, an instance of this class represents a single team.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	-------------------------------------------------------------------------------------------	
 * D.Durand  	2015-02-23  Creation
 * D.Durand     2015-02-25  Added code, member, function and class level comments. Also rewrote the AddNewTeam function to ->
 *                          chose the lowest non-negative integer not currently in use as the new Teams int.
 * T. Brennan	2015-03-16	Moved constructor to top of methods, Added
 * 							GetTeams, GetVisibleToTeam, AddNewUser,
 * 							AddNewUser, AddUser, RemoveUser, RemoveUser,
 * 							VerifyUsers stub.
 * T. Brennan	2015-03-17	Updated GetVisibleToTeam, Updated information on
 * 							adding users to accomodate permissions and branch
 * 							Added omnipresent and invisible object checking for
 * 							GetVisibleToTeam
 * T. Brennan	2015-03-18	Made AddNewUser and RemoveUser event driven. Created static version of UsernameInUse
 * 							that checks all teams. Changed non-static UsernameInUse to UsernameOnTeam.
 * D. Durand    2015-03-20  Made GetUser method.
 * T. Brennan	2015-03-22	Changed GetVisibleToTeam to account for not being able to see friendly subs when not linked
 * 							Fixed bug in UsernameInUse logic
 * A. Smith		2015-04-03	Added colors to teams
 */ 

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Team {

	/**
	 * Holds all of the teams being used by the system
	 */
	public static Dictionary<int,Team> Teams;

	/**
	 * The key that refers to this team in the teams dictionary
	 */
	private int MyKey;

	/**
	 * The name of this Team
	 */
	private string TeamName;

	/**
	 * The users on the team.
	 */
	private Dictionary<string, User> Users;

	/**
	 * 	Color of the team
	 */
	private float Color1;
	private float Color2;
	private float Color3;
	/**
	 * Constructs a new Team with the given name and key. Sets to default color, black.
	 */
	public Team (string teamName, int teamKey) {
		this.Users = new Dictionary<string, User>();
		this.TeamName = teamName;
		this.MyKey = teamKey;

        if (teamKey == 0)
        {
            this.Color1 = 0;
            this.Color2 = 0;
            this.Color3 = 0;    
        }
        if (teamKey == 1)
        {
            this.Color1 = 0;
            this.Color2 = 0;
            this.Color3 = 255;
        }
        if (teamKey == 2)
        {
            this.Color1 = 255;
            this.Color2 = 0;
            this.Color3 = 0;
        }
	}
	/**
	 * Adds a new team to the Teams dictionary
	 * Param: string teamName; The name of the new team
	 * Return: int; The number that the back end uses to piont to the new team. ->
	 *              This number should be the lowest non-negative number not in use.
	 */ 
	public static int AddNewTeam(string teamName) {
		// If the Teams Dictionary is null
		if (Teams == null) {
			// Make a new one and use 0 as the first teams int
			Teams = new Dictionary<int,Team> ();
			Teams.Add (0, new Team(teamName, 0));
			return 0;
		} else {
			// If not then we need to sort the keys so that we can find the smallest unused integer

			// Turn the keys into a list
			List<int> keys = new List<int>(Teams.Keys);

			// Sort the list of keys
			keys.Sort();

			// A variable to hold the new int that we will select
			int newTeamIndex;

			// If the first item in the sorted list is not 0
			if(keys[0] != 0) {
				// Then just use 0 since it is the smallest non-negative int not in use
				newTeamIndex = 0;
				Teams.Add (newTeamIndex, new Team(teamName, newTeamIndex));
				return newTeamIndex;
			}
			// If the first item is 0 then we need to iterate through the list
			for(int i = 0; i < (keys.Count - 1); i++) {

				// If the int we are looking at is not exactly one less than the next int
				if((keys[i] + 1) != keys[i + 1]) {

					// Then we know that the smallest unused non-negative int is this int plus one 
					newTeamIndex = keys[i] + 1;
					Teams.Add (newTeamIndex, new Team(teamName, newTeamIndex));
					return newTeamIndex;
				}
			}
			// Because of the return in the loop, then we know that if we have gotten to this ->
			// point the list is numerically sequential and starts with 0.
			// This means that the smallest unused positive int is the next number in the sequence.
			// The length of the list is the number we want.
			newTeamIndex = Teams.Count;
			Teams.Add(newTeamIndex, new Team(teamName, newTeamIndex));

			//Debug.Log("Added new Team: " + newTeamIndex + " " + teamName);

			NetworkManager.NetworkInstance.SendTeams();
			return newTeamIndex;
		}
	}

    public static void addTeamAtIndex(Team team, int index) {
        if (Teams == null)
        {

            Teams = new Dictionary<int, Team>();

        }
        Teams.Add(index, team);
        NetworkManager.NetworkInstance.SendTeams();
    }

	public static void addNewTeamAtIndex(string teamName, int index) {
		
		if(Teams == null) {
			
			Teams = new Dictionary<int, Team>();
			
		}
		Teams.Add(index, new Team(teamName, index));
		NetworkManager.NetworkInstance.SendTeams();
	}


	/**
	 * Creates a new user and adds them to the specified team.
	 * Calls other AddNewUser with unitGuid of null.
	 * @param permissionLevel
	 * 		The permission level of the user
	 * @param branch
	 * 		The branch of the military this user is playing for
	 * @param team
	 * 		The team the user will be added to
	 * @param username
	 * 		The name the user wants to use
	 * @return
	 * 		True	The user was added
	 * 		False	The was not created because the username already exists
	 */
	public static bool AddNewUser(PermissionLevel permissionLevel, MilitaryBranch branch, int team, string username) {
		
		return Team.AddNewUser(permissionLevel, branch, team, username, null);
		
	}
	
	
	/**
	 * Creates a new user and adds them to the specified team.
	 * 
	 * @param permissionLevel
	 * 		The permission level of the user
	 * @param branch
	 * 		The branch of the military this user is playing for
	 * @param team
	 * 		The team the user will be added to
	 * @param username
	 * 		The name the user wants to use
	 * @param unitGuid
	 * 		The string form of the Guid that represents the object.
	 * 
	 * @return
	 * 		True	The user was added
	 * 		False	The was not created because the username already exists
	 */
	public static bool AddNewUser(PermissionLevel permissionLevel, MilitaryBranch branch, int team, string username, string unitGuid) {
		
		// Check if the username is used by that team
		if (Team.UsernameInUse(username)) {
			// If the username is already in use
			//Debug.Log ("Username in use");
			// Return false because the user cannot be added.
			return false;
			
		}
		
		object[] arguments = new object[5];
		arguments[0] = permissionLevel;
		arguments[1] = branch;
		arguments[2] = team;
		arguments[3] = username;
		arguments[4] = unitGuid;

		GEvent e = EventFactory.CreateEvent(GEventType.PlayerJoinEvent , arguments);
		EventManager.Instance.AddEvent(e);
		//Debug.Log ("User Join event raised");
		return true;
		
	}


	/**
	 * Get the Team the the given integer points to
	 * Param: int team; The integer that uniquely identifies the Team of interest
	 * Return: Team; The Team object that corresponds to the given integer
	 */ 
	public static Team GetTeam(int team) {
		Team T = null;
        if (Teams != null)
        {
            Teams.TryGetValue(team, out T);
        }
		return T;
	}
	
	/**
	 * Get the Team the the given name corresponds to
	 */ 
	public static Team GetTeam(string name) {
		Team T = null;
        if (Teams != null)
        {
			for(int i = 0; i < Teams.Count; i++){;
				T = Teams[i];
				if(T.GetTeamName().Equals(name)){
					return T;
				}
			}
        }
		return null;
	}


	/* 
	 * Get a list of all team names
	 */
	public static List<string> GetTeamNames() {
		if(Teams == null) {
			return null;
		}
		List<string> TeamNames = new List<string>();

		foreach(Team t in Teams.Values.ToList()) {
			TeamNames.Add(t.GetTeamName());
		}

		return TeamNames;
	}


	/**
	 * Gets a list of all teams in game
	 */
	public static List<Team> GetTeams() {
		if (Teams == null) {
			return null;
		}
		return Teams.Values.ToList ();

	}

	public static Dictionary<int, Team> GetAllTeams()
	{
		return Teams;
	}


	/**
	 * Determines if the given int points to a team
	 * Param: int team; The int in question
	 * Return: bool; True if the Teams dictionary contains a key of int
	 */
	public static bool HasTeam(int team) {
		Team T;				
		Teams.TryGetValue (team, out T);		
		if (T == null) {
			return false;
		}
		return true;
	}


	/**
	 * Determines if there is a team with the given name
	 * Param: string team; The team name of interest
	 * Return: bool; True if there is a Team in the Teams dictionary with the given name
	 */
	public static bool HasTeam (string team) {
		foreach (Team value in Teams.Values) {
			if(value.GetTeamName() == team) {
				return true;
			}
		}
		return false;
	}

	/**
	 * Removes the Team from the game
	 * 
	 * @param team
	 * 		The team to be removed
	 */
	public static void RemoveTeam(int team) {
		Teams.Remove (team);
		NetworkManager.NetworkInstance.SendTeams();
	}


	/**
	 * Removes the user from the game
	 * 
	 * @param team
	 * 		The team the user is on.
	 * @param username
	 * 		The username of the user to be removed.
	 */
	public static void RemoveUser(int team, string username) {
		
		object[] arguments = new object[2];
		arguments[0] = username;
		arguments[1] = team;
		
		EventManager.Instance.AddEvent(EventFactory.CreateEvent(GEventType.PlayerLeaveEvent , arguments));
		
	}


	/**
	 * Determines if a user exists on any of the teams.
	 * 
	 * @param username
	 * 		The username to check for the existance of.
	 * 
	 * @return
	 * 		True	The username is in use(unavailable).
	 * 		False	The username is available.
	 */
	public static bool UsernameInUse(string username) {
		
		// Iterate over every team
		foreach (Team existingTeam in Teams.Values) {
			
			// Check if the username is used by that team
			if (existingTeam.UsernameOnTeam(username)) {
				// If the username is already in use

				// Return true because the username is in user.
				return true;
				
			}
			
		}
		
		return false;
		
	}
	
	
	/**
	 * TODO Implement this.
	 * Check that all the users are still connected and removes any users
	 * that not disconnected gracefully.
	 */
	public static void VerifyUsers() {
		
		//Debug.LogError("VerifyUsers has not been implemented yet!");
		
	}


	/**
	 * Adds the user to this instance of a team
	 * 
	 * @param newUser
	 * 		The new user to be added to the team.
	 */
	public void AddUser(User newUser) {
		Users.Add (newUser.Username, newUser);
		//Debug.Log("Added User: " + newUser.GetUsername());

		NetworkManager.NetworkInstance.SendTeams();
		
	}


	/**
	 * Gets the name of this Team
	 * 
	 * @return
	 * 		The name of this Team
	 */
	public string GetTeamName() {
		
		return this.TeamName;
		
	}


	/**
	 * Gets a list of all users on this team.
	 * 
	 * @return
	 * 		A list of all users on this team.
	 */
	public List<User> GetUsers() {
		
		return Users.Values.ToList();
		
	}

	/**
	 * Gets a User by username and TeamNumber
	 *
	 * @param username
	 * 		The user name of the desired user
	 * 
	 * @param teamNumber
	 * 		The number of the team that the desired user is on
	 *
	 * @return
	 * 		The requested User. Null if there is not a user with the given user name on the given team
	 */
	public static User GetUser(string username, int teamNumber) {
		Team t = GetTeam (teamNumber);
        if (t != null)
        {
            List<User> users = t.GetUsers();
            foreach (User u in users)
            {
                if (u.Username.Equals(username))
                {
                    return u;
                }
            }
        }
		return null;
	}


	/**
	 * Gets a list of all objects visible to this unit without duplicates.
	 * 
	 * @return
	 * 		A list of all game objects visible to the team
	 */
	public List<GameObject> GetVisibleToTeam() {
		
		// Initialize a list for storing all visible objects, with duplicates
		HashSet<GameObject> visibleObjects = new HashSet<GameObject>();

		////Debug.Log("Team.GetVisibleToTeam: At start of foreach loop. # of Loops: " + GuidList.GetObjectsOnTeam(this.MyKey).Count );

		// Iterate over all objects associated with the team
		foreach (GameObject obj in GuidList.GetObjectsOnTeam(this.MyKey)) {
			
			// Determines all objects visible to this object and add them to the list
			
			List<GameObject> visibletothisone = obj.GetComponent<DetectorController>().Ping();

			////Debug.Log("Team.GetVisibleToTeam: Inside of foreach loop. # of Units found by ping: " + visibletothisone.Count );

			////Debug.Log(visibletothisone.Count);
			
			visibleObjects.UnionWith(visibletothisone);
			
		}

		////Debug.Log("Team.GetVisibleToTeam: At end of foreach loop. # of unique Units found: " + visibleObjects.Count );


		// Check if friendly subs should be visible
		if (GlobalSettings.GetSubmarineSensorLinkState()) {
			// If friendly subs should be visible

			// Add all objects on the team
			visibleObjects.UnionWith(GuidList.GetObjectsOnTeam(MyKey));

		}
		else {
			// If friendly subs should not be visible

			// Add all non-sub objects
			visibleObjects.UnionWith(GuidList.GetNonSubsOnTeam(MyKey));

		}
		
		// Add all omnipresent objects
		visibleObjects.UnionWith(GuidList.GetOmnipresentObjects());
		
		// Remove all invisible objects
		visibleObjects.ExceptWith(GuidList.GetInvisibleObjects());
		
		// Convert back to a list and return.
		return visibleObjects.ToList<GameObject>();
		
	}


	/**
	 * Removes the user from this team.
	 * 
	 * @param username
	 * 		The name of the user to be removed.
	 */
	public void RemoveUser(string username) {
		
		Users.Remove(username);
		//Debug.Log ("Removed User: " + username);
		NetworkManager.NetworkInstance.SendTeams();
		
	}

	
	/**
	 * Sets the name of this Team
	 * Param: string teamName; The new name of this Team
	 */
	public void SetTeamName(string teamName) {
		
		this.TeamName = teamName;
		NetworkManager.NetworkInstance.SendTeams();
		
	}


	/**
	 * Determines of the username provided is in use for this team.
	 * 
	 * @return
	 * 		True	If the username is in use
	 * 		False	If no user has thatusername on this team
	 * 
	 */
	public bool UsernameOnTeam(string username) {

		return Users.ContainsKey(username);

	}

	/**
	 *  Changes the color of a team at the selected index
	 */
	public void SetTeamColor(Color c){

		this.Color1 = c.r;
		this.Color2 = c.g;
		this.Color3 = c.b;

	}

	/**
	 * Getter for team color
	 */
	public Color GetTeamColor(){
		return new Color(Color1, Color2, Color3);
	}

    public static void ClearTeams()
    {
        Teams.Clear();
    }
}