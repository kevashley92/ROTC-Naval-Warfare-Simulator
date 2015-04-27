using UnityEngine;
using System.Collections;

public class TurnRadar : MonoBehaviour {

	public float min = 0.0f;
	public float max = 1.0f;
	public float duration = 3.0f;
	private float startTimeIn;
	private float startTimeOut;
	private SpriteRenderer sprite;

	private AudioSource source;
	
	void Awake () {
		source = GetComponent<AudioSource> ();
		sprite = GetComponent<SpriteRenderer> ();
		startTimeIn = Time.time;
		startTimeOut = startTimeIn;
	}

	void Update () {

		if (source.isPlaying) {
			float t = (Time.time - startTimeIn) / duration;
			sprite.color = new Color (1f, 1f, 1f, Mathf.SmoothStep (min, max, t));
		} else {
			// Need to reset start time to fade out
			if (startTimeOut == startTimeIn)
				startTimeOut = Time.time;

			float t = (Time.time - startTimeOut) / duration;
			sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(max, min, t));
		}
		transform.Rotate (Vector3.forward * 2);

		if (sprite.color.a <= 0 && !source.isPlaying)
			Destroy (this);
	}
}
