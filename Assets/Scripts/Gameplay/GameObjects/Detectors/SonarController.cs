/*****
 * 
 * Name: SonarController
 * 
 * Date Created: 2015-02-04
 * 
 * Original Team: Gameplay
 * 
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------
 * S. Lang		2015-02-04	Initial commit. Added outline.
 * B. Croft		2015-02-06	Changed private virtual members to protected
 * T. Brennan	2015-02-17	Refactored. 
 * S. Lang		2015-02-26	Recreating lost elevation code
 * T. Dill		2015-03-15	Added WeatherStatusEffects.
 */
using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class SonarController : BaseDetectorController {
	
	public const float IDEAL_RANGE = 5000.0f; 

	public SonarController(DetectorController detectorParent) : base(detectorParent) {}

	/**
	 * Floating point value for the range which the visible/invisible bands
	 * occupy.
	 */
	public float VisibleBandRange { get; set; }


	protected override float GetWeatherStatusEffects(GameObject toObj) {

		return Weather.GetVisionModifier(GlobalSettings.GetCurrentWeatherIndex());


	}


	protected override float GetLandStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	protected override float GetMiscellaneousStatusEffects(GameObject toObj) {

		return IsInVisibleBand(toObj) ? 1.0f : 0.0f;

	}


	/**
	 * Determine if the specified GameObject is within one of the visible
	 * concentric circles of a sonar detector.
	 * 
	 * @param toObj
	 * 		The GameObject that is or is not within the visible band of
	 * 		the sonar detector.
	 * 
	 * @return
	 * 		true if toObj is within one of the visible "bands"
	 */
	protected bool IsInVisibleBand(GameObject toObj) {
		double d = Math.Sin(GetDistanceTo(toObj) * Math.PI / VisibleBandRange);
		return d >= 0.0f;
	}

	/**
	 * Determine the distance to a particular GameObject. Really just a helper
	 * method. (TODO: consider moving up to BaseDetectorController and using
	 * throughout.)
	 *
	 * @param toObj
	 * 		Object to which we are calculating a distance.
	 * @return
	 * 		A float value for the distance between this detector's parent
	 * 		object's location and toObj's location.
	 */
	protected float GetDistanceTo(GameObject toObj) {
		float dx = toObj.transform.position.x - GetParent().transform.position.x;
		float dy = toObj.transform.position.y - GetParent().transform.position.y;
		//float dz = ;
		return (float) Math.Sqrt((dx * dx) + (dy * dy));
	}


	public override float CostMultiplier(Terrain t) {

		// TODO
		return 1.0f;

	}

	public override bool CanSeeElevation(GameObject obj) {
		return BaseDetectorController.IsSubsurface(obj);
	}

	
	/**
	 * Use this for initialization
	 */
	void Start () {

		// TODO: Data Integration/Gameplay: use database values
		SetIdealVisibleRange(IDEAL_RANGE);

	}

}
