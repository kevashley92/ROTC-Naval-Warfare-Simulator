/*****
 * 
 * Name: NavyMover
 * 
 * Date Created: 2015-02-11
 *
 * Original Team: Gameplay
 * 
 * NavyMover will help control whether or not a Navy ship unit can move to a
 * specific position.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan 	2015-02-08	Initial Creation
 * S. Lang		2015-02-11	Generalized
 * S. Lang		2015-02-13	Range-based movement for navy, not tiled
 * T. Brennan	2015-02-17	Refactored
 * S. Lang		2015-02-27	Skeletal structuring navigation around land masses
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class NavyMover : UnitMover {

	/** string to represent a type of terrain for water */
	public static readonly string TYPE_TILE_WATER = "WATER";

	/**
	 * Override parent's function to determine if a tile is a valid position.
	 * 
	 * @param pos
	 * 		The a possible move position that needs validaton
	 * 
	 * @return
	 * 		True if the position is valid
	 * 		False if the position is invalid
	 */
	protected override bool ValidPosition(Vector3 pos) {

		World w = World.Instance;
		Terrain t = w.TerrainAt(new Vector2(pos.x, pos.y));

		if (t.Name.Equals(TYPE_TILE_WATER)) {

			return true;
		}
		else {

			//Debug.Log("Tile at [" + pos.x + "," + pos.y + "] is " + t.Name + " not " + TYPE_TILE_WATER);
			return false;
		}

		return t.Name.Equals(TYPE_TILE_WATER);

	}


	/**
	 * Overrides the parent functionality to check of the position is in range,
	 * only calling the parent if it is.
	 * 
	 * @param newPos
	 * 		The position we are trying to plan a move to
	 */ 
	public override void PlanMove (Vector3 newPos) {
		if (CanMoveTo(newPos)) {

			// TODO: check to make sure no island in way
			////Debug.Log (">movement is in range");
			//Debug.Log("Navy Mover: Position [" + newPos.x + "," + newPos.y + "] is in range. Planning Move.");
			base.PlanMove (newPos);

		} 
		else {

			//Debug.Log ("Navy Mover: Position [" + newPos.x + "," + newPos.y + "] is not in range.");

		}

	}

	/**
	 * Determine if this Mover instance can move to a position.
	 *
	 * @param newPos
	 * 		Vector representing a desired new position.
	 *
	 * @return
	 * 		true if the desired new position is within this mover's movement
	 * 		range after accounting for detours around islands
	 */
	protected bool CanMoveTo(Vector3 newPos) {

		//Debug.Log ("Navy Mover: Checking if position [" + newPos.x + "," + newPos.y + "] is a potential move.");

		bool inRange = IsInRange(newPos);
		if (inRange) {

			//Debug.Log ("Navy Mover: [" + newPos.x + "," + newPos.y + "] is in euclidian range.");

		}
		else {

			//Debug.Log ("Navy Mover: [" + newPos.x + "," + newPos.y + "] is not in euclidian range.");
			return false;

		}

		bool validEnd = ValidPosition(newPos);
		if (validEnd) {

			//Debug.Log ("Navy Mover: [" + newPos.x + "," + newPos.y + "] is a valid position.");

		}
		else {

			//Debug.Log ("Navy Mover: [" + newPos.x + "," + newPos.y + "] is not a valid position.");
			return false;
		}

		if (inRange && validEnd) {
			// check to see if line of sight path possible.
			// if not, find actual path
			Vector3 pos = gameObject.transform.position;
			return NavyMoverSearcher.LineOfSightPathBetween(pos, newPos) ||
				NavyMoverSearcher.CanMoveBetween(pos, newPos, GetMoveRange ());
		}
		return false;
	}


	/**
	 * Determine if the position is within range of this Mover's parent.
	 * 
	 * @param newPos
	 * 		New position to which we are attempting to move.
	 * 
	 * @return
	 * 		true if the distance to the new position is within
	 */
	protected bool IsInRange(Vector3 newPos) {

		float d2 = GetMoveRange() * GetMoveRange();
		float dx = gameObject.transform.position.x - newPos.x;
		float dy = gameObject.transform.position.y - newPos.y;

		return ((dx*dx) + (dy*dy)) <= d2;

	}

	/**
	 * TODO Fill this in
	 */
	public void DrawRange() {
		transform.FindChild ("Move Range Sprite").transform.localScale = new Vector3( (float)(.0033 * GetMoveRange()), (float)(.0033 * GetMoveRange()), 1);
		////Debug.Log ("Drawing move range of " + GetMoveRange());
		transform.FindChild ("Move Range Sprite").gameObject.SetActive (true);
		
	}
	
	/**
	 * TODO Fill this in
	 */
	public void DeleteCircle() {
		transform.FindChild ("Move Range Sprite").gameObject.SetActive (false);
	}
}
