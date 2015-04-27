using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*****
 * 
 * Name: ContainerController    
 * 
 * Date Created: 2015-04-09
 * 
 * Original Team: UI
 * 
 * A controller for the info box
 * 
 * Change Log
 * 
 * Name         Date        Comment
 * ----------   ----------  --------------------------------------------  
 * A. Smith     2015-04-09  Creation
 */

public class UIUnitInfoController : MonoBehaviour {

    //UI Text Components
    private Text Title;
    private Text TeamValue;
    private Text HealthValue;
    private Text LocationValue;
    private Text ContainedValue;

	void Awake() {
		Title = transform.FindChild("Title").GetComponent<Text>();
		TeamValue = transform.FindChild("TeamValue").GetComponent<Text>();
		HealthValue = transform.FindChild("HealthValue").GetComponent<Text>();
		LocationValue = transform.FindChild("LocationValue").GetComponent<Text>();
		ContainedValue = transform.FindChild("ContainedValue").GetComponent<Text>();
		transform.parent = GameObject.Find("Canvas").transform;
		transform.position = new Vector3(0, 0, 1);
	}

	// Use this for initialization
	void Start () {
//        Title = transform.FindChild("Title").GetComponent<Text>();
//        TeamValue = transform.FindChild("TeamValue").GetComponent<Text>();
//        HealthValue = transform.FindChild("HealthValue").GetComponent<Text>();
//        LocationValue = transform.FindChild("LocationValue").GetComponent<Text>();
//        ContainedValue = transform.FindChild("ContainedValue").GetComponent<Text>();
//        transform.parent = GameObject.Find("Canvas").transform;
//        transform.position = new Vector3(0, 0, 1);
	}

    /**
     * To be called immediately after unit creation. Populates the infobox with unit information
     */
    public void init(GameObject unit)
    {
        //Debug.Log("Unit passed into method: " + unit);
        if (unit != null && unit.GetComponent<IdentityController>() != null)
        {
            Title.text = unit.GetComponent<IdentityController>().GetFullName();
            TeamValue.text = Team.GetTeam(unit.GetComponent<IdentityController>().GetTeam()).GetTeamName();
            HealthValue.text = unit.GetComponent<HealthController>().GetCurrentHealth().ToString() + " / " 
                + unit.GetComponent<HealthController>().GetMaxHealth().ToString();
            Vector3 position = unit.transform.localPosition;
            position = MouseToGridAPI.GetGridCoordinateFromUnit(position);
			LocationValue.text = position.x + " , " + position.y;
            if(unit.GetComponent<ContainerController>() != null)
                ContainedValue.text = unit.GetComponent<ContainerController>().GetCount().ToString();
            else
                ContainedValue.text = "N/A";


            Vector2 pos;
            Canvas canvas = transform.parent.GetComponent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            transform.position = transform.parent.TransformPoint(pos);
            transform.localScale = new Vector3(1,1,1);
			if(transform.position.y > 20){
				transform.localScale = new Vector3(1, -1, 1);				
				foreach (Transform child in transform) {
					child.localScale = new Vector3(1,-1,1);
					if(child.name == "Title")
						child.localPosition = new Vector3( child.localPosition.x, 90, child.localPosition.z);
				}					
			}
			else{
				foreach (Transform child in transform) {
					child.localScale = new Vector3(1,1,1);
					if(child.name == "Title")
						child.localPosition = new Vector3( child.localPosition.x, 220 , child.localPosition.z);
				}	
			}

        }
    }

	
}
