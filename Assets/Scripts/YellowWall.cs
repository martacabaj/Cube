using UnityEngine;
using System.Collections;

public class YellowWall : MonoBehaviour {
	
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "FPC" && !GameObject.Find("CubeGlobal").GetComponent<CubeController>().smallSceneLoaded
			&& GameObject.Find("CubeGlobal").GetComponent<CubeController>().minigamesState[4]==0) {
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().smallSceneLoaded = true;
			DontDestroyOnLoad (GameObject.Find ("World"));
			Application.LoadLevelAdditive ("YellowScene");
		}
	}
	
	void OnMouseDown() {
		if(Vector3.Distance(transform.position,GameObject.Find("FPC").transform.position) < 10  && !GameObject.Find("CubeGlobal").GetComponent<CubeController>().smallSceneLoaded
			&& GameObject.Find("CubeGlobal").GetComponent<CubeController>().minigamesState[4]==0) {
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().smallSceneLoaded = true;
			DontDestroyOnLoad (GameObject.Find ("World"));
			Application.LoadLevelAdditive ("YellowScene");
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
