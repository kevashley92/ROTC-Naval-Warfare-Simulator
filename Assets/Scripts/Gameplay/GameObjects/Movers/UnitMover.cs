/*****
 * 
 * Name: UnitMover
 * 
 * Date Created: 2015-02-08
 * 
 * Original Team: Gameplay
 * 
 * This class will store data relevant to moving units at a generic level.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  
 * T. Brennan 	2015-02-08	Initial Creation. Added openList, closedList,
 * 							getMovementPossibilities, 
 * 							getPossibilitiesByDijkstras, 
 * 							getSuroundingPositions, and addToOpenList.
 * T. Brennan	2015-02-11	Overwrote moveAsPlanned. Added more detail
 * 							commenting to the last change log.
 * S. Lang		2015-02-11	Generalized
 * D. Durand	2015-02-11	Ovewrote moveAsPlanned()
 * D. Durand    2015-02-11  Changed validPosition() to be an empty method
 * 							instead of a method header. It was giving an
 * 							error before
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public abstract class UnitMover : MoverController {

	/**
	 * A list of all map nodes that still need to be evaluated by Dijkstras
	 * algorithm
	 */
	protected List<MapNode> OpenList;


	/**
	 * A collection of map nodes that the object could move to indexed by
	 * their position.
	 */
	protected Dictionary<Vector3, MapNode> ClosedList;


	/**
	 * Initialize the open and closed lists.
	 */
	void Start() {

		InitializeNewLists();

	}


	/**
	 * Calculates all possible locations that a unit can move to
	 * 
	 * @return
	 * 		A List of MapNodes representing where the unit could
	 * 		move
	 */
	public Dictionary<Vector3, MapNode> GetMovementPosibilities() {

		// Check if the closed list of movement possibilities has been populated

		if (ClosedList != null && ClosedList.Count > 0) {
			// If the closed list has been populated

			// Return the previously calculated list of possibilities
			return ClosedList;

		}
		else {
			// If the close list was not populated

			// Calculate and return the possibilities
			return GetPosibilitiesByDijkstras ();

		}

	}


	/**
	 * Uses Dijkstra's Algorithm to determine which positions the
	 * unit can move to
	 * 
	 * @returns
	 * 		A Dictionary of MapNodes representing where the unit
	 * 		could move with the position as the keys
	 */
	private Dictionary<Vector3, MapNode> GetPosibilitiesByDijkstras() {

		OpenList = new List<MapNode> ();

		ClosedList = new Dictionary<Vector3, MapNode> ();

		bool diagonal = false;

		OpenList.Add (new MapNode (gameObject.transform.position, diagonal, null));

		// Iterate until the open list is empty
		while (OpenList.Count != 0) {

			// Pop the head off the list
			MapNode curNode = OpenList[0];

			// Remove the head of the list
			OpenList.RemoveAt(0);

			// Add the current node to the closed list

			// Test weather the closed list already contains this position
			if(ClosedList.ContainsKey(curNode.Position)) {
				// If it does

				// Get the MapNode that is already at this position
				MapNode temp = null;
				ClosedList.TryGetValue(curNode.Position, out temp);

				// Test weather the path we just found (in curNode) is shorter than the path we already know (in temp).
				if (curNode.CostSoFar < temp.CostSoFar) {
					// If it does

					// Replace the old with the new
					ClosedList.Remove(curNode.Position);
					ClosedList.Add(curNode.Position, curNode);

				}

			} 
			else {

				// If not then just add it.
				ClosedList.Add(curNode.Position, curNode);

			}

			// Reset the diagonal boolean to false
			diagonal = false;

			// Iterate over each of the position of the curNode
			foreach (Vector3 curVec in GetSurroundingPositions(curNode.Position)) {

				// Check if the position has been solved for
				if(!ClosedList.ContainsKey(curVec)) {
					// If it has not

					// Create a node for the current vector
					MapNode tempNode = new MapNode(curVec, diagonal, curNode);

					// Check if the movement to the tile is in budget
					if (tempNode.CostSoFar <= GetMoveRange ()) {
						// If the movement is in budget

						// Add the node to the open list
						AddToOpenList(tempNode);

					}

				}

				// Switch from diagonal to not and vice versa
				diagonal = !diagonal;

			}

		}

		return ClosedList;

	}

	/**
	 * Gets a list of all positions around the provided position
	 * 
	 * @param curPos
	 * 		The position you want the surrounding points of
	 * 
	 * @return
	 * 		A list of position around the positon supplied
	 */
	private List<Vector3> GetSurroundingPositions(Vector3 curPos)
	{
		// Initilize the list
		List<Vector3> surroundings = new List<Vector3>();

		// Create a temporary vector to modify
		Vector3 temp = curPos;

		// Add up
		temp.y -= 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add up right
		temp.x += 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add right
		temp.y += 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add right down
		temp.y += 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add down
		temp.x -= 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add down left
		temp.x -= 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add left
		temp.y -= 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		// Add up left
		temp.y -= 1;
		if (ValidPosition(temp)) {
			surroundings.Add(temp);
		}

		return surroundings;

	}


	/**
	 * Determine if the position specified is valid by the mover's criteria
	 * to be moved into.
	 *
	 * @param pos
	 * 		Vector3 representing a position in the world
	 * 
	 * @return
	 * 		true if the specific mover can move into that position.
	 */
	protected abstract bool ValidPosition(Vector3 pos);


	/**
	 * Adds the new MapNode to the open list ordered by csf
	 * 
	 * @param newNode
	 * 		The node to add to the open list
	 */
	public void AddToOpenList(MapNode newNode) {

		// Itterate over every element in open list
		for (int i = 0; i < OpenList.Count; i++) {

			// Check if the new node is less expensive than the last
			if (OpenList[i].CostSoFar > newNode.CostSoFar) {
				// If we have found the location for the node

				// Insert the node at the current index
				OpenList.Insert(i, newNode);

				// Exit the loop early
				return;

			}

		}

		// If we get to the end of the loop without finding where the
		// node needs to go, then it needs to go at the end.
		OpenList.Add (newNode);

	}


	/**
	 * Helper method to reinitialize the open and closed list
	 */
	public void InitializeNewLists() {

		ClosedList = new Dictionary<Vector3, MapNode>();
		OpenList = new List<MapNode>();

	}


	/**
	 * Overrides the parent's game object to check if the new position is
	 * different from the current position and re-initializes the open and
	 * closed list if need be.
	 * 
	 * @param newPos
	 * 		The position from the event
	 */
	public override void MoveAsPlanned(Vector3 newPos) {

		// Check if the ship is going to need to recalculate closed list
		// the next time someone wants to move it.
		if (!newPos.Equals (gameObject.transform.position)) {
			// If they will need to be recalulated

			// Re-initialize the open and closed lists
			InitializeNewLists();

		}

		// Preform the base functionallity of this method in the parent class
		base.MoveAsPlanned (newPos);

	}


	/**
	 * Overrides the parent method to reinitialize the open and closed lists
	 * since they will not be the same after changing the movement range.
	 * 
	 * @param newRange
	 * 		The new range this object can move in a turn
	 */
	public override void SetMoveRange (float newRange) {

		// Re-initialize the open and closed lists
		InitializeNewLists();

		base.SetMoveRange (newRange);

	}

}

