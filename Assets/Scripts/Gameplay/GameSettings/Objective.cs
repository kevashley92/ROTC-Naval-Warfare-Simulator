/*****
 * 
 * Name: Objective
 * 
 * Date Created: 2015-03-26
 * 
 * Original Team: Gameplay
 * 
 * This class will store the Objective for the game as entered
 * by the administrator.  The text in this class should be retrieved from
 * a UI element, and it should be obtained through GetObjective().  This is 
 * a temporary class until we have more time to implement more beyond
 * a simple container for text.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * J. Landi		2015-03-26	Initial Commit.
 * A. Smith		2015-03-26	Added individual setters
 */

using UnityEngine;
using System.Collections.Generic;

public class Objective {

	private string objective = "";

	private List<string> individual_objectives = new List<string>();

	private static Objective instance;

	private Objective(){



	}

	public static Objective GetInstance(){

		if(instance == null){

			instance = new Objective();
			return instance;
		}
		else {

			return instance;

		}

	}

	public void SetObjective(string obj){

		objective = obj;

	}

	public string GetObjective(){

		return objective;

	}

	public void SetIndividualObjectives(List<string> individuals){

		individual_objectives = individuals;

	}


	public List<string> GetIndividualObjectives(){

		return individual_objectives;

	}

	public void setIndividualObjective( int index, string text ){
		individual_objectives[index] = text;
	}

	public void addObjective(string s){
		individual_objectives.Add (s);
	}

}
