using UnityEngine;
using System.Collections;

public class YellowWall : MonoBehaviour {
	
	bool loaded = false;
	
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "FPC" && !loaded) {
			loaded = true;
			DontDestroyOnLoad (GameObject.Find ("World"));
			Application.LoadLevelAdditive ("YellowScene");
		}
	}
	
	void OnMouseDown() {
		if(Vector3.Distance(transform.position,GameObject.Find("FPC").transform.position) < 10  && !loaded) {
			loaded = true;
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
