using UnityEngine;
using System.Collections;

public class ClickOpenButtonScript : MonoBehaviour {

	public SideMenuManager menuManager;
	public GameObject menuPrefab;

	// Sounds
	public AudioClip press;
	public AudioClip release;

	private AudioSource source;

	void Awake() {
		source = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		menuPrefab.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnMouseDown () {
		//Debug.Log("On Mouse Down: Click Open Button called");
		source.PlayOneShot (press, 1f);
	}

	public void OnMouseUp() {
		//Debug.Log("On Mouse Up: Click Open Button called with " + menuPrefab);
		source.PlayOneShot (release, 1f);
		menuManager.SwapMenuOnClick (menuPrefab);
	}
}
