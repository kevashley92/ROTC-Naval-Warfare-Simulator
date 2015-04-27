using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeCreditsPage : MonoBehaviour {

    public GameObject[] pages;
    public Text pageText;
    private int currentPage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextPage(){
        if(++currentPage >= pages.Length)
            currentPage = 0;
        for(int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == currentPage);
        pageText.text = (currentPage+1)+" of "+pages.Length;
    }

    public void PreviousPage(){
        if(--currentPage < pages.Length)
            currentPage = pages.Length - 1;
        for(int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == currentPage);
        pageText.text = (currentPage+1)+" of "+pages.Length;
    }
}
