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
	struct PlayerInfo {
		public bool usesJoystick;
		public int id;
	}

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
	public ArrayList players; //players with a player struct, used during SetUpRound
	private ArrayList joystickPlayers;
	private ArrayList mousePlayers;
	public bool usesMouse = true, tempMoveDisable;
	public Camera mainCam;

	bool init, levelReady, beforeRound, menuPointersOn;
	public bool singleMouse, forceStart;

	private Transform[] pointers;


	void Start() {
		players = new ArrayList();
		//init should be false now, so nothing will initialize
		if (forceStart) {
			PlayerInfo tempPlayer1 = new PlayerInfo();
			tempPlayer1.id = 0;
			tempPlayer1.usesJoystick = false;
			PlayerInfo tempPlayer2 = new PlayerInfo();
			tempPlayer2.id = 1;
			tempPlayer2.usesJoystick = true;
			players.Add(tempPlayer1);
			players.Add(tempPlayer2);
			//forcestart is when we have a level that is not selected by the main menu thingy
			Init();
			SetUpRound();
		}
	}

	/* To be called after the players array is populated
	 * Sets up player info and makes the round ready to go
	 */
	public void SetUpRound() {
		int numPlayers = 4;
		joystickPlayers = new ArrayList();
		mousePlayers = new ArrayList();
		menuPointersOn = false;
		int joystickNums = 1;
		int mouseNums = 0;
		int i = 0;
		foreach (PlayerInfo p in players) {
			GameObject curPlayer = GameObject.Find("Player" + (i + 1));
			if (curPlayer) {
				player playerScript = curPlayer.GetComponent<player>();
				playerScript.joystickController = p.usesJoystick;
				if (p.usesJoystick) {
					joystickPlayers.Add(playerScript);
					playerScript.joystickID = p.id;
					joystickNums++;
				} else {
					mousePlayers.Add(playerScript);
					playerScript.mouseID = p.id;
					mouseNums++;
				}
			
			} else {
				
			}
			i++;
		}
		levelReady = true;
	}
	/* TO be called after singleMouse is delegated
	 * 
	 */
	public void Init() { 
		if (GameObject.FindObjectsOfType<MouseInput>().Length > 1) {
			//if this instanced and there is another open, delete the other
			Destroy(this.gameObject);
		} else {
			pointers = new Transform[4];
			lastReticle = new Vector2[4];
			Transform canvas = transform.FindChild("PointerCanvas");
			canvas.gameObject.SetActive(true);
			for (int i = 0; i < 4; i++) {
				//print(transform.name);
				lastReticle[i] = Vector2.zero;
				pointers[i] = canvas.FindChild("Pointer" + (i+1));
				pointers[i].gameObject.SetActive(true);
				pointers[i].position = new Vector3(Screen.width / 2, Screen.height / 2);
			}
			mousePosition = new Vector2[NUM_MICE];
			lastMousePosition = new Vector2[NUM_MICE];
			Cursor.visible = false;
			if (!singleMouse) {
				//set up multiple mice
				unityWindow = (IntPtr)GetActiveWindow();
				//    foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) print(g);
				//print(playerCount);

				if (usesMouse) {
					mousedriver = new RawMouseDriver.RawMouseDriver();
					mice = new RawMouse[NUM_MICE];

				}
				//TODO set up individual pointers
			}
		}
		menuPointersOn = true;
		init = true;

	}
	void Awake() {
		//ensure it persists through stages
		DontDestroyOnLoad(this.transform);
	}
	void LateUpdate() {
		//we dont want to do any of this if not ready
		if (!init) return;
		if (timeSinceStart < 5) timeSinceStart += Time.deltaTime;
		//CrashDetector.SetExePoint("Whatever");
		if ((IntPtr)GetActiveWindow() != this.unityWindow && !already) resetMouse(); //to stop getting focus stuck on the mouse starter
		if (Input.GetKey ("escape") || (singleMouse && !levelReady)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;    
		}
		else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		if (!singleMouse) {
			for (int i = 0; i < mice.Length; i++) {
				try {
					mousedriver.GetMouse(i, ref mice[i]);
					// Cumulative mousePositionment
					lastMousePosition[i] = mousePosition[i];

					mousePosition[i] = new Vector3(mice[i].X / 100.0f * sensitivity, -mice[i].Y / 100.0f * sensitivity);

				} catch { 


				}
			}

		} else {
			//print(Input.GetAxis("Mouse0") + " " + Input.GetAxis("MouseX") + " " + Input.GetAxis("MouseY"));
			lastMousePosition[0] = mousePosition[0];
			mousePosition[0] =  new Vector3(Input.GetAxis("MouseX")/100f * sensitivity, Input.GetAxis("MouseY")/100f * sensitivity, 0);
		}

		if (menuPointersOn) {
			Camera c = GameObject.Find("Main Camera").GetComponent<Camera>();
			Vector3 defaultV = new Vector3(Screen.width / 2, Screen.height / 2);
			for (int i = 0; i < pointers.Length; i++) {
				
				if (this.singleMouse) {
					Vector3 oldPos = pointers[i].position;

					pointers[i].position = 500 * c.ScreenToWorldPoint(new Vector3(oldPos.x,oldPos.y, c.nearClipPlane));

				} else {
					Vector3 toGo = 100 * new Vector3(mousePosition[i].x - lastMousePosition[i].x, mousePosition[i].y - lastMousePosition[i].y  );

					pointers[i].position += 100 * new Vector3(mousePosition[i].x - lastMousePosition[i].x, mousePosition[i].y - lastMousePosition[i].y  );

				}
				if (pointers[i].position.x == defaultV.x && pointers[i].position.y == defaultV.y) {
					pointers[i].gameObject.SetActive(false);
				} else {
					pointers[i].gameObject.SetActive(true);
					if (!singleMouse) {
						if ((bool)mice [i].Buttons.GetValue (0)) {
							print("Click " + i + " at "  + pointers[i].position);
						}
					}
				}

			}

		} else {
			for (int i = 0; i < pointers.Length; i++) {
				pointers[i].gameObject.SetActive(false);
			}
		}

		if (!levelReady) return;



		Vector3 look;
		Vector3 playerPos;
		int lastReticleIndex = 0;
		//print(i);
		//foreach (player p in mousePlayers) print(p);
		if (!tempMoveDisable) {
			//print(mousePlayers.Count);
			foreach (player p in mousePlayers) {
				//print(currentMiceAmount + " " + p.mouseID);
				if (!singleMouse) {
					try {
						RawMouse r = mice[p.mouseID];
						r.ToString();
						//see if said mouse exists
					} catch {
						continue;
					}
					if (p == null || mice [p.mouseID] == null ) continue;
					look = new Vector3(mousePosition[p.mouseID].x - lastMousePosition[p.mouseID].x, mousePosition[p.mouseID].y - lastMousePosition[p.mouseID].y, 0);
				} else {
					look = new Vector3(mousePosition[0].x, mousePosition[0].y);
				}
				playerPos = p.transform.position;
				Transform reticle = p.transform.FindChild("Reticle" + p.playerid);
				Transform center =  p.transform.FindChild("AimingParent").FindChild("Center");
				//we use mouse

				//print(mousePosition[mouseNums].x + " " + mousePosition[mouseNums].y);
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
				if (!this.singleMouse) {
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
				} else {
					//for when we have a single mouse
					if (Input.GetAxis("Mouse0") > 0) {
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
						if (Input.GetAxis("Mouse1") > 0) {
							center.GetChild (1).SendMessage ("click");
						} else {
							center.GetChild (1).SendMessage ("unclick");
						}

					} else {
						if (Input.GetAxis("Mouse1") > 0) {
							p.GetComponent<GrappleLauncher> ().SendMessage ("fire");
						} else {
							p.GetComponent<GrappleLauncher> ().SendMessage ("mouseRelease");
						}    
					}


				}

				lastReticleIndex++;
			} 
			foreach (player p in joystickPlayers) {
				if (p == null) break;

				Transform reticle = p.transform.FindChild("Reticle" + p.playerid);
				Transform center = p.transform.FindChild("AimingParent").FindChild("Center");
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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;  
		if (!this.singleMouse) mousedriver.Dispose();
	}
	void DisablePlayers() {
		tempMoveDisable = true;
	}
	void EnablePlayers() {
		tempMoveDisable = false;
	}

}