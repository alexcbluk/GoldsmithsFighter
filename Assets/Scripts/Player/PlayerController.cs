using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	//Health
	private HealthScript playerHealth;
	private HealthScript opponentHealth;

	public string playerID;
	public string playerSpawnID;

	private Transform playerSpawn;

	public string opponentID;

	//Sound
	private AudioSource source;
	
	public AudioClip punchSound;
	public AudioClip kickSound;
	public AudioClip waterSound;
	public AudioClip fireballSound;
	
	//Combo Manager
	private DelayInputInterpreter<string> interpreter;
	private ComboKeyPressManager<string> keyComboManager;
	
	[Range(0.0f, 3.0f)]
	public float timeBetweenKeyPress = 0.2f;			//Time left until combo lost
	
	public string lightPunchInputName;
	public string heavyPunchInputName;
	public string lightKickInputName;
	public string heavyKickInputName;
	public string horizontalAxisName;
	public string verticalAxisName;
	public string Taunt1;
	public string Taunt2;
	
	private class PlayerInputInterpreter : DefaultInputInterpreter
	{
		private PlayerController parent;
		private string[] buttons;
		
		public PlayerInputInterpreter(PlayerController parent)
		{
			this.parent = parent;
			buttons = new string[] {
				parent.lightPunchInputName,
				parent.heavyPunchInputName,
				parent.lightKickInputName,
				parent.heavyKickInputName,
				parent.Taunt1,
				parent.Taunt2
			};
		}
		
		protected override string[] GetButtonIDs()
		{
			return buttons;
		}
		
		protected override string GetHorizontalAxisLabel()
		{
			return parent.horizontalAxisName;
		}
		
		protected override string GetVerticalAxisLabel()
		{
			return parent.verticalAxisName;
		}
	}

	void Awake()
	{
		source = gameObject.GetComponent<AudioSource>();
		
		//Keys
		interpreter = new DelayInputInterpreter<string>(new PlayerInputInterpreter(this));
		interpreter.Delay = timeBetweenKeyPress;

		keyComboManager = new ComboKeyPressManager<string>(interpreter);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(lightPunchInputName)
		}, DoLightPunch);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(heavyPunchInputName)
		}, DoHeavyPunch);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(lightKickInputName)
		}, DoLightKick);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(heavyKickInputName)
		}, DoHeavyKick);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 0.0f, 1.0f),
			new KeyPress<string>("Right", 0.0f, 1.0f),
			new KeyPress<string>(heavyPunchInputName)
		}, DoHadouken);
		
		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>("Down", 1.0f),
			new KeyPress<string>("Up", 0.0f, 1.0f),
			new KeyPress<string>(heavyKickInputName)
		}, DoWaterFall);

		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(Taunt1)
		}, DoTaunt1);

		keyComboManager.On(new KeyPress<string>[] {
			new KeyPress<string>(Taunt2)
		}, DoTaunt2);
		
	}
	
	// Use this for initialization
	void Start () {
		animation["LightPunch"].speed = 4;
		animation["MediumPunch"].speed = 3;
		animation["HeavyPunch"].speed = 3;
		animation["LightKick"].speed = 4;
		animation["HeavyKick"].speed = 3;
		animation["Move5"].speed = 4;
		animation["Win2"].speed = 1.40f;
		animation["Lose"].speed = 1.40f;
		animation["Move3"].speed = 5;
		animation["Move6"].speed = 5;
		animation["Idle"].wrapMode = WrapMode.Loop;
		
		//Health
		playerHealth = GameObject.Find(playerID).GetComponent<HealthScript>();
		playerSpawn = GameObject.Find(playerSpawnID).transform;
		opponentHealth = GameObject.Find(opponentID).GetComponent<HealthScript>();
	}
	
	// Update is called once per frame
	void Update () {
		keyComboManager.Poll();

		if (playerHealth.IsDead || opponentHealth.IsDead) {
			Vector3 lookAt = Camera.main.transform.position;
			lookAt.z = -lookAt.z;

			if(playerHealth.IsDead){
				gameObject.transform.position = new Vector3(-3,0,0);
				gameObject.transform.LookAt(lookAt);
				animation.CrossFade ("Lose");
			}
			
			if(opponentHealth.IsDead){
				gameObject.transform.position = new Vector3(0,0,0);
				gameObject.transform.LookAt(lookAt);
				animation.CrossFade ("Win2");
			}
		}
	}
	
	//Taunt
	private void DoTaunt1(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("Move1");
		animation.CrossFadeQueued ("Idle");
	}
	//Taunt
	private void DoTaunt2(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("Move2");
		animation.CrossFadeQueued ("Idle");
	}
	//Hadouken
	private void DoHadouken(KeyPress<string>[] combo) 
	{
		CreateFireBall();
		animation.CrossFade ("Move6");
		source.PlayOneShot(fireballSound);
		animation.CrossFadeQueued ("Idle");
	}
	//WaterFall
	private void DoWaterFall(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("Move3");
		CreateWaterFall();
		source.PlayOneShot(waterSound);
		animation.CrossFadeQueued ("Idle");
	}
	
	private void DoLightPunch(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("LightPunch");
		source.PlayOneShot(punchSound);
		// Can cause dt >= 0 assertion in Unity console due to animation length
		animation.CrossFadeQueued ("Idle");
	}
	
	private void DoHeavyPunch(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("MediumPunch");
		source.PlayOneShot(punchSound);
		animation.CrossFadeQueued ("Idle");
	}
	
	
	private void DoLightKick(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("LightKick");
		source.PlayOneShot(kickSound);
		animation.CrossFadeQueued ("Idle");
	}
	
	
	private void DoHeavyKick(KeyPress<string>[] combo) 
	{
		animation.CrossFade ("HeavyKick");
		source.PlayOneShot(kickSound);
		animation.CrossFadeQueued ("Idle");
	}
	
	
	
	void MovementAnimations(){
		if(Input.GetAxis(horizontalAxisName) < 0){
			animation.CrossFade("WalkFront");
		}
		
		if(Input.GetAxis(horizontalAxisName) > 0){
			animation.CrossFade("WalkBack");
		}
	}
	
	void CreateFireBall(){
		GameObject Fireball = Instantiate(Resources.Load("Fireball"), playerSpawn.position, Quaternion.identity) as GameObject;
		Fireball.rigidbody.AddForce((transform.forward * -1) * 2000);
	}
	
	void CreateWaterFall(){
		GameObject Water = Instantiate(Resources.Load("Water")) as GameObject;

		Water.transform.parent = playerSpawn;
		Water.transform.localPosition = Vector3.zero;
		Water.transform.localRotation = Quaternion.identity;
	}

}
