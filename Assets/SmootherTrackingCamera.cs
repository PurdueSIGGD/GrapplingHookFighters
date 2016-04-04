using UnityEngine;
using System.Collections;

public class SmootherTrackingCamera : MonoBehaviour {
	//how fast the camera should be moving
	public float movementSpeed = .2f;
	//how close you want the camera to zoom into (typically one player)
	public float minZoom = .8f;
	//maximum possible zoom you will allow the camera
	public float maxZoom = 20;
	//how far players can be without being ignored by the camera. Careful, players may take themselves as dead but not be.
	//public float ignoreDistance = 40;
	private bool atLeastOneAlive;
	public bool tracking;

	private Vector3 lastDistance;
	private int playerCount;
	private GameObject[] targets;
	// Use this for initialization
	void Start () {
		lastDistance = Vector3.zero;
		tracking = true;
		playerCount = 0;
		while (GameObject.Find("Player" + (playerCount + 1))) {
			playerCount++;
		}
		targets = new GameObject[playerCount];
		for (int i = 1; i <= playerCount; i++) {
			targets[i-1] = GameObject.Find("Player" + i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 desiredPosition = Vector3.zero;
		int trackingPlayers = 0;
		float furthestDistance = 0;
		bool atLeastOneAlive = false;
		Health gh;

		foreach (GameObject g in targets) {
			gh = g.GetComponent<Health> ();

			//if they crossed a boundary, use their last place they were before they died 
			float thisDistance = Vector2.Distance (transform.position, (gh.ignorePosition)?gh.boundaryPlace:g.transform.position);
			if (!gh.dead)
				atLeastOneAlive = true;
			//calculate furthest distance
			//print(atLeastOneAlive);
			bool b = true;
			if (gh.deadTime > 2.5f) {
				//print("moving");
				//if the player did not pass a boundary, ignore their position and use the new one
				if (!gh.ignorePosition) {
					//if it will cause problems and is the furthest, we will adjust it
					gh.boundaryPlace = gh.transform.position;
					gh.ignorePosition = true;
				}
				//this is to move the camrea back to the other positions afterwards, once we realized the player really died
				if (thisDistance > furthestDistance) {
					gh.boundaryPlace += 3 * Time.deltaTime * (transform.position - gh.boundaryPlace);
					gh.boundaryPlace += lastDistance; //to prevent the camera outrunning the boundaryPlace
				} else {
					//print("Ignoring forever " + g.name);
					gh.boundaryPlace = transform.position;
					//just ignore forever
					b = false;
					//trackingPlayers--;
					//desiredPosition -= gh.boundaryPlace;
				}
			}
			//if they crossed a boundary, use their last place they were before they died 
			if (b) {
				desiredPosition += (gh.ignorePosition)?gh.boundaryPlace:g.transform.position;
				trackingPlayers++;
			}
			//calculate furthest player
			if (thisDistance > furthestDistance) {
				furthestDistance = thisDistance;
			}

		}
		//print(tempAtLeastOneAlive);
		//atLeastOneAlive = tempAtLeastOneAlive;
		if (!tracking) {
			desiredPosition = Vector3.zero;
		} else if (trackingPlayers > 0 && atLeastOneAlive) {
			//average
			desiredPosition = desiredPosition / trackingPlayers;
		} else {
			//does not change
			desiredPosition = transform.position;
		}
		float cameraDistance = Vector2.Distance (desiredPosition, transform.position);
		if (cameraDistance > 0) {
			lastDistance = Time.deltaTime * movementSpeed * (cameraDistance + 3) * (desiredPosition - transform.position);
			transform.position += lastDistance;
		}
		
		//print (furthestDistance);
		if (furthestDistance > 0) {
			if (furthestDistance < minZoom)
				furthestDistance = minZoom;
			if (furthestDistance > maxZoom)
				furthestDistance = maxZoom;
			//add a value of 1 to give some buffer room
			float desiredSize = furthestDistance + 2;
			if (atLeastOneAlive) {
				if (trackingPlayers != 1) {
					GetComponentInChildren<Camera> ().orthographicSize = desiredSize;
				} else {
					//idk, do something less jerky
					if (desiredSize < GetComponentInChildren<Camera> ().orthographicSize) 
						GetComponentInChildren<Camera> ().orthographicSize = desiredSize;
					//if zoomed into one player, it tries to zoom out if they get too close.
					//this just won't let it zoon out. Yay!
					
				}
			}
		}
	}
	public void SetTracking(bool b) {
		this.tracking = b;
	}
	public void ResetCamera() {
		ResetCamera (minZoom, maxZoom);
	}
	public void ResetCamera(float newMinZoom, float newMaxZoom) {
		minZoom = newMinZoom;
		maxZoom = newMaxZoom;
		transform.position = Vector3.zero;
	}
}
