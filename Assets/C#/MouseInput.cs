﻿using UnityEngine;
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

	//checks to see if player has item
	private bool[] hasItem; 

	public Camera mainCam;
	
	// Use this for initialization
	void Start() {
		Cursor.visible = false;
		mousedriver = new RawMouseDriver.RawMouseDriver();
		mice = new RawMouse[NUM_MICE];
		mousePosition = new Vector2[NUM_MICE];
		lastMousePosition = new Vector2[NUM_MICE];
		hasItem = new bool[NUM_MICE];
		for(int i = 0; i< NUM_MICE;i++){
			hasItem[i] = false;
		}
		
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
            if (mice[i - 1] == null) {
                break;
            }

			Vector3 look;
			Vector3 playerPos;
			//print(i);
			if (GameObject.Find("Player" + i) == null) return;
			Transform playerTrans = GameObject.Find("Player" + i).transform;
			playerPos = GameObject.Find("Player" + i).transform.position;
			//look = new Vector3(mousePosition[i-1].x, mousePosition[i-1].y, playerPos.z);
			look = new Vector3(mousePosition[i-1].x - lastMousePosition[i-1].x, mousePosition[i-1].y - lastMousePosition[i-1].y, 0);
			//look = new Vector3(mice[i-1].XDelta * Time.deltaTime, -mice[i-1].YDelta * Time.deltaTime, 0);
			Transform rectTrans = GameObject.Find("Reticle" + i).transform;
			look = rectTrans.position + look;
			look = playerTrans.TransformVector(look);
			rectTrans.position = look;
			//clamp distance
			float dist = Vector3.Distance(rectTrans.position, playerPos);
			if (dist > 2) {
				rectTrans.position += (playerPos - rectTrans.position).normalized * (dist - 2);
			}
						
			GameObject.Find("Player" + i).transform.FindChild("Center").LookAt(GameObject.Find("Reticle" + i).transform.position);
			Vector3 rotation = new Vector3(0, 0, -GameObject.Find("Player" + i).transform.FindChild("Center").localEulerAngles.x);
			GameObject.Find("Player" + i).transform.FindChild("Center").transform.localEulerAngles = rotation;
			if(GameObject.Find("Reticle" + i).transform.position.x < GameObject.Find("Player" + i).transform.position.x) {
				GameObject.Find("Player" + i).transform.FindChild("Center").transform.localEulerAngles += new Vector3(0, 180, 0);
			}
			if (hasItem[i - 1]) {
	            if ((bool) mice[i - 1].Buttons.GetValue(0)) {
	                GameObject.Find("Player" + i).transform.FindChild("Center").GetChild(0).SendMessage("click");
	            } else {
	                GameObject.Find("Player" + i).transform.FindChild("Center").GetChild(0).SendMessage("unclick");
	            }
			}

            if ((bool)mice[i - 1].Buttons.GetValue(1)) {
                GameObject.Find("Player" + i).GetComponent<GrappleLauncher>().SendMessage("fire");
            } else {
                GameObject.Find("Player" + i).GetComponent<GrappleLauncher>().SendMessage("mouseRelease");
            }


            /*print(Vector3.Distance(playerPos, look));
            if (Vector3.Distance(playerPos, look) > 10) {
                Vector3 distanceSet = (playerPos - look).normalized * 5;
                print(distanceSet);
                mousePosition[i] = new Vector2(distanceSet.x, distanceSet.y);
                look = new Vector3(mousePosition[i].x, mousePosition[i].y, playerPos.z);
            }*/

        }
	}

	public void playerHasItem(int pID){
		hasItem [pID - 1] = true;
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