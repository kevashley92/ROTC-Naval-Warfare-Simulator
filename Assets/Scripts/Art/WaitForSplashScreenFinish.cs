using UnityEngine;
using System.Collections;

public class WaitForSplashScreenFinish : MonoBehaviour {

	public float fadeSpeed = 1.5f;
	public bool isDone = false;
	private float timeLeft = 10.0f;
	
	public Animator anim;
	// Use this for initialization
	void Start () {
		StartCoroutine (LoadAfterAnim ());
	}
	
	void FadeToBlack() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}
	
	public IEnumerator LoadAfterAnim() {
		yield return new WaitForSeconds (1);
		isDone = true;
	}
	
	void Update() {
		if (isDone && timeLeft <= 0) {
			guiTexture.enabled = true;
			FadeToBlack ();
			if (guiTexture.color.a >= 0.6f) {
				Application.LoadLevel ("LoginTransitionIn");
			}
		}

		timeLeft -= Time.deltaTime;
	}
}
