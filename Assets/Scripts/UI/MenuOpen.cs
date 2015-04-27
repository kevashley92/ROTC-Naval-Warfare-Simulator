using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuOpen : MonoBehaviour {
	RectTransform menu;

	int menuHeight = 0;
	public int menuMax = 100;
	public int speed = 5;

	bool menuOpening = false;
	bool menuOpen = false;
	bool menuClosing = false;

	// Use this for initialization
	void Start () {
		menu = GetComponent<RectTransform> ();
		//initialize height as 0
		menu.sizeDelta = new Vector2 (menu.rect.width, 0);
		print("Test");
	}
	
	// Update is called once per frame
	void Update () {
		menu.sizeDelta = new Vector2 (menu.rect.width, menuHeight);

		if (menuOpening) {
			if(menuHeight < menuMax)
				menuHeight+= speed;
			else{
				menuOpening = false;
				menuOpen = true;
			}
		}

		if(menuClosing){
			if(menuHeight>0){
				menuHeight-=speed;
			}else{
				menuClosing = false;
				menuOpen = false;
			}
		}
	}

	public void toggleMenu(){
		if (menuOpen)
			menuClosing = true;
		else
			menuOpening = true;
	}
}
