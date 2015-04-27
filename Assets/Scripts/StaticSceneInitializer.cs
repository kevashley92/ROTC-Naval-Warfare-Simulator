/*****
 * 
 * Name: StaticSceneInitializer
 * 
 * Date Created: 2015-02-23
 * 
 * Original Team: Everyone
 * 
 * This class will handle any instantiations, initialization, and configurations
 * of objects that must occur within every scene of a type for the game to work.
 * This should aleviate the need for having a bunch of "DummyScripts" or Demo
 * obects.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------   ---------- 	----------------------------------------------------	
 * T. Brennan	2015-03-27	Initial Commit. Added InitializeGenericSeverScene,
 * 							InitializeGenericClientScene, CreateAdminTeam,
 * 							CreateAdminUser, CreateDemoTeams
 */ 
using UnityEngine;
using System.Collections;

public static class StaticSceneInitializer {
	

	/**
	 * Performs any actions that need to happen on a server scene once it
	 * has been loaded.
	 */
	public static void InitializeGenericServerScene() {

		// Creates an Event Manager for the server
		CreateLocalEventManager();

		// Creates Turn Manager to sync across all users
		CreateNetworkedTurnManager();

		// Create a team for the admins that will have the index 0
		CreateAdminTeam();

		// Create a user for the admin who started the server
		CreateAdminUser();

		// Load extra teams to let user be able to join on the hard doced user
		// scene
		CreateDemoTeams();


	}


	/**
	 * Performs any actions that need to happen on a client scene once it
	 * has been loaded.
	 */
	public static void InitializeGenericClientScene() {

		// Connects to the server
		ConnectToServer();

		// Creates an Event Manager for the client
		CreateLocalEventManager();

	}


	/**
	 * Connects a client to the server.
	 * 
	 * Requires a server to exist.
	 */
	private static void ConnectToServer() {
		NetworkManager.NetworkInstance.ConnectToLocalServer(NetworkManager.ip, 25000, NetworkManager.pass);
	}


	/**
	 * Creates a team for the admins
	 */
	private static void CreateAdminTeam() {

		Team.AddNewTeam ("Admin");

	}


	/**
	 * Creates an admin user
	 * 
	 * Requires that CreateAdminTeam be run first
	 * 
	 * This is not event driven
	 */
	private static void CreateAdminUser() {

		Team AdminTeam=Team.GetTeam(0);
		AdminTeam.AddUser (new User(PermissionLevel.Admin, MilitaryBranch.Navy, 0, "Admin", null));

	}


	/**
	 * Creates any teams needed for a demo
	 * 
	 * Requires that CreateAdminTeam be run first
	 */
	private static void CreateDemoTeams() {

		Team.AddNewTeam ("Friendly Team");
		Team.AddNewTeam ("Hostile Team");

	}


	/**
	 * Creates an event manager locally
	 */
	private static void CreateLocalEventManager() {

		GameObject.Instantiate (Resources.Load ("Prefabs/EventManager"));

	}


	/**
	 * Creates a networked TurnManager
	 */
	private static void CreateNetworkedTurnManager() {

		Network.Instantiate (Resources.Load ("Prefabs/FirstPlayable/TurnManager"), new Vector3 (0, 0, 0), new Quaternion (), 0);

	}







}
