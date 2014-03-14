using UnityEngine;
using System.Collections;

public class ResetPosition : MonoBehaviour {

	public GameObject P1;
	public GameObject P2;
	HealthScript healthScript = new HealthScript();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Home)){
			P1.transform.position = new Vector3(3.418578f,0.2450832f,-1.13812f);
			P2.transform.position = new Vector3(-2.558077f,0.2450832f,0.2487449f);
			Messenger<int,int>.Broadcast("player 1 health update", 100, 100);
			Messenger<int,int>.Broadcast("player 2 health update", 100, 100);
		}
	}
}
