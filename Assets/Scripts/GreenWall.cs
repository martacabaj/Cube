﻿using UnityEngine;
using System.Collections;

public class GreenWall : MonoBehaviour {
	
	bool loaded = false;
	
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "FPC" && !loaded) {
			loaded = true;
			DontDestroyOnLoad (GameObject.Find ("World"));
			Application.LoadLevelAdditive ("GreenScene");
		}
	}
	
	void OnMouseDown() {
		if(Vector3.Distance(transform.position,GameObject.Find("FPC").transform.position) < 10  && !loaded) {
			loaded = true;
			DontDestroyOnLoad (GameObject.Find ("World"));
			Application.LoadLevelAdditive ("GreenScene");
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
