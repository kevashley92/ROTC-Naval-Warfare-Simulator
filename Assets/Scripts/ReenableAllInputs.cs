using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class ReenableAllInputs : MonoBehaviour
{
    public static List<GameObject> list = new List<GameObject>();
    public static List<Button> blist  = new List<Button>();
    public static List<ComboBox> cblist = new List<ComboBox>();
    public static List<EventTrigger> etlist = new List<EventTrigger>();

    public void Start()
    {
        list = new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>());

        int i = 0;
        //Filter out all non EventTriggers or Buttons or ComboBoxes
        while (i < list.Count)
        {
            GameObject go = list[i];
            if (go.GetComponent<Button>() == null &&
                go.GetComponent<ComboBox>() == null &&
                go.GetComponent<EventTrigger>() == null)
            {
                list.RemoveAt(i);
            }
            else 
            {
                i++;
            }
        }

        foreach (GameObject go in list)
        {
            go.GetComponent<Button>().enabled = true;
            go.GetComponent<Button>().interactable = true;
            go.GetComponent<ComboBox>().Interactable = true;
            go.GetComponent<ComboBox>().enabled = true;
            go.GetComponent<EventTrigger>().enabled = true;
        }
    }

    public static void Reenable()
    {
        
    }
}