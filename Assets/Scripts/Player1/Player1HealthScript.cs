using UnityEngine;
using System.Collections;

public class Player1HealthScript : MonoBehaviour {
	//Health
	private int maxHealth;
	private int currentHealth;
	private float healthBarLength;
	//EX
	public int maxEX;
	public int currentEX;
	private float EXBarLength;
	//GUI
	private int GUIBarLength;

	Player2HealthScript player2Health;

	// Use this for initialization
	void Start () {
		maxHealth = 100;
		currentHealth = maxHealth;

		maxEX = 100;
		currentEX = maxEX;

		GUIBarLength = 200;
		healthBarLength = GUIBarLength;
		EXBarLength = GUIBarLength;

		player2Health = GameObject.Find("Player 2").GetComponent<Player2HealthScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.L)){
			Attack();
		}
		if(Input.GetKeyUp(KeyCode.Q)){
			ConsumeEX();
		}
	}

	public void AdjustCurHealth(int adj){
		currentHealth += adj;

		if(currentHealth <= 0){ currentHealth = 0; }					//Make sure the current health will not go below zero
		if(currentHealth > maxHealth){ currentHealth = maxHealth;}	//Make sure the current health will not go above maxhealth
		if(maxHealth < 1){ maxHealth = 1;}							//Avoid Division by zero

		healthBarLength = (GUIBarLength) * (currentHealth/(float)maxHealth);	//Total Size multiple by the percentage of health.
	}

	private void Attack(){
		player2Health.AdjustCurHealth(-10);
		Messenger<int,int>.Broadcast("player 2 health update",player2Health.CurrentHealth,player2Health.MaxHealth);
		//Debug.Log("Attack Player 2!");
    }

	public void AdjustCurEX(int adj){
		currentEX += adj;
		
		if(currentEX <= 0){ currentEX = 0; }					//Make sure the current EX will not go below zero
		if(currentEX > maxEX){ currentEX = maxEX;}				//Make sure the current EX will not go above maxhealth
		if(maxEX < 1){ maxEX = 1;}								//Avoid Division by zero
		
		EXBarLength = (GUIBarLength) * (currentEX/(float)maxEX);	//Total Size multiple by the percentage of EX.
	}
	
	private void ConsumeEX(){
		AdjustCurEX(-10);

		Messenger<int,int>.Broadcast("player 1 EX update",currentEX,maxEX);
	}

	public int CurrentHealth{
		get{return currentHealth;}
		set{currentHealth = value;}
	}
	
	public int MaxHealth{
		get{return maxHealth;}
		set{maxHealth = value;}
	}

	/*
	void OnGUI(){
			GUI.Box(new Rect(55,30, healthBarLength,20), currentHealth + "/" + maxHealth);
	}
	 */
}
