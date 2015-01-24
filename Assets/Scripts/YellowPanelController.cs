using UnityEngine;
using System.Collections;
using System.Linq;

public class YellowPanelController : MonoBehaviour {
	int[] choices = {2,0,3,0,1};
	int[] shouldBe = {5,1,0,5,4};
	int chosenPanel = 1;//1-5
	bool canUse = true;
	
	// Use this for initialization
	void Start () {
		setColors ();
	}
	
	// Update is called once per frame
	void Update () {
		if (canUse) {
			if (Input.GetKeyDown (KeyCode.W) == true) {
				goUp ();
			}
			if (Input.GetKeyDown (KeyCode.S) == true) {
				goDown ();
			} 
			if (Input.GetKeyDown (KeyCode.A) == true) {
				goLeft ();
			}
			if (Input.GetKeyDown (KeyCode.D) == true) {
				goRight ();
			}
		}
		
	}
	
	void setColors() {
		for(int i=0; i<choices.Length; i++) {
			GameObject go = GameObject.Find("VerticalPanelYL"+(i+1));//.GetComponent<Transform>
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
	
	void setGColors() {
		for(int i=0; i<choices.Length; i++) {
			GameObject go = GameObject.Find("VerticalPanelYG"+(i+1));//.GetComponent<Transform>
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
	
	void goUp(){
		if (choices[chosenPanel] > 0) {
			choices[chosenPanel]--;
			setColors ();
			setGColors();
			checkVictory();
		}
	}
	
	void goDown(){
		if (choices[chosenPanel] < 5) {
			choices[chosenPanel]++;
			setColors ();
			setGColors();
			checkVictory();
		}
	}
	
	void goLeft(){
		if (chosenPanel > 0) {
			chosenPanel--;
			setColors ();
			setGColors();
			checkVictory();
		}
	}
	
	void goRight(){
		if (chosenPanel < 4) {
			chosenPanel++;
			setColors ();
			setGColors();
			checkVictory();
		}
	}
	
	void checkVictory(){
		if(choices.SequenceEqual(shouldBe)){
			canUse=false;
			
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().minigamesState[4] = 1;
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().checkIfGameWasWon();
			GameObject.Find("Yellow/PanelWallContainer/Panel").SetActiveRecursively(false);

			GameObject.Find("VerticalPanelYL1").SetActive(false);
			GameObject.Find("VerticalPanelYL2").SetActive(false);
			GameObject.Find("VerticalPanelYL3").SetActive(false);
			GameObject.Find("VerticalPanelYL4").SetActive(false);
			GameObject.Find("VerticalPanelYL5").SetActive(false);
			GameObject.Find("textL").SetActiveRecursively(true);
		}
	}
}