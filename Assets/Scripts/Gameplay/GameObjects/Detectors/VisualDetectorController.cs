/*****
 * 
 * Name: VisualDetectorController
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
 * T. Brennan	2015-02-17	Refactored. Deleted unused Update method.
 * S. Lang		2015-02-26	Recreating lost elevation functionality
 * T. Dill		2015-03-15	Added WeatherStatusEffects.
 */
using UnityEngine;
using System.Collections;

[System.Serializable]
public class VisualDetectorController : BaseDetectorController {
	
	public const float IDEAL_RANGE = 400.0f; 

	public VisualDetectorController(DetectorController detectorParent) : base(detectorParent) {}

	protected override float GetWeatherStatusEffects(GameObject toObj) {

		return Weather.GetVisionModifier(GlobalSettings.GetCurrentWeatherIndex());


	}


	protected override float GetLandStatusEffects(GameObject toObj) {

		return 1.0f;

	}
	
	protected override float GetMiscellaneousStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	public static readonly float BaseMarineCostMultiplier = 10.0f;
	/**
	 * Cost multiplier for a specific tile.
	 *
	 * @return
	 * 		The cost multiplier for traveling through the specified tile. this
	 * 		will be multiplied by the cost as a modifier.
	 */
	public override float CostMultiplier(Terrain t) {

		// TODO
		return BaseMarineCostMultiplier * 1.0f;

	}

	public override bool CanSeeElevation(GameObject obj) {
		// can see anything on ground or in the air
		return BaseDetectorController.IsOnSurface(obj) ||
				BaseDetectorController.IsInAir(obj);
	}


	// Use this for initialization
	void Start () {

		// TODO: Data Integration/Gameplay: use database values
		SetIdealVisibleRange(IDEAL_RANGE);

	}

}
