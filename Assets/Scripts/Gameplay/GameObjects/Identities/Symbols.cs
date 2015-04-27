/*****
 * 
 * Name: Symbols
 * 
 * Date Created: 2015-01-31
 * 
 * Original Team: Gameplay
 * 
 * This class will handle loading of sprites.
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	---------- 	-------------------------------------------------------- 	
 * D.Durand  	2015-01-31	Not sure if this class is necessary. But I made it anyway 
 * T. Brennan	2015-02-17	Refactored. Also don't know if we need this.
 */ 
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Symbols : MonoBehaviour {

	/**
	 * A dummy texture that we need to get textures out of the Dictionary
	 */
	private static Texture2D Symbol = Texture2D.blackTexture; 

	/**
	 * Dictionary to hold the images that we need for this game
	 */
	private static Dictionary<string,Texture2D> MySymbols = new Dictionary<string,Texture2D>();
	
	/**
	 * Keep track of wheather we are running a Navy game or Marine game
	 */
	public bool IsNaval;


	/**
	 * Sets if we are running Navy or Marine game
	 * 
	 * @param naval
	 * 		True 	if it is a naval game
	 * 		False 	if it is a marine game
	 */
	public bool SetNaval (bool naval) {

        return World.IsNavy; // TODO: FIX
	}
	
	public void LoadRed() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadOrange() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadYellow() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadGreen() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadBlue() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadPurple() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}


	/**
	 * TODO Fill this in
	 */
	public void LoadGhost() {

		if (IsNaval) {

			//load naval sprites

		}
		else {

			//load marine sprites

		}

	}

	/**
	 * Get the symbol for a give unit symbol name. Null if name is not in Dictionary
	 * 
	 * @param symbolName
	 * 		TODO Fill this in
	 * 
	 * @return
	 * 		TODO Fill this in
	 */
	public static Texture2D GetSymbol(string symbolName) {

		if (MySymbols.TryGetValue (symbolName, out Symbol)) {

			return Symbol;

		}

		return null;

	}

}

