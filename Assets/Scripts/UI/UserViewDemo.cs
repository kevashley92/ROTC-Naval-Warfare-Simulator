using UnityEngine;
using System.Collections;

public class UserViewDemo : MonoBehaviour {

/*	I wasn't sure which Start method to use so I just picked one.	

 	public GameObject FriendlySurface, FriendlyAir, FriendlyCarrier, FriendlyMarine, HostileSubsurface, HostileMarine, HostileSurface;

	// Use this for initialization
	void Start () {
		Team.AddNewTeam ("Admins");
		Team.AddNewTeam ("Friendly Team");
		Team.AddNewTeam ("Hostile Team");
		Team[] Teams = Team.GetTeams ().ToArray ();
		for (int i = 0; i < Teams.Length; i++) {
			//Debug.Log ("Team " + i + " (" + Teams[i].GetTeamName() + ") added.");
		}
	}
*/

	// Use this for initialization
	void Start () {
		
		/*GameObject.Instantiate (Resources.Load ("Prefabs/FirstPlayable/EventManagerServer"));

		Network.Instantiate (Resources.Load ("Prefabs/FirstPlayable/TurnManager"), new Vector3 (0, 0, 0), new Quaternion (), 0);

		Team.AddNewTeam ("Admin");
	
		Team.AddNewTeam ("Friendly Team");
		Team.AddNewTeam ("Hostile Team");		Team AdminTeam=Team.GetTeam(0);
		AdminTeam.AddUser (new User(PermissionLevel.Admin, MilitaryBranch.Navy, 0, "Admin", null));*/

		//StaticSceneInitializer.InitializeGenericServerScene();
		Invoke ("AddTeams", .1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddTeams(){
		Team.AddNewTeam ("Admin");
		
		Team.AddNewTeam ("Friendly Team");
		Team.AddNewTeam ("Hostile Team 1");
		Team.AddNewTeam ("Hostile Team 2"); 
	}
}
