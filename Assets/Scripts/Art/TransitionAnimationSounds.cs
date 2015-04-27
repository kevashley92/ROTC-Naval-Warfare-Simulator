using UnityEngine;
using System.Collections;

public class TransitionAnimationSounds : MonoBehaviour {
	
	public AudioClip device_clinking;
	public AudioClip fabric_rustling;
	public AudioClip startup;

	private AudioSource source;
	
	void Awake () {
		source = GetComponent<AudioSource> ();
	}

	public void playSound(int i) {
		if (i == 0)
			source.PlayOneShot (device_clinking, 1f);
		else if (i == 1)
			source.PlayOneShot(fabric_rustling, 1f);
		else if (i == 2)
			source.PlayOneShot(fabric_rustling, 0.7f);
		else if (i == 3)
			source.PlayOneShot (startup, 1f);

	}

}
