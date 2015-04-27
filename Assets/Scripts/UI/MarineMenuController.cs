using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MarineMenuController : MonoBehaviour {
	public InputField xInput;
	public InputField yInput;
	public InputField radiusInput;
	public InputField damageInput;
	public Text damageText;

	public Text xPos;
	public Text yPos;

	private bool isBomb = false;

	public void Update(){
		xPos.text = MouseToGridAPI.GetGridCoordinate( Input.mousePosition ).x.ToString();
		yPos.text = MouseToGridAPI.GetGridCoordinate (Input.mousePosition).y.ToString();
	}

	public void bombSelected(){
		damageText.text = "Damage"; //TODO: localization
		isBomb = true;
	}

	public void healthSelected(){
		damageText.text = "Health"; //TODO: localization
		isBomb = false;
	}

	public void submit(){
		//Validation
		int x = 0;
		int y = 0;
		int r = 0;
		int d = 0;

		if (!int.TryParse (xInput.text, out x)) {
			//Debug.Log ("DROP ERROR: x coordinate"); //TODO: print error message for user
			return;
		}
		if (!int.TryParse (yInput.text, out y)){
			//Debug.Log ("DROP ERROR: y coordinate"); //TODO: print error message for user
			return;
		}
		if (!int.TryParse (radiusInput.text, out r)){
			//Debug.Log ("DROP ERROR: radius"); //TODO: print error message for user
			return;
		}
		if (!int.TryParse (damageInput.text, out d)){
			//Debug.Log ("DROP ERROR: damage"); //TODO: print error message for user
			return;
		}

		Explosion explosion = new Explosion( x , y , isBomb , d , r );
		explosion.ExplosionEvent ();
		//Debug.Log ("Explosion Created");
	}


}
