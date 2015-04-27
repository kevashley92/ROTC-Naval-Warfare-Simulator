using UnityEngine;
using System.Collections;

public class ClientHostPull: MonoBehaviour
{
		public static ClientHostPull HostInstance;
		public string[] names;
		private HostData[] hostData;
        public string ip = "152.46.20.37";
		void Awake ()
		{
				if (HostInstance == null) {
						HostInstance = this;
				}
				MasterServer.ipAddress = ip;
				MasterServer.ClearHostList ();
				MasterServer.RequestHostList ("NavySim");
		}
        public void set(string ip){
            MasterServer.ipAddress = ip;
        }
		void Update ()
		{   
				if(!ip.Equals(NetworkManager.NetworkInstance.MASTER.ip)) {
					ip = NetworkManager.NetworkInstance.MASTER.ip;
					MasterServer.ipAddress = ip;
					MasterServer.ClearHostList ();
					MasterServer.RequestHostList ("NavySim");
				}
				if (Application.loadedLevelName.Equals ("Login")) {
						if (MasterServer.PollHostList ().Length != 0) {
								hostData = MasterServer.PollHostList ();
								int i = 0;
								names = new string[hostData.Length];
								while (i < hostData.Length) {
										names [i] = hostData [i].gameName;
				
										i++;
								}
								MasterServer.ClearHostList ();
								MasterServer.RequestHostList ("NavySim");
						}
				}
		}

		public HostData GetData (string s)
		{
				int i = 0;
				while (i < hostData.Length) {
						//if(!hostData[i].gameName.Equals("CHECK THIS SHIT"){
						if (hostData [i].gameName.Equals (s)) {
								return hostData [i];

						}
						i++;
				}
				return null;
		}
}