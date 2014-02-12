using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	
	private KeyComboManager keyCombo;
	[Range(1, 5)]
	public int maxKeyComboLength = 4;
	[Range(0.0f, 3.0f)]
	public float timeBetweenKeyPress = 0.2f;

	void Awake()
	{
		keyCombo = new KeyComboManager(timeBetweenKeyPress, maxKeyComboLength);

		keyCombo.On(new string[] { "Fire1" }, DoLightPunch);
		keyCombo.On(new string[] { "Fire2" }, DoMediumPunch);
		keyCombo.On(new string[] { "Fire3" }, DoHeavyPunch);
		keyCombo.On(new string[] { "Down", "Right", "Fire1" }, DoHadouken);
		keyCombo.On(new string[] { "Down", "Right", "Fire2" }, DoHadouken);
		keyCombo.On(new string[] { "Down", "Right", "Fire3" }, DoHadouken);
	}

	void Update()
	{
		keyCombo.Poll();
	}

	private void DoLightPunch(string[] combo) 
	{
		Debug.Log("Light Punch");
	}

	private void DoMediumPunch(string[] combo) 
	{
		Debug.Log("Medium Punch");
	}

	private void DoHeavyPunch(string[] combo) 
	{
		Debug.Log("Heavy Punch");
	}
	
	private void DoHadouken(string[] combo) 
	{
		string type = " ";

		if (combo[2] == "Fire2") {
			type = "Super ";
		} else if (combo[2] == "Fire3") {
			type = "Ultra ";
		} 

		Debug.Log(type + "Hadouken!");
	}
}
