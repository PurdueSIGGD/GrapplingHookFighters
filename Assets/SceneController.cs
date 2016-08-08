using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
    /* For control of level transition 
	 * More gamemodes to be added soon.
	 */
    public GameObject map;
	public int playerCount;
	public int[] levelPlan, playerScores;
	//public Scene[] levels;
	private int lastScene;
	//public float distanceInBetween = 30;
	public int gameMode; //defaults to 0, which will be FFA
	private GameObject myCamera;
	private GameObject[] players;
	private Vector2[] mapPlacements;
	public float timeBeforeRoundEnd = 4f;
	public float timeForPointsAwarded = 2.5f;
    public float timeBeforeRoundStart = 3f;

    private int currentMapIndex, currentMapQueue;
	private int deathCount;
	private float endTimer, startTimer;
	private GameObject currentMap, lastMap, cameraThing;
	private bool transitioning, readyToStart, countDown, endScene, pointYet, gameover;
    public bool loadInstantly = false;
    private GUIText countDownText;
    private GUITexture countDownBackground;
	void Start() {
		//int index = 0;
		/*mapPlacements = new Vector2[9];
		int index = 0;
		for (int y = -1; y <= 1; y++) {
			for (int x = -1; x <= 1; x++) {
				//populates array with a matrix, each going from the max ends to the min ends
				mapPlacements[index] = distanceInBetween * (new Vector2(x, y));
				index++;
			}
		}*/
		if (loadInstantly) StartCoroutine (LoadLevelIntitial());
		//OnLevelWasLoaded(0);
	}
    public void LoadNow()
    {
        StartCoroutine(LoadLevelIntitial());
    }
	void Awake() {
		DontDestroyOnLoad(this.transform);
	}

	IEnumerator LoadLevelIntitial() {
		cameraThing = GameObject.Find("AutoZoomCamParent");
        countDownText = cameraThing.transform.parent.FindChild("CountdownText").GetComponent<GUIText>();
        countDownBackground = cameraThing.transform.parent.FindChild("CountdownBackground").GetComponent<GUITexture>();
        /*playerCount = 0;
		GameObject g;
		while ((g=GameObject.Find("Player" + playerCount)) != null && g.activeInHierarchy) playerCount++; */
        players = new GameObject[playerCount];
		playerScores = new int[playerCount];
		for (int i = 1; i <= playerCount; i++) {
			GameObject gg = GameObject.Find("Player" + i);
			if (gg.activeInHierarchy)
				players[i-1] = gg;
		}





		if (loadInstantly) for (int i = playerCount + 1; i <= 4; i++) GameObject.Find("Player" + i).transform.parent.gameObject.SetActive(false);
		for (int i = 0; i < playerCount; i++) {
			players[i].SendMessage("DisablePlayers");
		}
		GameObject.Find("MouseInput").SendMessage("DisablePlayers");
		currentMapIndex = 4; // 0  1  2     We are at 4, the center
		                     // 3  4  5
		                     // 6  7  8
		//Spawn map 
		int sceneIndex = levelPlan[0];
		yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
		lastScene = sceneIndex;

		if (!myCamera) myCamera = GameObject.Find ("AutoZoomCamParent");
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

	IEnumerator StartNewScene() {



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
		for (int i = 0; i < playerCount; i++) DisconnectPlayers (i);
		SceneManager.UnloadScene (lastScene);

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


	//	print(players[0].transform.position);
		//EditorApplication.isPaused = true;
		//Kill all players
		for (int i = 0; i < playerCount; i++) KillPlayer(i);
		//clean map, all leftoveres 
		//Debug.Log("is it cleanup?");
		CleanScene ();
		//Debug.Log ("it is not cleanup");
		//GameObject.DestroyImmediate(lastMap); //no longer needed?
		GameObject.Find("MouseInput").SendMessage("DisablePlayers");
		//set player positions
		ArrayList spawns = new ArrayList();
		spawns.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));
		for (int i = 0; i < playerCount; i++) {
			int spawnIndex = Random.Range(0,spawns.Count);
			players[i].transform.position = ((GameObject)spawns[spawnIndex]).transform.position;
			spawns.Remove(spawns[spawnIndex]);
			players[i].GetComponent<Rigidbody2D>().isKinematic = true;
			players[i].GetComponent<Rigidbody2D>().isKinematic = false;

		}
		//respawn players
		for (int i = 0; i < playerCount; i++) RespawnPlayer(i);

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
			
			startTimer = timeBeforeRoundStart;
			countDown = true;
            
			cameraThing.SendMessage("SetTracking",false);
		}
		if (countDown) {
			if (startTimer > 0) {
				startTimer -= Time.deltaTime;
                if (startTimer < 0) startTimer = 0;
                string newText = "" + startTimer;
                if (newText.Length >= 4) newText = newText.Substring(0, 4);
                countDownText.text = newText;
                countDownBackground.gameObject.SetActive(true);
                if (startTimer > timeBeforeRoundStart/2)
                {
                    
                    //hide scoreboard fade out
                }
                else if (startTimer > (3*timeBeforeRoundStart) / 4) {
					//not tracking, get a glimpse of the map here
					cameraThing.SendMessage("SetTracking",false);

				} else {
					cameraThing.SendMessage("SetTracking",true);
                }
            } else {
                //Begin!!!
                countDownText.text = "";
                countDownBackground.gameObject.SetActive(false);

                for (int i = 0; i < playerCount; i++) {
					players[i].SendMessage("EnablePlayers");
				}
				GameObject.Find("MouseInput").SendMessage("EnablePlayers");			
				countDown = false;
				transitioning = false;
				readyToStart = false;
			}

		}
		if (IsRoundOver()) {
			if (!endScene && !transitioning) {
				endScene = true;
				endTimer = timeBeforeRoundEnd;
			}
			endTimer -= Time.deltaTime;
			//print("done here");
			if (endTimer < timeBeforeRoundEnd-timeForPointsAwarded && !transitioning && !pointYet) {
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
            if (endTimer < (timeBeforeRoundEnd-timeForPointsAwarded)/2 && !transitioning)
            {
                //show scoreboard fade in
            }
            if (endTimer <= 0 && !transitioning) {
				//print();
				pointYet = false;
				transitioning = true;
				endScene = false;
				endTimer = 0;
				int newIndex = Random.Range(0, 7);
				if (newIndex >= currentMapIndex) newIndex++; // so we don't go to the same place as currently
				StartCoroutine(StartNewScene());
				currentMapIndex = newIndex;
				return;

			}
		}
		if (Input.GetKeyDown(KeyCode.Backspace) && gameover) RestartNewGame();

	}
	void RestartNewGame() {
        if (loadInstantly)
        {
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject o in objects)
            {
                GameObject.Destroy(o.gameObject);
            }
            SceneManager.LoadScene(0);
        } else
        {
            RespawnPeeps();
            SceneManager.UnloadScene(lastScene);

            for (int i = 0; i < playerCount; i++)
            {
                players[i].transform.position = new Vector3(-7 + (2.5f * i), 2, 0);
                players[i].SendMessage("EnablePlayers");
            }
            GameObject.Find("MouseInput").SendMessage("EnablePlayers");
            GameObject.Destroy(myCamera.transform.parent.gameObject);
            map.SetActive(true);
           
            GameObject.Destroy(this.gameObject);
        }

    }
	void AddDeath() {
		deathCount++;
	}
	void KillPlayer(int i) {
		GameObject g = players[i];
		g.SendMessage("killPlayer");
	}
	void DisconnectPlayers(int i) {
		GameObject g = players[i];
		g.GetComponent<GrappleLauncher> ().SendMessage ("Disconnect");
	}
	void RespawnPlayer(int i) {
		GameObject g = players[i];
		 
	//	print(rePos.position);
		//g.transform.parent = rePos;
		g.GetComponent<GrappleLauncher> ().firedGrapple.transform.position = g.transform.position;
		g.GetComponent<GrappleLauncher> ().SendMessage ("Disconnect");
        g.GetComponent<GrappleLauncher>().SendMessage("NotDeath");

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
        gameover = true;
	}
	bool IsRoundOver() {
		switch (gameMode) {
			case 0: 
				return deathCount >= playerCount - 1;
			default: return false;
		}
	}
	string PrintRoundEnd() {
        string s = "";
		switch (gameMode) {
		case 0: 
			s = "Player Scores:\n";
			for (int i = 0; i < playerCount; i++) {
				s += ("Player " + (i + 1) + ": " + playerScores[i] + "\n");
			}
                break;
           
		default: s += "";
                break;
		}
        return s + "\nPress Backspace to return to main menu";
    }

	void CleanScene() {
		myCamera.GetComponent<SmootherTrackingCamera> ().ResetCamera ();
		Scene scene = SceneManager.GetActiveScene ();
		List<GameObject> roots = new List<GameObject> (scene.rootCount + 1);
		scene.GetRootGameObjects (roots);
		foreach (GameObject g in roots) {
			if (g.CompareTag("Item") || g.CompareTag("DualItem") || g.CompareTag("Effect") || g.CompareTag("PlayerGibs") /* ragdoll */) {
				if (!g.GetComponentInChildren<GrappleScript>() && !g.GetComponentInChildren<GrappleLauncher>() && !g.GetComponentInChildren<player>() && 
					!g.GetComponent<GrappleScript>() && !g.GetComponent<GrappleLauncher>() && !g.GetComponent<player>()) {

					Destroy (g);
				}
			}
		}
	}
}
