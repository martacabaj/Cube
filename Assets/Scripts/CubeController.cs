using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;


public class CubeController : MonoBehaviour
{
		[DllImport ("kurwadzialaj")]
		private static extern int getColor ();
		private float
				rotationTime = 1.0f;
		private bool isRotating = false;
		private Quaternion initialRotation;
		public GameObject yellowFloor, whiteFloor, blueFloor, greenFloor, redFloor, orangeFloor;
		public GameObject currentWall;
		// Use this for initialization
		void Start ()
		{
				initialRotation = transform.rotation;
				GetGameObjects ();
		}
		void Awake ()
		{
				//new MyPluginsClass ();
				Thread thread = new Thread (new ThreadStart (ColorThread));
				thread.Start ();
		}
		public void ColorThread ()
		{
				Debug.Log (getColor ());
		}
		private void GetGameObjects ()
		{
				yellowFloor = GameObject.Find ("Yellow/Floor");
				whiteFloor = GameObject.Find ("White/Floor");
				blueFloor = GameObject.Find ("Blue/Floor");
				greenFloor = GameObject.Find ("Green/Floor");
				redFloor = GameObject.Find ("Red/Floor");
				orangeFloor = GameObject.Find ("Orange/Floor");
				currentWall = blueFloor;

				SetWallProperties (GameObject.Find ("Orange"), "Orange");
				SetWallProperties (GameObject.Find ("Blue"), "Blue");
				SetWallProperties (GameObject.Find ("White"), "White");
		}

		// Update is called once per frame
		void Update ()
		{
				if (isRotating)
						return;
				if (Input.GetKeyDown (KeyCode.Y)) {
						StartCoroutine (SmoothRotation (yellowFloor));
				} else if (Input.GetKeyDown (KeyCode.Q)) {
						StartCoroutine (SmoothRotation (whiteFloor));
				} else if (Input.GetKeyDown (KeyCode.B)) {
						StartCoroutine (SmoothRotation (blueFloor));
				} else if (Input.GetKeyDown (KeyCode.G)) {
						StartCoroutine (SmoothRotation (greenFloor));
				} else if (Input.GetKeyDown (KeyCode.R)) {
						StartCoroutine (SmoothRotation (redFloor));
				} else if (Input.GetKeyDown (KeyCode.O)) {
						StartCoroutine (SmoothRotation (orangeFloor));
				}
		}

		IEnumerator SmoothRotation (GameObject dest)
		{
				isRotating = true;
				
				Quaternion finalRotation = Quaternion.FromToRotation (dest.transform.position, currentWall.transform.position) * initialRotation;
				
				float time = 0.0f;
				while (time <= rotationTime) {
						transform.rotation = Quaternion.Lerp (initialRotation, finalRotation, time);
						time += Time.deltaTime / 2;
						yield return new WaitForEndOfFrame ();
				}
				transform.rotation = initialRotation = finalRotation;
				currentWall = dest;
				isRotating = false;
		}

		void SetWallProperties (GameObject wall, string resourceName)
		{
				foreach (Transform t in wall.GetComponentsInChildren<Transform>()) {
						if (t.renderer == null || (t.gameObject.name != "Cube" && t.gameObject.name != "PanelWall"))
								continue;
						//Rigidbody gameObjectsRigidBody = t.gameObject.AddComponent<Rigidbody> (); // Add the rigidbody.
						//	gameObjectsRigidBody.mass = 5;
						//	gameObjectsRigidBody.useGravity = false;
						//	gameObjectsRigidBody.isKinematic = true;
						//	gameObjectsRigidBody.detectCollisions = true;
						//	gameObjectsRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
						//if(t.renderer == null || (t.gameObject.name != "Cube" && t.gameObject.name != "OrangeWall")) continue;
						t.renderer.material = (Material)Resources.Load (resourceName, typeof(Material));
				}
		}
}
