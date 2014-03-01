using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	public AudioClip impact;
	// Use this for initialization
	void Start () {
		AudioSource source = (AudioSource)gameObject.GetComponent("AudioSource");


		impact = (AudioClip)Resources.Load("punch-block");
		impact = (AudioClip)Resources.Load("kick-block");
		impact = (AudioClip)Resources.Load("swing");
		impact = (AudioClip)Resources.Load("fire-ball");
		impact = (AudioClip)Resources.Load("water-ball");
		source.PlayOneShot(impact);
	}
	
	// Update is called once per frame
	void Update () {
			
	
	}
}
