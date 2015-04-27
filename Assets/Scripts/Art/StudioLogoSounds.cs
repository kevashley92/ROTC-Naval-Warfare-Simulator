using UnityEngine;
using System.Collections;

public class StudioLogoSounds : MonoBehaviour {

	public AudioClip whoosh;
	public AudioClip small_screwbit;
	public AudioClip mechanical_clink;
	public AudioClip rotation;
	public AudioClip drill_connect;
	public AudioClip clink;
	public AudioClip constant_drill;
	public AudioClip heavy_mechanical_bang;
	public AudioClip outro;

	private AudioSource source;

	// Use this for initialization
	void Start () {
	
		source = GetComponent<AudioSource> ();

	}

	// Play sounds at specific frames in the animation
	void playSound(int id) {
		AudioClip sound = new AudioClip ();
		float vol = 1f;

		// Is there a better way to do this?
		if (id == 1) {
			sound = whoosh;
		} else if (id == 2) {
			sound = small_screwbit;
			vol = 0.25f;
		} else if (id == 3) {
			sound = mechanical_clink;
			vol = 0.25f;
		} else if (id == 4) {
			sound = rotation;
			vol = 0.5f;
		} else if (id == 5) {
			sound = clink;
			vol = 0.5f;
		} else if (id == 6) {
			sound = drill_connect;
		} else if (id == 7) {
			sound = constant_drill;
			vol = 0.5f;
		} else if (id == 8) {
			sound = heavy_mechanical_bang;
		} else if (id == 9) {
			sound = outro;
		}

		// Not sure if this is safe
		source.PlayOneShot (sound, vol);

	}
}
