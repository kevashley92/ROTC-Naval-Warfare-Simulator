/*****
 * 
 * Name: AirRadarController
 * 
 * Date Created: 2015-02-26
 * 
 * Original Team: Gameplay
 * 
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * S. Lang		2015-02-26	Initial commit.
 */
using UnityEngine;

[System.Serializable]
public class AirRadarController : RadarController {

	public AirRadarController(DetectorController detectorParent) : base(detectorParent) {}

	public override bool CanSeeElevation(GameObject obj) {
		return BaseDetectorController.IsInAir(obj);
	}

}
