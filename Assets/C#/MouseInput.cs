using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawInputSharp;
using System.Runtime.InteropServices;



public class MouseInput : MonoBehaviour {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	[DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
	static extern uint GetActiveWindow();	
	[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
	static extern bool SetForegroundWindow(IntPtr hWnd);
	void resetMouse() {
		SetForegroundWindow(unityWindow);
	}
#endif
	public float sensitivity;
	
	RawMouseDriver.RawMouseDriver mousedriver;
	private RawMouse[] mice;
	private Vector2[] mousePosition;
	private Vector2[] lastMousePosition;
	private const int NUM_MICE = 4;
	private Vector2 lastReticle;
	IntPtr unityWindow;

	//checks to see if player has item
	private bool[] hasItem; 
	private bool[] hasItem2; 

	public Camera mainCam;
	 
	// Use this for initialization
	void Start() { 
		lastReticle = Vector2.right;
		Cursor.visible = false;
		unityWindow = (IntPtr)GetActiveWindow();
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

		int joystickNum = 0;
		for (int i = 1; i <= mousePosition.Length; i++) {
			if (GameObject.Find("Player" + i).GetComponent<player>().joystickController) {
				GameObject.Find("Player" + i).GetComponent<player>().joystickID = ++joystickNum;
			}
		}
		
	}

	void Update() {
		if ((IntPtr)GetActiveWindow() != this.unityWindow && Time.timeSinceLevelLoad < 4) resetMouse(); //to stop getting focus stuck on the mouse starter
		if (Input.GetKey ("escape")) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;	
		}
		else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		
		// Loop through all the connected mice
		for (int i = 0; i < mice.Length; i++) {
			try {
				mousedriver.GetMouse(i, ref mice[i]);
				// Cumulative mousePositionment
				lastMousePosition[i] = mousePosition[i];
				
				mousePosition[i] = new Vector3(mice[i].X / 100.0f * sensitivity, -mice[i].Y / 100.0f * sensitivity);
				
			} catch { }
		}
		int joystickNums = 0;
		for (int i = 1; i <= mousePosition.Length; i++) {
            if (mice[i - 1] == null) {
                break;
            }
			GameObject player = GameObject.Find("Player" + i);
			GameObject reticle = GameObject.Find("Reticle" + i);
			Transform center = player.transform.FindChild("Center");
			Vector3 look;
			Vector3 playerPos;
			//print(i);
			if (player == null) return;
			Transform playerTrans = player.transform;
			playerPos = player.transform.position;
			if (player.GetComponent<player>().joystickController) {
				joystickNums++;
				look = new Vector3(Input.GetAxis("JoyX" + joystickNums), Input.GetAxis("JoyY" + joystickNums), 0);
				if (Vector2.SqrMagnitude(look) > .2f) { //continue, do not update reticle
					look = look/Vector2.SqrMagnitude(look);
					reticle.transform.localPosition = look;
					lastReticle = look;
					center.LookAt(reticle.transform.position);
					Vector3 rotation = new Vector3(0, 0, -center.localEulerAngles.x);
					center.transform.localEulerAngles = rotation;
					center.transform.localEulerAngles = rotation;
					if(reticle.transform.position.x < player.transform.position.x) {
						center.transform.localEulerAngles += new Vector3(0, 180, 0);
					}
					//center.transform.localEulerAngles = new Vector3(0,0,0);
				} else {
					reticle.transform.localPosition = lastReticle;
				}
			} else {
				look = new Vector3(mousePosition[i-1].x - lastMousePosition[i-1].x, mousePosition[i-1].y - lastMousePosition[i-1].y, 0);
				Transform rectTrans = reticle.transform;
				look = rectTrans.position + look;
				look = playerTrans.TransformVector(look);
				rectTrans.position = look;
				//clamp distance
				float dist = Vector3.Distance(rectTrans.position, playerPos);
				if (dist > 2) {
					rectTrans.position += (playerPos - rectTrans.position).normalized * (dist - 2);
				}
				center.LookAt(reticle.transform.position);
				Vector3 rotation = new Vector3(0, 0, -center.localEulerAngles.x);
				center.transform.localEulerAngles = rotation;
				if(reticle.transform.position.x < player.transform.position.x) {
					center.transform.localEulerAngles += new Vector3(0, 180, 0);
				}
			}
			//look = new Vector3(mousePosition[i-1].x, mousePosition[i-1].y, playerPos.z);
			//look = new Vector3(mousePosition[i-1].x - lastMousePosition[i-1].x, mousePosition[i-1].y - lastMousePosition[i-1].y, 0);
			//look = new Vector3(mice[i-1].XDelta * Time.deltaTime, -mice[i-1].YDelta * Time.deltaTime, 0);


			if (hasItem[i - 1] && center.childCount >= 1 && center.GetChild(0).childCount > 0 && (center.GetChild (0).GetComponent<gun>() || center.GetChild (0).GetComponent<grenade>() || center.GetChild (0).GetComponent<PortalGun>())) {
				if (player.GetComponent<player>().joystickController) {

					if (Input.GetAxis("JFire" + joystickNums) < 0) {
						center.GetChild (0).SendMessage ("click");
					} else {
						center.GetChild (0).SendMessage ("unclick");
					}
				} else {
					if ((bool)mice [i - 1].Buttons.GetValue (0)) {
						center.GetChild (0).SendMessage ("click");
					} else {
						center.GetChild (0).SendMessage ("unclick");
					}
				}
			}
			if (hasItem2[i - 1] && center.childCount > 1 && center.GetChild(1).childCount > 0 && !center.GetChild (1).GetComponent<player>()) {
				if (player.GetComponent<player>().joystickController) {
					if (Input.GetAxis("JFire" + joystickNums) > 0) {
						center.GetChild (1).SendMessage ("click");
					} else {
						center.GetChild (1).SendMessage ("unclick");
					}
				} else {
					if ((bool)mice [i - 1].Buttons.GetValue (1)) {
						center.GetChild (1).SendMessage ("click");
					} else {
						center.GetChild (1).SendMessage ("unclick");
					}
			}
				} else {
				if (player.GetComponent<player>().joystickController) {
					if (Input.GetAxis("JFire" + joystickNums) > 0) {
						player.GetComponent<GrappleLauncher> ().SendMessage ("fire");
					} else {
						player.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
					}
				} else {
					if ((bool)mice [i - 1].Buttons.GetValue (1)) {
						player.GetComponent<GrappleLauncher> ().SendMessage ("fire");
					} else {
						player.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
					}
				}
			}
            

        }
	}

	public void playerHasItem (int pID){
		hasItem [pID - 1] = true;
	}
	public void playerHasNotItem (int pID){
		hasItem [pID - 1] = false;
	}
	public void playerHasItem2 (int pID){
		hasItem2 [pID - 1] = true;
	}
	public void playerHasNotItem2( int pID){
		hasItem2 [pID - 1] = false;
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