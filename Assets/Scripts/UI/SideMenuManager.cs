using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SideMenuManager : MonoBehaviour {
    public static SideMenuManager _this;

    public GameObject currentActiveMenu;

    // Use this for initialization
    void Start() {
        _this = this;

        currentActiveMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        //as a note, instantiate and destroy prefabs is really expensive on garbage collection
        //TODO: move this to OnEnable/OnDisable

        //		if (Input.GetMouseButtonUp (0))
        //			SwapMenuOnClick ();  // Show left mouse
        ////Debug.Log("Update // Current menu: " + currentActiveMenu + (currentActiveMenu != null ? "    " + currentActiveMenu.activeInHierarchy + "," + currentActiveMenu.activeSelf : ""));
    }


    public void SwapMenuOnClick(GameObject menuToActive) {
        //Debug.Log("Swapping between '" + currentActiveMenu + "' and '" + menuToActive + "'.");

        if (currentActiveMenu != null) {
            currentActiveMenu.SetActive(false);
            ////Debug.Log("Deactivated menu: " + currentActiveMenu.name + " (" + currentActiveMenu.activeInHierarchy + ", " + currentActiveMenu.activeSelf + ")");
        }

        currentActiveMenu = menuToActive;

        if (currentActiveMenu != null) {
            currentActiveMenu.SetActive(true);
            //TerrainEditor.RefreshList();
            ////Debug.Log("Activated menu: " + currentActiveMenu.name + " (" + currentActiveMenu.activeInHierarchy + ", " + currentActiveMenu.activeSelf + ")");
        }

        ////Debug.Log("Swap // Current menu: " + currentActiveMenu + (currentActiveMenu != null ? "    " + currentActiveMenu.activeInHierarchy + "," + currentActiveMenu.activeSelf : ""));
    }
}
