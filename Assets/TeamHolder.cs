using UnityEngine;
using System.Collections;

public class TeamHolder : MonoBehaviour {

	public Team AdminTeam;

	// Use this for initialization
	void Start () {
	
		Team.AddNewTeam("Admin");
		AdminTeam=Team.GetTeam(0);
		AdminTeam.AddUser (new User(PermissionLevel.Admin, MilitaryBranch.Navy, 0, "Admin", null));
        //Debug.Log("test");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
