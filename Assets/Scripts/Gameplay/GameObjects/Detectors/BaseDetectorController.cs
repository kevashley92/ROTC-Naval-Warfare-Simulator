/*****
 * 
 * Name: BaseDetectorController
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 *
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * S. Lang		2015-01-30	Initial commit. Added outline.
 * S. Lang		2015-02-04	More fleshing out of actual coding logic behind the scenes.
 * B. Croft		2015-02-06	Changed private virtual members to protected, minor bugfixes
 * S. Lang		2015-02-10	Added Bresenham's algorithm to detect terrain, etc.
 * S. Lang		2015-02-11	Bug fixes
 * S. Lang		2015-02-12	More bug fixes
 * B. Croft		2015-02-14	Add SetValues Method
 * T. Brennan	2015-02-17	Refactored, removed the unused update method
 * S. Lang		2015-02-26	Recreating lost functionality: namely elevation
 * S. Lang		2015-03-15	Refactored name to be BaseDetectorController
 * T. Dill		2015-03-15	Added effects for weather and health thresholds.
 * James Woods  2015-03-13  Added GetValues method for DataIntegration.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public abstract class BaseDetectorController {

	/**
	 * An integer representing the ideal range that the Detector can "see"
	 * an enemy asset. This is the range within which an enemy can be detected
	 * without any obscurers--such as rain or snow--present.
	 * N.B. Units not yet known.
	 */
	public float IdealVisibleRange;

	public const float DEFAULT_IDEAL_VISIBLE_RANGE = 100f;

	public static readonly float ReasonablyWithinZero = 0.00001f;
	public static readonly float InAirElevationZ = -0.0004f;
	public static readonly float OnSurfaceElevationZ = -0.0002f;
	public static readonly float SubsurfaceElevationZ = 0.0000f;

	protected DetectorController DetectorParent { get; set; }

	public BaseDetectorController(DetectorController detectorParent) {
		DetectorParent = detectorParent;
	}


	/**
	 * Perform what I will refer to as a single radar 'ping.' Sweep the scene
	 * for an ArrayList of GameObjects within a range of visibility.
	 * 
	 * N.B. Will need to update later with only playable units at a later time.
	 * 
	 * @return		
	 * 		An ArrayList of all GameObjects that are within the
	 * 		visible range of the detector.
	 */
	public List<GameObject> Ping() {
		List<GameObject> gos = FindUnitsInWorld();
		List<GameObject> ret = new List<GameObject>(gos.Count / 10);

		int i = 0;
		foreach (GameObject go in gos) {
			if (!OnSameTeam(go) && CanSee(go) /*&& !OnSameTeam(go)*/) {
				// double check to make sure neither id component is null
				// 	if either was, then result will have been false, thus
				// 	we could have gotten here improperly
				if (GetParent().GetComponent<IdentityController>() != null &&
						go.GetComponent<IdentityController>() != null)
					ret.Add(go);

			}
			i++;
		}

		return ret;

	}


	/**
	 * Determine if the unit is on the same team as this detector's parent unit.
	 *
	 * @param obj
	 * 		GameObject to compare teams with.
	 *
	 * @return
	 * 		true if and only the two GameObjects' teams are the same
	 * 		false if obj belongs to a team other than the one to which this
	 * 		Detector instance's parent belongs.
	 */
	protected bool OnSameTeam(GameObject obj) {
		if(obj != null){
            IdentityController parentId = GetParent().GetComponent<IdentityController>();
		IdentityController objId = obj.GetComponent<IdentityController>();
		// because of previous check in FindUnitsInWorld(), we can safely assume
		// 	that both IdentityControllers are non-null and represent units'
		// 	identities
		// however, because I'm paranoid, a null for either will result in a
		// 	return value of false
		if (parentId == null || objId == null) {
			return false;
		}

		return parentId.GetTeam() == objId.GetTeam();
        }else{
            return false;
        }
	}


	/**
	 * Retrieve a List of (both enemy and friendly) units in the world. At this
	 * time, this list covers all units on the map, but this can be updated
	 * later to allow for quicker region-searching if performance becomes a
	 * concern.
	 *
	 * @return 		
	 * 		A List of GameObjects that represent the units in the
	 * 		world.
	 */
	protected virtual List<GameObject> FindUnitsInWorld() {
		return GuidList.GetAllObjects();
	}


	/**
	 * Determine if the parent object can see another object.
	 *
	 * @param obj	
	 * 		The object the parent is trying to see
	 * 
	 * @return		
	 * 		true if the GameObject is within the ideal visual range and
	 * 		close enough to be seen with the reduced range due to
	 * 		weather or terrain status effects.
	 */
	public bool CanSee(GameObject obj) {

		if(obj != null){
            float PercentHealth = ((HealthController)GetParent().GetComponent("HealthController")).GetCurrentPercentHealth();

		if(PercentHealth < GlobalSettings.GetHealthThresholdForNoDetectors()){
			return false;
		}

		// Retrieve the parent GameObject 
		GameObject parent = GetParent();

		// compute the square of the distance between the two objects
		float dx = parent.transform.position.x - obj.transform.position.x;
		float dy = parent.transform.position.y - obj.transform.position.y;
		//float dz = parent.transform.position.z - obj.transform.position.z;
		float dist2 = (dx * dx) + (dy * dy); // + (dz * dz);

		// if the object is beyond the ideal visible range, stop early
		// TODO: probably need to re-do for costs of tiles rather than direct
		// 		line-of-sight
		float visRange2 = GetIdealVisibleRange() * GetIdealVisibleRange();

		if (dist2 > visRange2) {
			return false;
		}

		// check to make sure the detector can see unit with its current
		// elevation statistics
		// 	e.g. A sonar detector should not be able to see an airplane and vice
		// 	versa.
		if (!CanSeeElevation(obj)) {
			return false;
		}

		// determine if the object is still within range with status effects
		// Use something like Bresenham's Midpoint Algorithm to find out which
		// 		tiles are important.
		List<Terrain> intersects = GetIntersectingTerrains(
				(int) parent.transform.position.x, (int) parent.transform.position.y,
				(int) obj.transform.position.x, (int) obj.transform.position.y
				);
		float canPayCost = GetIdealVisibleRange() * GetStatusEffects(obj);
		if (canPayCost <= ReasonablyWithinZero) {
			return false;
		}
		float cost = 0;
		foreach (Terrain t in intersects) {
			cost += Cost(t);
			if (cost > canPayCost) {
				break;
			}
		}
//		//Debug.Log(System.String.Format("BaseDetector.CanSee()\n"
//					+ "\tCost:\t{0}\n"
//					+ "\tCanPayCost:\t{1}\n"
//					+ "\t\tIdealVisibleRange:\t{2}\n"
//					+ "\t\tStatusEffects:\t{3}\n"
//					+ "\tReturning: {4}",
//					cost,
//					canPayCost,
//					GetIdealVisibleRange(),
//					GetStatusEffects(obj),
//					(cost <= canPayCost)
//					)
//				);
		return cost <= canPayCost;
        }else{
            return false;
        }

	}

	/**
	 * Determine if this detector instance's parent is on the same elevation as
	 * the object to be possibly detected. This is to prevent things such as
	 * sonar picking up an airplane in its Pint() sweep.
	 *
	 * N.B. It has been de facto agreed between Gameplay and UI that the
	 * elevation parameter is to be limited between -1.0 and +1.0 to keep the
	 * effects on camera zoom to a minimum.
	 *
	 * @param obj
	 * 		GameObject instance that is to be compared for elevation visibility
	 *
	 * @return
	 * 		true if this Detector can see the object at its elevation
	 */
	public abstract bool CanSeeElevation(GameObject obj);


	/**
	 * Use a form of Bresenham's Midpoint Algorithm to find out which tiles
	 * are being intersected via a line of sight and which terrain types are
	 * represented by those intersected tiles.
	 * 
	 * N.B. Code used from example found on Wikipedia page.
	 * 	http://en.wikipedia.org/wiki/Bresenham's_line_algorithm
	 *
	 * @param x0, y0
	 * 		x,y coordinates for the first object
	 * @param x1, y1
	 * 		x,y coordinates for the second object
	 * 
	 * @return
	 * 		A list of the terrains that are in between the two objects
	 */
	public static List<Terrain> GetIntersectingTerrains(int x0, int y0, int x1, int y1) {
		// Setup
		World world = GetGameWorldInstance();
		List<Terrain> ret = new List<Terrain>();
		// swap p1 and p2 now to make sure later for-loop doesn't exit prematurely
		if (x1 < x0) {
			int tx = x0;
			int ty = y0;
			x0 = x1;
			y0 = y1;
			x1 = tx;
			y1 = ty;
		}

		// start actual work of Bresenham's algorithm
		int dx = x1 - x0;
		int dy = y1 - y0;
		// decision variable
		int d = 2*dy - dx;
		ret.Add(world.TerrainAt(new Vector2(x0+0.0f, y0+0.0f)));
		int y = y0;

		for (int x = x0 + 1; x < x1; x++) {
			if (d > 0) {
				y++;
				ret.Add(world.TerrainAt(new Vector2(x+0.0f, y+0.0f)));
				d = d + (2*dy - 2*dx);
			} else {
				ret.Add(world.TerrainAt(new Vector2(x+0.0f, y+0.0f)));
				d = d + (2*dy);
			}
		}

		return ret;
	}

	/**
	 * Retrieve the cost multiplier for a specific tile. This is dependent upon
	 * what type of Detector is being utilized. As such, this is to remain a
	 * purely abstract function at this level of implementation.
	 *
	 * @param t
	 * 		Terrain object that the Detector is "looking" through to
	 * 		determine what sort of effects are acting upon the viewer.
	 * 
	 * @return
	 *		The cost multiplier for traveling through the specified tile.
	 */
	public abstract float CostMultiplier(Terrain t);


	/**
	 * Determine what the cost of a single tile will be.
	 *
	 * @param t
	 * 		Terrain to find the cost of (including multipliers for terrain
	 * 		type, etc.)
	 * 
	 * @return
	 * 		A float representing what the cost of the tile is to "look"
	 * 		through	it.
	 */
	protected virtual float Cost(Terrain t) {

		return CostMultiplier(t) * TileCost;

	}


	/** 
	 * The cost for seeing through a tile as a whole. 
	 */
	public float TileCost { get; set; }


	/**
	 * Retrieve the parent GameObject of this Detector instance.
	 * 
	 * @return 
	 * 		A reference to the GameObject that contains this Detector
	 * 		as a parent.
	 */
	protected GameObject GetParent() {
		return DetectorParent.gameObject;
	}


	/**
	 * Retrieve a numeric representation of the total percentage of visual
	 * impairment that will be caused by a set of status effects.
	 * The float returned will be a percentage of the ideal visual range of
	 * the detector. E.g. if 0.75 is returned, the status effects have caused
	 * a decrease of 25% to a total visibility of 75% of the unit's ideal 
	 * range.
	 *
	 * @param toObj
	 * 		The GameObject to which
	 * 
	 * @return 
	 * 		A float representing how much of the ideal visible range
	 * 		the unit can see as a result of status effects and terrain.
	 */
	public float GetStatusEffects(GameObject toObj) {

		float weatherStatEff = GetWeatherStatusEffects(toObj);
		float landStatEff = GetLandStatusEffects(toObj);
		float miscStatEff = GetMiscellaneousStatusEffects(toObj);

		return landStatEff * weatherStatEff * miscStatEff;

	}


	/**
	 * Retrieve the current terrain's status effect on visible range.
	 * This can vary depending on the detector type, so it is therefore a 
	 * virtual function.
	 * 
	 * @param toObj
	 * 		The GameObject to which the status effect is being
	 * 		computed.
	 * 
	 * @return 
	 * 		A float representing the proportion of the ideal visible
	 * 		range that the terrain affects visibility as a decimal.
	 */
	protected virtual float GetLandStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	/**
	 * Retrieve the current weather's status effect on visible range.
	 * This can vary depending on detection strategy, so it is left as a virtual
	 * function.
	 * 
	 * @param toObj
	 * 		The GameObject to which the status effect is being
	 * 		computed.
	 * 
	 * @return
	 * 		A float representing the proportion of the ideal visible
	 * 		range that the terrain affects visibility as a decimal.
	 */
	protected virtual float GetWeatherStatusEffects(GameObject toObj) {

		return 1.0f;

	}


	/**
	 * Retrieve any other status effects that may effect the visibility of
	 * a GameObject, e.g. sonar "bands."
	 * 
	 * @param toObj
	 * 		The GameObject to which the status effect is being
	 * 		computed.
	 * 
	 * @return
	 * 		A float representing the proportion of the ideal visible
	 * 		range that the terrain affects visibility as a decimal.
	 */
	protected virtual float GetMiscellaneousStatusEffects(GameObject toObj) {

		return 1.0f;

	}

	/**
	 * Retrieve the game World object instance that is associated with this
	 * game.
	 *
	 * @return
	 * 		The current game's World instance.
	 */
	public static World GetGameWorldInstance() {

		return World.Instance;

	}


	/**
	 * Initialization method
	 */
	void Start() {
		SetIdealVisibleRange(DEFAULT_IDEAL_VISIBLE_RANGE);
	}

	void Update() {

	}
	

	/**
	 * Sets the new ideal visible range. This would be the value before
	 * any status effects are incorperated.
	 * 
	 * @param visRange
	 * 		The new ideal visible range
	 */
	public void SetIdealVisibleRange(float visRange) {

		IdealVisibleRange = visRange;

	}


	/**
	 * Gets the current ideal visible range without any status effects.
	 * 
	 * @return
	 * 		The current ideal visible range
	 */
	public float GetIdealVisibleRange() {

		return IdealVisibleRange;

	}

	/**
	 * Determine if a GameObject is located below the surface (of the water)
	 *
	 * @param obj
	 * 		GameObject instance that is either subsurface or not
	 *
	 * @return
	 * 		true if the GameObject instance is located below the surface
	 */
	public static bool IsSubsurface(GameObject obj) {
        return FloatsEqual (obj.transform.position.z, SubsurfaceElevationZ);
	}

	public static bool IsOnSurface(GameObject obj) {
        return FloatsEqual (obj.transform.position.z, OnSurfaceElevationZ);
	}

	public static bool IsInAir(GameObject obj) {
        return FloatsEqual (obj.transform.position.z, InAirElevationZ);
	}

    public static bool FloatsEqual(float a, float b) {
        return mag (a - b) <= ReasonablyWithinZero;
    }
    public static float mag(float f) {
        return abs (f);
    }
    public static float abs(float f) {
        return f < 0 ? -f : f;
    }

}
