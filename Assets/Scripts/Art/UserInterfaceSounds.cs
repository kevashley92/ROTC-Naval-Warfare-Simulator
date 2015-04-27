using UnityEngine;
using System.Collections;

public class UserInterfaceSounds : MonoBehaviour {

	public AudioClip option_highlighting;
	public AudioClip option_selection;
	public AudioClip option_rejection;
	public AudioClip button_down;
	public AudioClip button_up;

	private AudioSource source;

	void Awake () {
		source = GetComponent<AudioSource> ();
	}

	public void playSound(int id) {
		AudioClip sound = new AudioClip ();
		float vol = 1f;
		
		// Is there a better way to do this?
		if (id == 0) {
			sound = option_highlighting;
			vol = 0.5f;
		} else if (id == 1) {
			sound = option_selection;
			vol = 0.25f;
		} else if (id == 2) {
			sound = option_rejection;
		} else if (id == 3) {
			sound = button_down;
		} else if (id == 4) {
			sound = button_up;
		} 

		source.PlayOneShot (sound, vol);
	}
	
}
