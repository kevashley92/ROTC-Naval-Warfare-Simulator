/*****
 * 
 * Name: SubmarineController
 * 
 * Date Created: 2015-03-06
 * 
 * Original Team: Gameplay
 * 
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * M.Schumacher	2015-03-06	Initial commit.
 */
using UnityEngine;

//TODO implement features
//Underwater units cannot see air units on radar.

//Surfaced underwater units would have radar-based visibility, 
//where they see everything in their vision radius.  
//Submerged underwater units would have sonar-based visibility, 
//which would include the “blind ring” around the unit.  


[System.Serializable]
public class SubmarineController : BaseDetectorController{

	public SubmarineController(DetectorController detectorParent) : base(detectorParent) {
		Radar = new RadarController(detectorParent);
		Sonar = new SonarController(detectorParent);
	}

	private RadarController Radar; // = new RadarController(DetectorParent);
	private SonarController Sonar; // = new SonarController(DetectorParent);
	private BaseDetectorController currentController;
	
	private void setCurrentState(){
		if(((SubmarineMover)GetParent().GetComponent("SubmarineMover")).GetSurfaced()){
			currentController = Radar;
		}
		else{
			currentController = Sonar;
		}
	}
	
    public override bool CanSeeElevation(GameObject obj){
		setCurrentState();
		return currentController.CanSeeElevation(obj);

	}
	
	public override float CostMultiplier(Terrain t){
		setCurrentState();
		return currentController.CostMultiplier(t);

	}
	
	
	
	
	

}
