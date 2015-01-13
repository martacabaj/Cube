﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
public class PlayerController : MonoBehaviour
{
	[DllImport ("rubik")]
	private static extern int getColor ();
	#region Variables (private)
	
		private bool grounded = false;
		private Vector3 groundVelocity;
		private CapsuleCollider capsule;
	
		// Inputs Cache
		private bool jumpFlag = false;
	
	#endregion
	
	#region Properties (public)
	
		// Speeds
		public float walkSpeed = 8.0f;
		public float walkBackwardSpeed = 4.0f;
		public float runSpeed = 14.0f;
		public float runBackwardSpeed = 6.0f;
		public float sidestepSpeed = 8.0f;
		public float runSidestepSpeed = 12.0f;
		public float maxVelocityChange = 10.0f;
	
		// Air
		public float inAirControl = 0.1f;
		public float jumpHeight = 2.0f;
	
		// Can Flags
		public bool canRunSidestep = true;
		public bool canJump = true;
		public bool canRun = true;
		private Dictionary <int, GameObject> colorFloorMap;
		static int color = -1;
			GameObject lookAt;
	
	#endregion
	
	#region Unity event functions
	
		/// <summary>
		/// Use for initialization
		/// before rhe game starts
		/// </summary>
		void Awake ()
		{
				capsule = GetComponent<CapsuleCollider> ();
				//Stops the Rigidbody rotating around the world X, Y and Z axes selectively
				rigidbody.freezeRotation = true;
				rigidbody.useGravity = true;

							
		}


		
		public void ColorThread ()
		{
				color = getColor();
				Debug.Log (color);
				
		}
		/// <summary>
		/// Use this for initialization
		/// </summary>
		void Start ()
		{
			colorFloorMap = new Dictionary<int, GameObject>();
			colorFloorMap.Add(0, GameObject.Find ("White/Floor"));
			colorFloorMap.Add(1, GameObject.Find ("Red/Floor"));
			colorFloorMap.Add(2, GameObject.Find ("Orange/Floor"));
			colorFloorMap.Add(3, GameObject.Find ("Yellow/Floor"));
			colorFloorMap.Add(4, GameObject.Find ("Green/Floor"));
			colorFloorMap.Add(5, GameObject.Find ("Blue/Floor"));
			lookAt = new GameObject();

		}
	private const int samplingMax = 150;
		private int samplingTime = 0;
		/// <summary>
		/// Update is called once per frame
		/// </summary>
		void Update ()
		{
				// Cache the input
				if (Input.GetButtonDown ("Jump"))
						jumpFlag = true;

				samplingTime++;
		
				if (samplingTime == samplingMax) {
					//	Debug.Log ("call");
						Thread thread = new Thread (new ThreadStart (ColorThread));
						thread.Start ();
						samplingTime = 0;
				}
				//Debug.Log (grounded);
		}
	

		/// <summary>
		/// Update for physics
		/// This function is called every fixed framerate frame
		/// FixedUpdate should be used instead of Update when dealing with Rigidbody
		/// </summary>
		void FixedUpdate ()
		{
if(color!=-1){


	Camera.main.transform.LookAt(colorFloorMap[color].transform);

				//	Rotating(45f, 0f);
				// Cache de input
				//Vector3(left/right, up/down, forward/back)

				var inputVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0.3f);

		
				// On the ground
				if (grounded) {
						// Apply a force that attempts to reach our target velocity
						var velocityChange = CalculateVelocityChange (inputVector);
						//ForceMode elocityChange- Add an instant velocity change to the rigidbody, ignoring its mass.
						rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
			
						// Jump
						if (canJump && jumpFlag) {
								jumpFlag = false;
								rigidbody.velocity = new Vector3 (rigidbody.velocity.x, rigidbody.velocity.y + CalculateJumpVerticalSpeed (), rigidbody.velocity.z);
						}
			
						// By setting the grounded to false in every FixedUpdate we avoid
						// checking if the character is not grounded on OnCollisionExit()
						//grounded = false;
				}
		// In mid-air
		else {
						// Uses the input vector to affect the mid air direction
						var velocityChange = Camera.main.transform.TransformDirection (inputVector) * inAirControl;
						rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
				}
			}
		}
	
		// Unparent if we are no longer standing on our parent
		//OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider
		void OnCollisionExit (Collision collision)
		{
				//Debug.Log ("exit");
				if (collision.transform == transform.parent)
						transform.parent = null;
		}
	
		// If there are collisions check if the character is grounded
		//OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider
		void OnCollisionStay (Collision col)
		{
			
				TrackGrounded (col);
		}
		//OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
		void OnCollisionEnter (Collision col)
		{
	
				//if(col.gameObject.name == "Panel")
				//	Application.LoadLevel("OrangeScene");
			
				TrackGrounded (col);
		}
	
	#endregion
	
	#region Methods
	
		// From the user input calculate using the set up speeds the velocity change
		private Vector3 CalculateVelocityChange (Vector3 inputVector)
		{
				// Calculate how fast we should be moving
				var relativeVelocity = Camera.main.transform.TransformDirection (inputVector);
				if (inputVector.z > 0) {
						relativeVelocity.z *= (canRun && Input.GetButton ("Sprint")) ? runSpeed : walkSpeed;
				} else {
						relativeVelocity.z *= (canRun && Input.GetButton ("Sprint")) ? runBackwardSpeed : walkBackwardSpeed;
				}
				relativeVelocity.x *= (canRunSidestep && Input.GetButton ("Sprint")) ? runSidestepSpeed : sidestepSpeed;
		
				// Calcualte the delta velocity
				var currRelativeVelocity = rigidbody.velocity - groundVelocity;
				var velocityChange = relativeVelocity - currRelativeVelocity;
				velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
		
				return velocityChange;
		}
	
		// From the jump height and gravity we deduce the upwards speed for the character to reach at the apex.
		private float CalculateJumpVerticalSpeed ()
		{
				return Mathf.Sqrt (2f * jumpHeight * Mathf.Abs (Physics.gravity.y));
		}
	
		// Check if the base of the capsule is colliding to track if it's grounded
		private void TrackGrounded (Collision collision)
		{
				var maxHeight = capsule.bounds.min.y + capsule.radius * .9f;
				foreach (var contact in collision.contacts) {
						if (contact.point.y < maxHeight) {
							
								if (isKinematic (collision)) {
										Debug.Log ("1");
										// Get the ground velocity and we parent to it
								
										groundVelocity = collision.rigidbody.velocity;
										transform.parent = collision.transform;
									
								} else if (isStatic (collision)) {
										Debug.Log ("2");
										// Just parent to it since it's static
										transform.parent = collision.transform;
									
								} else {
									
										// We are standing over a dinamic object,
										// set the groundVelocity to Zero to avoid jiggers and extreme accelerations
										groundVelocity = Vector3.zero;
								}
				
								
								grounded = true;
						} else {
						

								transform.rotation = Quaternion.identity;
									
									
						}
			
						break;
				}
		}
	
		private bool isKinematic (Collision collision)
		{
				return isKinematic (collider.transform);
		}
	
		private bool isKinematic (Transform transform)
		{
				return transform.rigidbody && transform.rigidbody.isKinematic;
		}
	
		private bool isStatic (Collision collision)
		{
				return isStatic (collision.transform);
		}
	
		private bool isStatic (Transform transform)
		{
				return transform.gameObject.isStatic;
		}
		public float turnSmoothing = 15f; 
		public void Rotating (float horizontal, float vertical)
    	{
	        // Create a new vector of the horizontal and vertical inputs.
	        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        
	        // Create a rotation based on this new vector assuming that up is the global y axis.
	        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
	        
	        // Create a rotation that is an increment closer to the target rotation from the player's rotation.
	        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
	        Debug.Log("rot");
	        // Change the players rotation to this new rotation.
	        transform.rigidbody.MoveRotation(newRotation);
    }
    
	
	#endregion
}
