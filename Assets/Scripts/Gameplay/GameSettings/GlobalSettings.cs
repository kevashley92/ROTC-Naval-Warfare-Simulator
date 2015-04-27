/*****
 * 
 * Name: GlobalSettings
 * 
 * Date Created: 2015-02-26
 * 
 * Original Team: Gameplay
 * 
 * This class will act as storage and accessor for global variables.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-02-26	Initial Commit. Set up singleton 
 * 							functionallity. Added CurrentWeatherIndex,
 * 							HealthThresholdForHalfMovement,
 * 							HealthThresholdForNoAircraftLaunching,
 * 							HealthThresholdForNoDetectors,
 * 							HealthThresholdForNoMovement,
 * 							HealthThresholdForNoWeapons, Muted,
 * 							RetaliationPercentChance, RetaliationDamage,
 * 							SubmarineSensorLinkState, and associated
 * 							getters and setters.
 * 							
 * 
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GlobalSettings : MonoBehaviour{

	/**
	 * The singlton reference to this object
	 */
	private static GlobalSettings Self;

	/**
	 * Private constructor of the singleton
	 */
	private GlobalSettings() {
		
		// Set defaults for all variables
		this.CurrentWeatherIndex = 0;
		this.HealthThresholdForHalfMovement = 80.0f;
		this.HealthThresholdForNoAircraftLaunching = 60.0f;
		this.HealthThresholdForNoDetectors = 20.0f;
		this.HealthThresholdForNoMovement = 40.0f;
		this.HealthThresholdForNoWeapons = 60.0f;
		this.Muted = true;
		this.RetaliationPercentChance = 10.0f;
		this.RetaliationDamage = 10;
		this.SubmarineSensorLinkState = false;
	}
	
	/**
	 * Read-Only getter for the singleton instance
	 */
	public static GlobalSettings Instance(){
		
		if (Self == null) {
			Self = GameObject.FindObjectOfType<GlobalSettings> ();
			
			//Add weather TODO do it here or somewhere else
			Weather.WeatherTypes = new Dictionary<int, Weather>();
			Weather.AddWeatherType("Normal Conditions", 1f, 1f);
			Weather.AddWeatherType("Rain", .75f, .75f);
			Weather.AddWeatherType("Heavy Storm", .5f, .5f);
			
		}
		return Self;
	}

	/**
	 *
	 */
	public IDictionary<string, float> GetValues() {

		IDictionary<string, float> values = new Dictionary<string, float>();

		values.Add(CurrentWeatherIndexString, Convert.ToSingle(CurrentWeatherIndex));
        values.Add ("half", HealthThresholdForHalfMovement);
        values.Add ("unembark", HealthThresholdForNoAircraftLaunching);
        values.Add ("detectors", HealthThresholdForNoDetectors);
        values.Add ("movement", HealthThresholdForNoMovement);
        values.Add ("weapons", HealthThresholdForNoWeapons);

		return values;

	}


	/**
	 * 
	 */
	public void SetValues(IDictionary values) {
        //Debug.Log (Convert.ToSingle (values ["half"]));
        SetHealthThresholdForHalfMovement(Convert.ToSingle (values ["half"]));
        SetHealthThresholdForNoWeapons(Convert.ToSingle (values ["weapons"]));
        SetHealthThresholdForNoAircraftLaunching(Convert.ToSingle (values ["unembark"]));
        SetHealthThresholdForNoDetectors(Convert.ToSingle (values ["detectors"]));
        SetHealthThresholdForNoMovement(Convert.ToSingle (values ["movement"]));
		if (values.Contains(CurrentWeatherIndexString)) {

			//Debug.Log("In SetValues changing weather to: " + Weather.GetWeatherType(Convert.ToInt32 (values[CurrentWeatherIndexString])));

			CurrentWeatherIndex = Convert.ToInt32(Convert.ToInt32 (values[CurrentWeatherIndexString]));

			//Debug.Log ("Current Weather is now: " + Weather.GetWeatherType(CurrentWeatherIndex));

		}
		else {

			//Debug.Log ("In SetValues, values had " + values.Count + " key value pairs.");

		}

	}


	/**
	 * Method for gui code to call
	 */
	public void throwWeatherChangeEvent(int newWeather){
		object[] arguments = new object[1];
		arguments[0] = newWeather;
		
		EventFactory.ThrowEvent (GEventType.WeatherChangeEvent, arguments);
	}
	
	/**
	 * The index into the weather dictionary of the current weather
	 */
	private int CurrentWeatherIndex;

	private const string CurrentWeatherIndexString = "CurrentWeatherIndex";

	/**
	 * The percentage of a unit's health below which it moves at half speed.
	 */
	private float HealthThresholdForHalfMovement;
	
	/**
	 * The percenage of a unit's health below which it cannot launch aircraft
	 */
	private float HealthThresholdForNoAircraftLaunching;
	
	/**
	 * The percenage of a unit's health below which it cannot detect other
	 * units.
	 */
	private float HealthThresholdForNoDetectors;
	
	/**
	 * The percenage of a unit's health below which it cannot move
	 */
	private float HealthThresholdForNoMovement;
	
	/**
	 * The percenage of a unit's health below which it cannot fire weapons
	 */
	private float HealthThresholdForNoWeapons;
	
	/**
	 * true  	
	 * 		If sound should not play
	 * false 	
	 * 		If sound should play
	 */
	private bool Muted;
	
	/**
	 * The percent chance that a unit will be retaliated against when it
	 * attacks.
	 */
	private float RetaliationPercentChance;
	
	/**
	 * The damage done to a ship when it is retaliated against
	 */
	private float RetaliationDamage;

	/**
	 * true		
	 * 		If the submarine players share map visibility with other 
	 * 		units.
	 * false
	 * 		If the sumbarine players can only see what their own
	 * 		sensors reveal.
	 */
	private bool SubmarineSensorLinkState;

	/**
	 * Static getter method for the singleton's CurrentWeatherIndex variable
	 * 
	 * @return
	 * 		The index of the current weather condition in the
	 * 		Weather dictionary
	 */

	public static int GetCurrentWeatherIndex() {		
		return Instance().CurrentWeatherIndex;
	}

	/**
	 * Static getter method for the singleton's 
	 * HealthThresholdForHalfMovement variable
	 * 
	 * @return
	 * 		The percent health below which the unit can move at no
	 * 		more than half their original speed.
	 */
	public static float GetHealthThresholdForHalfMovement() {
		return Instance().HealthThresholdForHalfMovement;
	}
	

	/**
	 * Static getter method for the singleton's 
	 * HealthThresholdForNoAircraftLaunching variable
	 * 
	 * @return
	 * 		The percent health below which the aircraft carrier can
	 * 		no longer launch aircraft
	 */
	public static float GetHealthThresholdForNoAircraftLaunching() {
		
		return Instance().HealthThresholdForNoAircraftLaunching;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * HealthThresholdForNoDetectors variable
	 * 
	 * @return
	 * 		The percent health below which detectors stop working
	 */
	public static float GetHealthThresholdForNoDetectors() {
		
		return Instance().HealthThresholdForNoDetectors;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * HealthThresholdForNoMovement variable
	 * 
	 * @return
	 * 		The percent health below which the unit cannot move
	 */
	public static float GetHealthThresholdForNoMovement() {
		
		return Instance().HealthThresholdForNoMovement;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * HealthThresholdForNoWeapons variable
	 * 
	 * @return
	 * 		The percent health below which the unit cannot fire
	 * 		weapons
	 */
	public static float GetHealthThresholdForNoWeapons() {
		
		return Instance().HealthThresholdForNoWeapons;
		
	}

	/**
	 * Static getter method for the singleton's Muted variable
	 * 
	 * @return
	 * 		True 	If music should not be heard
	 * 
	 * 		False	If there should be music
	 */
	public static bool GetMuted() {
		
		return Instance().Muted;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * RetaliationPercentChance variable
	 * 
	 * @return
	 * 		The percent chance that a ship will be fired upon when it
	 * 		momentarily reveals its location by attacking
	 */
	public static float GetRetaliationPercentChance() {
		
		return Instance().RetaliationPercentChance;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * RetaliationDamage variable
	 * 
	 * @return
	 * 		The amount of damage done to a unit when another ship
	 * 		retaliates against being attacked.
	 */
	public static float GetRetaliationDamage() {
		
		return Instance().RetaliationDamage;
		
	}

	/**
	 * Static getter method for the singleton's 
	 * SubmarineSensorLinkState variable
	 * 
	 * @return
	 * 		True 	If the submarine players share map visibility
	 * 				with other units.
	 * 
	 * 		False 	If the sumbarine players can only see what
	 * 				their own sensors reveal.
	 */
	public static bool GetSubmarineSensorLinkState() {
		
		return Instance().SubmarineSensorLinkState;
		
	}
	
	
	/**
	 * Static setter method for the singleton's CurrentWeatherIndex variable
	 * 
	 * @param currentWeatherIndex
	 * 		The index of the current weather condition in the
	 * 		Weather dictionary
	 */
	public static void SetCurrentWeatherIndex(int currentWeatherIndex) {

		//Debug.Log ("SetCurrentWeatherIndex = " + currentWeatherIndex);

		Instance().CurrentWeatherIndex = currentWeatherIndex;

		NetworkManager.NetworkInstance.SendGlobalSettings();
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * HealthThresholdForHalfMovement variable
	 * 
	 * @param healthThresholdForHalfMovement
	 * 		The percent health below which the unit can move at no
	 * 		more than half their original speed.
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetHealthThresholdForHalfMovement(float healthThresholdForHalfMovemnt) {
		if (0.0 <= healthThresholdForHalfMovemnt && healthThresholdForHalfMovemnt <= 100.0) {
			Instance().HealthThresholdForHalfMovement = healthThresholdForHalfMovemnt;
		}
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * HealthThresholdForNoAircraftLaunching variable
	 * 
	 * @param healthThresholdForNoAircraftLaunching
	 * 		The percent health below which the aircraft carrier can
	 * 		no longer launch aircraft
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetHealthThresholdForNoAircraftLaunching(float healthThresholdForNoAircraftLaunching) {
		
		if (0.0 <= healthThresholdForNoAircraftLaunching && healthThresholdForNoAircraftLaunching <= 100.0) {
			
			Instance().HealthThresholdForNoAircraftLaunching = healthThresholdForNoAircraftLaunching;
			
		}
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * HealthThresholdForNoDetectors variable
	 * 
	 * @param healthThresholdForNoDetectors
	 * 		The percent health below which detectors stop working
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetHealthThresholdForNoDetectors(float healthThresholdForNoDetectors) {
		
		if (0.0 <= healthThresholdForNoDetectors && healthThresholdForNoDetectors <= 100.0) {
			
			Instance().HealthThresholdForNoDetectors = healthThresholdForNoDetectors;
			
		}
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * HealthThresholdForNoMovement variable
	 * 
	 * @param healthThresholdForNoMovement
	 * 		The percent health below which the unit cannot move
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetHealthThresholdForNoMovement(float healthThresholdForNoMovement) {
		
		if (0.0 <= healthThresholdForNoMovement && healthThresholdForNoMovement <= 100.0) {
			
			Instance().HealthThresholdForNoMovement = healthThresholdForNoMovement;
			
		}
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * HealthThresholdForNoWeapons variable
	 * 
	 * @param healthThresholdForNoWeapons
	 * 		The percent health below which the unit cannot fire
	 * 		weapons
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetHealthThresholdForNoWeapons(float healthThresholdForNoWeapons) {
		
		if (0.0 <= healthThresholdForNoWeapons && healthThresholdForNoWeapons <= 100.0) {
			
			Instance().HealthThresholdForNoWeapons = healthThresholdForNoWeapons;
			
		}
		
	}
	
	
	/**
	 * Static setter method for the singleton's Muted variable
	 * 
	 * @param muted
	 * 		True 	If music should not be heard
	 * 
	 * 		False	If there should be music
	 */
	public static void SetMuted(bool muted) {
		
		Instance().Muted = muted;
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * RetaliationPercentChance variable
	 * 
	 * @param retaliationPercentChance
	 * 		The percent chance that a ship will be fired upon when it
	 * 		momentarily reveals its location by attacking
	 * 		Restricted to values between 0.0 and 100.0
	 */
	public static void SetRetaliationPercentChance(float retaliationPercentChance) {
		if (0.0 <= retaliationPercentChance && retaliationPercentChance <= 100.0) {
			Instance().RetaliationPercentChance = retaliationPercentChance;
		}

	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * RetaliationDamage variable
	 * 
	 * @param retaliationDamage
	 * 		The amount of damage done to a unit when another ship
	 * 		retaliates against being attacked.
	 */
	public static void SetRetaliationDamage(float retaliationDamage) {
		
		Instance().RetaliationDamage = retaliationDamage;
		
	}
	
	
	/**
	 * Static setter method for the singleton's 
	 * SubmarineSensorLinkState variable
	 * 
	 * @param submarineSensorLinkState
	 * 		True 	If the submarine players share map visibility
	 * 				with other units.
	 * 
	 * 		False 	If the sumbarine players can only see what
	 * 				their own sensors reveal.
	 */
	public static void SetSubmarineSensorLinkState(bool submarineSensorLinkState) {
		Instance().SubmarineSensorLinkState = submarineSensorLinkState;
	}
}

