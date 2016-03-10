using UnityEngine;
using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using RawInputSharp;
using System.Runtime.InteropServices;
using System.Collections;



public class MouseInput : MonoBehaviour {
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	//for setting the foreground window, so when opening the editor we go back to the main screen
	IntPtr unityWindow;
	private bool already;
	[DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
	static extern uint GetActiveWindow();    
	[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
	static extern bool SetForegroundWindow(IntPtr hWnd);
	void resetMouse() {
		SetForegroundWindow(unityWindow);
		already = true;
	}
	#endif
	public float sensitivity;
	private float timeSinceStart;
	RawMouseDriver.RawMouseDriver mousedriver;
	private RawMouse[] mice;
	private Vector2[] mousePosition;
	private Vector2[] lastMousePosition;
	private const int NUM_MICE = 4;
	private int currentMiceAmount;
	private Vector2[] lastReticle;

	//checks to see if player has item
	private bool[] hasItem; 
	private bool[] hasItem2; 
	//if player uses a joystick
	private ArrayList joystickPlayers;
	private ArrayList mousePlayers;
	public bool usesMouse = true, tempMoveDisable;
	public Camera mainCam;

	// Use this for initialization
	void Start() { 
		if (GameObject.FindObjectsOfType<MouseInput>().Length > 1) {
			//if this instanced and there is another open, delete the other
			Destroy(this.gameObject);
		} else {
			unityWindow = (IntPtr)GetActiveWindow();
			//    foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) print(g);
			//print(playerCount);
			lastReticle = new Vector2[4];
			for (int i = 0; i < 4; i++) {
				lastReticle[i] = Vector2.right;
			}
			if (usesMouse) {
				Cursor.visible = false;
				mousedriver = new RawMouseDriver.RawMouseDriver();
				mice = new RawMouse[NUM_MICE];
				mousePosition = new Vector2[NUM_MICE];
				lastMousePosition = new Vector2[NUM_MICE];
			}


			int numPlayers = 4;
			joystickPlayers = new ArrayList();
			mousePlayers = new ArrayList();

			int joystickNums = 1;
			int mouseNums = 0;

			for (int i = 0; i < numPlayers; i++) {
				GameObject curPlayer = GameObject.Find("Player" + (i + 1));
				if (curPlayer) {
					player playerScript = curPlayer.GetComponent<player>();
					//print(playerScript);
					if (playerScript.joystickController) {
						playerScript.joystickID = joystickNums; //this number will be decided in the selection menu, so we know what joystick they plan on using
						joystickPlayers.Add(playerScript);
						joystickNums++;
					} else {
						//print(playerScript + " " + mouseNums);
						playerScript.mouseID = mouseNums;  //this number will be decided in the selection menu, so we know what mouse they plan on using
						mousePlayers.Add(playerScript);
						mouseNums++;
						//print(playerScript + " successfully added");
					}
				}
			}
		}

	}
	void Awake() {
		//ensure it persists through stages
		DontDestroyOnLoad(this.transform);
	}
	void LateUpdate() {
		//if (Input.GetKeyDown(KeyCode.Space)) Application.LoadLevel(Application.loadedLevel);
		//print("Start update function mouse");
		if (timeSinceStart < 5) timeSinceStart += Time.deltaTime;
		//CrashDetector.SetExePoint("Whatever");
		if ((IntPtr)GetActiveWindow() != this.unityWindow && !already) resetMouse(); //to stop getting focus stuck on the mouse starter
		if (Input.GetKey ("escape")) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;    
		}
		else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		// Loop through all the connected mice
		//Debug.Log ("Start try");
		if (usesMouse) {
			for (int i = 0; i < mice.Length; i++) {
				try {
					mousedriver.GetMouse(i, ref mice[i]);
					// Cumulative mousePositionment
					lastMousePosition[i] = mousePosition[i];

					mousePosition[i] = new Vector3(mice[i].X / 100.0f * sensitivity, -mice[i].Y / 100.0f * sensitivity);

				} catch { 


				}
			}

		}
		//Debug.Log ("End try");


		Vector3 look;
		Vector3 playerPos;
		int lastReticleIndex = 0;
		//print(i);
		//foreach (player p in mousePlayers) print(p);
		if (!tempMoveDisable) {
			//print(mousePlayers.Count);
			foreach (player p in mousePlayers) {
				//print(currentMiceAmount + " " + p.mouseID);
				try {
					RawMouse r = mice[p.mouseID];
					//see if said mouse exists
				} catch {
					continue;
				}
				if (p == null || mice [p.mouseID] == null ) continue;
				playerPos = p.transform.position;
				Transform reticle = p.transform.FindChild("Reticle" + p.playerid);
				Transform center = p.transform.FindChild("Center");
				//we use mouse

				//print(mousePosition[mouseNums].x + " " + mousePosition[mouseNums].y);
				look = new Vector3(mousePosition[p.mouseID].x - lastMousePosition[p.mouseID].x, mousePosition[p.mouseID].y - lastMousePosition[p.mouseID].y, 0);
				if (Vector2.SqrMagnitude(look) > 0) { //continue

					Transform rectTrans = reticle.transform;
					look = rectTrans.position + look;
					look = p.transform.TransformVector(look);
					rectTrans.position = look;

					//clamp distance
					float dist = Vector3.Distance(rectTrans.position, playerPos);
					if (dist > 2) {
						rectTrans.position += (playerPos - rectTrans.position).normalized * (dist - 2);
					}

					lastReticle[lastReticleIndex] = reticle.position;
				} else {
					//empty, so we aren't throwing 0's around like hotcakes
				}
				bool hasItem1 = center.childCount >= 1;
				bool hasItem2 = center.childCount == 2;

				if ((bool)mice [p.mouseID].Buttons.GetValue (0)) {
					if (hasItem1) {
						center.GetChild (0).SendMessage ("click");
					} else {
						p.SendMessage("Punch");
					}

				} else {
					if (hasItem1) {
						center.GetChild (0).SendMessage ("unclick");
					}
				}


				if (hasItem2) {
					if ((bool)mice [p.mouseID].Buttons.GetValue (1)) {
						center.GetChild (1).SendMessage ("click");
					} else {
						center.GetChild (1).SendMessage ("unclick");
					}

				} else {
					if ((bool)mice [p.mouseID].Buttons.GetValue (1)) {
						p.GetComponent<GrappleLauncher> ().SendMessage ("fire");
					} else {
						p.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
					}    
				}

				lastReticleIndex++;
			} 
			foreach (player p in joystickPlayers) {
				if (p == null) break;

				Transform reticle = p.transform.FindChild("Reticle" + p.playerid);
				Transform center = p.transform.FindChild("Center");
				look = new Vector3(Input.GetAxis("JoyX" + p.joystickID), Input.GetAxis("JoyY" + p.joystickID ), 0);
				if (Vector2.SqrMagnitude(look) > .2f) { //continue, do not update reticle
					look = look/Vector2.SqrMagnitude(look);
					reticle.localPosition = look;
					lastReticle[lastReticleIndex] = look;
				} else {
					reticle.localPosition = lastReticle[lastReticleIndex];
				}


				bool hasItem1 = center.childCount >= 1;
				bool hasItem2 = center.childCount == 2;

				if (Input.GetAxisRaw("JFire" + p.joystickID) > 0) {                    
					if (hasItem1) {
						center.GetChild (0).SendMessage ("click");
					} else {
						p.SendMessage("Punch");
					}

				} else {
					if (hasItem1) {
						center.GetChild (0).SendMessage ("unclick");
					}
				}
				if (Input.GetAxisRaw("JFireAlt" + p.joystickID) > 0) {                    

					if (hasItem2) {
						center.GetChild (1).SendMessage ("click");
					} else {
						p.GetComponent<GrappleLauncher> ().SendMessage ("fire");
					}

				} else {
					if (hasItem2) {
						center.GetChild (1).SendMessage ("unclick");
					} else {
						p.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
					}

				}
				lastReticleIndex++;
			}
		}
	}

	void OnApplicationQuit() {
		// Clean up
		if (usesMouse) mousedriver.Dispose();
	}
	void DisablePlayers() {
		tempMoveDisable = true;
	}
	void EnablePlayers() {
		tempMoveDisable = false;
	}
}