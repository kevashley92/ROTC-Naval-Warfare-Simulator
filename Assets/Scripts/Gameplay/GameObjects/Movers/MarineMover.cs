/*****
 * 
 * Name: MarineMover
 * 
 * Date Created: 2015-02-XX
 * 
 * Original Team: Gameplay
 * 
 * This class holds functionality needed specifically for all marine
 * movement functionality.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-02-16 	Changed to be event driven. Made possible
 * 							moves be added as a child of the same obj
 * T. Brennan	2015-02-17	Refactored and removed unused Update method
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarineMover : UnitMover {

	/**
	 * The prefab instance used to instanciate instances of PossibleMove.
	 * Set through the Unity GUI.
	 */
	public PossibleMove PossibleMovePrefab;

	/**
	 * The prefab of an object to hold all the new possible moves
	 */
	public GameObject MovesParentPrefab;

	/**
	 * The parent object holding all of the possible moves for this mover
	 */
	protected GameObject MyPossibleMovesParent;
	

	/**
	 * Use this for initialization
	 */
	protected void Start () {

		// Define a movement range.
		// TODO Remove this hardcoding
		//this.MoveRange = 20;

	}


	/**
	 * Validate weather a given position is within the map and is not on
	 * water
	 * 
	 * @param pos
	 * 		The location in question
	 * 
	 * @return
	 * 		True if the position is valid. False if the position is not
	 * 		valid.
	 */
	protected override bool ValidPosition (Vector3 pos) {
		// Test lower bounds of map
		if(pos.x < 0 || pos.y < 0) {
			return false;
		}

		// Get the map we are currently ons
		World world = World.Instance;

		// Test upper bounds of map
		if (pos.x >= world.Size.x || pos.y >= world.Size.y) {
			return false;
		}

		// Get the tile we are currently on
		Vector2 posV2 = new Vector2 (pos.x, pos.y);
		Terrain terrainAtPos = world.TerrainAt (posV2);

		// Test the tile
		if(terrainAtPos.Name == "WATER") {
			return false;
		}

		return true;
	}


	/**
	 * Test if this unit canMove to a given position
	 * 
	 * @param pos
	 * 		The position we want to know if the unit can move to.
	 * 
	 * @return
	 * 		true if it can move there. false if it cannot.
	 */
	public bool CanMove(Vector3 pos) {

		return ClosedList.ContainsKey (pos);

	}


	/**
	 * Creates possible move objects for this instances list of possible
	 * moves
	 */
	public void GeneratePossibleMoves() {

		// Gets a dictionary of all MapNodes that this unit can move to
		Dictionary<Vector3, MapNode> possibleMoves = GetMovementPosibilities ();

		// Instanciates the empty game object
		MyPossibleMovesParent = Instantiate(MovesParentPrefab ,new Vector3(0,0,0),Quaternion.identity) as GameObject;

		// Create a  possible move for every map node this unit can move to
		foreach (MapNode node in possibleMoves.Values) {

			PossibleMove temp;

			// Instanciates an instance of a possible move
			temp = Instantiate(PossibleMovePrefab,node.Position,Quaternion.identity) as PossibleMove;

			// Set the new possible move to be the child of the empty game object
			temp.transform.parent = MyPossibleMovesParent.transform;

			// Set its mover to be this mover so it know who's methods to call
			temp.SetMover(this);

			// Set the location of the possible move relative to the parent game object
			temp.SetPos(node.Position);
			
		}

	}


	/**
	 * Deletes all of the possible moves associated with this mover
	 */
	public void DestroyPossibleMoves()
	{

		Destroy (MyPossibleMovesParent);

	}

}
