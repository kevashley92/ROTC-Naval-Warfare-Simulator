using UnityEngine;
using System.Collections;

public class Master {

    public string ip;
    public int port = 23466;
    public string gameTypeName = "NavySim";

	public Master() {
		if(PlayerPrefs.HasKey("MasterIP")) {
			ip = PlayerPrefs.GetString("MasterIP");
		}
		else {
			ip = "152.46.20.37";
			PlayerPrefs.SetString("MasterIP",ip);
		}
	}

    public void Connect(HostData hostData, string password)
    {
        MasterServer.ipAddress = ip;
        Network.Connect(hostData.guid, password);
    }
    public void Register(string gameName)
    {
        MasterServer.ipAddress = ip;
        MasterServer.port = port;
		MasterServer.RegisterHost (gameTypeName, gameName, "Comment");
	}

	public void SetAddress(string newip) {
		PlayerPrefs.SetString("MasterIP",newip);
		ip = newip;
		ClientHostPull.HostInstance.ip = ip;
	}
}
