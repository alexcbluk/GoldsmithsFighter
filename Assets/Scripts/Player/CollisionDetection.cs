using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

	public string playerGameObjectID;
	private HealthScript playerHealth;

	public string opponentBodyTag;

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.Find(playerGameObjectID);
		playerHealth = (player == null ? null : player.GetComponent<HealthScript>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider other){
		if (playerHealth != null)
		{
			if (other.tag == "Projectile")
			{
				playerHealth.Attack();
			}
			if (other.tag == opponentBodyTag)
			{
				playerHealth.Attack();
			}
		}
	}
}
