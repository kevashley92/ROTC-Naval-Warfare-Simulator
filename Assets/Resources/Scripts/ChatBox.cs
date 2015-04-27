using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour {

	Text textContent;
	int i = 0;

	// Use this for initialization
	void Start () {
		textContent = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if ((i % 10 == 0) && i < 3000) {
			textContent.text = textContent.text + "Hello " + i + "\n";
		}

		i++;
	}
}
