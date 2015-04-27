using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveProgressDriver : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        gameObject.GetComponentInChildren<Text>().text = ScenarioFactory.SaveStatus;
	}
}
