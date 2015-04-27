using UnityEngine;
using System.Collections;

public class WaitForLoginAnimFinish : MonoBehaviour {

	public float fadeSpeed = 1.5f;
	public bool isDone = false;
	private float timeLeft = 7.0f;

	public Animator anim;
	// Use this for initialization
	void Start () {
		StartCoroutine (LoadAfterAnim ());
	}

	void FadeToBlack() {
		guiTexture.color = Color.Lerp (guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	public IEnumerator LoadAfterAnim() {
		yield return new WaitForSeconds (anim.animation.clip.length);
		isDone = true;
	}

	void Update() {
		if (isDone && timeLeft <= 0) {
			guiTexture.enabled = true;
			FadeToBlack ();
			if (guiTexture.color.a >= 0.6f) {
				Application.LoadLevel ("Login");
			}
		}

		timeLeft -= Time.deltaTime;
	}
}
