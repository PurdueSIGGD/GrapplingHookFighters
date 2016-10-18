using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
    /* For control of level transition 
	 * More gamemodes to be added soon.
	 */
    public SpriteSet spriteSet;
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
	private GUIText score1, score2, score3, score4, gameOverText;
	private GUITexture scorePlayer1, scorePlayer2, scorePlayer3, scorePlayer4, scoreBoard;

	private bool transitioning, readyToStart, countDown, endScene, pointYet, gameover;
    public bool loadInstantly = false;
    private GUIText countDownText;
    private GUITexture countDownBackground;

	private bool fading; //false = fading in, true = fading out
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
		//if (loadInstantly) StartCoroutine (LoadLevelIntitial());
		//OnLevelWasLoaded(0);
	}
	public void LoadNow(int playerCount, Sprite p1Sprite, Sprite p2Sprite, Sprite p3Sprite, Sprite p4Sprite, Color p1C, Color p2C, Color p3C, Color p4C)
    {
		StartCoroutine(LoadLevelIntitial(playerCount, p1Sprite,  p2Sprite,  p3Sprite,  p4Sprite,  p1C,  p2C,  p3C,  p4C));
    }
	void Awake() {
		DontDestroyOnLoad(this.transform);
	}
	void SetTransparency(float a) {
		scoreBoard.color = new Color(scoreBoard.color.r, scoreBoard.color.g, scoreBoard.color.b, a);

		score1.color = new Color(score1.color.r, score1.color.g, score1.color.b, a);
		score2.color = new Color(score2.color.r, score2.color.g, score2.color.b, a);
		score3.color = new Color(score3.color.r, score3.color.g, score3.color.b, a);
		score4.color = new Color(score4.color.r, score4.color.g, score4.color.b, a);

		scorePlayer1.color = new Color(scorePlayer1.color.r, scorePlayer1.color.g, scorePlayer1.color.b, a);
		scorePlayer2.color = new Color(scorePlayer2.color.r, scorePlayer2.color.g, scorePlayer2.color.b, a);
		scorePlayer3.color = new Color(scorePlayer3.color.r, scorePlayer3.color.g, scorePlayer3.color.b, a);
		scorePlayer4.color = new Color(scorePlayer4.color.r, scorePlayer4.color.g, scorePlayer4.color.b, a);

		if (gameover) gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, a);
	}
	IEnumerator LoadLevelIntitial(int playerCount, Sprite p1Sprite, Sprite p2Sprite, Sprite p3Sprite, Sprite p4Sprite, Color p1C, Color p2C, Color p3C, Color p4C) {
		cameraThing = GameObject.Find("AutoZoomCamParent");
        countDownText = cameraThing.transform.parent.FindChild("CountdownText").GetComponent<GUIText>();
        countDownBackground = cameraThing.transform.parent.FindChild("CountdownBackground").GetComponent<GUITexture>();
		scoreBoard = cameraThing.transform.parent.FindChild("Scoreboard").GetComponent<GUITexture>();
		gameOverText = scoreBoard.transform.FindChild("GameOver").GetComponent<GUIText>();
		score1 = scoreBoard.transform.FindChild("Score1").FindChild("Score").GetComponent<GUIText>();
		scorePlayer1 = scoreBoard.transform.FindChild("Score1").FindChild("Icon").GetComponent<GUITexture>();
		score2 = scoreBoard.transform.FindChild("Score2").FindChild("Score").GetComponent<GUIText>();
		scorePlayer2 = scoreBoard.transform.FindChild("Score2").FindChild("Icon").GetComponent<GUITexture>();
		if (playerCount < 2) {
			score2.gameObject.SetActive(false);
			scorePlayer2.gameObject.SetActive(false);
		}
		score3 = scoreBoard.transform.FindChild("Score3").FindChild("Score").GetComponent<GUIText>();
		scorePlayer3 = scoreBoard.transform.FindChild("Score3").FindChild("Icon").GetComponent<GUITexture>();
		if (playerCount < 3) {
			score3.gameObject.SetActive(false);
			scorePlayer3.gameObject.SetActive(false);
		}
		score4 = scoreBoard.transform.FindChild("Score4").FindChild("Score").GetComponent<GUIText>();
		scorePlayer4 = scoreBoard.transform.FindChild("Score4").FindChild("Icon").GetComponent<GUITexture>();
		if (playerCount < 4) {
			score4.gameObject.SetActive(false);
			scorePlayer4.gameObject.SetActive(false);
		}



		scorePlayer1.texture = p1Sprite.texture;
		scorePlayer2.texture = p2Sprite.texture;
		scorePlayer3.texture = p3Sprite.texture;
		scorePlayer4.texture = p4Sprite.texture;

		scorePlayer1.color = new Color(p1C.r/2, p1C.g/2, p1C.b/2);
		scorePlayer2.color = new Color(p2C.r/2, p2C.g/2, p2C.b/2);
		scorePlayer3.color = new Color(p3C.r/2, p3C.g/2, p3C.b/2);
		scorePlayer4.color = new Color(p4C.r/2, p4C.g/2, p4C.b/2);



		SetTransparency(0);
        /*playerCount = 0;
		GameObject g;
		while ((g=GameObject.Find("Player" + playerCount)) != null && g.activeInHierarchy) playerCount++; */
        players = new GameObject[playerCount];
		playerScores = new int[4];
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
        SpriteSet.ApplySprites(spriteSet); //Apply sprites for this stage
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
			//if tie, add another map (from one of our previous) and continue
			int max = -1;
			bool tie = false;
			for (int i = 0; i < playerScores.Length; i++) {
				if (playerScores[i] >= max) {
					tie = false;
					if (playerScores[i] == max) {
						tie = true;
					}
					max = playerScores[i];
				} 

			}
			if (tie) {
				int[] newLevelPlan = new int[levelPlan.Length + 1];
				for (int i = 0; i < levelPlan.Length; i++) {
					print(levelPlan[i]);
					newLevelPlan[i] = levelPlan[i];
				}
				newLevelPlan[newLevelPlan.Length - 1] = levelPlan[Random.Range(0, levelPlan.Length - 1)];
				levelPlan = newLevelPlan;
			} else {
				fading = true;
				//we're done, break
				End();

				yield break;

			}



		}
		//print(currentMapQueue);
		int nextMap = levelPlan[currentMapQueue];
		int sceneIndex = nextMap;
		fading = true;
		yield return new WaitForSeconds(2);
		for (int i = 0; i < playerCount; i++) DisconnectPlayers (i);

		yield return new WaitForSeconds(1);

		SceneManager.UnloadScene (lastScene);

		yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
		lastScene = sceneIndex;
		RespawnPeeps ();
        SpriteSet.ApplySprites(spriteSet); //Apply sprites for this stage
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
		for (int i = 0; i < playerCount; i++) {
			KillPlayer(i);
			players[i].SendMessage("Reset");
		}

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
		if (!fading) {
			//false = fading in, true = fading out
			if (score1.color.a > 0) {
				SetTransparency(score1.color.a - Time.deltaTime);
			} else {
				if (score1.color.a != 0) SetTransparency(0);
			}
		} else {
			if (score1.color.a < 1) {
				SetTransparency(score1.color.a + Time.deltaTime);
			} else {
				if (score1.color.a != 1) SetTransparency(1);
			}
		}
		if (readyToStart && !countDown && startTimer <= 0) {
			
			startTimer = timeBeforeRoundStart;
			countDown = true;
            
			cameraThing.SendMessage("SetTracking",false);
		}
		if (countDown) {
			if (startTimer > 0) {
				startTimer -= Time.deltaTime;
                if (startTimer < 0) startTimer = 0;
				string newText = "" + (startTimer+1);
                if (newText.Length >= 4) newText = newText.Substring(0, 1);
                countDownText.text = newText;
                countDownBackground.gameObject.SetActive(true);
                if (startTimer > timeBeforeRoundStart/2)
                {
					fading = false;
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
						/*if (ps.playerid == 1) {
							score1.text = "Score: " + playerScores[ps.playerid - 1];
						}
						else if (ps.playerid == 2) {
							score2.text = "Score: " + playerScores[ps.playerid - 1];
						}
						else if (ps.playerid == 3) {
							score3.text = "Score: " + playerScores[ps.playerid - 1];
						}
						else if (ps.playerid == 4) {
							score4.text = "Score: " + playerScores[ps.playerid - 1];
						}*/
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

				score1.text = "Score: " + playerScores[0];
				score2.text = "Score: " + playerScores[1];
				score3.text = "Score: " + playerScores[2];
				score4.text = "Score: " + playerScores[3];

				return;

			}
		}
		if (Input.GetKeyDown(KeyCode.Backspace) && gameover)  RestartNewGame();

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
			CleanScene();
            RespawnPeeps();
            SceneManager.UnloadScene(lastScene);

            for (int i = 0; i < playerCount; i++)
            {
				players[i].transform.position = new Vector3(-5 + (3f * i>1?i+2:i), 2, 0);
                players[i].SendMessage("EnablePlayers");
            }
            GameObject.Find("MouseInput").SendMessage("EnablePlayers");
            GameObject.Destroy(myCamera.transform.parent.gameObject);
            map.SetActive(true);
			GUITexture fader = GameObject.Find("Fader").GetComponent<GUITexture>();
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, 1);

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

		g.GetComponent<GrappleLauncher> ().SendMessage ("Reset");
	}
	void RespawnPlayer(int i) {
		GameObject g = players[i];
		 
	//	print(rePos.position);
		//g.transform.parent = rePos;
		//g.GetComponent<GrappleLauncher> ().firedGrapple.transform.position = g.transform.position;  
		//g.GetComponent<GrappleLauncher> ().SendMessage ("Reset");
        g.GetComponent<GrappleLauncher>().SendMessage("NotDeath");

        g.transform.eulerAngles = Vector3.zero;
		g.GetComponent<Health>().resetPlayer();
		g.GetComponent<player>().death = false;
		g.BroadcastMessage("NotDeath");
		g.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
	void End() {
		
		//GUIText guit = this.gameObject.AddComponent<GUIText>();
		//guit.text = PrintRoundEnd();
		//guit.transform.position = new Vector3(0, 1, 0);
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
			if (g.CompareTag("Item") || g.CompareTag("DualItem")|| g.CompareTag("MapItem")  || g.CompareTag("Effect") || g.CompareTag("PlayerGibs") /* ragdoll */ ) {
				if (!g.GetComponentInChildren<GrappleScript>() && !g.GetComponentInChildren<GrappleLauncher>() && !g.GetComponentInChildren<player>() && 
					!g.GetComponent<GrappleScript>() && !g.GetComponent<GrappleLauncher>() && !g.GetComponent<player>()) {

					Destroy (g);
				}
			}
		}
	}
}
