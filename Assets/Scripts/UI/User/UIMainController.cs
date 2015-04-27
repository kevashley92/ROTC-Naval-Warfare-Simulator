/*****
 * 
 * Name: UIMainController
 * 
 * Date Created: 2015-01-XX
 * 
 * Original Team: UI
 * 
 * This class holds functionality needed to process a user's mouse commands to select and control units and controls the transitions between UI panels.
 * 
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIMainController : MonoBehaviour {
	
	/**
	 * The camera to raycast from for unit selection
	 */
	public Camera MapCamera;
	/**
	 * The user setup UI Panel
	 */
	public GameObject UserSetupPanel;
	/**
	 * The unit inspector UI Panel
	 */
	public GameObject InspectorPanel;
	/**
	 * The carrier UI Panel
	 */
	public GameObject CarrierPanel;
	/**
	 * The settings panel
	 */
	public GameObject SettingsPanel;
	/**
	 * The button that brings up the user setup panel
	 */
	public GameObject UserSetupButton;
	/**
	 * The button that brings up the carrier panel
	 */
	public GameObject CarrierDisplayButton;
	/**
	 * The button initializes embarking for the selected unit
	 */
	public GameObject EmbarkUnitButton;
	public GameObject ChatView;
	/**
	 * The button that brings up the inspector panel
	 */
	public GameObject InspectorDisplayButton;
	/**
	 * The input field for typing in a display name
	 */
	public GameObject DisplayNameSpecifier;
	/**
	 * The gameobject that holds all units in the game
	 */
	public GameObject UnitContainer;
	/**
	 * The weather text
	 */
	public GameObject WeatherDisplay;
	public GameObject TeamDropdownObject;
	private ComboBox TeamDropdown;
	public GameObject SubDropdownObject;
	private ComboBox SubDropdown;
	public ComboBox LanguageDropdown;
	/**
	 * The unit currently selected
	 */
	private GameObject SelectedUnit;
	/**
	 * The unit currently moused over
	 */
	private GameObject MouseOverUnit;
	/**
	 * If the currently selected unit is targeting
	 */
	private bool Targeting;
	/**
	 * If the currently selected unit is moving
	 */
	private bool Moving;
	/**
	 * If the currently selected unit is embarking
	 */
	private bool Embarking;
	/**
	 * If the user has joined a team
	 */
	private bool JoinedTeam = false;
	/**
	 * If the user is a submarine player
	 */
	private bool SubPlayer = false;
	private List<GameObject> TeamSubs = new List<GameObject>();
	private GameObject SelectedSub = null;
	public GameObject SubToggleObject, MouseCoordinates, ScenarioInfo;
	private Toggle SubToggle;
	/**
	 * The current user
	 */
	private User ThisUser;
	/**
	 * The username of the current user
	 */
	private string DisplayName;
	/**
	 * Thea team ID of the current user
	 */
	private int TeamID = 1;
	private GameObject UniversalUIObject;
	public AudioClip UIBloop;
	
	// Use this for initialization
	void Start () {
		Invoke("TeamDropdownSetup", .1f);
	}
	
	// Update is called once per frame
	void Update () {
		//		if (!JoinedTeam) {
		//		}

		if (SelectedUnit != null && Input.GetMouseButtonDown (1)) {
			SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
		}
		WeatherDisplay.GetComponent<Text> ().text = Weather.GetWeatherType (GlobalSettings.GetCurrentWeatherIndex ());
		
		// Since the user is added by the server, there might be lag in getting user
		if(ThisUser == null){
			// If there is lag
			
			// Try to get the user again
			ThisUser = Team.GetUser(DisplayName, TeamID);
			
			if(ThisUser == null){
				
				//Debug.Log ("User " + DisplayName + " has still not been added. Trying again next frame.");
				
			}
			else {
				
				//Debug.Log("User " + DisplayName + " has been sucessfully retrieved.");
				
			}
			
		}
		GameObject[] SurfaceUnits = GameObject.FindGameObjectsWithTag("Surface");
		GameObject[] SubSurfaceUnits = GameObject.FindGameObjectsWithTag("Subsurface");
		GameObject[] AirUnits = GameObject.FindGameObjectsWithTag("Air");
		GameObject[] MarineUnits = GameObject.FindGameObjectsWithTag("Marine");
		// Radar: initially set all units as invisible...
		foreach(GameObject Unit in SurfaceUnits){
			Unit.GetComponent<SpriteRenderer>().enabled = false;
			Unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
			Unit.GetComponent<CircleCollider2D>().enabled = false;
		}
		foreach(GameObject Unit in SubSurfaceUnits){
			Unit.GetComponent<SpriteRenderer>().enabled = false;
			Unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
			Unit.GetComponent<CircleCollider2D>().enabled = false;
		}
		foreach(GameObject Unit in AirUnits){
			Unit.GetComponent<SpriteRenderer>().enabled = false;
			Unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
			Unit.GetComponent<CircleCollider2D>().enabled = false;
		}
		foreach(GameObject Unit in MarineUnits){
			Unit.GetComponent<SpriteRenderer>().enabled = false;
			Unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
			Unit.GetComponent<CircleCollider2D>().enabled = false;
		}
		if (JoinedTeam && ThisUser != null) {
			// Then reveal the visible ones
			if(SubPlayer){
				SelectedSub.GetComponent<SpriteRenderer>().enabled = true;
				UpdateObjectiveSprite( SelectedSub );
				SelectedSub.GetComponent<CircleCollider2D>().enabled = true;
				List<GameObject> VisibleUnits = SelectedSub.GetComponent<DetectorController>().Ping();
				foreach (GameObject Unit in VisibleUnits) {
					if(Unit.GetComponent<IdentityController>().GetTeam() != TeamID){
						if(Unit.GetComponent<EmbarkerController>() != null){
							if(!Unit.GetComponent<EmbarkerController>().getIsInSomething()){
								if(ThisUser.MyPermissionLevel != PermissionLevel.Spectator){
									Unit.GetComponent<CircleCollider2D>().enabled = true;
								}
								Unit.GetComponent<SpriteRenderer>().enabled = true;
								UpdateObjectiveSprite( Unit );
							}
						}
						else{
							if(ThisUser.MyPermissionLevel != PermissionLevel.Spectator){
								Unit.GetComponent<CircleCollider2D>().enabled = true;
							}
							Unit.GetComponent<SpriteRenderer>().enabled = true;
							UpdateObjectiveSprite( Unit );
						}
					}
				}
			}
			else{
				List<GameObject> VisibleUnits = Team.GetTeam(TeamID). GetVisibleToTeam();
				foreach (GameObject Unit in VisibleUnits) {
					if(Unit.GetComponent<EmbarkerController>() != null){
						if(!Unit.GetComponent<EmbarkerController>().getIsInSomething()){
							if(ThisUser.MyPermissionLevel != PermissionLevel.Spectator){
								Unit.GetComponent<CircleCollider2D>().enabled = true;
							}
							Unit.GetComponent<SpriteRenderer>().enabled = true;
							UpdateObjectiveSprite( Unit );
						}
					}
					else{
						if(ThisUser.MyPermissionLevel != PermissionLevel.Spectator){
							Unit.GetComponent<CircleCollider2D>().enabled = true;
						}
						Unit.GetComponent<SpriteRenderer>().enabled = true;
						UpdateObjectiveSprite( Unit );
					}
				}
			}
		}
		else{
			for (int i = 0; i < UnitContainer.transform.childCount; i++) {
				UnitContainer.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
				UnitContainer.transform.GetChild(i).FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
				UnitContainer.transform.GetChild(i).gameObject.GetComponent<CircleCollider2D>().enabled = false;
			}
		}
		// Check if mousing over and/or selecting new unit/target
		// If mousing over a unit...
		GameObject HitObject = RaycastForObject ();
		if (HitObject != null && HitObject.GetComponent<UIUnitStateController>() != null){
			// Play sound when initially mosuing over if sound turned on

			if(MouseOverUnit != null){
				MouseOverUnit.GetComponent<UIUnitStateController>().GetMouseOver().GetComponent<SpriteRenderer>().enabled = false;
				MouseOverUnit.GetComponent<UIUnitStateController>().SetSelected(false);
			}
			MouseOverUnit = HitObject;
			// If this unit isn't already selected, enable the mouseover sprite
			if(MouseOverUnit.GetComponent<UIUnitStateController>().IsSelected() == false){
				MouseOverUnit.GetComponent<UIUnitStateController>().GetMouseOver().GetComponent<SpriteRenderer>().enabled = true;
				// If user left clicks on moused over unit, either select the unit or target it
				if(Input.GetMouseButtonDown(0)){
					// If selecting a target for currently selected unit, target the moused over unit
					if(Targeting){
						//						if(UniversalUIObject.GetComponent<UIUniversalController>().IsSoundEnabled()){
						//							audio.PlayOneShot(UIBloop);
						//						}
						gameObject.GetComponent<UIUnitInspectorController>().TargetUnit(SelectedUnit, MouseOverUnit);
						Targeting = false;
					}
					if(Embarking){
						if(MouseOverUnit.GetComponent<ContainerController>() != null){
							if(SelectedUnit.GetComponent<SubmarineMover>() != null){
								if(Vector3.Distance(SelectedUnit.transform.position, MouseOverUnit.transform.position) <= SelectedUnit.GetComponent<SubmarineMover>().GetMoveRange()){
									MouseOverUnit.GetComponent<ContainerController>().Receive(SelectedUnit.GetComponent<IdentityController>().GetGuid());
									SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
									SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
									gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
									SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
									SelectedUnit = null;
								}
								else{
									//Debug.Log("Unit not within embarking range.");
								}
							}
							else if(SelectedUnit.GetComponent<NavyMover>() != null){
								if(Vector3.Distance(SelectedUnit.transform.position, MouseOverUnit.transform.position) <= SelectedUnit.GetComponent<NavyMover>().GetMoveRange()){
									MouseOverUnit.GetComponent<ContainerController>().Receive(SelectedUnit.GetComponent<IdentityController>().GetGuid());
									SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
									SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
									gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
									SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
									SelectedUnit = null;
								}
								else{
									//Debug.Log("Unit not within embarking range.");
								}
							}
							else if(SelectedUnit.GetComponent<AirCraftMover>() != null){
								if(Vector3.Distance(SelectedUnit.transform.position, MouseOverUnit.transform.position) <= SelectedUnit.GetComponent<AirCraftMover>().GetMoveRange()){
									MouseOverUnit.GetComponent<ContainerController>().Receive(SelectedUnit.GetComponent<IdentityController>().GetGuid());
									SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
									SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
									gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
									SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
									SelectedUnit = null;
								}
								else{
									//Debug.Log("Unit not within embarking range.");
								}
							}
							else if(SelectedUnit.GetComponent<MarineMover>() != null){
								if(Vector3.Distance(SelectedUnit.transform.position, MouseOverUnit.transform.position) <= SelectedUnit.GetComponent<MarineMover>().GetMoveRange() / 10){
									MouseOverUnit.GetComponent<ContainerController>().Receive(SelectedUnit.GetComponent<IdentityController>().GetGuid());
									SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
									SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
									gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
									SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
									SelectedUnit = null;
								}
								else{
									//Debug.Log("Unit not within embarking range.");
								}
							}
						}
						else{
							//Debug.Log("Cannot embark this unit.");
						}
						StopEmbarking();
					}
					else{
						SettingsPanel.SetActive(false);
						CarrierPanel.SetActive(false);
						UserSetupPanel.SetActive(false);
						InspectorPanel.SetActive(true);
						// If a unit is already selected, first clear the targeting animations of any targeted units
						if(SelectedUnit != null){
							SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
							SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
							gameObject.GetComponent<UIUnitInspectorController>().ClearTargets();
							SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
							CancelMoving();
						}
						// Then select the new unit and display its information in the side panel
						SelectedUnit = MouseOverUnit;

						if(SelectedUnit.GetComponent<ContainerController>() != null){
							CarrierDisplayButton.SetActive(true);
						}
						else{
							CarrierDisplayButton.SetActive(false);
						}
						if(SelectedUnit.GetComponent<EmbarkerController>() != null){
							EmbarkUnitButton.SetActive(true);
						}
						else{
							EmbarkUnitButton.SetActive(false);
						}
						gameObject.GetComponent<UIUnitInspectorController>().Inspect(SelectedUnit);
						gameObject.GetComponent<UIUnitInspectorController>().DisplayWeapons(SelectedUnit);
						SelectedUnit.GetComponent<UIUnitStateController>().GetMouseOver().GetComponent<SpriteRenderer>().enabled = false;
						SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = true;
						SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(true);
					}
				}
			}
		}
		// If no longer mousing over a unit, disable the mouseover sprite
		else{
			if(MouseOverUnit != null){
				MouseOverUnit.GetComponent<UIUnitStateController>().GetMouseOver().GetComponent<SpriteRenderer>().enabled = false;
				MouseOverUnit = null;
			}
		}
		if (Moving) {
			// Get new movement plan
			if(Input.GetMouseButtonDown(0)){
				Vector3 mouse = Input.mousePosition;
				mouse.z = MapCamera.transform.position.z * -1;
				Vector3 moveTo = MapCamera.ScreenToWorldPoint(mouse);
				moveTo.z = SelectedUnit.transform.position.z;
				
				if(SelectedUnit.GetComponent<SubmarineMover>() != null){
					SelectedUnit.GetComponent<SubmarineMover>().PlanMove(moveTo);
				}
				else if(SelectedUnit.GetComponent<NavyMover>() != null){
					SelectedUnit.GetComponent<NavyMover>().PlanMove(moveTo);
				}
				else if(SelectedUnit.GetComponent<AirCraftMover>() != null){
					SelectedUnit.GetComponent<AirCraftMover>().PlanMove(moveTo);
				}
				else if(SelectedUnit.GetComponent<MarineMover>() != null){
					// Need to delete movement nodes here?
				}
				CancelMoving();
				
			}
		}
		// If the user right clicks, cancel any ongoing target or movement events and unselect the selected unit if there is one
		if(Input.GetMouseButtonDown(1)){
            DeselectUnit();
		}
		
		
	}

    public void DeselectUnit()
    {
        if (Moving)
        {
            CancelMoving();
        }
        if (Targeting)
        {
            gameObject.GetComponent<UIUnitInspectorController>().StopTargeting();
            ////Debug.Log("Stopped targeting.");
            Targeting = false;
        }
        if (SelectedUnit != null)
        {
            SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
            SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
            SelectedUnit.transform.FindChild("Radar Range Sprite").gameObject.SetActive(false);
            SelectedUnit = null;
            gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
            CarrierDisplayButton.SetActive(false);
            CarrierPanel.SetActive(false);
            EmbarkUnitButton.SetActive(false);
        }
    }
	
	public void SetSelectedUnit(GameObject Unit){
		SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
		this.SelectedUnit = Unit;
		gameObject.GetComponent<UIUnitInspectorController>().Inspect (Unit);
	}

    public GameObject GetSelectedUnit()
    {
        return (SelectedUnit == null) ? null : SelectedUnit;
    }
	
	public void DisplayNextWeapon(){
		if (Targeting) {
			Targeting = false;
			gameObject.GetComponent<UIUnitInspectorController>().StopTargeting();
		}
		gameObject.GetComponent<UIUnitInspectorController>().NextWeapon ();
	}
	
	public void DisplayPreviousWeapon(){
		if (Targeting) {
			Targeting = false;
			gameObject.GetComponent<UIUnitInspectorController>().StopTargeting();
		}
		gameObject.GetComponent<UIUnitInspectorController>().PreviousWeapon ();
	}
	
	public void SetTargeting(bool Targeting){
		this.Targeting = Targeting;
		if (Targeting) {
			Moving = false;
			Embarking = false;
		}
	}
	
	public void StartTargeting(){
		CancelMoving ();
		gameObject.GetComponent<UIUnitInspectorController>().StartTargeting ();
	}
	
	public void CancelTarget(){
		gameObject.GetComponent<UIUnitInspectorController>().CancelTarget ();
	}
	
	public void SelectWeaponTarget(){
		CarrierDisplayButton.SetActive(false);
		gameObject.GetComponent<UIUnitInspectorController>().SelectWeaponTarget ();
	}
	
	public void DisplayNextTarget(){
		gameObject.GetComponent<UIUnitInspectorController>().NextTarget ();
	}
	
	public void DisplayPreviousTarget(){
		gameObject.GetComponent<UIUnitInspectorController>().PreviousTarget ();
	}
	
	public void StartMoving(){
		gameObject.GetComponent<UIUnitInspectorController>().StopTargeting();
		Targeting = false;
		Embarking = false;
		Moving = true;
		if(SelectedUnit.GetComponent<SubmarineMover>() != null){
			SelectedUnit.GetComponent<SubmarineMover> ().DrawRange ();
		}
		else if(SelectedUnit.GetComponent<NavyMover>() != null){
			SelectedUnit.GetComponent<NavyMover> ().DrawRange ();
		}
		else if(SelectedUnit.GetComponent<AirCraftMover>() != null){
			SelectedUnit.GetComponent<AirCraftMover> ().DrawRange ();
		}
		else if(SelectedUnit.GetComponent<MarineMover>() != null){
			SelectedUnit.GetComponent<MarineMover> ().GeneratePossibleMoves ();
		}
	}
	
	public void CancelMoving(){
		Moving = false;
		if(SelectedUnit.GetComponent<SubmarineMover>() != null){
			SelectedUnit.GetComponent<SubmarineMover>().DeleteCircle();
		}
		else if(SelectedUnit.GetComponent<NavyMover>() != null){
			SelectedUnit.GetComponent<NavyMover>().DeleteCircle();
		}
		else if(SelectedUnit.GetComponent<AirCraftMover>() != null){
			SelectedUnit.GetComponent<AirCraftMover>().DeleteCircle();
		}
		else if(SelectedUnit.GetComponent<MarineMover>() != null){
			SelectedUnit.GetComponent<MarineMover> ().DestroyPossibleMoves ();
		}
	}
	
	public void JoinTeam(int TeamID){
		if(!Team.UsernameInUse(DisplayNameSpecifier.GetComponent<Text> ().text) || !DisplayNameSpecifier.GetComponent<Text> ().text.Equals("")){
			//TODO: Check if user/spectator and if navy/marines
			this.TeamID = TeamID;
			this.DisplayName = DisplayNameSpecifier.GetComponent<Text> ().text;
			Team.AddNewUser (GameObject.Find("UniversalUIObject").GetComponent<UIUniversalController>().permission, GameObject.Find("UniversalUIObject").GetComponent<UIUniversalController>().branch, TeamID, DisplayNameSpecifier.GetComponent<Text> ().text);
			ThisUser = Team.GetUser(DisplayNameSpecifier.GetComponent<Text> ().text, TeamID);
			UserSetupPanel.SetActive(false);
			UnitContainer.SetActive(true);
			UserSetupButton.SetActive(false);
			JoinedTeam = true;
			ChatView.SetActive(true);
		}
		else{
			//TODO: in game alert
			//Debug.Log("Username in use or left blank. Please choose a new username.");
		}
	}
	
	public void DisplayCarrierPanel(){
		if(SelectedUnit != null){
			CancelMoving ();
			CancelTarget ();
		}
		CarrierDisplayButton.SetActive (false);
		SettingsPanel.SetActive (false);
		InspectorDisplayButton.SetActive (true);
		InspectorPanel.SetActive (false);
		CarrierPanel.SetActive(true);
		gameObject.GetComponent<UICarrierDisplayController> ().Display (SelectedUnit);
	}
	
	public void DisplayInspectorPanel(){
		if(SelectedUnit != null){
			CancelMoving ();
			CancelTarget ();
		}
		SettingsPanel.SetActive (false);
		if (SelectedUnit.GetComponent<ContainerController> () != null) {
			CarrierDisplayButton.SetActive (true);
		}
		else{
			CarrierDisplayButton.SetActive (false);
		}
		InspectorDisplayButton.SetActive (false);
		CarrierPanel.SetActive(false);
		InspectorPanel.SetActive (true);
	}
	
	public void DisplaySettingsPanel(){
		if(SelectedUnit != null){
			CancelMoving ();
			CancelTarget ();
			SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
			SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
			SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
			SelectedUnit = null;
			gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
		}
		UserSetupPanel.SetActive (false);
		SettingsPanel.SetActive (true);
		CarrierDisplayButton.SetActive (false);
		InspectorDisplayButton.SetActive (false);
		CarrierPanel.SetActive(false);
		InspectorPanel.SetActive (false);
	}
	
	public void DisplayUserSetupPanel(){
		if(SelectedUnit != null){
			CancelMoving ();
			CancelTarget ();
			SelectedUnit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
			SelectedUnit.GetComponent<UIUnitStateController>().SetSelected(false);
			SelectedUnit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
			SelectedUnit = null;
			gameObject.GetComponent<UIUnitInspectorController>().StopInspecting();
		}
		SettingsPanel.SetActive (false);
		CarrierDisplayButton.SetActive (false);
		InspectorDisplayButton.SetActive (false);
		CarrierPanel.SetActive(false);
		InspectorPanel.SetActive (false);
		UserSetupPanel.SetActive (true);
	}
	
	public void StartEmbarking(){
		Embarking = true;
	}
	
	public void StopEmbarking(){
		Embarking = false;
	}

	public User GetUser(){
		return ThisUser;
	}
	
	private GameObject RaycastForObject()
	{
		RaycastHit2D hit = Physics2D.Raycast(MapCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit != null && hit.collider != null)
			return hit.transform.gameObject;
		return null;
	}
	
	private void UpdateTeam(int index){
	}
	
	private void UpdateLanguage(int index){
	}
	
	private void TeamDropdownSetup(){
		if(Team.GetTeams() == null || Team.GetTeams ().Count <= 1) {
			Invoke("TeamDropdownSetup", .1f);
			return;
		}
		MouseCoordinates.SetActive (true);
		TeamDropdown = TeamDropdownObject.GetComponent<ComboBox> ();
		int TeamNum = Team.GetTeams ().Count - 1;
		string[] TeamNames = new string[TeamNum];
		for (int i = 0; i < TeamNum; i++) {
			TeamNames[i] = Team.GetTeam(i + 1).GetTeamName();
		}
		TeamDropdown.ClearItems ();
		TeamDropdown.AddItems (TeamNames);
		SubToggle = SubToggleObject.GetComponent<Toggle> ();
		//SubDropdownSetup ();
		TeamDropdown.OnSelectionChanged += (int index) => {
			TeamID = index + 1;
			if(SubToggle.isOn){
				SubDropdownSetup();
			}
		};
		LanguageDropdown.OnSelectionChanged += (int index) => {
			UpdateLanguage (index); 
		};
	}
	
	public void SubDropdownSetup(){
		UniversalUIObject = GameObject.Find ("UniversalUIObject");
		SubDropdown = SubDropdownObject.GetComponent<ComboBox> ();
		GameObject[] SubSurfaceUnits = GameObject.FindGameObjectsWithTag("Subsurface");
		TeamSubs = new List<GameObject> ();
		for (int i = 0; i < SubSurfaceUnits.Length; i++) {
			if(SubSurfaceUnits[i].GetComponent<IdentityController>().TeamNumber == TeamID){
				TeamSubs.Add(SubSurfaceUnits[i]);
			}
		}
		string[] SubNames = new string[TeamSubs.Count];
		for (int i = 0; i < TeamSubs.Count; i++) {
			SubNames[i] = TeamSubs[i].GetComponent<IdentityController>().GetName();
		}
		SubDropdown.ClearItems ();
		SubDropdown.AddItems (SubNames);
		SelectedSub = TeamSubs[0];
		SubDropdown.OnSelectionChanged += (int index) => {
			SelectedSub = TeamSubs[index];
		};
	}
	
	public void ToggleSubPlayer(){

		if(SubToggle.isOn){
			//Debug.Log("Toggle on");
			SubPlayer = true;
			SubDropdownObject.SetActive(true);
			SubDropdownSetup();
		}
		else{
			SubPlayer = false;
			SubDropdownObject.SetActive(false);
		}
	}
	
	public void JoinGame(){
		JoinTeam (TeamID);
	}

	public void ShowLog(){
		ScenarioInfo.SetActive (true);
	}

	private void UpdateObjectiveSprite(GameObject unit){
		if(unit == null)
			return;
		if( unit.GetComponent<IdentityController>().IsObjective )
			unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = true;
		else
			unit.transform.FindChild("Objective").GetComponent<SpriteRenderer>().enabled = false;
	}
}
