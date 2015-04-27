using UnityEngine;
using System.Collections;

public class ScreenFadeInOut : MonoBehaviour {

	public float fadeSpeed = 1.5f;

	private bool sceneStarting = true;

	// Use this for initialization
	void Awake () {
	
		guiTexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);

	}
	
	// Update is called once per frame
	void Update () {
	
		if (sceneStarting)
			StartScene ();

	}

	void FadeToClear() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	void FadeToBlack() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	void StartScene() {
		FadeToClear ();

		if (guiTexture.color.a <= 0.05f) {
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
			sceneStarting = false;
		}
	}

	public void EndScene() {
		guiTexture.enabled = true;

		FadeToBlack ();
		if (guiTexture.color.a >= 0.95f) {
			// Whatever you want to happen after fading to black
		}
	}


}
