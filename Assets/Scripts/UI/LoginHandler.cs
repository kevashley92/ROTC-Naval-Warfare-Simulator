using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoginHandler : MonoBehaviour
{

		// Sounds
		public AudioClip loginSuccess;
		public AudioClip loginFailure;
		private AudioSource source;

		//to prevent spamming the button
		public int connectionCode = -1;
		private bool connecting = false;

		//for selecting between admin, user, and spectator login.
		public int code = 1;
		private const int ADMIN_CODE = 1;
		private const int USER_CODE = 2;
		private const int SPECTATOR_CODE = 3;

		// For Knowing what scene to load
		public const string ADMIN_SCENE_NAME = "AdminView_AJ_Kenny";
		public const string USER_SCENE_NAME = "UserViewIke";
		public const string SPECTATOR_SCENE_NAME = "SpectatorView";

		//For selecting between navy and marines.
		public int teamcode = 1;
		public const int NAVY_CODE = 1;
        public const int MARINES_CODE = 2;

        public ComboBox hostSelector;
		public ComboBox hostSelector2;
		public GameObject gamePasswordInput;
		public GameObject gameNameInput;
		public GameObject userPasswordInput;
		public GameObject ipInput;
		public GameObject invalidPasswordLabel;
		public GameObject userInvalidPasswordLabel;
		public GameObject adminViewPanel;
		public GameObject userViewPanel;
		public GameObject passwordPanel;
		public GameObject settingsPanel;
        public GameObject masterInput;
        public GameObject incorrectPasswordPopup;
		private string[] names;
		private bool Master;
		//private LoginHandler loginHandler;

		public static LoginHandler LoginHandlerInstance;

		void Awake ()
		{
				source = GetComponent<AudioSource> ();

				if (LoginHandlerInstance == null) {
						LoginHandlerInstance = this;
				}
				Master = true;
				hostSelector2.ClearItems();
		}

		private LoginHandler ()
		{
				connectionCode = -1;
		}

		/*public static LoginHandler instance ()

	{
		if (loginHandler = null)
		{
			loginHandler = new LoginHandler ();
		}
		return loginHandler;
	}*/

		// Use this for initialization
		void Start ()
		{
				userViewPanel.SetActive (false); //default to login as admin
				settingsPanel.SetActive (false);
                names = ClientHostPull.HostInstance.names;
		}
	
		// Update is called once per frame
		void Update ()
		{
            if (GameObject.Find ("Canvas") != null) {
                GameObject.Destroy(GameObject.Find ("Canvas") );
                GameObject.Destroy(GameObject.Find ("UniversalUIObject"));
            }

            var tempNames = ClientHostPull.HostInstance.names;
            if (tempNames != null && tempNames.Length != 0) 
            {
                bool refresh = false;
                if (tempNames.Length == names.Length)
                {
                    var toTest = new List<string>(tempNames);
                    foreach (string stringToCompare in names)
                    {
                        if (!toTest.Contains(stringToCompare))
                        {
                            refresh = true;
                            break;
                        }
                    }
                }
                else
                    refresh = true;

                if (refresh)
                {
                    names = tempNames;
                    hostSelector2.ClearItems();
                    hostSelector2.AddItems(names);
                    hostSelector.ClearItems();
                    hostSelector.AddItems(names);
                }
            }
			
			if (connecting) {
					if (connectionCode == 1) {
							switch (code) {
							case ADMIN_CODE:
									Application.LoadLevel (ADMIN_SCENE_NAME);
									break;
							case USER_CODE:
										
									Application.LoadLevel (USER_SCENE_NAME);
									//Application.LoadLevel ("UserSceneTest");
									break;
							case SPECTATOR_CODE:
									Application.LoadLevel (SPECTATOR_SCENE_NAME);
									break;
							}
							connecting = false;
					} else if (connectionCode == 0) {
							//display failed to connect
							connecting = false;
					}
			}
		}

		public void setTeam (int c)
		{
			
				this.teamcode = c;
				NetworkManager.teamcode = c;
		}

		public void setUser (int c)
		{
                if(isSettingsActive)
                    toggleSettingsPanel();
                this.code = c;
				switch (code) {
				case ADMIN_CODE:
						userViewPanel.SetActive (false);
						adminViewPanel.SetActive (true);
						if (ClientHostPull.HostInstance.names != null) {
							hostSelector2.ClearItems();
							hostSelector2.AddItems (ClientHostPull.HostInstance.names);
							names = ClientHostPull.HostInstance.names;
						}
						break;
				case SPECTATOR_CODE: //fall through
				case USER_CODE:
						adminViewPanel.SetActive (false);
						userViewPanel.SetActive (true);
						if (ClientHostPull.HostInstance.names != null) {
							hostSelector.ClearItems();
							hostSelector.AddItems (ClientHostPull.HostInstance.names);
							names = ClientHostPull.HostInstance.names;
					
						}
						break;
				}
		}

		public void quit ()
		{
				Application.Quit ();
		}

		private bool isSettingsActive = false;

		public void toggleSettingsPanel ()
		{
				settingsPanel.SetActive (!isSettingsActive);
				passwordPanel.SetActive (isSettingsActive);
				isSettingsActive = !isSettingsActive;
		}

		public void go ()
		{
				if (!connecting) {
						string password;
						switch (code) {
						case ADMIN_CODE:
								password = gamePasswordInput.GetComponent<InputField> ().text;
								NetworkManager.NetworkInstance.SetGameName(gameNameInput.GetComponent<InputField>().text);
								connecting = true;
					//Tell user that the password was wrong
					//Reset the password entry field.
					//Don't log them in.
								//Assuming that login is successful if we got here
								//But this sound doesn't finish playing before it
								//moves onto the next scene
								source.PlayOneShot(loginSuccess, 0.3f);
								
                                NetworkManager.NetworkInstance.StartServer(40, 25000, true);
								NetworkManager.NetworkInstance.SetServerPassword (password);
								return;
						case USER_CODE:
					//Connect to a server.
								if (teamcode == NAVY_CODE) {
										//do something.
								} else {
										//do something else.
								}
								password = userPasswordInput.GetComponent<InputField> ().text;
								string ip = ipInput.GetComponent<InputField> ().text;

								NetworkManager.pass = password;
								NetworkManager.ip = ip;
								
								Application.LoadLevel (USER_SCENE_NAME);
								
					//Display connecting message...
								break;
						case SPECTATOR_CODE:				
								if (teamcode == NAVY_CODE) {
								} else {
								}
								password = userPasswordInput.GetComponent<InputField> ().text;
								string ipaddr = ipInput.GetComponent<InputField> ().text;
								NetworkManager.NetworkInstance.ConnectToLocalServer (ipaddr, 25000, password);
								//Assuming that login is successful if we got here
								//But this sound doesn't finish playing before it
								//moves onto the next scene
								source.PlayOneShot(loginSuccess, 0.3f);
								break;
						}
						connecting = true;
				}
		}
	public void hostConnect ()
	{ 
			NetworkManager.ip = "-1";
			string password = userPasswordInput.GetComponent<InputField> ().text;
			NetworkManager.pass = password;
			string s = names [hostSelector.SelectedIndex];
			HostData d = ClientHostPull.HostInstance.GetData (s);
			NetworkManager.hostData = d;

			//Assuming that login is successful if we got here
			//But this sound doesn't finish playing before it
			//moves onto the next scene
			source.PlayOneShot(loginSuccess, 0.3f);
			Application.LoadLevel (USER_SCENE_NAME);

	}
	public void adminConnect()
	{
			NetworkManager.ip = "-1";
			string password = gamePasswordInput.GetComponent<InputField> ().text;
			if(password.Contains("admin-")){
                password = password.Substring(6);
                NetworkManager.pass = password;
			    string s = names [hostSelector2.SelectedIndex];
			    HostData d = ClientHostPull.HostInstance.GetData (s);
			    NetworkManager.hostData = d;
                source.PlayOneShot(loginSuccess, 0.3f);
                Application.LoadLevel (ADMIN_SCENE_NAME);
            }else{
                //TODO RAPHAEL GIMME THAT POP UP YO! This is for missing admin password
                incorrectPasswordPopup.SetActive(true);
            }

                
			
			//Assuming that login is successful if we got here
			//But this sound doesn't finish playing before it
			//moves onto the next scene
			
	}
    public void MasterChange(){
        NetworkManager.NetworkInstance.MASTER.SetAddress(masterInput.GetComponent<InputField> ().text);

        ClientHostPull.HostInstance.ip =  masterInput.GetComponent<InputField> ().text;
        ClientHostPull.HostInstance.set (masterInput.GetComponent<InputField> ().text);
    }

}
