using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using System.Threading;
public class GreenPanelController : MonoBehaviour {
	[DllImport("rubikCube102")]
	private static extern int getDirection ();
	int[] choices = {2,0,3,0,1};
	int[] shouldBe = {1,0,1,1,2};
	int chosenPanel = 1;//1-5
	bool canUse = true;
		bool continueThreads=true;
	int dir=-1;
	bool used=true;
	public void DirectionThread (EventHandler onThreadStop)
		{
			int previous = dir;
			if(used){
				dir = getDirection();
				if(dir>=0 && previous!=dir){
					Debug.Log(dir);
					used=false;
				}
			}
			
		
			OnThreadStop (null, EventArgs.Empty);

		}
		void OnThreadStop (object sender, EventArgs e)
		{
				if (continueThreads) {
						new Thread (new ThreadStart (() => {
								DirectionThread (OnThreadStop);
						})).Start ();
        
				}
		}
	// Use this for initialization
	void Start () {
		
		Debug.Log("start");
		choices = GameObject.Find("CubeGlobal").GetComponent<CubeController>().choices[1];
		setColors ();

			
		
      
	}
		void OnDestroy ()
		{
				continueThreads = false;
				PlayerController pc=GameObject.Find("FPC").GetComponent("PlayerController") as PlayerController;
				pc.StartColor();
		}
		void Awake(){
			PlayerController pc=GameObject.Find("FPC").GetComponent("PlayerController") as PlayerController;
			pc.StopColor();
			Debug.Log("awake");

		   	new Thread (new ThreadStart (() =>
			{
				Debug.Log("dir");
				DirectionThread (OnThreadStop);
			})).Start ();
			
		}

	// Update is called once per frame
	void Update () {
		if (canUse) {
	
			if (dir==1 && !used) {
				goUp ();
				Debug.Log("DIR " +dir);
				used = true;
			}
			if (dir==3&& !used) {
				goDown ();
				used = true;
				Debug.Log("DIR " +dir);
			} 
			if (dir==0&& !used) {
				goLeft ();
				used = true;
				Debug.Log("DIR "+ dir);
			}
			if (dir==2&& !used) {
				goRight ();
				Debug.Log("DIR "+ dir);
				used = true;
			}
		}
		
	}
	
	void setColors() {
		for(int i=0; i<choices.Length; i++) {
			GameObject go = GameObject.Find("VerticalPanelGL"+(i+1));//.GetComponent<Transform>
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
			GameObject go = GameObject.Find("VerticalPanelGG"+(i+1));//.GetComponent<Transform>
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
		GameObject.Find("CubeGlobal").GetComponent<CubeController>().choices[2] = choices;
		if(choices.SequenceEqual(shouldBe)){
			canUse=false;
			
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().minigamesState[2] = 1;
			GameObject.Find("CubeGlobal").GetComponent<CubeController>().checkIfGameWasWon();
			GameObject.Find("Green/PanelWallContainer/Panel").SetActiveRecursively(false);

			GameObject.Find("VerticalPanelGL1").SetActive(false);
			GameObject.Find("VerticalPanelGL2").SetActive(false);
			GameObject.Find("VerticalPanelGL3").SetActive(false);
			GameObject.Find("VerticalPanelGL4").SetActive(false);
			GameObject.Find("VerticalPanelGL5").SetActive(false);
			GameObject.Find("textL").SetActiveRecursively(true);
		}
	}
}
