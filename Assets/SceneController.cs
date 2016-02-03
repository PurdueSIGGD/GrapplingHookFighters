using UnityEngine;
using System.Collections;
using UnityEditor;

public class SceneController : MonoBehaviour {
	/* For control of level transition 
	 * More gamemodes to be added soon.
	 */

	public int playerCount;
	public int[] levelPlan;
	public GameObject[] levels;
	public float distanceInBetween = 30;

	//public int gameMode; //defaults to 0, which will be FFA
	public GameObject[] players;
	private Vector2[] mapPlacements;
	private int currentMapIndex, currentMapQueue;
	private int deathCount;
	public float endTimer, startTimer;
	public GameObject currentMap, lastMap;
	public bool transitioning, readyToStart, countDown, endScene;
	void Start() {
		mapPlacements = new Vector2[9];
		int index = 0;
		for (int y = -1; y <= 1; y++) {
			for (int x = -1; x <= 1; x++) {
				//populates array with a matrix, each going from the max ends to the min ends
				mapPlacements[index] = distanceInBetween * (new Vector2(x, y));
				index++;
			}
		}
		OnLevelWasLoaded(0);
	}

	void Awake() {
		DontDestroyOnLoad(this.transform);
	}

	void OnLevelWasLoaded(int level) {
		players = new GameObject[playerCount];
		for (int i = 1; i <= playerCount; i++) {
			players[i-1] = GameObject.Find("Player" + i);
		}
		for (int i = 0; i < playerCount; i++) {
			players[i].SendMessage("DisablePlayers");
		}
		GameObject.Find("MouseInput").SendMessage("DisablePlayers");
		currentMapIndex = 4; // 0  1  2     We are at 4, the center
		                     // 3  4  5
		                     // 6  7  8
		//Spawn map
		currentMap = (GameObject)GameObject.Instantiate(levels[levelPlan[0]], Vector3.zero, Quaternion.identity);
		//Spawn players
		for (int i = 0; i < playerCount; i++) {
			players[i].transform.position = GameObject.Find("Player" + (i+1) + "Spawn").transform.position;
		}
		readyToStart = true;
		/*for (int i = 0; i < playerCount; i++) {
			//Every prefab (scene) is going to need something like Player1Spawn, except for all players.
			GameObject.Instantiate(players[i], GameObject.Find("Player" + (i+1) + "Spawn").transform.position, Quaternion.identity); 
		}*/
		//print("stuff");
	}
	void StartNewScene(Vector2 nextPosition) {
		
		//Assign last map
		lastMap = currentMap;
		//Create new map
		currentMapQueue++;
		if (currentMapQueue == levelPlan.Length) {
			//we're done, break
			End();
			return;
		}
		//print(currentMapQueue);
		int nextMap = levelPlan[currentMapQueue];
		currentMap = (GameObject)GameObject.Instantiate(levels[nextMap], nextPosition, Quaternion.identity);
		//Tell camera to move towards another place, will call RespawnPeeps when halfway to next location
		GameObject.Find("AutoZoomCamParent").SendMessage("SetNewPlace", nextPosition);
		//EditorApplication.isPaused = true;


	}
	void RespawnPeeps() {
		//halfway there, called from camera
		//delete last map
	//	print(players[0].transform.position);
		//EditorApplication.isPaused = true;

		GameObject.DestroyImmediate(lastMap);
	//	print(players[0].transform.position);
		//EditorApplication.isPaused = true;
		//Kill all players
		for (int i = 0; i < playerCount; i++) KillPlayer(i);
		//respawn players
		for (int i = 0; i < playerCount; i++) RespawnPlayer(i);
		//set player positions
		for (int i = 0; i < playerCount; i++) {
			players[i].transform.position = GameObject.Find("Player" + (i+1) + "Spawn").transform.position;
		}
		//set inputmanagerthing to disbale movement until we get an "all clear"
		for (int i = 0; i < playerCount; i++) {
			players[i].SendMessage("DisablePlayers");
		}
		GameObject.Find("MouseInput").SendMessage("DisablePlayers");		
		//Set deathcount to 0
		deathCount = 0;
		//Countdown starts here, 5 seconds
		readyToStart = true;
		transitioning = false;
	}

	void Update () {
		if (readyToStart && !countDown && startTimer <= 0) {
			startTimer = 5;
			countDown = true;
		}
		if (countDown) {
			if (startTimer > 0) {
				startTimer -= Time.deltaTime;
			} else {
				//Begin!!!
				for (int i = 0; i < playerCount; i++) {
					players[i].SendMessage("EnablePlayers");
				}
				GameObject.Find("MouseInput").SendMessage("EnablePlayers");							
				GameObject.Find("AutoZoomCamParent").SendMessage("StopNewPlace");
				countDown = false;
				transitioning = false;
				readyToStart = false;
			}

		}
		if (deathCount >= playerCount - 1) {
			if (!endScene && !transitioning) {
				endScene = true;
				endTimer = 3;
			}
			endTimer -= Time.deltaTime;
			//print("done here");
			if (endTimer <= 0 && !transitioning) {
				transitioning = true;
				endScene = false;
				endTimer = 0;
				int newIndex = Random.Range(0, 7);
				if (newIndex >= currentMapIndex) newIndex++; // so we don't go to the same place as currently
				StartNewScene(mapPlacements[newIndex]);
				return;

			}
		}

	}
	void AddDeath() {
		deathCount++;
	}
	void KillPlayer(int i) {
		GameObject g = players[i];
		g.SendMessage("killPlayer");
	}

	void RespawnPlayer(int i) {
		GameObject g = players[i];

		Transform rePos = GameObject.Find("Player" + (i+1) + "Spawn").transform;
	//	print(rePos.position);
		//g.transform.parent = rePos;
		g.transform.position = rePos.position;
		g.GetComponent<GrappleLauncher>().firedGrapple.transform.position = rePos.position;
		g.GetComponent<GrappleLauncher>().SendMessage("Disconnect");
		g.transform.eulerAngles = Vector3.zero;
		g.GetComponent<Health>().resetPlayer();
		g.GetComponent<player>().death = false;
		g.BroadcastMessage("NotDeath");
		g.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
	void End() {
		print("Its overrr");
	}
}
