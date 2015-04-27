/*****
 * 
 * Name: UIUniversalController
 * 
 * Date Created: 2015-03-XX
 * 
 * Original Team: UI
 * 
 * This class holds functionality needed to acces universal UI features like enabling sound, changing the language, and logging out
 * 
 */

using UnityEngine;
using System.Collections;

public class UIUniversalController : MonoBehaviour {

	/**
	 * Whether or not sound is enabled
	 */
	private bool SoundEnabled = false;
	/**
	 * The permission level of user, set on login
	 */
	public PermissionLevel permission;
	/**
	 * The military branch of user, set on login
	 */
	public MilitaryBranch branch;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DontDestroyOnLoad (gameObject);
	}

	/**
	 * Logs the user out and removes them from their current team
	 */
	public void Logout(){

		//Remove user from team
		UIMainController UIMC = Object.FindObjectOfType<UIMainController>();
		User user = UIMC.GetUser();
		Team.RemoveUser(user.TeamNumber, user.GetUsername());

		Application.Quit ();
	}

	/**
	 * Enables sound
	 */
	public void EnableSound(){
		SoundEnabled = true;
	}

	/**
	 * Disables sound
	 */
	public void DisableSound(){
		SoundEnabled = false;
	}

	/**
	 * Checks if sound is enabled
	 * @return
	 * 		true if it is enabled, false if it isn't
	 */
	public bool IsSoundEnabled(){
		return SoundEnabled;
	}

}
