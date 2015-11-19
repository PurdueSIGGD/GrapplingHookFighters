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

	//checks to see if player has item
	private bool[] hasItem, hasItem2; 
	
	// Use this for initialization
	void Start() {
		Cursor.visible = false;
		mousedriver = new RawMouseDriver.RawMouseDriver();
		mice = new RawMouse[NUM_MICE];
		mousePosition = new Vector2[NUM_MICE];
		lastMousePosition = new Vector2[NUM_MICE];
		hasItem = new bool[NUM_MICE];
		hasItem2 = new bool[NUM_MICE];
		for(int i = 0; i< NUM_MICE;i++){
			hasItem[i] = false;
			hasItem2[i] = false;
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
			if (GameObject.Find ("Player" + i) != null) {
				if (mice [i - 1] == null || GameObject.Find ("Player" + i).GetComponent<Health> ().dead) {
					GameObject.Find ("Reticle" + i).GetComponent<SpriteRenderer> ().enabled = false;
					break;
				}

				Vector3 look;
				Vector3 playerPos;
				//print(i);
				Transform Reticle = GameObject.Find ("Reticle" + i).transform;
				Transform Player = GameObject.Find ("Player" + i).transform;
				if (GameObject.Find ("Player" + i) == null)
					return;
				playerPos = Player.transform.position;
				//look = new Vector3(mousePosition[i-1].x, mousePosition[i-1].y, playerPos.z);
				look = new Vector3 (mousePosition [i - 1].x - lastMousePosition [i - 1].x, mousePosition [i - 1].y - lastMousePosition [i - 1].y, 0);
				Reticle.position += look;
			
				//print(mousePosition[i-1].x + ", " + mousePosition[i-1].y);
				Transform Center = Player.FindChild("Center");
				Center.LookAt (Reticle.position);
				Vector3 rotation = new Vector3 (0, 0, -Center.localEulerAngles.x);
				Center.localEulerAngles = rotation;
				if (Reticle.position.x < Player.position.x) {
					Center.localEulerAngles += new Vector3 (0, 180, 0);
				}
				if (hasItem[i - 1] && Center.GetChild(0).childCount > 0 && (Center.GetChild (0).GetComponent<gun>() || Center.GetChild (0).GetComponent<PortalGun>() || Center.GetChild (0).GetComponent<grenade>())) {
                    if (Player.GetComponent<player>().joystickController) {
						if (Input.GetAxis("JFire" + i) > 0) {
							Center.GetChild (0).SendMessage ("click");
						} else {
							Center.GetChild (0).SendMessage ("unclick");
						}
					} else {
						if ((bool)mice [i - 1].Buttons.GetValue (0)) {
							Center.GetChild (0).SendMessage ("click");
						} else {
							Center.GetChild (0).SendMessage ("unclick");
						}
					}
				}
				if (hasItem2[i - 1] && Center.childCount > 1 && Center.GetChild(1).childCount > 0 && !Center.GetChild (1).GetComponent<player>()) {
					if (Player.GetComponent<player>().joystickController) {
						if (Input.GetAxis("AltJFire" + i) > 0) {
							Center.GetChild (1).SendMessage ("click");
						} else {
							Center.GetChild (1).SendMessage ("unclick");
						}
					} else {
						if ((bool)mice [i - 1].Buttons.GetValue (1)) {
							Center.GetChild (1).SendMessage ("click");
						} else {
							Center.GetChild (1).SendMessage ("unclick");
						}
					}
				} else {
					if (Player.GetComponent<player>().joystickController) {
						if (Input.GetAxis("AltJFire" + i) > 0) {
							Player.GetComponent<GrappleLauncher> ().SendMessage ("fire");
						} else {
							Player.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
						}
					} else {
						if ((bool)mice [i - 1].Buttons.GetValue (1)) {
							Player.GetComponent<GrappleLauncher> ().SendMessage ("fire");
						} else {
							Player.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
						}
					}
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
	}

	public void playerHasItem(int pID){
		hasItem [pID - 1] = true;
	}
	public void playerHasNotItem(int pID){
		hasItem [pID - 1] = false;
	}
	public void playerHasItem2(int pID){
		hasItem2 [pID - 1] = true;
	}
	public void playerHasNotItem2(int pID){
		hasItem2 [pID - 1] = false;
	}
	
	void OnApplicationQuit() {
		// Clean up
		mousedriver.Dispose();
	}
}