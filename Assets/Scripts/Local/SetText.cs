using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
	//The json file the text is stored in
	public string file;
	//The label in the json file to load
	//If this is blank, will default to the name of the text object in unity
	public string textLabel;
	//If in Debug, this will add the specified string to the json chart
	public string englishText;

	void Start (){
        LanguageManager.instance.register(this);
		setText ();
	}

	public void setText (){
		//If the file exists, loads it
		if (!string.IsNullOrEmpty (file)) {
			//Calls the function in the LanguageManager to display the correct string
			//If the string is not in the json file, it is added to the file
			if (Debug.isDebugBuild)
				LanguageManager.instance.displayText (this.GetComponent<Text> (), file, string.IsNullOrEmpty (textLabel) ? this.name : textLabel, englishText);
					//Calls the function in the LanguageManager to display the correct string
		else
				LanguageManager.instance.displayText (this.GetComponent<Text> (), file, string.IsNullOrEmpty (textLabel) ? this.name : textLabel);
		}
	}
}
