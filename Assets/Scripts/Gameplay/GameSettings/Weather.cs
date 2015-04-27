/*****
 * 
 * Name: Weather
 * 
 * Date Created: 2015-02-26
 * 
 * Original Team: Gameplay
 * 
 * This class will govern the weather
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	--------------------------------------------  	
 * T. Brennan	2015-02-26	Initial Commit. Added MovementModifier,
 * 							VisionModifier, WeatherType, WeatherTypes,
 * 							Basic Constructor, AddWeatherType,
 * 							GetMovementModifier, GetWeathers,
 * 							GetWeatherType, and RemoveWeatherType
 * 							
 * 
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weather {

	/**
	 * The number that the movement range is multiplied byto get the
	 * movement range in this weather condition.
	 */
	 public float MovementModifier;

	/**
	 * The number that sensor range is multiplied by to get the sensor
	 * range in this weather condition.
	 */
	public float VisionModifier;

	/**
	 * The string name of the weather type
	 */ 
	public string WeatherType;

	/**
	 * The collection of all different weather types in this game
	 */
	public static Dictionary<int, Weather> WeatherTypes;
	

	/**
	 * Default constructor for the Weather class
	 * 
	 * @param weatherType
	 * 		The string form of the new weather type
	 * @param movementModifier
	 * 		The multiplier that affects movement
	 * @param visionModifier
	 * 		The multiplier that affects vision
	 */
	public Weather(string weatherType, float movementModifier, float visionModifier) {

		this.WeatherType = weatherType;
		this.MovementModifier = movementModifier;
		this.VisionModifier = visionModifier;

	}


	/**
	 * Adds a new weather type option
	 * 
	 * @param weatherType
	 * 		The string form of the new weather type
	 * @param movementModifier
	 * 		The multiplier that affects movement
	 * @param visionModifier
	 * 		The multiplier that affects vision
	 * 
	 * @return
	 * 		The int that is the index of this weather type
	 */
	public static int AddWeatherType(string weatherType, float movementModifier, float visionModifier) {

		// Set an initial index level
		int index = 0;

		// Find the lowest available key for the new weather type
		while (WeatherTypes.ContainsKey(index)) {
			// If the key is in use

			// Increment the key and try it again
			index++;

		}

		// Create the new weather
		Weather newWeather = new Weather(weatherType, movementModifier, visionModifier);

		// Record the weather in the dictionary of weathers
		WeatherTypes.Add(index, newWeather);

		// Return the index of the newly created weather
		return index;

	}


	/**
	 * Gets the movement modifier for the specified weather
	 * 
	 * @param index
	 * 		The index of the weather in question
	 * 
	 * @return
	 * 		The modifier gets multiplied by the movement capability
	 * 		of a unit to get the unit's movement capability in the given
	 * 		weather.
	 */
	public static float GetMovementModifier(int index) {

		return WeatherTypes[index].MovementModifier;

	}


	/**
	 * Gets the vision modifier for the specified weather
	 * 
	 * @param index
	 * 		The index of the weather in question
	 * 
	 * @return
	 * 		The modifier that gets multiplied by the vision capability
	 * 		of a unit to get the unit's vision capability in the given
	 * 		weather condition.
	 */
	public static float GetVisionModifier(int index) {

		return WeatherTypes[index].VisionModifier;

	}
	

	/**
	 * Gets the dictionary of all weather types.
	 * 
	 * @return
	 * 		The dictionary of all weather types.
	 */
	public static Dictionary<int, Weather> GetWeathers() {

		return WeatherTypes;
	
	}


	/**
	 * Gets the name of the type of weather
	 * 
	 * @param index
	 * 		The index of the weather type
	 * 
	 * @return
	 * 		The name of the weather type associated with that index
	 */
	public static string GetWeatherType(int index) {

		return WeatherTypes[index].WeatherType;

	}


	/**
	 * Removes the weather type from the dictionary
	 * 
	 * @param index
	 * 		The index of the weather to be removed.
	 */
	public static void RemoveWeatherType(int index) {

		WeatherTypes.Remove(index);

	}

}
