using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIUnitInspectorController : MonoBehaviour {
	
	public Camera MapCamera;
	public GameObject InspectorPanel;
	public GameObject UnitNameDisplay;
	public GameObject UnitHealthDisplay;
	public GameObject UnitPositionDisplay;
	public GameObject UnitMoveRangeDisplay;
	public GameObject UnitWeaponsNavigator;
	public GameObject WeaponAmmoDisplay; 
	public GameObject WeaponNumberDisplay;
	public GameObject WeaponTargetsNavigator;
	public GameObject TargetShotCountSpecifier;
	public GameObject CarrierInfoButton;
	public GameObject EmbarkButton;
	private GameObject Unit;
	private LinkedList<Weapon> CurrentUnitWeapons = new LinkedList<Weapon>();
	private static LinkedList<string> CurrentWeaponTargets = new LinkedList<string>();
	private static LinkedList<int> CurrentWeaponTargetShots = new LinkedList<int> ();
	private int CurrentTargetIndex = 1;
	private int CurrentWeaponIndex = 1;
	private int RoundsToShoot = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Unit != null) {
			UnitWeaponsNavigator.SetActive(true);
			InspectorPanel.transform.FindChild("Position Label").gameObject.SetActive(true);
			
			if(gameObject.GetComponent<UIMainController>().GetUser() != null && gameObject.GetComponent<UIMainController>().GetUser().TeamNumber == Unit.GetComponent<IdentityController>().GetTeam()){
				UnitWeaponsNavigator.SetActive(true);
				if(Unit.GetComponent<ContainerController>() != null){
					CarrierInfoButton.SetActive(true);
				}
				if(Unit.GetComponent<EmbarkerController>() != null){
					EmbarkButton.SetActive(true);
				}
				InspectorPanel.transform.FindChild("Weapons Label").gameObject.SetActive(true);
				InspectorPanel.transform.FindChild("Health Label").gameObject.SetActive(true);
				InspectorPanel.transform.FindChild("Move Range Label").gameObject.SetActive(true);
				InspectorPanel.transform.FindChild("Move Button").gameObject.SetActive(true);
			}
			else{
				UnitWeaponsNavigator.SetActive(false);
				CarrierInfoButton.SetActive(false);
				EmbarkButton.SetActive(false);
				InspectorPanel.transform.FindChild("Move Button").gameObject.SetActive(false);
				InspectorPanel.transform.FindChild("Weapons Label").gameObject.SetActive(false);
				InspectorPanel.transform.FindChild("Health Label").gameObject.SetActive(false);
				InspectorPanel.transform.FindChild("Move Range Label").gameObject.SetActive(false);
			}
		}
		else{
			InspectorPanel.transform.FindChild("Health Label").gameObject.SetActive(false);
			InspectorPanel.transform.FindChild("Position Label").gameObject.SetActive(false);
			InspectorPanel.transform.FindChild("Weapons Label").gameObject.SetActive(false);
			InspectorPanel.transform.FindChild("Move Range Label").gameObject.SetActive(false);
			UnitWeaponsNavigator.SetActive(false);
			InspectorPanel.transform.FindChild("Move Button").gameObject.SetActive(false);
			CarrierInfoButton.SetActive(false);
		}
		
		
		// Display all relevant information in inspector panel
		if (Unit == null) {
			UnitNameDisplay.GetComponent<Text> ().text = "";
			UnitHealthDisplay.GetComponent<Text> ().text = "";
			UnitPositionDisplay.GetComponent<Text> ().text = "";
			UnitMoveRangeDisplay.GetComponent<Text> ().text = "";
		} 
		else {
			if(gameObject.GetComponent<UIMainController>().GetUser() != null && gameObject.GetComponent<UIMainController>().GetUser().TeamNumber == Unit.GetComponent<IdentityController>().GetTeam()){
				if(Unit.tag.Equals("Air")){
					UnitMoveRangeDisplay.GetComponent<Text> ().text = "" + Unit.GetComponent<AirCraftMover>().GetCurrentRange();
				}
				else{
					UnitMoveRangeDisplay.GetComponent<Text> ().text = "" + Unit.GetComponent<MoverController>().GetMoveRange();
				}
				UnitHealthDisplay.GetComponent<Text> ().text = Unit.GetComponent<HealthController>().CurrentHealth.ToString() + " / " + Unit.GetComponent<HealthController>().MaxHealth.ToString();
			}
			else{
				UnitHealthDisplay.GetComponent<Text> ().text = "";
				UnitMoveRangeDisplay.GetComponent<Text> ().text = "";
			}
            UnitNameDisplay.GetComponent<Text> ().text = Unit.GetComponent<IdentityController>().GetFullName();
			string xPos = System.Convert.ToString(Unit.transform.position.x);
			string yPos = System.Convert.ToString(Unit.transform.position.y);
			// Trim coordinates to 7 characters each
			if(xPos.Length > 7){
				xPos = xPos.Remove(7);
			}
			if(yPos.Length > 7){
				yPos = yPos.Remove(7);
			}
			UnitPositionDisplay.GetComponent<Text> ().text = "(" + xPos + ", " + yPos + ")";
			
			// Display weapons and targets
			if(CurrentUnitWeapons.Count > 0){
				UnitWeaponsNavigator.transform.FindChild("Weapons Navigator").GetComponent<Text>().text = CurrentWeaponIndex + " of " + CurrentUnitWeapons.Count;
                UnitWeaponsNavigator.transform.FindChild("Weapon Type").GetComponent<Text>().text = LanguageManager.instance.getString(Unit.tag.Equals("Marine") ? "Marine_Weapon" : "Weapons", CurrentUnitWeapons.First.Value.GetName().Substring(0, CurrentUnitWeapons.First.Value.GetName().LastIndexOf("-")));
				UnitWeaponsNavigator.transform.FindChild("Range").GetComponent<Text>().text = "" + CurrentUnitWeapons.First.Value.GetRange();
				UnitWeaponsNavigator.transform.FindChild("Damage").GetComponent<Text>().text = "" + CurrentUnitWeapons.First.Value.GetDamage();
				UnitWeaponsNavigator.transform.FindChild("Accuracy").GetComponent<Text>().text = "" + CurrentUnitWeapons.First.Value.GetHitChance();
				if(CurrentUnitWeapons.First.Value.GetCurAmmo() == -1){
					WeaponAmmoDisplay.GetComponent<Text>().text = "INFINITE";
				}
				else{
					WeaponAmmoDisplay.GetComponent<Text>().text = CurrentUnitWeapons.First.Value.GetCurAmmo() + " / " + CurrentUnitWeapons.First.Value.GetMaxAmmo();
				}
				UnitWeaponsNavigator.transform.FindChild("Total Shots Fired").GetComponent<Text>().text = CurrentUnitWeapons.First.Value.GetCurShotsTargeted() + " of " + CurrentUnitWeapons.First.Value.GetMaxShots();
				
				if(CurrentWeaponTargets.Count > 0){
					for(int i = 0; i < CurrentWeaponTargets.Count; i++){
						string targetGuid = CurrentWeaponTargets.First.Value;
						CurrentWeaponTargets.RemoveFirst();
						if(GuidList.GetGameObject(targetGuid) != null){
							GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(true);
						}
						CurrentWeaponTargets.AddLast(targetGuid);
					}
					WeaponTargetsNavigator.GetComponent<Text>().text = CurrentTargetIndex + " of " + CurrentWeaponTargets.Count;
					try{
						GameObject Target = GuidList.GetGameObject(CurrentWeaponTargets.First.Value);
						if(Target != null){
							WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = Target.GetComponent<IdentityController>().GetFullName();
						}
						else{
							WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = "";
						}
					}
					catch(KeyNotFoundException e){
						WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = "";
					}
					WeaponTargetsNavigator.transform.FindChild("Shots Fired at Target").GetComponent<Text>().text = "" + CurrentWeaponTargetShots.First.Value;
				}
				else{
					WeaponTargetsNavigator.GetComponent<Text>().text = "No Targets";
					WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = "";
					WeaponTargetsNavigator.transform.FindChild("Shots Fired at Target").GetComponent<Text>().text = "";
				}
				
			}
		}
	}
	
	/**
	 * Starts inspecting the given unit.
	 * 
	 * @param Unit
	 * 		The unit to be inspected.
	 */
	public void Inspect(GameObject Unit){
		if (Unit != null) {
			Unit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
			Unit.transform.FindChild ("Attack Range Sprite").gameObject.SetActive (false);
			Unit.transform.FindChild ("Move Range Sprite").gameObject.SetActive (false);
		}
		WeaponTargetsNavigator.GetComponent<Text> ().text = "";
		WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = "";
		WeaponTargetsNavigator.transform.FindChild("Shots Fired at Target").GetComponent<Text>().text = "";
		UnitWeaponsNavigator.transform.FindChild("Weapons Navigator").GetComponent<Text>().text = "No Weapons";
		UnitWeaponsNavigator.transform.FindChild("Weapon Type").GetComponent<Text>().text = "";
		UnitWeaponsNavigator.transform.FindChild("Range").GetComponent<Text>().text = "";
		UnitWeaponsNavigator.transform.FindChild("Damage").GetComponent<Text>().text = "";
		UnitWeaponsNavigator.transform.FindChild("Accuracy").GetComponent<Text>().text = "";
		WeaponAmmoDisplay.GetComponent<Text>().text = "";
		UnitWeaponsNavigator.transform.FindChild("Total Shots Fired").GetComponent<Text>().text = "";
		CurrentWeaponTargets.Clear ();
		CurrentUnitWeapons.Clear ();
		CurrentWeaponTargetShots.Clear ();
		this.Unit = Unit;
		InspectorPanel.SetActive (true);
		double scale = 0;
		if(Unit.tag.Equals("Surface")){
			scale = Unit.GetComponent<DetectorController>().SurfaceRange;
		}
		else if(Unit.tag.Equals("Subsurface")){
			scale = Unit.GetComponent<DetectorController>().SonarRange;
		}
		else if(Unit.tag.Equals("Air")){
			scale = Unit.GetComponent<DetectorController>().AirRange;
		}
		else if(Unit.tag.Equals("Marine")){
			scale = Unit.GetComponent<DetectorController>().VisualRange;
		}
		if (gameObject.GetComponent<UIMainController> ().GetUser () != null && gameObject.GetComponent<UIMainController> ().GetUser ().TeamNumber == Unit.GetComponent<IdentityController> ().GetTeam ()) {
			Unit.transform.FindChild ("Radar Range Sprite").transform.localScale = new Vector3( (float)(.0033 * scale), (float)(.0033 * scale), 1);
			Unit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (true);
		}
	}
	
	/**
	 * Stops inspecting the current unit and disables the inspector
	 * 
	 */
	public void StopInspecting(){
		Unit.transform.FindChild ("Move Range Sprite").gameObject.SetActive (false);
		Unit.transform.FindChild ("Attack Range Sprite").gameObject.SetActive (false);
		Unit.transform.FindChild ("Radar Range Sprite").gameObject.SetActive (false);
		this.Unit = null;
		UnitWeaponsNavigator.SetActive(false);
		if(CurrentWeaponTargets.Count > 0){
			for(int i = 0; i < CurrentWeaponTargets.Count; i++){
				string targetGuid = CurrentWeaponTargets.First.Value;
				CurrentWeaponTargets.RemoveFirst();
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(false);
                }
				CurrentWeaponTargets.AddLast(targetGuid);
			}
		}
		WeaponTargetsNavigator.GetComponent<Text> ().text = "";
		WeaponTargetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = "";
		WeaponTargetsNavigator.transform.FindChild("Shots Fired at Target").GetComponent<Text>().text = "";
		CurrentWeaponTargets.Clear();
		CurrentWeaponTargetShots.Clear();
		CurrentUnitWeapons.Clear();
		InspectorPanel.SetActive (false);
	}
	
	public void DisplayWeapons(GameObject SelectedUnit){
		CurrentUnitWeapons.Clear();
		CurrentWeaponIndex = 1;
		foreach (Weapon weapon in SelectedUnit.GetComponent<AttackController>().GetWeapons().Values)
		{
			CurrentUnitWeapons.AddLast(weapon);
		}
		if (CurrentUnitWeapons.Count > 0) {
			CurrentWeaponTargets.Clear();
			foreach (string targetGuid in CurrentUnitWeapons.First.Value.GetTargets().Keys)
			{
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    CurrentWeaponTargets.AddLast(targetGuid);
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(true);
                }
			}
			CurrentWeaponTargetShots.Clear();
			foreach (int shots in CurrentUnitWeapons.First.Value.GetTargets().Values)
			{
				CurrentWeaponTargetShots.AddLast(shots);
			}
			CurrentTargetIndex = 1;
		}
	}
	
	public void NextWeapon(){
		if (Unit != null && CurrentUnitWeapons.Count > 0) {
			foreach (string targetGuid in CurrentWeaponTargets)
			{
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(false);
                }
			}
			Weapon currentWeapon = CurrentUnitWeapons.First.Value;
			CurrentUnitWeapons.RemoveFirst();
			CurrentUnitWeapons.AddLast(currentWeapon);
			CurrentWeaponIndex++;
			if(CurrentWeaponIndex > CurrentUnitWeapons.Count){
				CurrentWeaponIndex = 1;
			}
			else if(CurrentWeaponIndex < 1){
				CurrentWeaponIndex = CurrentUnitWeapons.Count;
			}
			CurrentWeaponTargets.Clear();
			foreach (string targetGuid in CurrentUnitWeapons.First.Value.GetTargets().Keys)
			{
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    CurrentWeaponTargets.AddLast(targetGuid);
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(true);
                }
			}
			CurrentWeaponTargetShots.Clear();
			foreach (int shots in CurrentUnitWeapons.First.Value.GetTargets().Values)
			{
				CurrentWeaponTargetShots.AddLast(shots);
			}
			CurrentTargetIndex = 1;
		}
	}
	
	public void PreviousWeapon(){
		if (Unit != null && CurrentUnitWeapons.Count > 0) {
            foreach (string targetGuid in CurrentWeaponTargets)
            {
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(false);
                }
            }
			Weapon currentWeapon = CurrentUnitWeapons.Last.Value;
			CurrentUnitWeapons.RemoveLast();
			CurrentUnitWeapons.AddFirst(currentWeapon);
			CurrentWeaponIndex--;
			if(CurrentWeaponIndex > CurrentUnitWeapons.Count){
				CurrentWeaponIndex = 1;
			}
			else if(CurrentWeaponIndex < 1){
				CurrentWeaponIndex = CurrentUnitWeapons.Count;
			}
			CurrentWeaponTargets.Clear();
			foreach (string targetGuid in CurrentUnitWeapons.First.Value.GetTargets().Keys)
			{
                if (GuidList.GetGameObject(targetGuid) != null)
                {
                    CurrentWeaponTargets.AddLast(targetGuid);
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(true);
                }
			}
			CurrentWeaponTargetShots.Clear();
			foreach (int shots in CurrentUnitWeapons.First.Value.GetTargets().Values)
			{
				CurrentWeaponTargetShots.AddLast(shots);
			}
			CurrentTargetIndex = 1;
		}
	}
	
	public void StartTargeting(){
		try{
			RoundsToShoot = System.Convert.ToInt32(TargetShotCountSpecifier.GetComponent<Text> ().text);
		}
		catch(System.FormatException e){
			//Debug.Log ("Not a valid shot count input.");
			UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().Select();
			UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().text = "";
			TargetShotCountSpecifier.GetComponent<Text> ().text = "";
			return;
		}
		
		//Debug.Log(CurrentUnitWeapons.Count);
		
		if (CurrentUnitWeapons.First.Value.EnoughSpareRounds (RoundsToShoot)) {
			if(RoundsToShoot == 0){
				//Debug.Log("No rounds to fire.");
			}
			else{
				//Debug.Log("Select target.");
				Unit.transform.FindChild ("Attack Range Sprite").transform.localScale = new Vector3( (float)(.0033 * CurrentUnitWeapons.First.Value.GetRange()), (float)(.0033 * CurrentUnitWeapons.First.Value.GetRange()), 1);
				Unit.transform.FindChild ("Attack Range Sprite").gameObject.SetActive (true);
				gameObject.GetComponent<UIMainController>().SetTargeting(true);
			}
		}
		else{
			//Debug.Log("Not enough spare rounds.");
		}
	}
	
	public void TargetUnit(GameObject SelectedUnit, GameObject MouseOverUnit){
		int i = 1;
		foreach (Weapon weapon in SelectedUnit.GetComponent<AttackController>().GetWeapons().Values){
			if(i == CurrentWeaponIndex){
				weapon.SetTargetEvent(MouseOverUnit.GetComponent<IdentityController>().MyGuid, RoundsToShoot);
				MouseOverUnit.transform.FindChild("Attack Pulse").gameObject.SetActive(true);
				CurrentWeaponTargets.AddLast(MouseOverUnit.GetComponent<IdentityController>().MyGuid);
				CurrentWeaponTargetShots.AddLast(RoundsToShoot);
				//targetsNavigator.transform.FindChild("Target Name").GetComponent<Text>().text = mouseOverUnit.GetComponent<IdentityController>().GetFullName();
				//targetsNavigator.transform.FindChild("Target Name").GetComponent<Button>().interactable = true;
				//Debug.Log("Target Acquired.");
				RoundsToShoot = 0;
				UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().Select();
				UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().text = "";
				TargetShotCountSpecifier.GetComponent<Text> ().text = "";
				break;
			}
			i++;
		}
		Unit.transform.FindChild ("Attack Range Sprite").gameObject.SetActive (false);
	}
	
	public void SelectWeaponTarget(){
		GameObject newSelected = GuidList.GetGameObject(CurrentWeaponTargets.First.Value);
		if(GuidList.GetGameObject(Unit.GetComponent<IdentityController>().GetGuid()) != null){
			Unit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = false;
			Unit.GetComponent<UIUnitStateController>().SetSelected(false);
			if(CurrentWeaponTargets.Count > 0){
				for(int i = 0; i < CurrentWeaponTargets.Count; i++){
					string targetGuid = CurrentWeaponTargets.First.Value;
					CurrentWeaponTargets.RemoveFirst();
					GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(false);
					CurrentWeaponTargets.AddLast(targetGuid);
				}
			}
			CurrentWeaponTargets.Clear();
			CurrentWeaponTargetShots.Clear();
			CurrentUnitWeapons.Clear();
		}
		gameObject.GetComponent<UIMainController>().SetSelectedUnit(newSelected);
		CurrentUnitWeapons.Clear();
		CurrentWeaponIndex = 1;
		foreach (Weapon weapon in Unit.GetComponent<AttackController>().GetWeapons().Values)
		{
			CurrentUnitWeapons.AddLast(weapon);
		}
		if (CurrentUnitWeapons.Count > 0) {
			CurrentWeaponTargets.Clear();
			foreach (string targetGuid in CurrentUnitWeapons.First.Value.GetTargets().Keys)
			{
				CurrentWeaponTargets.AddLast(targetGuid);
				GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(true);
			}
			CurrentWeaponTargetShots.Clear();
			foreach (int shots in CurrentUnitWeapons.First.Value.GetTargets().Values)
			{
				CurrentWeaponTargetShots.AddLast(shots);
			}
		}
		Unit.GetComponent<UIUnitStateController>().GetMouseOver().GetComponent<SpriteRenderer>().enabled = false;
		Unit.GetComponent<UIUnitStateController>().GetSelect().GetComponent<SpriteRenderer>().enabled = true;
		Unit.GetComponent<UIUnitStateController>().SetSelected(true);
		CurrentTargetIndex = 1;
	}
	public void ClearTargets(){
		if(CurrentWeaponTargets.Count > 0){
			for(int i = 0; i < CurrentWeaponTargets.Count; i++){
				string targetGuid = CurrentWeaponTargets.First.Value;
				CurrentWeaponTargets.RemoveFirst();
                GameObject go = GuidList.GetGameObject(targetGuid);
                if (go != null)
                {
                    GuidList.GetGameObject(targetGuid).transform.FindChild("Attack Pulse").gameObject.SetActive(false);
                }
				CurrentWeaponTargets.AddLast(targetGuid);
			}
		}
		CurrentWeaponTargets.Clear();
		CurrentWeaponTargetShots.Clear();
	}
	
	public void StopTargeting(){
		RoundsToShoot = 0;
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().Select();
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().text = "";
		TargetShotCountSpecifier.GetComponent<Text> ().text = "";
	}
	
	public void CancelTarget(){
		int i = 1;
		foreach (Weapon weapon in Unit.GetComponent<AttackController>().GetWeapons().Values){
			if(i == CurrentWeaponIndex){
				if(CurrentWeaponTargets.Count > 0){
					weapon.RemoveTarget(CurrentWeaponTargets.First.Value);
					//Debug.Log("Canceled attack.");
				}
				break;
			}
			i++;
		}
		CurrentWeaponTargets.RemoveFirst ();
		CurrentWeaponTargetShots.RemoveFirst ();
		CurrentTargetIndex--;
		if (CurrentTargetIndex <= 0) {
			if(CurrentWeaponTargets.Count == 0){
				CurrentTargetIndex = 1;
			}
			else{
				CurrentTargetIndex = CurrentWeaponTargets.Count;
			}
		}
	}
	
	public void NextTarget(){
		RoundsToShoot = 0;
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().Select();
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().text = "";
		TargetShotCountSpecifier.GetComponent<Text> ().text = "";
		if (CurrentWeaponTargets.Count > 0) {
			string lastTarget = CurrentWeaponTargets.First.Value;
			int lastTargetShots = CurrentWeaponTargetShots.First.Value;
			CurrentWeaponTargets.RemoveFirst();
			CurrentWeaponTargetShots.RemoveFirst();
			CurrentWeaponTargets.AddLast(lastTarget);
			CurrentWeaponTargetShots.AddLast(lastTargetShots);
			CurrentTargetIndex++;
			if(CurrentTargetIndex > CurrentWeaponTargets.Count){
				CurrentTargetIndex = 1;
			}
		}
	}
	
	public void PreviousTarget(){
		RoundsToShoot = 0;
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().Select();
		UnitWeaponsNavigator.transform.FindChild("Shots to Fire Input").gameObject.GetComponent<InputField>().text = "";
		TargetShotCountSpecifier.GetComponent<Text> ().text = "";
		if (CurrentWeaponTargets.Count > 0) {
			string lastTarget = CurrentWeaponTargets.Last.Value;
			int lastTargetShots = CurrentWeaponTargetShots.Last.Value;
			CurrentWeaponTargets.RemoveLast();
			CurrentWeaponTargetShots.RemoveLast();
			CurrentWeaponTargets.AddFirst(lastTarget);
			CurrentWeaponTargetShots.AddFirst(lastTargetShots);
			CurrentTargetIndex--;
			if(CurrentTargetIndex <= 0){
				CurrentTargetIndex = CurrentWeaponTargets.Count;
			}
		}
	}
}
