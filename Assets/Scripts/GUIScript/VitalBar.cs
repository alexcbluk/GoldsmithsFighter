/// <summary>
/// Vital bar.cs
/// Alex Luk
/// 14/02/2013
/// 
/// This class display's the healthbar of the character/monster.
/// </summary>

using UnityEngine;
using System.Collections;

public class VitalBar : MonoBehaviour {
	
	public bool isPlayer1HealthBar;		//Boolean to keep track it is set to work with the player
	private int maxBarLength;			//Maximum length to indicate 100% Vital Bar
	private int curBarLength;			//Current Length of the vital bar
	private GUITexture displayHealthBar;
	
	private int heightOffSet = 0;
	// Use this for initialization
	void Start () {

		
		//isPlayerHealthBar = true;
		//Take the reference of the ingame GUI Texture of the health bar
		displayHealthBar = gameObject.GetComponent<GUITexture>();
		//Store the length of the health bar
		maxBarLength = (int)displayHealthBar.pixelInset.width;
		
		onEnable();
		
	}

	public GUITexture healthBarTexture{
		get{ return displayHealthBar; }
		set{ displayHealthBar = value; }
	}
	
	//This function will be called when the gameobject is targetted. To add an listener
	public void onEnable(){
		//Check to see if the target is a player or not.
		if(isPlayer1HealthBar){
		//If it is then it will add an event listener to liste for "player health update"
		 Messenger<int, int>.AddListener("player 1 health update", changeHealthBarSize);
		} else {
			//If is player 2 then...
			//otherwise it is an enemy then it will listen to the event "mob health update"
			Messenger<int, int>.AddListener("player 2 health update", changeHealthBarSize);	
		}
	}
	
	//This function will be called when the gameobject is deselected. To remove the listener
	public void onDisable(){
		if(isPlayer1HealthBar){
			//Removing the listener from player 1
			Messenger<int, int>.RemoveListener("player 1 health update", changeHealthBarSize);
		} else {
			//Removing the listener from player 2
			Messenger<int, int>.RemoveListener("player 2 health update", changeHealthBarSize);
		}
		

	}
	
	// This function sets the VitalBar class to work with Player 1's health bar
	public void setPlayer1Health(bool b){
		isPlayer1HealthBar = b;
	}
	
	//This function will calculate the size of the health bar base on the current health of the target in comparsion to its maximum health.
	public void changeHealthBarSize(int currentHealth, int maximumHealth){
		ReassigningDisplayHealthBar();
		//Debug.Log ("***** Receiving PlayerHealthBar : " + isPlayer1HealthBar + ": " + currentHealth + "/" + maximumHealth);
		
	//												Percentage             * maximum bar length
		curBarLength = (int)(((float)currentHealth / (float)maximumHealth) * maxBarLength);
		//CalculatePosition returns the calculated Rect to be set as the pixelInset
		displayHealthBar.pixelInset = CalculatePosition();
	}
	
	//This function is used when PC loses its connection with the gameobject from switching between scenes.
	private void ReassigningDisplayHealthBar(){
		 if(displayHealthBar == null){
			if(isPlayer1HealthBar){
				displayHealthBar = GameObject.Find("Player1HealthBar").GetComponent<GUITexture>();
			} else {
				//It is player 2
				displayHealthBar = GameObject.Find("Player2HealthBar").GetComponent<GUITexture>();	
			}
		}	
	}
	
	//This method returns the calculated rect of the health bar.
	private Rect CalculatePosition(){
	 	ReassigningDisplayHealthBar();
		
		float yPos = (displayHealthBar.pixelInset.y) - heightOffSet;
		//This creates the offset of the mob health bar	((maxBarLength / 4 + 10))
		float xPos = (maxBarLength - curBarLength) - (maxBarLength / 4 + 10);
			
		//If is player 2's health bar
		if(!isPlayer1HealthBar){
			//Return player 2's health bar
			return new Rect(xPos, yPos, curBarLength, displayHealthBar.pixelInset.height);
		}
		//Return the player 1's health bar
		return new Rect(displayHealthBar.pixelInset.x, displayHealthBar.pixelInset.y, curBarLength, displayHealthBar.pixelInset.height);
	}
	

}
