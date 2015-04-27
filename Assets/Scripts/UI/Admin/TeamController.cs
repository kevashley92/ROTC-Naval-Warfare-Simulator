using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TeamController : MonoBehaviour {

	public ComboBox teamDropdown;
	public InputField teamName;
	public HSVPicker selectedColor;

	// Use this for initialization
	void Start () {
        teamDropdown.ClearItems();
        teamDropdown.AddItems(getNameArray(Team.GetTeams()));

		if (Team.GetTeams ().Count > 0)
			updateUI (Team.GetTeams () [0]);

		teamDropdown.OnSelectionChanged += (int index) => {
			updateUI( Team.GetTeams()[index] ); 
		};
	}
	
	private void updateUI( Team team ){
		teamName.text = team.GetTeamName ();
        //Debug.Log("Selected Team Name: " + team.GetTeamName());
		//selectedColor.currentColor = team.GetTeamColor ();
        selectedColor.AssignColor(team.GetTeamColor());
        //Debug.Log("Selected color updated");
        RefreshTeams(teamDropdown);
	}


	public void UpdateCurrentTeam(){
        int selectedIndex = teamDropdown.SelectedIndex;
        Team.GetTeams() [selectedIndex].SetTeamColor(selectedColor.currentColor);
        Team.GetTeams() [selectedIndex].SetTeamName(teamName.text);

        foreach (Team team in Team.GetTeams())
            //Debug.Log(team.GetTeamName());

        RefreshTeams(teamDropdown);
	}


	/**
	 * Adds a new team using team input and color selections
	 */
	public void AddTeam(){
		int teamIndex = Team.AddNewTeam (teamName.text);
		Team.GetTeams () [teamIndex].SetTeamColor (selectedColor.currentColor);
		teamDropdown.ClearItems ();
		teamDropdown.AddItems (getNameArray(Team.GetTeams ()));
        teamDropdown.SelectedIndex = teamIndex;
	}

    private static string[] getNameArray(List<Team> teams){
        string[] names = new string[teams.Count];
        for (int i = 0; i < teams.Count; i++)
        {
            names[i] = teams[i].GetTeamName();
        }
        return names;
    }

    public static void RefreshTeams(ComboBox dropdown){
        int selectedIndex = dropdown.SelectedIndex;
        dropdown.ClearItems();
        dropdown.AddItems(getNameArray(Team.GetTeams()));
        dropdown.SelectedIndex = selectedIndex;
        //Debug.Log("Team combobox refreshed!");
    }
}
