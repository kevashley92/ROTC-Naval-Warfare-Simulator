/*****
 * 
 * Name: UIMenuController
 * 
 * Date Created: 2015-04-04
 * 
 * Original Team: UI
 * 
 * Handles creating / switching between menus
 * 
 * Change Log
 * 
 * Name         Date        Comment
 * ----------   ----------  ------------------------------------------------------------------------------------------- 
 * A. Smith     2015-04-04  Creation
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenuController : MonoBehaviour {
    
    private static List<GameObject> MenuItems;

	public GameObject ScenarioInfo;
    
    void Start(){
        MenuItems = new List<GameObject> ();
        int childCount = transform.childCount;
        if(childCount >0){
            for(int i = 0; i <childCount; i++){
                GameObject item = transform.GetChild( i ).gameObject;
                item.SetActive( false );
                MenuItems.Add( item );
            }
        }
    }
    
    /**
     * Switches to the menu passed as parameter.
     */
    public void SwitchMenu(GameObject menu){
        //First check if the menus is in the list
        int index = 0;
        if (alreadyInMenu (menu,out index)) {
            foreach(GameObject item in MenuItems){
                item.SetActive( false );
            }
            MenuItems[index].SetActive( true );
        } 
        else {
            //deactive all other menus
            foreach(GameObject item in MenuItems){
                item.SetActive ( false );
            }
            //Create and activate new one
            GameObject go = Instantiate(menu) as GameObject;
            go.transform.SetParent( this.transform , false );
            MenuItems.Add(go);
            go.SetActive( true );
            go.name = menu.name;
            return;
        }
    }
    
    /**
     *  Checks to see if item is already in menu.  If so index is set to index of the item
     */
    private static bool alreadyInMenu(GameObject menu, out int index){
        string name = menu.name;
        index = 0;
        foreach( GameObject item in MenuItems ){
            if(item.name == name)
                return true;
            index++;
        }
        return false;
    }
    
    /**
     * Checks to see if the menu passed as a parameter isActive
     */
    public static bool IsActiveMenu ( GameObject menu ){
        int index = 0;
        if (alreadyInMenu (menu, out index))
            if (MenuItems [index].activeSelf == true)
                return true;
        return false;
    }

	public void OpenLog(){
		ScenarioInfo.SetActive (true);
	}
}
