/*****
 * 
 * Name: RadarController
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
 * S. Lang		2015-02-12	Bug Fixes
 * T. Brennan	2015-02-17	Refactored. Deleted unused Update method.
 * S. Lang		2015-02-26	Recreating lost elevation functionality
 */
using UnityEngine;
using System.Collections;

[System.Serializable]
public class RadarController : BaseDetectorController {

	public const float IDEAL_RANGE = 10000.0f; 


	public RadarController(DetectorController detectorParent) : base(detectorParent) {}

	protected override float GetWeatherStatusEffects(GameObject toObj) {
		return Weather.GetVisionModifier(GlobalSettings.GetCurrentWeatherIndex());
	}


	protected override float GetLandStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	protected override float GetMiscellaneousStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	public override float CostMultiplier(Terrain t) {

		// TODO
		return 1.0f;

	}

	public override bool CanSeeElevation(GameObject obj) {
		return BaseDetectorController.IsOnSurface(obj); // ||
				//BaseDetectorController.IsInAir(obj);
	}


	/**
	 * Use this for initialization
	 */
	void Start () {

		// TODO: Data Integration/Gameplay: use database values
		SetIdealVisibleRange(IDEAL_RANGE);

	}

}
