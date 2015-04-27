using UnityEngine;
using System.Collections;

public class Example_onclick : MonoBehaviour {

	public ComboBox combobox;
	// Use this for initialization
	void Start () {
		combobox.OnSelectionChanged += (int index) => {
			//Debug.Log ("You selected index: " + index);
		};
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
