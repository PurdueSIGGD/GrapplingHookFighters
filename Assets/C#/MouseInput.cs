using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawInputSharp;

public class MouseInput : MonoBehaviour {
	
	public float sensitivity;
	
	RawMouseDriver.RawMouseDriver mousedriver;
	private RawMouse[] mice;
	private Vector2[] mousePosition;
	private Vector2[] lastMousePosition;
	private const int NUM_MICE = 4;
	
	// Use this for initialization
	void Start() {
		Cursor.visible = false;
		mousedriver = new RawMouseDriver.RawMouseDriver();
		mice = new RawMouse[NUM_MICE];
		mousePosition = new Vector2[NUM_MICE];
		lastMousePosition = new Vector2[NUM_MICE];
		
	}
	
	void Update() {
		
		if (Input.GetKey ("escape"))
			Screen.lockCursor = false;
		else Screen.lockCursor = true;
		
		// Loop through all the connected mice
		for (int i = 0; i < mice.Length; i++) {
			try {
				mousedriver.GetMouse(i, ref mice[i]);
				// Cumulative mousePositionment
				lastMousePosition[i] = mousePosition[i];
				
				mousePosition[i] = new Vector3(mice[i].X / 100.0f * sensitivity, -mice[i].Y / 100.0f * sensitivity);
				
			} catch { }
		}
		for (int i = 1; i <= mousePosition.Length; i++) {
			Vector3 look;
			Vector3 playerPos;
			//print(i);
			if (GameObject.Find("Player" + i) == null) return;
			playerPos = GameObject.Find("Player" + i).transform.position;
			//look = new Vector3(mousePosition[i-1].x, mousePosition[i-1].y, playerPos.z);
			look = new Vector3(mousePosition[i-1].x - lastMousePosition[i-1].x, mousePosition[i-1].y - lastMousePosition[i-1].y, 0);
			GameObject.Find("Reticle" + i).transform.position += look;
			
			//print(mousePosition[i-1].x + ", " + mousePosition[i-1].y);
			
			GameObject.Find("Player" + i).transform.FindChild("Center").LookAt(GameObject.Find("Reticle" + i).transform.position);
			Vector3 rotation = new Vector3(0, 0, -GameObject.Find("Player" + i).transform.FindChild("Center").localEulerAngles.x);
			GameObject.Find("Player" + i).transform.FindChild("Center").transform.localEulerAngles = rotation;
			if(GameObject.Find("Reticle" + i).transform.position.x < GameObject.Find("Player" + i).transform.position.x) {
				GameObject.Find("Player" + i).transform.FindChild("Center").transform.localEulerAngles += new Vector3(0, 180, 0);
			}
			
			//print("Mouse" + i + " Clicked: " + mice[i - 1].Buttons.GetValue(0));    //left click boolean
			
			/*print(Vector3.Distance(playerPos, look));
            if (Vector3.Distance(playerPos, look) > 10) {
                Vector3 distanceSet = (playerPos - look).normalized * 5;
                print(distanceSet);
                mousePosition[i] = new Vector2(distanceSet.x, distanceSet.y);
                look = new Vector3(mousePosition[i].x, mousePosition[i].y, playerPos.z);
            }*/
			
		}
	}
	
	void OnGUI() {
		GUILayout.Label("Connected:");
		for (int i = 0; i < mice.Length; i++) {
			if (mice[i] != null)
				GUILayout.Label("");
		}
	}
	
	void OnApplicationQuit() {
		// Clean up
		mousedriver.Dispose();
	}
}