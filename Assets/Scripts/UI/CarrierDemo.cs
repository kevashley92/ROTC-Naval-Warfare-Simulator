using UnityEngine;
using System.Collections;

public class CarrierDemo : MonoBehaviour {

	public GameObject UnitToReceive1;
	public GameObject UnitToReceive2;
	// Use this for initialization
	void Start () {
		Invoke ("ReceiveUnits", 1f);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void ReceiveUnits(){
		gameObject.GetComponent<ContainerController>().Receive(UnitToReceive1.GetComponent<IdentityController>().GetGuid());
		gameObject.GetComponent<ContainerController>().Receive(UnitToReceive2.GetComponent<IdentityController>().GetGuid());
	}
}
