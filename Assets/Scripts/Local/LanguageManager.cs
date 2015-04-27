using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class LanguageManager : MonoBehaviour
{
	//The language the game is currently set to
	private string language = "_en";
    private List<SetText> listOfLabels;
	//Singleton class
	private static bool singleton = false;
	public static LanguageManager instance;
	public string Language {
		get {
			return language;
		}
	}

	//Dictionary containing the Dictionaries for each language
	//While this uses up space, it keeps all the langauge dictionaries in memory so they are easily accessed
	Dictionary<string, Dictionary<string, string>> tables = new Dictionary<string, Dictionary<string, string>> ();

	//LanguageManager will not be destroyed whenever a new scene is loaded
	//This allows the Manager to be instantiated once
	//If another LanguageManager already exists then it's deleted
	void Awake ()
	{
		Vector2 offset = new Vector2 (10, 3);
		Cursor.SetCursor (Resources.Load("User Interface/Map Editor/select_icon") as Texture2D, offset , CursorMode.Auto);
		//If one already exits
		if (singleton)
			//DESTROYED
			Destroy (gameObject);
		else {
			//Sets this as the existing LanguageManager
			singleton = true;
			instance = this;
			//This manager won't be destroyed
			DontDestroyOnLoad (gameObject);
            listOfLabels = new List<SetText>();
		}
	}

    void OnLevelWasLoaded(int level){
        listOfLabels.Clear();
    }

    public void register (SetText add){
        listOfLabels.Add(add);
    }

	//Sets the language to English
	//Changes the text on every text field in the current scene
	public void english ()
	{
		language = "_en";
        foreach (SetText t in listOfLabels)
			t.setText ();
		ComboBox[] comboBreakers = FindObjectsOfType<ComboBox> ();
		foreach (ComboBox c in comboBreakers)
			c.OnEnable ();
	}

	//Sets the language to Spanish
	//Changes the text on every text field in the current scene
	public void spanish ()
	{
		language = "_sp";
        foreach (SetText t in listOfLabels)
			t.setText ();
		ComboBox[] comboBreakers = FindObjectsOfType<ComboBox> ();
		foreach (ComboBox c in comboBreakers)
			c.OnEnable ();
	}

	//Sets the language to French
	//Changes the text on every text field in the current scene
	public void french ()
	{
		language = "_fr";
        foreach (SetText t in listOfLabels)
			t.setText ();
		ComboBox[] comboBreakers = FindObjectsOfType<ComboBox> ();
		foreach (ComboBox c in comboBreakers)
			c.OnEnable ();
	}

	//Sets the language to Italian
	//Changes the text on every text field in the current scene
	public void italian ()
	{
		language = "_it";
        foreach (SetText t in listOfLabels)
			t.setText ();
		ComboBox[] comboBreakers = FindObjectsOfType<ComboBox> ();
		foreach (ComboBox c in comboBreakers)
			c.OnEnable ();
	}

	//Sets the language to Japanese
	//Changes the text on every text field in the current scene
	public void japanese ()
	{
		language = "_jp";
        foreach (SetText t in listOfLabels)
			t.setText ();
		ComboBox[] comboBreakers = FindObjectsOfType<ComboBox> ();
		foreach (ComboBox c in comboBreakers)
			c.OnEnable ();
	}

	public string getString (string file, string label)
	{
		//Creates a dictionary for the current file
		Dictionary<string, string> table;
		//If the dictionary has already been read in, get it from the tables dictionary
		if (tables.ContainsKey (file))
			table = tables [file];
		//If it is a new dictionary, parses it using my super cool parser
		//Adds it to the dictionary of dictionaries
		else {
            table = RaphaelsSuperCoolJsonParser.toStringDictionary ("Assets/Resources/Local/" + file + ".txt");
			tables.Add (file, table);
		}
		//If the label is not in the dictionary, doesn't chanege anything
		if (!table.ContainsKey (label + language)) {
			//Debug.LogWarning("File " + file + " does not contain " + label + language);
			return label;
		}
		if (string.IsNullOrEmpty (table [label + language].ToString ()))
			return null;
		return table [label + language].ToString ();
	}

	public string getString(LocalizedItem item)
	{
		return getString(item.file, item.label) + item.append;
	}

	public string getString (string file, string label, string englishText)
	{
		Dictionary<string, string> table;
		//If the dictionary has already been read in, get it from the tables dictionary
		if (tables.ContainsKey (file))
			table = tables [file];
		//If it is a new dictionary, parses it using my super cool parser
		//Adds it to the dictionary of dictionaries
		else {
            table = RaphaelsSuperCoolJsonParser.toStringDictionary ("Assets/Resources/Local/" + file + ".txt");
			tables.Add (file, table);
		}
		//bool for whether or not the table was changed
		bool add = false;
		//If the table does not contain the key, adds the key/object pair
		if (!table.ContainsKey (label + language)) {
			table.Add (label + language, englishText);
			add = true;
		}
		//If the table contains the key, but the string it contains is different from englishText
		//Changes the string at key to englishText
		else if (!(table [label + language].ToString ().Equals (englishText))) {
			table [label + "_en"] = englishText;
			table [label + "_sp"] = "";
			table [label + "_fr"] = "";
			table [label + "_it"] = "";
			table [label + "_jp"] = "";
			add = true;
		}
		//If the table was changed
		if (add) {
			//Writes a new json file
			string newJson = "{\n";
			foreach (string s in table.Keys)
				newJson += "\t\"" + s + "\": \"" + table [s] + "\",\n";
			newJson = newJson.Remove (newJson.Length - 2) + "\n}";
			//Writes the new one
			File.WriteAllText ("Assets/Resources/Local/Source/" + file + ".txt", newJson);
		}
		return table [label + language].ToString ();
	}

	//Sets text on a text object
	//First parameter is the object to change
	//Second parameter is the json file to open
	//Third parameter is the label of the string in the json file
	public void displayText (Text obj, string file, string label)
	{
		//Sets the text on the in game object to the text in the table
		string result = getString (file, label);
		if (!string.IsNullOrEmpty (result))
			obj.text = result; 
	}

	//Overloaded version of the previous function
	//If there is no label parameter, then the name of the gameobject is used
	public void displayText (Text obj, string file)
	{
		displayText (obj, file, obj.gameObject.name);
	}

	//Overloaded version of the previous two functions
	//Only works if this is a debug build, otehrwise calls the 3 parameter displayText function
	//Fourth parameter is the english text for the label
	//If this text is different from the text in the json file, the text in the file is updated
	//If the label is not in the json file, it is added to the json file with the text
	//If englishtext is blank, calls the 3 parameter version of the function
	public void displayText (Text obj, string file, string label, string englishText)
	{
		//If debug build or blank english text, call 3 parameter version
		if (!Debug.isDebugBuild || string.IsNullOrEmpty (englishText) || !language.Equals ("_en")) {
			displayText (obj, file, label);
			return;
		}
		//Creates a dictionary for the current file

		//Sets the text on the in game object to the text in the table
		string result = getString (file, label, englishText);
		if (!string.IsNullOrEmpty (result))
			obj.text = result; 
	}

}
