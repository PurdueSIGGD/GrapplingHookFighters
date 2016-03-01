using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
	/* For control of level transition 
	 * More gamemodes to be added soon.
	 */

	public int playerCount;
	public int[] levelPlan, playerScores;
	//public Scene[] levels;
	private int lastScene;
	public float distanceInBetween = 30;
	public int gameMode; //defaults to 0, which will be FFA

	private GameObject[] players;
	private Vector2[] mapPlacements;

	private int currentMapIndex, currentMapQueue;
	private int deathCount;
	private float endTimer, startTimer;
	private GameObject currentMap, lastMap;
	private bool transitioning, readyToStart, countDown, endScene, pointYet;
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
		StartCoroutine (LoadLevelIntitial());
		//OnLevelWasLoaded(0);
	}

	void Awake() {
		DontDestroyOnLoad(this.transform);
	}

	IEnumerator LoadLevelIntitial() {
		players = new GameObject[playerCount];
		playerScores = new int[playerCount];
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
		//currentMap = (GameObject)GameObject.Instantiate(levels[levelPlan[0]], Vector3.zero, Quaternion.identity);
		int sceneIndex = levelPlan[0];
		yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
		lastScene = sceneIndex;
		//Spawn players
		//Scene loadedScene = SceneManager.GetSceneAt(lastScene-1);
		//GameObject sceneRoot = loadedScene.GetRootGameObjects()[0];
		/*for (int i = 0; i < playerCount - 1; i++) {
			players[i].transform.position = GameObject.Find("Player" + (i+1) + "Spawn").transform.position;
		}*/
		RespawnPeeps ();
		readyToStart = true;
		/*for (int i = 0; i < playerCount; i++) {
			//Every prefab (scene) is going to need something like Player1Spawn, except for all players.
			GameObject.Instantiate(players[i], GameObject.Find("Player" + (i+1) + "Spawn").transform.position, Quaternion.identity); 
		}*/
		//print("stuff");
		//yield break;
	}

	IEnumerator StartNewScene(Vector2 nextPosition) {
		
		//Assign last map
		//lastMap = currentMap;
		//Create new map
		currentMapQueue++;
		if (currentMapQueue == levelPlan.Length) {
			//we're done, break
			End();
			yield break;
		}
		//print(currentMapQueue);
		int nextMap = levelPlan[currentMapQueue];
		int sceneIndex = nextMap;
		SceneManager.UnloadScene (lastScene);
		CleanScene ();
		yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
		lastScene = sceneIndex;
		RespawnPeeps ();
		//currentMap = (GameObject)GameObject.Instantiate(levels[nextMap], nextPosition, Quaternion.identity);
		//Tell camera to move towards another place, will call RespawnPeeps when halfway to next location
		//GameObject.Find("AutoZoomCamParent").SendMessage("SetNewPlace", nextPosition);
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
			startTimer = 3;
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
				//GameObject.Find("AutoZoomCamParent").SendMessage("StopNewPlace");
				countDown = false;
				transitioning = false;
				readyToStart = false;
			}

		}
		if (IsRoundOver()) {
			if (!endScene && !transitioning) {
				endScene = true;
				endTimer = 3;
			}
			endTimer -= Time.deltaTime;
			//print("done here");
			if (endTimer < 1.5f && !transitioning && !pointYet) {
				foreach (GameObject g in players) {
					player ps = g.GetComponent<player>();
					if (!ps.death) {
						print("Counting one point for Player " + ps.playerid);
						playerScores[ps.playerid - 1] += 1;
					}
				}
				//print("Counting one point for Player ");
				pointYet = true;
			}
			if (endTimer <= 0 && !transitioning) {
				//print();
				pointYet = false;
				transitioning = true;
				endScene = false;
				endTimer = 0;
				int newIndex = Random.Range(0, 7);
				if (newIndex >= currentMapIndex) newIndex++; // so we don't go to the same place as currently
				StartCoroutine(StartNewScene(mapPlacements[newIndex]));
				currentMapIndex = newIndex;
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
		
		GUIText guit = this.gameObject.AddComponent<GUIText>();
		guit.text = PrintRoundEnd();
		guit.transform.position = new Vector3(0, 1, 0);
	}
	bool IsRoundOver() {
		switch (gameMode) {
			case 0: 
				return deathCount >= playerCount - 1;
			default: return false;
		}
	}
	string PrintRoundEnd() {
		switch (gameMode) {
		case 0: 
			string s = "Player Scores:\n";
			for (int i = 0; i < playerCount; i++) {
				s += ("Player " + (i + 1) + ": " + playerScores[i] + "\n");
			}
			return s;
		default: return "";
		}
	}

	void CleanScene() {
		Scene scene = SceneManager.GetActiveScene ();
		List<GameObject> roots = new List<GameObject> (scene.rootCount + 1);
		scene.GetRootGameObjects (roots);
		foreach (GameObject g in roots) {
			if (g.GetComponent<HeldItem> ()) {
				Destroy (g);
			}
		}
	}
}
