using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour {
	private Objective objective;

	public ComboBox dropdown;
	public InputField input;

    private static ObjectiveController objectiveControllerInstance;
		
	// Use this for initialization
	void Start () {
		objective = Objective.GetInstance ();
		dropdown.AddItems (objective.GetIndividualObjectives ().ToArray());
        dropdown.HideFirstItem = false;
		dropdown.OnSelectionChanged += (int index) => {
			updateInput(index); 
		};

        objectiveControllerInstance = this;
	}

	public void updateInput( int index ){
		input.text = objective.GetIndividualObjectives()[index];
	}

	public void saveObjective(){
		objective.setIndividualObjective (dropdown.SelectedIndex, input.text);
		dropdown.ClearItems ();
		dropdown.AddItems (objective.GetIndividualObjectives ().ToArray ());
	}

	public void addObjective(){
		objective.addObjective (input.text);  
		dropdown.AddItems (input.text);
	}

    public void UpdateDropDown()
    {
        var objectives = objective.GetIndividualObjectives();
        dropdown.ClearItems();

        dropdown.AddItems(objectives.ToArray());

        dropdown.SelectedIndex = 0;

        if (objectives.Count > 0)
            updateInput(0);
    }

    public static ObjectiveController GetInstance()
    {
        return objectiveControllerInstance;
    }
}
