using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighlightSelected : MonoBehaviour {
	private Toggle toggle;
	private Image image;
	private Color originalColor;

	public Color deselectedColor = new Color(128,128,128, 255);

	// Use this for initialization
	void Start () {
		toggle = GetComponent<Toggle> ();
		image = GetComponent<Image> ();
		originalColor = image.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (toggle.isOn)
			image.color = originalColor;
		else
			image.color = originalColor * deselectedColor;
	}
}
