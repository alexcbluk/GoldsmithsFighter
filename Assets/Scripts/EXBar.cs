/// <summary>
/// Vital bar.cs
/// Alex Luk
/// 14/02/2013
/// 
/// This class display's the EXbar of the character/monster.
/// </summary>

using UnityEngine;
using System.Collections;

public class EXBar : MonoBehaviour {
	
	public bool isPlayer1EXBar;		//Boolean to keep track it is set to work with the player
	private int maxBarLength;			//Maximum length to indicate 100% Vital Bar
	private int curBarLength;			//Current Length of the vital bar
	private GUITexture displayEXBar;
	
	private int heightOffSet = 0;
	// Use this for initialization
	void Start () {

		
		//isPlayerEXBar = true;
		//Take the reference of the ingame GUI Texture of the EX bar
		displayEXBar = gameObject.GetComponent<GUITexture>();
		//Store the length of the EX bar
		maxBarLength = (int)displayEXBar.pixelInset.width;
		
		onEnable();
		
	}

	public GUITexture EXBarTexture{
		get{ return displayEXBar; }
		set{ displayEXBar = value; }
	}
	
	//This function will be called when the gameobject is targetted. To add an listener
	public void onEnable(){
		//Check to see if the target is a player or not.
		if(isPlayer1EXBar){
		//If it is then it will add an event listener to liste for "player EX update"
			Messenger<int, int>.AddListener("player 1 EX update", changeEXBarSize);
		} else {
			//If is player 2 then...
			//otherwise it is an enemy then it will listen to the event "mob EX update"
			Messenger<int, int>.AddListener("player 2 EX update", changeEXBarSize);	
		}
	}
	
	//This function will be called when the gameobject is deselected. To remove the listener
	public void onDisable(){
		if(isPlayer1EXBar){
			//Removing the listener from player 1
			Messenger<int, int>.RemoveListener("player 1 EX update", changeEXBarSize);
		} else {
			//Removing the listener from player 2
			Messenger<int, int>.RemoveListener("player 2 EX update", changeEXBarSize);
		}
		

	}
	
	// This function sets the VitalBar class to work with Player 1's EX bar
	public void setPlayer1EX(bool b){
		isPlayer1EXBar = b;
	}
	
	//This function will calculate the size of the EX bar base on the current EX of the target in comparsion to its maximum EX.
	public void changeEXBarSize(int currentEX, int maximumEX){
		ReassigningDisplayEXBar();
		Debug.Log ("***** Receiving PlayerEXBar : " + isPlayer1EXBar + ": " + currentEX + "/" + maximumEX);
		
	//												Percentage             * maximum bar length
		curBarLength = (int)(((float)currentEX / (float)maximumEX) * maxBarLength);
		//CalculatePosition returns the calculated Rect to be set as the pixelInset
		displayEXBar.pixelInset = CalculatePosition();
	}
	
	//This function is used when PC loses its connection with the gameobject from switching between scenes.
	private void ReassigningDisplayEXBar(){
		 if(displayEXBar == null){
			if(isPlayer1EXBar){
				displayEXBar = GameObject.Find("Player1EXBar").GetComponent<GUITexture>();
			} else {
				//It is player 2
				displayEXBar = GameObject.Find("Player2EXBar").GetComponent<GUITexture>();	
			}
		}	
	}
	
	//This method returns the calculated rect of the EX bar.
	private Rect CalculatePosition(){
	 	ReassigningDisplayEXBar();
		
		float yPos = (displayEXBar.pixelInset.y) - heightOffSet;
		//This creates the offset of the mob EX bar	((maxBarLength / 4 + 10))
		float xPos = (maxBarLength - curBarLength) - (maxBarLength / 4 + 10);
			
		//If is player 2's EX bar
		if(!isPlayer1EXBar){
			//Return player 2's EX bar
			return new Rect(xPos, yPos, curBarLength, displayEXBar.pixelInset.height);
		}
		//Return the player 1's EX bar
		return new Rect(displayEXBar.pixelInset.x, displayEXBar.pixelInset.y, curBarLength, displayEXBar.pixelInset.height);
	}
	

}
