using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UnitEditorManager : MonoBehaviour {
	
	//GUI components for displaying unit info
	private GameObject NameVal;
	string name = "n/a";
	private GameObject MaxHealthVal;
	float maxhp = 0f;
	private GameObject CurHealthVal;
	float curhp = 0f;
	private GameObject AirRadarVal;
	float rAir = 0f;
	private GameObject SurfRadarVal;
	float rSurf = 0f;
	private GameObject SubSonarVal;
	float rSub = 0f;
	private GameObject MoveVal;
	float move = 0f;
	
	private ComboBox WepDropdown;
	
	private GameObject WepNameVal;
	string wname = "n/a";
	//public GameObject WepClassVal;
	//string wclass = "n/a";
	//public GameObject WepTypeVal;
	//string wtype = "n/a";
	private GameObject WepRangeVal;
	float wrange = 0f;
	private GameObject WepLethalityVal;
	float wlethal = 0f;
	private GameObject WepProbVal;
	float wprob = 0f;
	private GameObject WepAmmoVal;
	float wammo = 0f;
	private GameObject WepCurrAmmoVal;
	float wcurrammo = 0f;
	private GameObject WepRateVal;
	float wrate = 0f;

    private Toggle ObjectiveToggle;
	
	GameObject currentObject;
	
	// Use this for initialization
	void Awake () {
		
		NameVal = transform.Find ("UnitNameInput").gameObject;
		MaxHealthVal = transform.Find ("UnitAttributes").Find ("MaxHealthInput").gameObject;
		CurHealthVal = transform.Find ("UnitAttributes").Find ("CurrHealthInput").gameObject;
		AirRadarVal = transform.Find ("UnitAttributes").Find ("AirRadarRangeInput").gameObject;
		SurfRadarVal = transform.Find ("UnitAttributes").Find ("SurfRadarRangeInput").gameObject;
		SubSonarVal = transform.Find ("UnitAttributes").Find ("SonarRangeInput").gameObject;
		MoveVal = transform.Find ("UnitAttributes").Find ("MoveInput").gameObject;
        ObjectiveToggle = transform.Find("ObjectiveToggle").GetComponent<Toggle>();
		
		WepDropdown = transform.Find ("ComboBox").gameObject.GetComponent<ComboBox> ();
		WepDropdown.OnSelectionChanged += (int index) => {
			string comboname = WepDropdown.Items [index].LocalItem.ToString();
			FillWeaponFields(comboname);
		};
		
		WepNameVal = transform.Find ("WeaponPanel").Find ("WepNameInput").gameObject;
		//WepClassVal = transform.Find ("WeaponPanel").Find ("WepClassInput").gameObject;
		//WepTypeVal = transform.Find ("WeaponPanel").Find ("WepTypeInput").gameObject;
		WepRangeVal = transform.Find ("WeaponPanel").Find ("RangeInput").gameObject;
		WepLethalityVal = transform.Find ("WeaponPanel").Find ("LethalityInput").gameObject;
		WepProbVal = transform.Find ("WeaponPanel").Find ("ProbInput").gameObject;
		WepAmmoVal = transform.Find ("WeaponPanel").Find ("AmmoInput").gameObject;
		WepCurrAmmoVal = transform.Find ("WeaponPanel").Find ("CurrAmmoInput").gameObject;
		WepRateVal = transform.Find ("WeaponPanel").Find ("RateInput").gameObject;
		
		FillUnitFields ();
	}
	
	public void FillUnitFields(GameObject unit){
		
		IdentityController uid = unit.GetComponent <IdentityController>();
		MoverController umov = unit.GetComponent <MoverController>();
		DetectorController udet = unit.GetComponent <DetectorController>();
		HealthController uhp = unit.GetComponent <HealthController>();
		
		name = uid.GetLocalizedName ();
		InputField namefield = NameVal.GetComponent<InputField> ();
		namefield.text = name;
		InputField currentHealth = CurHealthVal.GetComponent <InputField> ();
		curhp = uhp.GetCurrentHealth();
		currentHealth.text = uhp.GetCurrentHealth ().ToString();
		InputField maxHealth = MaxHealthVal.GetComponent <InputField> ();
		maxhp = uhp.GetMaxHealth ();
		maxHealth.text = uhp.GetMaxHealth ().ToString();
		InputField mov = MoveVal.GetComponent <InputField> ();
		move = umov.GetIdealMoveRange();
		mov.text = move.ToString();
		
		InputField surfrange = SurfRadarVal.GetComponent<InputField> ();
		surfrange.text = udet.SurfaceRange.ToString();
		
		InputField airrange = AirRadarVal.GetComponent<InputField> ();
		airrange.text = udet.AirRange.ToString ();
		
		InputField subrange = SubSonarVal.GetComponent<InputField> ();
		subrange.text = udet.SonarRange.ToString ();
		
		FillWeaponDropdown (unit);
		
	}
	public void FillUnitFields(){
		currentObject = transform.parent.Find ("UnitMenu").Find ("UnitSelect").gameObject.GetComponent<UnitManager> ().GetCurrentObject ();
		if (currentObject == null)
			return;
		FillUnitFields (currentObject);
	}
	
	public void FillWeaponFields(string weapon_key){
		AttackController uat = currentObject.GetComponent <AttackController>();
		
		Weapon w = uat.GetWeapon (weapon_key);
		
		InputField namefield = WepNameVal.GetComponent<InputField> ();
		wname = w.GetName ();
		namefield.text = wname;
		
		InputField rangefield = WepRangeVal.GetComponent<InputField> ();
		wrange = w.GetRange ();
		rangefield.text = wrange.ToString();
		
		InputField lethfield = WepLethalityVal.GetComponent<InputField> ();
		wlethal = w.GetDamage ();
		lethfield.text = wlethal.ToString ();
		
		InputField probfield = WepProbVal.GetComponent<InputField> ();
		wprob = w.GetHitChance ();
		probfield.text = wprob.ToString ();
		
		InputField ammofield = WepAmmoVal.GetComponent<InputField> ();
		wammo = w.GetMaxAmmo ();
		ammofield.text = wammo.ToString();

		InputField currammofield = WepCurrAmmoVal.GetComponent<InputField> ();
		wcurrammo = w.GetCurAmmo ();
		currammofield.text = wcurrammo.ToString ();
		
		InputField ratefield = WepRateVal.GetComponent<InputField> ();
		wrate = w.GetMaxShots ();
		ratefield.text = wrate.ToString ();
		
	}
	public void FillWeaponDropdown(GameObject unit){
		WepDropdown.ClearItems ();
		AttackController uat = unit.GetComponent <AttackController>();
		LocalizedItem[] stringlist = new LocalizedItem[uat.GetWeapons ().Count];
		int listidx = 0;
		foreach(string key in uat.GetWeapons().Keys){
            int i = key.LastIndexOf("-");
            stringlist[listidx++] = new LocalizedItem(unit.tag.Equals("Marine") ? "Marine_Weapon" : "Weapons", key.Substring(0, i), key.Substring(i));
		}
		WepDropdown.AddItems (stringlist);
		WepDropdown.SelectedIndex = 0;
		if(stringlist.Length > 0)
			FillWeaponFields (stringlist [WepDropdown.SelectedIndex].ToString());
	}
	
	
	public void AlterUnitData(){
		//		IdentityController uid = currentObject.GetComponent <IdentityController>();
		//		MoverController umov = currentObject.GetComponent <MoverController>();
		//		DetectorController udet = currentObject.GetComponent <DetectorController>();
		//		AttackController uat = currentObject.GetComponent <AttackController>();
		//		HealthController uhp = currentObject.GetComponent <HealthController>();
		IdentityController uid = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Identity", currentObject)) as IdentityController;
		MoverController umov = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Mover", currentObject)) as MoverController;
		DetectorController udet = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Detector", currentObject)) as DetectorController;
		AttackController uat = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Attack", currentObject)) as AttackController;
		HealthController uhp = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Health", currentObject)) as HealthController;
		
		//Debug.Log ("max health at start: " + uhp.GetMaxHealth ());
		
		Single validator = 0.0f;
		//validate hp input
		string maxhpstring = MaxHealthVal.GetComponent<InputField> ().text;
		bool maxhpvalid = Single.TryParse (maxhpstring,out validator);
		
		float maxhp_event = uhp.GetMaxHealth();
		if (maxhpvalid) { maxhp_event = Single.Parse (maxhpstring); 
			//uhp.SetMaxHealth(maxhp_event);
		}

		string namestring = NameVal.GetComponent<InputField>().text;
		
		string curhpstring = CurHealthVal.GetComponent<InputField> ().text;
		bool curhpvalid = Single.TryParse (curhpstring,out validator);
		float curhp_event = uhp.GetCurrentHealth();
		if (curhpvalid) { curhp_event = Single.Parse(curhpstring); 
			//uhp.SetCurrentHealth(curhp_event);
		}
		//same for movement
		string movstring = MoveVal.GetComponent<InputField> ().text;
		bool movvalid = Single.TryParse (movstring,out validator);
		float mov_event = umov.GetMoveRange ();
		if (movvalid) { mov_event = Single.Parse (movstring); 
			//umov.SetMoveRange(mov_event);
		}
		
		////Debug.Log ("max health after: " + uhp.GetMaxHealth ());
		
		//same for ranges
		
		string airstring = AirRadarVal.GetComponent<InputField> ().text;
		bool airvalid = Single.TryParse (airstring, out validator);
		
		float airval = 0f;
		if (airvalid) { 
			airval = Single.Parse (airstring);
		} else {
			airval = udet.AirRange;
		}
		
		string surfstring = SurfRadarVal.GetComponent<InputField> ().text;
		bool surfvalid = Single.TryParse (surfstring, out validator);
		float surfval = 0f;
		if (surfvalid) { 
			surfval = Single.Parse(surfstring);
		}
		else{
			surfval = udet.SurfaceRange;
		}
		string substring = SubSonarVal.GetComponent<InputField> ().text;
		bool subvalid = Single.TryParse (substring, out validator);
		float subval = 0f;
		if (subvalid) { 
			subval = Single.Parse(substring);
			
		}
		else{
			subval = udet.SonarRange;
		}

		if(!namestring.Equals (name))
			uid.ThrowNameChangeEvent (namestring);

		uhp.ThrowMaxHealthChangeEvent (maxhp_event);
		uhp.ThrowCurrentHealthChangeEvent (curhp_event);
		umov.ThrowRangeChangeEvent (mov_event);
		udet.ThrowRangeChangeEvent (DetectorController.Event_Air_Change, airval);
		udet.ThrowRangeChangeEvent (DetectorController.Event_Surf_Change, surfval);
		udet.ThrowRangeChangeEvent (DetectorController.Event_Sonar_Change, subval);

		int index = WepDropdown.SelectedIndex;
		string comboname = WepDropdown.Items [index].LocalItem.ToString();
		AlterWeaponData (comboname);
		
	}
	public void AlterWeaponData(string key){
		AttackController uat = currentObject.GetComponent(
			ObjectFactoryHelper.DetermineControllerLiveObject(
			"Attack", currentObject)) as AttackController;
		Single validator = 0.0f;
		Weapon wep = uat.GetWeapon (key);
		
		string wnameval = WepNameVal.GetComponent<InputField> ().text;
		
		string wlethstring = WepLethalityVal.GetComponent<InputField> ().text;
		bool lethvalid = Single.TryParse (wlethstring, out validator);
		float wlethval = 0f;
		if (lethvalid) {
			wlethval = Single.Parse (wlethstring);
		}else{wlethval = wep.GetDamage();}
		
		string wrangestring = WepRangeVal.GetComponent<InputField> ().text;
		bool rangevalid = Single.TryParse (wrangestring, out validator);
		float wrangeval = 0f;
		if (rangevalid) {
			wrangeval = Single.Parse (wrangestring);
		} else {wrangeval = wep.GetRange();}

		string wprobstring = WepProbVal.GetComponent<InputField> ().text;
		bool probvalid = Single.TryParse (wprobstring, out validator);
		float wprobval = 0f;
		if (probvalid) {
			wprobval = Single.Parse (wprobstring);
		} else {wprobval = wep.GetHitChance ();}

		string wammostring = WepAmmoVal.GetComponent<InputField> ().text;
		bool ammovalid = Single.TryParse (wammostring, out validator);
		float wammoval = 0f;
		if (ammovalid) {
			wammoval = Single.Parse (wammostring);
		} else {wammoval = wep.GetMaxAmmo();}

		string wcurrammostring = WepCurrAmmoVal.GetComponent<InputField> ().text;
		bool currammovalid = Single.TryParse (wcurrammostring, out validator);
		float wcurrammoval = 0f;
		if (currammovalid) {
			wcurrammoval = Single.Parse (wcurrammostring);
		} else {wcurrammoval = wep.GetCurAmmo();}

		string wratestring = WepRateVal.GetComponent<InputField> ().text;
		bool ratevalid = Single.TryParse (wratestring, out validator);
		float wrateval = 0f;
		if (ratevalid) {
			wrateval = Single.Parse (wratestring);
		} else {wrateval = wep.GetMaxShots();}
		
		uat.ThrowDamageChangeEvent (key, wlethval);
		uat.ThrowRangeChangeEvent (key, wrangeval);
		uat.ThrowHitChanceChangeEvent (key, wprobval);
		uat.ThrowMaxAmmoChangeEvent (key, (int)wammoval);
		uat.ThrowCurrentAmmoChangeEvent (key, (int)wcurrammoval);
		uat.ThrowMaxShotsChangeEvent(key,(int)wrateval);
	}

    public void OnObjectiveToggle(){
        if(currentObject != null){
            if(ObjectiveToggle.isOn){
                currentObject.GetComponent<IdentityController>().SetIsObjective( true );
                currentObject.transform.Find("Objective").gameObject.SetActive( true );
            }
            else{
                currentObject.GetComponent<IdentityController>().SetIsObjective( false );
                currentObject.transform.Find("Objective").gameObject.SetActive( false );
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void OnOpen() {

    }

    public void OnClose() {

    }
}
