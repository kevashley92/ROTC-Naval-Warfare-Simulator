using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUpdateLogAdmin : MonoBehaviour {

	public Scrollbar Scroll;

	void OnEnable(){
		Scroll.value = 1f;
		string text = System.IO.File.ReadAllText ("DebugLog.txt");
		if (text != null) {
				transform.GetComponent<Text> ().text = text; 
		}
	}
}
