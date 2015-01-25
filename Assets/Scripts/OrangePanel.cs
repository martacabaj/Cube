using UnityEngine;
using System.Collections;

public class OrangePanel : MonoBehaviour {

	int[] choices = {2,0,3,0,1};
	int chosenPanel = 1;//1-5
	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.name == "FPC")
		{
			Application.LoadLevel("OrangeScene");
		}
	}

	void OnMouseDown() {
		if(Vector3.Distance(transform.position,GameObject.Find("FPC").transform.position) < 10)
			Application.LoadLevel("OrangeScene");
	}

	// Use this for initialization
	void Start () {
		setColors ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void setColors() {
		for(int i=0; i<choices.Length; i++) {
			GameObject go = GameObject.Find("VerticalPanelOG"+(i+1));//.GetComponent<Transform>
			go.transform.Find("BlackSphere").transform.renderer.enabled = false;
			if(i==chosenPanel)
				go.transform.Find("BlackSphere").transform.renderer.enabled = true;
			go.transform.Find("WhiteFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			go.transform.Find("RedFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			go.transform.Find("OrangeFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			go.transform.Find("YellowFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			go.transform.Find("GreenFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			go.transform.Find("BlueFront").transform.renderer.material = (Material) Resources.Load("Grey", typeof(Material));
			switch(choices[i]) {
				case 0: {
					go.transform.Find("WhiteFront").transform.renderer.material = (Material) Resources.Load("White", typeof(Material));
					break;
				}
				case 1: {
					go.transform.Find("RedFront").transform.renderer.material = (Material) Resources.Load("Red", typeof(Material));
					break;
				}
				case 2: {
					go.transform.Find("OrangeFront").transform.renderer.material = (Material) Resources.Load("Orange", typeof(Material));
					break;
				}
				case 3: {
					go.transform.Find("YellowFront").transform.renderer.material = (Material) Resources.Load("Yellow", typeof(Material));
					break;
				}
				case 4: {
					go.transform.Find("GreenFront").transform.renderer.material = (Material) Resources.Load("Green", typeof(Material));
					break;
				}
				case 5: {
					go.transform.Find("BlueFront").transform.renderer.material = (Material) Resources.Load("Blue", typeof(Material));
					break;
				}
			}
		}
	}
}
