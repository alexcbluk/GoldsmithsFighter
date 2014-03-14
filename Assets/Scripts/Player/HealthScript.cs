using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {
	//Health
	public int maxHealth = 100;
	private int currentHealth;

	//EX
	public int maxEX = 100;
	private int currentEX;

	public string opponentGameObjectID;
	private HealthScript opponentHealthScript;

	public string exUpdateMessageID;
	public string opponentHealthUpdateMessageID;
	
	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		currentEX = maxEX;
		
		GameObject opponent = GameObject.Find(opponentGameObjectID);
		opponentHealthScript = (opponent == null ? null : opponent.GetComponent<HealthScript>());
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Attack() {
		if (opponentHealthScript != null) {
			opponentHealthScript.CurrentHealth -= 10;
			Messenger<int,int>.Broadcast(opponentHealthUpdateMessageID, opponentHealthScript.CurrentHealth, opponentHealthScript.maxHealth);
		}
	}

	private void ConsumeEX() {
		CurrentEx -= 10;
		Messenger<int,int>.Broadcast(exUpdateMessageID, currentEX, maxEX);
	}

	public bool IsDead {
		get { return currentHealth == 0; }
	}

	public int CurrentHealth {
		get { return currentHealth; }
		set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
	}

	public int CurrentEx {
		get { return currentEX; }
		set { currentEX = Mathf.Clamp(value, 0, maxEX); }
	}

}