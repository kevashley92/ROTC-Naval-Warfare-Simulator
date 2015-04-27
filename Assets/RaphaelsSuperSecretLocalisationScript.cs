using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RaphaelsSuperSecretLocalisationScript : MonoBehaviour {

    private bool kenny = true;
    private bool on= false;

	public void Flip(){
        if(kenny)
            GetComponent<Text>().text = "I LOVE KENNY\n<3 <3 <3 <3";
        else
            GetComponent<Text>().text = "I LOVE ROBBIE\n<3 <3 <3 <3";
        kenny = !kenny;
    }

    public void Toggle(){
        on = !on;
        gameObject.SetActive(on);
    }
}
