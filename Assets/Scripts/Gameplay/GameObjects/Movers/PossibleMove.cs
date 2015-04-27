/*****
 * 
 * Name: PossibleMove
 * 
 * Date Created: 2015-02-14
 * 
 * Original Team: Gameplay
 * 
 * This class will contain functionallity for moving a unit when this
 * object is clicked.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-02-16	Changed call to myMover.planMove to 
 * 							myMover.moveUnit, then changed it back...?
 * T. Brennan	2015-02-17	Refactored. Removed unused Start and Update
 */ 
using UnityEngine;
using System.Collections;

public class PossibleMove : MonoBehaviour {

	/**
	 * The mover associated with this possible move
	 */
	MarineMover MyMover;


	/**
	 * When something clicks on this possible move, create a move event
	 * for the mover's object and delete all of the possibe move objects.
	 */ 
	void OnMouseDown() {

		// Plan to move here
		MyMover.PlanMove (this.transform.position);

		// Delete all of the possible move objects for the movers object
		MyMover.DestroyPossibleMoves ();

		// Deletes the contents of the open and closed lists,
		// since these are not synced over the network
		MyMover.InitializeNewLists();

	}


	/**
	 * Set the mover that this possible move is associated with
	 * 
	 * @param mover
	 * 		The mover this possible move is associated with
	 */
	public void SetMover(MarineMover mover) {

		MyMover = mover;

	}


	/**
	 * Sets the position of this possible move
	 * 
	 * @param pos
	 * 		The position of this possible move
	 */
	public void SetPos(Vector3 pos){

		this.transform.position = pos;

	}

}
