using UnityEngine;
using System.Collections;

public class LoginLanguageCombo : MonoBehaviour {

	public LanguageManager lm;
	public ComboBox combobox;
	// Use this for initialization
	void Start () {
		combobox.OnSelectionChanged += (int index) => {
			switch (index) {
			case 0:
				lm.english();
				break;
			case 1:
				lm.spanish();
				break;
			case 2:
				lm.french();
				break;
			case 3:
				lm.italian();
				break;
			case 4:
				lm.japanese();
				break;
			}
			//Debug.Log ("You selected index: " + index);
		};
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
