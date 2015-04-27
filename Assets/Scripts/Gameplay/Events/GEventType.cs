/*****
 * 
 * Name: GEventType
 * 
 * Date Created: 2015-01-30
 * 
 * Original Team: Gameplay
 * 
 * This enum will store the various types of events available to GEvents
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * B. Croft		2015-01-30	Created.  Added several members.
 * B. Croft		2015-02-11	Added FullAttackEvent
 * B. Croft		2015-02-16	Added StartOfTurnEvent
 */

using UnityEngine;
using System.Collections;

public enum GEventType {
	MoveEvent,
	AttackEvent,
	DieEvent,
	EndOfTurnEvent,
	StartOfTurnEvent,
	WeatherChangeEvent,
	AdminSpawnEvent,
	AdminStatisticChangeEvent,
	PlayerJoinEvent,
	PlayerLeaveEvent,
	WeaponTargetEvent,
	BackfireEvent,
	RemoveTargetEvent,
	UnitEmbarksEvent,
	UnitUnEmbarksEvent,
	ExplosionEvent,
	PlanMoveEvent
}
