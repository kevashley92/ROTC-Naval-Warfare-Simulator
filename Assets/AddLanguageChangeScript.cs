using UnityEngine;
using System.Collections;

public class AddLanguageChangeScript : MonoBehaviour {

    public const int EN = 0;
    public const int SP = 1;
    public const int FR = 2;
    public const int IT = 3;
    public const int JP = 4;

    public void Awake(){
        GetComponent<ComboBox>().OnSelectionChanged += (int index) => {
            if(index == EN){
                LanguageManager.instance.english();
            } 
            else if(index == SP){
                LanguageManager.instance.spanish();
            }
            else if(index == FR){
                LanguageManager.instance.french();
            }
            else if(index == IT){
                LanguageManager.instance.italian();
            }
            else if(index == JP){
                LanguageManager.instance.japanese();
            }
        };
    }
}
