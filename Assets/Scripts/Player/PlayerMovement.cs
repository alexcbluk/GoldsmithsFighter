using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class PlayerMovement : MonoBehaviour {
	
	public float speed = 6.0f; 
	public float jumpSpeed = 8.0f; 
	public float gravity = 20.0f;
	
	public string horizontalAxisName;
	public string verticalAxisName;

	private Vector3  moveDirection = Vector3.zero; 
	private bool grounded = false; 

	private CharacterController characterController;

	// Use this for initialization
	void Start () {
		characterController = gameObject.GetComponent<CharacterController>(); 
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void FixedUpdate() { 
		if (grounded) { // We are grounded, so recalculate movedirection directly from axes
			moveDirection = new Vector3(Input.GetAxis(horizontalAxisName), 0); 
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if (Input.GetAxis(verticalAxisName) > 0) { 
				moveDirection.y = jumpSpeed;
			} 
		} 
		
		// Apply gravity 
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller 
		CollisionFlags flags = characterController.Move(moveDirection * Time.deltaTime); 
		grounded = (flags & CollisionFlags.CollidedBelow) != 0; 
	} 
	
}