using UnityEngine;
using System.Collections;

public class LeaveSplashScreen : MonoBehaviour {

	private float animationEnd = 11.0f;
	private float fadeSpeed = 1.5f;

	void Awake() {
		// Make sure the texture is as big as the screen
		guiTexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
		guiTexture.color = Color.clear;
		guiTexture.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (animationEnd > 0) 
		{
			animationEnd -= Time.deltaTime;
		}
		else
		{
			guiTexture.enabled = true;
			fadeToBlack ();
		}
	
		if (guiTexture.color.a >= 0.95f);
			//Application.LoadLevel ("Login");
	}

	void fadeToClear() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	void fadeToBlack() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}

}
