using UnityEngine;
using System.Collections;

public class GoBack : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		Debug.Log ("go back");
		Destroy(GameObject.Find("Root"));
		Destroy(GameObject.Find("Canvas"));
		//Application.LoadLevel (Application.loadedLevel("Cube"));
	}
}
