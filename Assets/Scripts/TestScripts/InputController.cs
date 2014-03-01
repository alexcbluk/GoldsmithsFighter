using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	
	private DelayInputInterpreter<string> interpreter;
	private ComboKeyPressManager<string> keyComboManager;

	[Range(0.0f, 3.0f)]
	public float timeBetweenKeyPress = 0.2f;

	InputController() :
		base()
	{
		interpreter = new DelayInputInterpreter<string>(new DefaultInputInterpreter());
		keyComboManager = new ComboKeyPressManager<string>();

		keyComboManager.InputInterpreter = interpreter;
	}

	void Awake()
	{
		interpreter.Delay = timeBetweenKeyPress;

		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Fire1")
		}, DoLightPunch);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Fire2")
		}, DoMediumPunch);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Fire3")
		}, DoHeavyPunch);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 0.0f, 1.0f),
			new KeyPress<string>("Right", 0.0f, 1.0f),
			new KeyPress<string>("Fire1")
		}, DoHadouken);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 0.0f, 1.0f),
			new KeyPress<string>("Right", 0.0f, 1.0f),
			new KeyPress<string>("Fire2")
		}, DoHadouken);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 0.0f, 1.0f),
			new KeyPress<string>("Right", 0.0f, 1.0f),
			new KeyPress<string>("Fire3")
		}, DoHadouken);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 2.0f),
			new KeyPress<string>("Up", 0.0f, 1.0f),
			new KeyPress<string>("Fire1")
		}, SpinningBirdKick);
	}

	void Update()
	{
		if (keyComboManager.Poll() != null)
		{
			// Combo registered
		}
	}

	private void DoLightPunch(KeyPress<string>[] combo) 
	{
		Debug.Log("Light Punch");
	}

	private void DoMediumPunch(KeyPress<string>[] combo) 
	{
		Debug.Log("Medium Punch");
	}

	private void DoHeavyPunch(KeyPress<string>[] combo) 
	{
		Debug.Log("Heavy Punch");
	}
	
	private void DoHadouken(KeyPress<string>[] combo) 
	{
		string type = string.Empty;

		if (combo[2].Key == "Fire2") {
			type = "Super ";
		} else if (combo[2].Key == "Fire3") {
			type = "Ultra ";
		} 

		Debug.Log(type + "Hadouken!");
	}

	private void SpinningBirdKick(KeyPress<string>[] combo) 
	{
		Debug.Log("Spinning Bird Kick!");
	}
}
