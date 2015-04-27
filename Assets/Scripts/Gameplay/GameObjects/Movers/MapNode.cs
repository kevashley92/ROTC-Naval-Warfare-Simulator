/*****
 * 
 * Name: MapNode
 * 
 * Date Created: 2015-02-08
 * 
 * Original Team: Gameplay
 * 
 * This class will store data relevant to moving marines.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan 	2015-02-08	Initial Creation. Added position,
 * 							TileMovementCost, costSoFar, and constructor.
 * 							Overwrote Equals.
 * T. Brennan	2015-02-09	Changed scope of position, tileMovementCost,
 * 							and costSoFar to public
 * T. Brennan	2015-02-17	Refactored
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapNode {

	/**
	 * The position on the tile in game units
	 */
	public Vector3 Position;

	/**
	 * The cost of moving to this tile with terrain factored in
	 */
	public float TileMovementCost;

	/**
	 * The movement cost to get to this tile
	 */
	public float CostSoFar;

	/**
	 * The path that the unit followed to get here
	 */
	public List<MapNode> Path;


	/**
	 * The basic constructor for a MapNode.
	 * Sets the location of the node, and 
	 * TODO grabs the terrain information for TileMovementCost
	 * 
	 * @param position	
	 * 		The position on the map that this node will represent
	 * @param diagonal
	 * 		True 	if the position is diagonal to the last node
	 * 		False 	if the position is beside the last node
	 * @param parent
	 * 		The node that cause this node to be created
	 */
	public MapNode(Vector3 position, bool diagonal, MapNode parent) {

		// Sets the position
		this.Position = position;

		// Initialize the path
		Path = new List<MapNode> ();

		// Initialize the costSoFar
		CostSoFar = 0;

		// Set the tile movement to a default value
		TileMovementCost = 10;

		// Get the tile at the position in the wolrd we are currently in
		World world = World.Instance;
		Terrain terrainAtPos = world.TerrainAt (position);

		// Modify movement cost by terrain type
		float modifier = terrainAtPos.ModifierMovement;
		if(modifier != 0) {

			TileMovementCost *= modifier;

		}

		// Check if the movement to this tile was diagonal
		if (diagonal) {
			// If it was

			// Modify by the perfect square hypotenus factor
			TileMovementCost *= Mathf.Sqrt(2);

		}

		// Check to see if there was a parent node
		if (null != parent) {
			// If there was

			// Get the new cost so far
			CostSoFar = parent.CostSoFar + TileMovementCost;

			// Set this path to be the parent's path
			Path = parent.Path;

		}

		// Add the new node to its path
		Path.Add (this);

	}


	/**
	 * Checks if other is the same node by comparing positions
	 * 
	 * @param other	
	 * 		The other node
	 */
	public bool Equals(MapNode other) {

		return this.Position.Equals (other.Position);

	}

}
