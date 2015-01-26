using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;


public class CubeController : MonoBehaviour
{
		public bool smallSceneLoaded = false;
		private float rotationTime = 2.0f;
		private bool isRotating = false;
		private Quaternion initialRotation;
		public GameObject yellowFloor, whiteFloor, blueFloor, greenFloor, redFloor, orangeFloor;
		public GameObject currentWall;
		public int[][] choices = new int[6][];
		public int[] minigamesState;
		public int[] victoryState = {1,1,1,1,1,1};
		Vector3 positionW = new Vector3(10000, 0, 0);
		//Vector3 positionY = new Vector3(-10000, 0, 0);
		//Vector3 positionR = new Vector3(0, 0, 10000);
		//Vector3 positionO = new Vector3(0, 0, -10000);
		//Vector3 positionG = new Vector3(0, 10000, 0);
		//Vector3 positionB = new Vector3(0, -10000, 0);
		bool moveWalls = false;
		//private const int samplingMax = 150;
	//	private int samplingTime = 0;
		// Use this for initialization
		void Start ()
		{
			
			choices = new int[6][];
			for(int i=0; i<6; i++) {
				choices[i] = new int[5];
				choices[i][0] = 2;
				choices[i][1] = 0;
				choices[i][2] = 3;
				choices[i][3] = 0;
				choices[i][4] = 1;//{2,0,3,0,1};
			}
				initialRotation = transform.rotation;
				GetGameObjects ();
				//states from walls goes as orange, blue, gree, white, yellow, red
				int[] minigamesStateTmp = {0,0,0,0,0,0};
				minigamesState = minigamesStateTmp;
				GameObject.Find("Orange/PanelWallContainer/PanelWall/New Sprite").SetActive(false);
				GameObject.Find("Red/PanelWallContainer/PanelWall/New Sprite").SetActive(false);
				GameObject.Find("White/PanelWallContainer/PanelWall/New Sprite").SetActive(false);
				GameObject.Find("Yellow/PanelWallContainer/PanelWall/New Sprite").SetActive(false);
				GameObject.Find ("Green/PanelWallContainer/PanelWall/New Sprite").SetActive (false);
				GameObject.Find ("Blue/PanelWallContainer/PanelWall/New Sprite").SetActive (false);

				GameObject.Find("Orange/PanelWallContainer/PanelWall/vtext").SetActive(false);
				GameObject.Find("Red/PanelWallContainer/PanelWall/vtext").SetActive(false);
				GameObject.Find("White/PanelWallContainer/PanelWall/vtext").SetActive(false);
				GameObject.Find("Yellow/PanelWallContainer/PanelWall/vtext").SetActive(false);
				GameObject.Find ("Green/PanelWallContainer/PanelWall/vtext").SetActive (false);
				GameObject.Find ("Blue/PanelWallContainer/PanelWall/vtext").SetActive (false);
				
				GameObject.Find("Orange/PanelWallContainer/PanelWall/bckgrn").SetActive(false);
				GameObject.Find("Red/PanelWallContainer/PanelWall/bckgrn").SetActive(false);
				GameObject.Find("White/PanelWallContainer/PanelWall/bckgrn").SetActive(false);
				GameObject.Find("Yellow/PanelWallContainer/PanelWall/bckgrn").SetActive(false);
				GameObject.Find ("Green/PanelWallContainer/PanelWall/bckgrn").SetActive (false);
				GameObject.Find ("Blue/PanelWallContainer/PanelWall/bckgrn").SetActive (false);
		}
		void Awake ()
		{
				
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
				SetWallProperties (GameObject.Find ("Red"), "Red");
				SetWallProperties (GameObject.Find ("Green"), "Green");
				SetWallProperties (GameObject.Find ("Yellow"), "Yellow");
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
					StartRotation(orangeFloor);
				}

				if (moveWalls) {
					float speed = 3.0f * Time.deltaTime;
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("white_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, whiteFloor.transform.position*10000, speed);// positionW, speed);
					}
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("orange_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, orangeFloor.transform.position*10000, speed);
					}
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("green_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, greenFloor.transform.position*10000, speed);
					}
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("blue_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, blueFloor.transform.position*10000, speed);
					}
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("yellow_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, yellowFloor.transform.position*10000, speed);
					}
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("red_wall") ){
						go.transform.position = Vector3.MoveTowards(go.transform.position, redFloor.transform.position*10000, speed);
					}
					if(positionW.Equals(GameObject.Find("White/Floor").transform.position)){
						moveWalls = false;
					}
				}
		}
        
        public void StartRotation(GameObject wall)
        {
            StartCoroutine(SmoothRotation(wall));
        }

		public IEnumerator SmoothRotation (GameObject dest)
		{
				isRotating = true;
				
				Quaternion finalRotation = Quaternion.FromToRotation (dest.transform.position, currentWall.transform.position) * initialRotation;
				
				float time = 0.0f;
				while (time <= rotationTime) {
						transform.rotation = Quaternion.Lerp (initialRotation, finalRotation, time);
						time += Time.deltaTime / 4;
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

	public void checkIfGameWasWon () {
		if (minigamesState.SequenceEqual (victoryState)) {
			Debug.Log ("global game victory");
			Destroy(GameObject.Find("Root"));
			Destroy(GameObject.Find("Canvas"));
			GameObject.Find ("FPC").transform.position = new Vector3(0,0,0);
			GameObject.Find ("FPC").rigidbody.useGravity = false;
			GameObject.Find ("EndGameText").GetComponent<CanvasGroup>().alpha=1;
			moveWalls = true;

		} else {
			//states from walls goes as orange, blue, gree, white, yellow, red
			if(minigamesState[0]==1){GameObject.Find ("Orange/PanelWallContainer/PanelWall").SetActiveRecursively(true);}
			if(minigamesState[1]==1){GameObject.Find ("Blue/PanelWallContainer/PanelWall").SetActiveRecursively (true);}
			if(minigamesState[2]==1){GameObject.Find ("Green/PanelWallContainer/PanelWall").SetActiveRecursively (true);}
			if(minigamesState[3]==1){GameObject.Find ("White/PanelWallContainer/PanelWall").SetActiveRecursively (true);}
			if(minigamesState[4]==1){GameObject.Find ("Yellow/PanelWallContainer/PanelWall").SetActiveRecursively (true);}
			if(minigamesState[5]==1){GameObject.Find ("Red/PanelWallContainer/PanelWall").SetActiveRecursively (true);}
			Debug.Log("Not yet");
		}
	}
}
