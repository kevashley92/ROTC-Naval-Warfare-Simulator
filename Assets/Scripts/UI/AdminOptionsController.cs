using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdminOptionsController : MonoBehaviour {

	public ComboBox weatherDropdown;
	public ComboBox localDropdown;

	public const int EN = 0;
	public const int SP = 1;
	public const int FR = 2;
	public const int IT = 3;
	public const int JP = 4;

	private InputField HMInput;
	private InputField NDInput;
	private InputField NWInput;
	private InputField NMInput;

	public void Awake(){
		Transform HealthLabel = transform.Find ("HealthLabel");
		HMInput = HealthLabel.FindChild ("HMInput").GetComponent<InputField>();
		NDInput = HealthLabel.FindChild ("NDInput").GetComponent<InputField>();
		NWInput = HealthLabel.FindChild ("NWInput").GetComponent<InputField>();
		NMInput = HealthLabel.FindChild ("NMInput").GetComponent<InputField>();

		HMInput.text = GlobalSettings.GetHealthThresholdForHalfMovement ().ToString();
		NDInput.text = GlobalSettings.GetHealthThresholdForNoDetectors ().ToString();
		NWInput.text = GlobalSettings.GetHealthThresholdForNoWeapons ().ToString();
		NMInput.text = GlobalSettings.GetHealthThresholdForNoMovement ().ToString();

		LanguageManager langManager = GameObject.FindObjectOfType<LanguageManager> ();

		localDropdown.OnSelectionChanged += (int index) => {
			if(index == EN){
				langManager.english();
			} 
			else if(index == SP){
				langManager.spanish();
			}
			else if(index == FR){
				langManager.french();
			}
			else if(index == IT){
				langManager.italian();
			}
			else if(index == JP){
				langManager.japanese();
			}
		};

        LocalizedItem[] weathers = new LocalizedItem[Weather.GetWeathers().Count];
        foreach(int i in Weather.GetWeathers().Keys)
            weathers[i] = new LocalizedItem("UI_Admin", "Text_WeatherType_"+Weather.GetWeathers()[i].WeatherType);
        weatherDropdown.AddItems(weathers);
	}

	void OnEnable(){
		HMInput.text = GlobalSettings.GetHealthThresholdForHalfMovement ().ToString ();
		NDInput.text = GlobalSettings.GetHealthThresholdForNoDetectors ().ToString ();
		NWInput.text = GlobalSettings.GetHealthThresholdForNoWeapons ().ToString ();
		NMInput.text = GlobalSettings.GetHealthThresholdForNoMovement ().ToString ();
	}
		

	public void updateWeather(){
		int index = weatherDropdown.SelectedIndex;
		//Debug.Log ("WEATHER INDEX: " + index);
		GlobalSettings.Instance ().throwWeatherChangeEvent (index);
	}

	public void updateThresholds(){
		GlobalSettings.SetHealthThresholdForHalfMovement ( float.Parse(HMInput.text) );
		GlobalSettings.SetHealthThresholdForNoWeapons (float.Parse (NWInput.text));
		GlobalSettings.SetHealthThresholdForNoMovement( float.Parse( NMInput.text ));
		GlobalSettings.SetHealthThresholdForNoDetectors( float.Parse(NDInput.text));
		//Debug.Log ("Thresholds Updated");
        NetworkManager.NetworkInstance.SendGlobalSettings ();
	}

}
