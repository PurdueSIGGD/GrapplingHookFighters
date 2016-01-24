using UnityEngine;
using System.Collections;

public class MapSettings : MonoBehaviour {
	/* For settings including stock, life, and map time limits
	 * Also includes spawning of weapons and such
	 * 
	 */
	public float spawnRate = 1, currentSpawnTime;
	public GameObject[] weapons;
	public bool[] canWeaponsSpawn;
	private Transform[] spawnPoints;
	private bool inLevel;
	private int spawnPointsSize;
	void Start () {
		spawnPoints = new Transform[20]; //up to 20 places to spawn
	}
	void OnLevelWasLoaded(int level) {
		if (level > 0) { //if the level is not level 0, or the main menu
			inLevel = true;
			int index = 0;
			while (true) {
				if (GameObject.Find("Spawnpoint" + (index+1))) {
					spawnPoints[index] = GameObject.Find("Spawnpoint" + index).transform;
					index++;
				} else {
					break;
				}
			}
			spawnPointsSize = index;

		} else {
			inLevel = false;
			spawnRate = 1;
			spawnPoints = new Transform[20];
		}
		int usedWeapons = 0;
		for (int i = 0; i < canWeaponsSpawn.Length; i++) {
			if (canWeaponsSpawn[i]) usedWeapons++;
		}
		spawnRate *= (spawnRate / usedWeapons); //so if we only have one wepon, spawnRate is multipled by itself. If we have a bunch, spawnRate barely changes. 
	}
	void Awake() {
		DontDestroyOnLoad(this.transform);
	}
	// Update is called once per frame
	void Update () {
		if (inLevel) {
			if (currentSpawnTime <= 0) {
				//we can spawn a weapon now
				int pointIndex = Random.Range(0, spawnPointsSize); //which point we want to spawn at
				int weaponIndex = Random.Range(0, weapons.Length); //what weapon we will use, however it may be disabled
				GameObject.Instantiate(weapons[weaponIndex],spawnPoints[pointIndex].position, Quaternion.identity);

				//reset currentSpawnTime
				currentSpawnTime = Random.Range(1/spawnRate, 20/spawnRate);
			} else {
				//waiting for timer to go down
				currentSpawnTime -= Time.deltaTime; 
			}
		}
	}
}
