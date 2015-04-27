using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//Parser for Json Files
//Written by Raphael Scott
//Parses a json file and converts it to a dictionary or a hashtable
//Future improvements will allow it to convert a file to any class

public class RaphaelsSuperCoolJsonParser : MonoBehaviour
{

	//Parses the json file and returns a dictionary
	//Dictionary uses strings as the key and strings as the values
	public static Dictionary<string, string> toStringDictionary (string file)
	{
		//Creates the dictionary
		Dictionary<string, string> table = new Dictionary<string, string> ();
		//Loads the json file
		StreamReader jsonFile = new StreamReader (file);
		if (jsonFile.EndOfStream)
			return table;
		//Splits the json file by commas
		string[] lines = splitJsonFile (jsonFile);
		//Removes starting and ending parentheses
		//If they aren't there then the file is inavlid
		if (lines [0].ToCharArray () [0] != '{') {

		} else if (lines [lines.Length - 1].ToCharArray () [lines [lines.Length - 1].ToCharArray ().Length - 1] != '}') {
			Debug.LogWarning ("Invalid json file: " + file + ". Does not end with \'}\'.");
			return null;
		} else {
			lines [0] = lines [0].Substring (1);
			lines [lines.Length - 1] = lines [lines.Length - 1].Remove (lines [lines.Length - 1].Length - 2);
		}
		//Places each line in the dictionary
		for (int i = 0; i < lines.Length; i++) {
			//Split line by comma
			string[] stuff = splitJsonLine (lines [i]);
            ////Debug.LogError(stuff[0]+" Raphael "+stuff[1]);
			if (stuff == null) {
				Debug.LogWarning ("Invalid json file: " + file + ". Line " + (i + 1) + " is invalid.");
				return null;
			}
			//Adds the new line to the dictionary in key, object format
            if (!table.ContainsKey(stuff[0]))
            {
                table.Add(stuff[0], stuff[1]);
            }
            else
            {
                Debug.LogWarning("Duplicate key in file: " + file + ". Key is " + stuff[0]);
            }
		}
		//Returns the dictionary
		return table;
	}

    public static IDictionary<string, object> toDictionary (string file)
        {
            //Creates the dictionary
            IDictionary<string, object> table = new Dictionary<string, object> ();
            //Loads the json file
            StreamReader jsonFile = new StreamReader (file);
            if (jsonFile.EndOfStream)
                return table;
            //Splits the json file by commas
            string[] lines = splitJsonFile (jsonFile);
            //Removes starting and ending parentheses
            //If they aren't there then the file is inavlid
            if (lines [0].ToCharArray () [0] != '{') {
            } else if (lines [lines.Length - 1].ToCharArray () [lines [lines.Length - 1].ToCharArray ().Length - 1] != '}') {
                Debug.LogWarning ("Invalid json file: " + file + ". Does not end with \'}\'.");
                return null;
            } else {
                lines [0] = lines [0].Substring (1);
                lines [lines.Length - 1] = lines [lines.Length - 1].Remove (lines [lines.Length - 1].Length - 2);
            }
            Stack<object[]> tempDictionarysAndLists = new Stack<object[]> ();
            //Places each line in the dictionary
            for (int i = 0; i < lines.Length; i++) {
                //Split line by comma
                string[] stuff = splitJsonLine (lines [i]);
                if (stuff == null) {
                    Debug.LogWarning ("CBAInvalid json file: " + file + ". Line " + (i + 1) + " is invalid.");
                    return null;
                }
                if (string.IsNullOrEmpty (stuff [0])) {}
                else if (stuff [1].Equals ("{") || stuff [0].Equals ("{")) {
                    object[] newDictionary = {stuff [0], new Dictionary<string, object> (), true};
                    tempDictionarysAndLists.Push (newDictionary);
                } else if (stuff [1].Equals ("[")) {
                    object[] newList = {stuff [0], new List<object> (), false};
                    tempDictionarysAndLists.Push (newList);
                } else if (tempDictionarysAndLists.Count > 0) {
                    if (stuff [0].Equals ("}") || stuff [0].Equals ("]")) {
                        object[] pop = tempDictionarysAndLists.Pop ();
                        if (tempDictionarysAndLists.Count > 0) {
                            if ((bool)tempDictionarysAndLists.Peek () [2]) {
                                if (!((Dictionary<string,object>)tempDictionarysAndLists.Peek () [1]).ContainsKey ((string)pop [0]))
                                    ((Dictionary<string,object>)tempDictionarysAndLists.Peek () [1]).Add ((string)pop [0], pop [1]);
                            } else if (!((List<object>)tempDictionarysAndLists.Peek () [1]).Contains (pop [1]))
                                ((List<object>)tempDictionarysAndLists.Peek () [1]).Add (pop [1]);
                        } else {
                            if (!table.ContainsKey ((string)pop [0]))
                                table.Add ((string)pop [0], pop [1]);
                        }
                    } else {
                        Debug.LogWarning (((string)tempDictionarysAndLists.Peek () [0]) + "\t\tADD\t\t" + stuff [0] + "\t\t" + stuff [1]);
                        if (!((Dictionary<string,object>)tempDictionarysAndLists.Peek () [1]).ContainsKey (stuff [0]))
                            ((Dictionary<string,object>)tempDictionarysAndLists.Peek () [1]).Add (stuff [0], stuff [1]);
                    }
                } else {
                    //Adds the new line to the dictionary in key, object format
                    if (!table.ContainsKey(stuff[0]))
                    {
                        table.Add(stuff[0], stuff[1]);
                    }
                    else
                    {
                        Debug.LogWarning("Duplicate key in file: " + file + ". Key is " + stuff[0]);
                    }
                }
            }
            //Returns the dictionary
            return table;
        }
        
        //Parses the json file and returns a Hashtable
        //Hashtable uses strings as the key and strings as the values
        public static Hashtable toHashtable (string file)
        {
            //Creates the hashtable
            Hashtable table = new Hashtable ();
            //Loads the json file
            StreamReader jsonFile = new FileInfo ("Assets/Resources/Local/" + file + ".txt").OpenText ();
            if (jsonFile.EndOfStream)
                return table;
            //Splits the json file by commas
            string[] lines = splitJsonFile (jsonFile);
            //Removes starting and ending parentheses
            //If they aren't there then the file is inavlid
            //Places each line in the hashtable
            for (int i = 0; i < lines.Length; i++) {
                string[] stuff = splitJsonLine (lines [i]);
                if (stuff == null) {
                    Debug.LogWarning ("Invalid json file: " + file + ". Line " + (i + 1) + " is invalid.\n" + stuff.Length + "\n" + lines [i]);
                    return null;
                }
                //Adds the new line to the hashtable in key, object format
                if (!table.ContainsKey(stuff[0]))
                {
                    table.Add(stuff[0], stuff[1]);
                }
                else
                {
                    Debug.LogWarning("Duplicate key in file: " + file + ". Key is " + stuff[0]);
                }
            }
            //Returns the hashtable
            return table;
        }
        
        //Splits a json line into key, object pairs
        private static string[] splitJsonLine (string line)
        {
            //key/object
            string[] lines = {"", ""};
            //current string in the line
            int currentString = 0;
            //Goes through the line character by character
            for (int i = 0; i < line.Length; i++) {
                //If it encoutners a ", switches to in a string
                //Doesn't add " to the current string
                if (line.ToCharArray () [i] == '\"') {
                    int tempInt = line.IndexOf ('\"', i + 1);
                    while(line.ToCharArray()[tempInt - 1] == '\\'){
                        tempInt = line.IndexOf ('\"', tempInt + 1);
                    }
                    ////Debug.LogWarning (i + "\t" + tempInt + "\t" + line.Substring (i));
                    if( i <  0 || tempInt < 0){
                        Debug.LogWarning(i+" "+tempInt+" Error in line: "+line);
                        return null;
                    }
                    lines [currentString] = ((string)lines [currentString]) + line.Substring (i + 1, tempInt - i - 1);
                    i = tempInt;
                }
                //If not in a string and encounters a : it goes to the next string
                else if (line.ToCharArray () [i] == ':') {
                    //If the total number of strings is greater than 2 (breaks key/object pairing) returns null
                    if (++currentString > 1) {
                        Debug.LogWarning ("Error in line: "+line);
                        return null;
                    }
                }
                //If it encounters whitespace in a string, adds it to the current line
                //If it encounters whitespace not in a string, ignores it
                else if (!char.IsWhiteSpace (line.ToCharArray () [i]))
                    lines [currentString] += line.ToCharArray () [i];
            }
            //Returns the key/object pair
            return lines;
        }
        
        //Splits a json string into multiple strings using the given character
        //Splits a json string into multiple strings using the given character
        private static string[] splitJsonFile (StreamReader file)
        {
            //List of lines in the json file
            List<string> lines = new List<string> ();
            //bool for currently in a string
            bool inString = false;
            //Current string in the file
            int currentString = 0;
            //Puts the first string in the lines arraylist
            lines.Add ("");
            if (file.Read () != '{') {
                //Debug.LogWarning ("Invalid json file: " + file + ". Does not start with \'{\'.");
                return null;
            }
            //Goes through the file character by character
            while (!file.EndOfStream) {
                //If it encounters a ", switches to being in a string
                //Adds " to the currnent string
                char next = (char)file.Read ();
                if (next == '\\'){
                    char temp = (char)file.Read();
                    ////Debug.LogError(lines[currentString]+" R "+temp);
                    lines[currentString] += next;
                    lines[currentString] += temp;

                }
                else if (next == '\"') {
                    inString = !inString;
                    
                    lines [currentString] += next;
                }
                //If not in string and encounters a , it goes to the next part of the array
                else if (!inString && next == ',') {
                    currentString++;
                    lines.Add ("");
                } else if (!inString && (next == '{' || next == '[')) {
                    lines [currentString] += next;
                    currentString++;
                    lines.Add ("");
                } else if (!inString && (next == '}' || next == ']')) {
                    currentString++;
                    lines.Add (next.ToString ());
                    currentString++;
                    lines.Add ("");
                }
                //If it encounters whitespace in a string, adds it to the current line
                //If it encounters whitespace not in a string, ignores it
                else if (inString || !char.IsWhiteSpace (next))
                    lines [currentString] += next;
                
            }
            //If the last line is a } and whitespace, merges it to the previous line
            while (string.IsNullOrEmpty(lines[lines.Count - 1]))
                lines.RemoveAt (lines.Count - 1);
            if (lines [lines.Count - 1].Trim () == "}")
                lines.RemoveAt (lines.Count - 1);
            else if (lines [lines.Count - 1].ToCharArray () [lines [lines.Count - 1].Length - 1] == '}')
                lines [lines.Count - 1] = lines [lines.Count - 1].Substring (0, lines [lines.Count - 1].Length - 1);
            else {
                Debug.LogWarning ("Invalid json file: " + file + ". Does not end with \'}\'.");
                return null;
            }
            //Converts the arrayList to a string array
            string[] array = new string[lines.Count];
            for (int i = 0; i < lines.Count; i++)
                array [i] = (string)lines [i];
            //returns the string array
            return array;
        }
    }
