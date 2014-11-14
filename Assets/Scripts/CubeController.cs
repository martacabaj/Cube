using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour
{
		private float rotationTime = 1.0f;
		private bool isRotating = false;
		private Quaternion initialRotation;
		public GameObject world;
		// Use this for initialization
		void Start ()
		{
				
				initialRotation = transform.rotation;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (isRotating)
						return;
				if (Input.GetKeyDown (KeyCode.X))
						StartCoroutine (SmoothRotation (Vector3.right));
				else if (Input.GetKeyDown (KeyCode.Y))
						StartCoroutine (SmoothRotation (Vector3.up));
				else if (Input.GetKeyDown (KeyCode.Z))
						StartCoroutine (SmoothRotation (Vector3.forward));
	
		}

		IEnumerator SmoothRotation (Vector3 direction)
		{
				isRotating = true;
				Quaternion finalRotation = Quaternion.AngleAxis (90.0f, direction) * initialRotation;
		
				float deltaDegree = 90.0f * Time.deltaTime / rotationTime;
				Quaternion deltaRotation = Quaternion.Euler (direction * Time.deltaTime);
		
				float time = 0.0f;
				while (time <= rotationTime) {
						transform.rotation = Quaternion.Lerp (initialRotation, finalRotation, time);
			
						time += Time.deltaTime;
						yield return new WaitForEndOfFrame ();
				}
				transform.rotation = initialRotation = finalRotation;
		
				isRotating = false;
		}

}
