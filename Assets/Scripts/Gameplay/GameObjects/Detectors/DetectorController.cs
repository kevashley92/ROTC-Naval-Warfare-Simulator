/*****
 * 
 * Name: DetectorController
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 *
 * 
 * Class is to be used to act as a container for all Detectors at once.
 * This is because the database team will be passing all data to a single
 * controller and it must construct the necessary sub-detectors themselves.
 * 
 *
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * S. Lang		2015-03-13	Initialization, refactoring program structure
 * S. Lang		2015-03-15	Renamed to DetectorController; separating Navy/Marine
 * S. Lang		2015-03-19	Added actual code to GetValues()
 * S. Lang		2015-03-26	Added some helper getters and setters for ease of
 * 							use and readability for unit editing
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DetectorController : Controller {
	
	/** */
	public IDictionary<string,BaseDetectorController> Detectors { get; set; }

	/** Sets an individual value after admin change */
	public override void SetValue(string paramName,object value) {
		// to modify IdealVisibleRange, paramName is "range" and value is desired value
		if(paramName.Equals("Air")){
			AirRange = (float)value;
		}
		if(paramName.Equals("Surface")){
            SurfaceRange = (float)value;
		}
		if(paramName.Equals("Subsurface")){
			SonarRange = (float)value;
		}
	}

	public static string Event_Air_Change = "Air";
	public static string Event_Surf_Change = "Surface";
	public static string Event_Sonar_Change = "Subsurface";

	/**
         * Code for gui to call to throw event
         */
	public void ThrowRangeChangeEvent(string detect_type, float value){
		object[] arguments = new object[4];
		arguments[0] = this.gameObject.GetComponent<IdentityController>().GetGuid();
		arguments[1] = "DetectorController";
		arguments[2] = detect_type;
		arguments[3] = value;
		
		EventFactory.ThrowEvent(GEventType.AdminStatisticChangeEvent, arguments);
	}
	
	#region Helper Getters and Setters
	public float AirRange {
		get {
			if (Detectors.ContainsKey(KeyAir)) {
				return Detectors[KeyAir].GetIdealVisibleRange();
			} else {
				return 0.0f;
			}
		}
		set {
			if (value > 0) {
				if (!Detectors.ContainsKey(KeyAir)) {
					Detectors[KeyAir] = new AirRadarController(this);
				}
				Detectors[KeyAir].SetIdealVisibleRange(value);
			} else {
				if (Detectors.ContainsKey(KeyAir)) {
					Detectors.Remove(KeyAir);
				}
			}
		}
	}
	
	public float SurfaceRange {
		get {
			if (Detectors.ContainsKey(KeySurf)) {
				return Detectors[KeySurf].GetIdealVisibleRange();
			} else {
				return 0.0f;
			}
		}
		set {
			if (value > 0) {
				if (!Detectors.ContainsKey(KeySurf)) {
					Detectors[KeySurf] = new RadarController(this);
				}
				Detectors[KeySurf].SetIdealVisibleRange(value);
			} else {
				if (Detectors.ContainsKey(KeySurf)) {
					Detectors.Remove(KeySurf);
				}
			}
		}
	}
	
	public float SonarRange {
		get {
			if (Detectors.ContainsKey(KeySonar)) {
				return Detectors[KeySonar].GetIdealVisibleRange();
			} else {
				return 0.0f;
			}
		}
		set {
			if (value > 0) {
				if (!Detectors.ContainsKey(KeySonar)) {
					Detectors[KeySonar] = new SonarController(this);
				}
				Detectors[KeySonar].SetIdealVisibleRange(value);
				// double check to default band size if not supplied later
				(Detectors[KeySonar] as SonarController).VisibleBandRange = (value / 3.0f);
			} else {
				if (Detectors.ContainsKey(KeySonar)) {
					Detectors.Remove(KeySonar);
				}
			}
		}
	}
	
	public float SonarBand {
		get {
			if (Detectors.ContainsKey(KeySonar)) {
				return (Detectors[KeySonar] as SonarController).VisibleBandRange;
			} else {
				return 0.0f;
			}
		}
		set {
			if (value > 0) {
				if (!Detectors.ContainsKey(KeySonar)) {
					Detectors[KeySonar] = new SonarController(this);
				}
				(Detectors[KeySonar] as SonarController).VisibleBandRange = value;
			}
		}
	}
	
	
	public float VisualRange {
		get {
			if (Detectors.ContainsKey(KeyVision)) {
				return Detectors[KeyVision].GetIdealVisibleRange();
			} else {
				return 0.0f;
			}
		}
		set {
			if (value > 0) {
				if (!Detectors.ContainsKey(KeyVision)) {
					Detectors[KeyVision] = new VisualDetectorController(this);
				}
				Detectors[KeyVision].SetIdealVisibleRange(value);
			} else {
				if (Detectors.ContainsKey(KeyVision)) {
					Detectors.Remove(KeyVision);
				}
			}
		}
	}
	#endregion // helper getter sand setters
	
	
	public static readonly string KeyAir = "radar_air";
	public static readonly string KeySurf = "radar_surf";
	public static readonly string KeySonar = "radar_sub";
	public static readonly string KeySonarBand = "radar_sub_band";
	public static readonly string KeyVision = "visibility";
	
	/**
	 * Set up the values for all (sub)detectors as necessary.
	 *
	 * @param values
	 *		Input dictionary. In JSON Example, will be everything within
	 *		"Detector" section of dictionary (does not include "Detector").
	 *		prototype:
	 *		{
	 *			"air": int,
	 *			"inner_surf": int,
	 *			"mid_surf": int,
	 *			"outer_surf": int,
	 *			"inner_sonar": int,
	 *			"mid_sonar": int,
	 *			"outer_sonar": int
	 *		}
	 */
	public override void SetValues(IDictionary values) {
		AirRange = Convert.ToSingle(values[KeyAir]);
		SurfaceRange = Convert.ToSingle(values[KeySurf]);
		SonarRange = Convert.ToSingle(values[KeySonar]);
		try {
			SonarBand = Convert.ToSingle(values[KeySonarBand]);
		} catch {}
		VisualRange = Convert.ToSingle(values[KeyVision]);
	}
	
	public override IDictionary GetValues() {
		if (IsNavySet(Detectors)) {
			return GetValuesNavy();
		} else {
			return GetValuesMarines();
		}
	}
	
	public static bool IsNavySet(IDictionary<string,BaseDetectorController> detectors) {
		return detectors.ContainsKey(KeyAir) ||
			detectors.ContainsKey(KeySurf) ||
				detectors.ContainsKey(KeySonar);
	}
	
	public static bool IsMarinesSet(IDictionary<string,BaseDetectorController> detectors) {
		return detectors.ContainsKey(KeyVision);
	}
	
	protected IDictionary GetValuesNavy() {
		Dictionary<string,object> values = new Dictionary<string,object>();
		
		values[KeyAir] = AirRange;
		values[KeySurf] = SurfaceRange;
		values[KeySonar] = SonarRange;
		values[KeySonarBand] = SonarBand;
		
		return values;
	}
	
	protected IDictionary GetValuesMarines() {
		Dictionary<string,object> values = new Dictionary<string,object>();

		values[KeyVision] = VisualRange;

		return values;
	}
	
	/**
	 * Determine if a supplied JSON Dictionary specifies values for a Navy unit.
	 *
	 * @param dict
	 *		Dictionary object to determine if it's for navy units.
	 *
	 * @return
	 * 		true if the supplied Dictionary is for Navy units
	 */
	protected virtual bool IsNavyDictionary(IDictionary dict) {
		return (dict as IDictionary<string,object>).ContainsKey(KeyAir); // TODO: redo in better fashion
	}
	
	/**
	 * Determine if a supplied JSON Dictionary specifies values for Marine Corps
	 * units.
	 *
	 * @param dict
	 *		Dictionary object to determine if it's for marines units.
	 *
	 * @return
	 * 		true if the supplied Dictionary is for Navy units
	 */
	protected virtual bool IsMarinesDictionary(IDictionary dict) {
		return (dict as IDictionary<string,object>).ContainsKey(KeyVision); // TODO: redo in better fashion
	}
	
	/**
	 * Handle creating "sub" detectors and setting values for a Marine unit.
	 * This method is really added mostly for the sake of readability.
	 */
	public override void SetValuesMarines(IDictionary values) {
		SetValues(values);
	}
	
	/**
	 * handle creating "sub" detectors and setting values for a Navy unit.
	 * This method is mainly added for the sake of readability than necessity,
	 * per se.
	 */
	public override void SetValuesNavy(IDictionary values) {
		SetValues(values);
	}
	
	public List<GameObject> Ping() {
		List<GameObject> ret = new List<GameObject>();
		foreach (BaseDetectorController d in Detectors.Values) {
			ret.AddRange(d.Ping());
		}
		return ret;
	}
	
	/** TODO */
	void Start() {
		//Detectors = new Dictionary<string,BaseDetectorController>();
	}
	
	/** TODO */
	void Update() {
		
	}
	
	public DetectorController() : base() {
		Detectors = new Dictionary<string,BaseDetectorController>();
	}
	
}
