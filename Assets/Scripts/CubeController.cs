using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour
{
		private float rotationTime = 1.0f;
		private bool isRotating = false;
		private Quaternion initialRotation;
		public GameObject yellow, white, blue, green, red, orange;
		public GameObject currentWall;
		// Use this for initialization
		void Start ()
		{
			initialRotation = transform.rotation;
			GetGameObjects ();
		}
		private void GetGameObjects ()
		{
			yellow = GameObject.Find ("Yellow");
			white = GameObject.Find ("White");
			blue = GameObject.Find ("Blue");
			green = GameObject.Find ("Green");
			red = GameObject.Find ("Red");
			orange = GameObject.Find ("Orange");
			currentWall = blue;
			/*foreach(Renderer r in orange.GetComponentsInChildren<Renderer>()) {
				r.material = (Material) Resources.Load("Orange", typeof(Material));
			}*/
			foreach (Transform t in orange.GetComponentsInChildren<Transform>()) {
				if(t.renderer == null || (t.gameObject.name != "Cube" && t.gameObject.name != "OrangeWall")) continue;
				//Rigidbody gameObjectsRigidBody = t.gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
				//gameObjectsRigidBody.mass = 5;
				//gameObjectsRigidBody.useGravity = false;
				//gameObjectsRigidBody.isKinematic = true;
				//gameObjectsRigidBody.detectCollisions = true;
				//gameObjectsRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				//if(t.renderer == null || (t.gameObject.name != "Cube" && t.gameObject.name != "OrangeWall")) continue;
				t.renderer.material = (Material) Resources.Load("Orange", typeof(Material));
			}
		}

	
		// Update is called once per frame
		void Update ()
		{
				if (isRotating)
						return;
				if (Input.GetKeyDown (KeyCode.Y)) {
						StartCoroutine (SmoothRotation (yellow));
				} else if (Input.GetKeyDown (KeyCode.Q)) {
						StartCoroutine (SmoothRotation (white));
				} else if (Input.GetKeyDown (KeyCode.B)) {
						StartCoroutine (SmoothRotation (blue));
				} else if (Input.GetKeyDown (KeyCode.G)) {
						StartCoroutine (SmoothRotation (green));
				} else if (Input.GetKeyDown (KeyCode.R)) {
						StartCoroutine (SmoothRotation (red));
				} else if (Input.GetKeyDown (KeyCode.O)) {
						StartCoroutine (SmoothRotation (orange));
				}
	
		}

		IEnumerator SmoothRotation (GameObject dest)
		{
				isRotating = true;
			
				
				Quaternion finalRotation = Quaternion.FromToRotation (dest.transform.position, currentWall.transform.position) * initialRotation;
				
				float time = 0.0f;
				while (time <= rotationTime) {
						transform.rotation = Quaternion.Lerp (initialRotation, finalRotation, time);
			
						time += Time.deltaTime/2;
						
						yield return new WaitForEndOfFrame ();
				}
				transform.rotation = initialRotation = finalRotation;
				currentWall = dest;
				isRotating = false;
		}

}
