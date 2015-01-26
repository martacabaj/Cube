using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using System.Threading;
 using UnityEditor;
 using System.IO;
public class PanelController : MonoBehaviour {
protected bool continueThreads=true;
protected	int dir=-1;
protected	bool used=true;
	[DllImport("direction")]
	private static extern int getDirection ();
    [DllImport("direction")]
    private static extern void start ();
    [DllImport("direction")]
    private static extern void end ();
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
		public void OnThreadStop (object sender, EventArgs e)
		{
				if (continueThreads) {
						new Thread (new ThreadStart (() => {
								DirectionThread (OnThreadStop);
						})).Start ();
        
				}else{
                    end();
                }
		}
		void Start () {
		
		new Thread (new ThreadStart (() =>
                {

                        start();
                })).Start ();
		   		new Thread (new ThreadStart (() =>
				{

						DirectionThread (OnThreadStop);
				})).Start ();
      
	}
	// Update is called once per frame
	
}
