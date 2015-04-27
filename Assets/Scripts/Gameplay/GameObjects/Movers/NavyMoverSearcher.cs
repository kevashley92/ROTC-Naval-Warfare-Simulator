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
 * Sean Lang	2015-02-27	Initial creation
 * Sean Lang	2015-03-02	More A* programming
 * Sean Lang	2015-03-05	A* work; PathNode and Location fleshed out
 */ 

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[SerializableAttribute]
public class NavyMoverSearcher {

	/**
	 * Determine if a Navy unit can move between a start and end position.
	 *
	 * @param startPos
	 * 		Starting position of the navy unit.
	 *
	 * @param endPos
	 * 		Ending position that is to be attempted to find a path to.
	 *
	 * @param moveRange
	 * 		Maximum movement range of the Navy unit. If this is a negative value,
	 * 		then the range is to be ignored.
	 *
	 * @return
	 * 		true if a path can be found between the start and end positions and
	 * 		(if moveRange is positive) the target can be reached within the
	 * 		specified movement range
	 */
	public static bool CanMoveBetween(Vector3 startPos, Vector3 endPos, float moveRange) {
		Path pathBetween = PathBetween(startPos, endPos);
		if (moveRange < 0) {
			return pathBetween != null;
		} else {
			return pathBetween != null && pathBetween.Cost <= moveRange;
		}
	}

	/**
	 * Determine if there is a direct line-of-sight path between a starting
	 * position and an ending position.
	 *
	 * @param starPos
	 * 		Starting position vector.
	 *
	 * @param endPos
	 * 		Ending positional vector.
	 *
	 * @return
	 * 		true if a direct line can be drawn between the starting and ending
	 * 		position and all crossed tiles are valid tiles.
	 */
	public static bool LineOfSightPathBetween(Vector3 startPos, Vector3 endPos) {
		int x0 = (int) startPos.x, y0 = (int) startPos.y;
		int x1 = (int) endPos.x, y1 = (int) endPos.y;
		List<Terrain> intersects =
			BaseDetectorController.GetIntersectingTerrains(x0, y0, x1, y1);

		foreach (Terrain t in intersects) {
			if (!ValidTerrain(t)) {
				return false;
			}
		}

		return true;
	}

	/**
	 * Using unidirectional A* search, find out if there is a path between the
	 * start and end positions on the current map. Also, act with a bit of
	 * intelligence: when we "explore" a new node, we will check to see if there
	 * is a direct line-of-sight path to the target so we can quickly navigate
	 * small islands.
	 *
	 * N.B. if performance for this becomes an issue, set LineOfSightStop to
	 * false
	 *
	 * @param startPos
	 * 		Starting position vector
	 *
	 * @param endPos
	 * 		Ending or goal position vector
	 *
	 * @return
	 * 		true if and only if a path has been found between the two positions
	 */
	public static Path PathBetween(Vector3 startPos, Vector3 endPos) {
		// setup

		// Map from integer (x,y) coordinates to nodes and scores
		Dictionary<Location, PathNode> visitedList =
			new Dictionary<Location, PathNode>();
		List<PathNode> toVisitList = new List<PathNode>();
		Location start = new Location((int) startPos.x, (int) startPos.y);
		PathNode startNode = new PathNode(start, 0.0f, endPos);
		toVisitList.Add(startNode);

		// do search (euclidean distance heuristic)
		while (toVisitList.Count > 0) {
			// Ensure sorted list
			toVisitList.Sort();
			
			// grab and pop node with best expectation
			PathNode current = toVisitList[0];
			toVisitList.RemoveAt(0);

			// if it's a winner, stop
			if (current.IsAtGoalTile()) {
				return new Path(current);
			}

			// save currently popped node
			visitedList.Add(current.loc, current);

			// for all neighbors
			foreach (PathNode neighbor in current.Neighbors()) {

				// if we've already visited the node, check to see if the
				// 	current route is cheaper than the saved route
				if (visitedList.ContainsKey(neighbor.loc)) {

					// peek
					PathNode tmp = null;
					visitedList.TryGetValue(neighbor.loc, out tmp);

					if (neighbor.Score() < tmp.Score()) {
						// If cheaper to use the current's path rather than
						// 	other path the some node, replace with cheaper score
						visitedList.Remove(neighbor.loc);
						visitedList.Add(neighbor.loc, neighbor);
					} /*else {
						// otherwise, not cheaper, just leave as is
						continue;
					}*/
				} else {
					visitedList.Add(neighbor.loc, neighbor);
				}

			}
		}

		// fail if never found path
		return null;
	}

	/** Basically, the x,y coordinate of the lower left-hand corner */
	[SerializableAttribute]
	public class Location : IEquatable<Location> {
		public int x { get; set; }
		public int y { get; set; }
		public Location(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public bool Equals(Location otherLoc) {
			if (otherLoc == null)
				return false;
			return otherLoc.x == x && otherLoc.y == y;
		}
		public int GetHashCode() {
			return 0;
		}
	}


	[SerializableAttribute]
	public class PathNode : IComparable<PathNode>, IEquatable<PathNode> {
		public PathNode last { get; set; }
		public Location loc { get; set; }
		public float CostSoFar { get; set; }
		public Vector3 goal { get; set; }
		public float CostToGoal() {
			Vector3 center = new Vector3(loc.x + 0.500f, loc.y + 0.500f);
			return NavyMoverSearcher.DistanceBetween(center, goal); 
		}
		public float Score() {
			return CostSoFar + CostToGoal();
		}
		public PathNode(Location loc, float costSoFar, Vector3 goal) {
			this.loc = loc;
			this.CostSoFar = costSoFar;
			this.goal = goal;
			this.last = null;
		}

		public int CompareTo(PathNode otherPathNode) {
			// N.B.: smaller number, closer to beginning of list
			if (otherPathNode == null)
				return -1;
			return signum((int)(this.Score() - otherPathNode.Score()));
		}

		public bool Equals(PathNode otherPathNode) {
			if (otherPathNode == null)
				return false;
			return loc.Equals(otherPathNode.loc);
		}
		public override int GetHashCode() {
			return 0;
		}

		public bool IsAtGoalTile() {
			return CostToGoal() <= 1.00f;
		}

		public static readonly float Direct = 1.000000f;
		public static readonly float Diagonal = 1.414213f;
		public List<PathNode> Neighbors() {
			List<PathNode> ret = new List<PathNode>();
			// starting x, y
			int x = loc.x, y = loc.y;

			// up
			y++;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode up = new PathNode(new Location(x,y), CostSoFar + Direct, goal);
				up.last = this;
				ret.Add(up);
			}

			// up right
			x++;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode upRight = new PathNode(new Location(x,y), CostSoFar + Diagonal, goal);
				upRight.last = this;
				ret.Add(upRight);
			}

			// right
			y--;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode right = new PathNode(new Location(x,y), CostSoFar + Direct, goal);
				right.last = this;
				ret.Add(right);
			}

			// down right
			y--;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode downRight = new PathNode(new Location(x,y), CostSoFar + Diagonal, goal);
				downRight.last = this;
				ret.Add(downRight);
			}

			// down
			x--;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode down = new PathNode(new Location(x,y), CostSoFar + Direct, goal);
				down.last = this;
				ret.Add(down);
			}

			// down left
			x--;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode downLeft = new PathNode(new Location(x,y), CostSoFar + Diagonal, goal);
				downLeft.last = this;
				ret.Add(downLeft);
			}

			// left
			y++;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode left = new PathNode(new Location(x,y), CostSoFar + Direct, goal);
				left.last = this;
				ret.Add(left);
			}

			// up left
			y++;
			if (NavyMoverSearcher.ValidTile(x, y)) {
				PathNode upLeft = new PathNode(new Location(x,y), CostSoFar + Diagonal, goal);
				upLeft.last = this;
				ret.Add(upLeft);
			}

			return ret;
		}
	}

	/**
	 * Determine if a tile is valid for movement or occupation by a navy unit.
	 */
	public static bool ValidTile(int x, int y) {
		World w = World.Instance;
		Terrain t = w.TerrainAt(new Vector2(x+0.00f, y+0.00f));
		return ValidTerrain(t);
	}

	public static readonly string TileTypeWater = "WATER";
	/**
	 * Determine if a terrain type is valid for movement or occupation by a navy
	 * unit.
	 */
	public static bool ValidTerrain(Terrain terr) {
		return terr.Name.Equals(TileTypeWater);
	}

	public static int signum(int x) {
		return x == 0 ? 0 : (x < 0 ? -1 : 1);
	}

	[SerializableAttribute]
	public class Path {
		public float Cost { get; set; }
		protected List<Location> path;
		public Path(PathNode node) {
			path = RecreatePath(node);
			Cost = node.CostSoFar;
		}
		public static List<Location> RecreatePath(PathNode node) {
			List<Location> ret = new List<Location>();
			PathNode p = node;
			while (p.last != null) {
				ret.Add(p.loc);
				p = p.last;
			}
			return ret;
		}
	}

	// Necessary because don't care about z and want to completely ignore
	public static float DistanceBetween(Vector3 posA, Vector3 posB) {
		float dx = posA.x - posB.x;		
		float dy = posA.y - posB.y;
		return (float) Math.Sqrt((dx * dx) + (dy * dy));
	}

}
