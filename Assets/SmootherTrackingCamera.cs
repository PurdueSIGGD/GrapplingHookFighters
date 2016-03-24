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

	private int playerCount;
	private GameObject[] targets;
	// Use this for initialization
	void Start () {
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
			float thisDistance = Vector2.Distance (transform.position, gh.passedBoundary?gh.boundaryPlace:g.transform.position);
			if (!gh.dead)
				atLeastOneAlive = true;
			if (gh.deadTime < 2.5f) { 
				//calculate furthest distance
				//if they crossed a boundary, use their last place they were before they died 
				desiredPosition += gh.passedBoundary?gh.boundaryPlace:g.transform.position;
				trackingPlayers++;
				//calculate furthest player
				if (thisDistance > furthestDistance) {
					furthestDistance = thisDistance;
				}
			}
		}
		if (trackingPlayers > 0 && atLeastOneAlive) {
			//average
			desiredPosition = desiredPosition / trackingPlayers;
		} else {
			//does not change
			desiredPosition = transform.position;
		}
		float cameraDistance = Vector2.Distance (desiredPosition, transform.position);
		if (cameraDistance > 0)
			transform.position += Time.deltaTime * movementSpeed * (cameraDistance + 2) * (desiredPosition - transform.position);
		
		//print (furthestDistance);
		if (furthestDistance > 0) {
			if (furthestDistance < minZoom)
				furthestDistance = minZoom;
			if (furthestDistance > maxZoom)
				furthestDistance = maxZoom;
			//add a value of 1 to give some buffer room
			float desiredSize = furthestDistance + 1;

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
	public void ResetCamera() {
		ResetCamera (minZoom, maxZoom);
	}
	public void ResetCamera(float newMinZoom, float newMaxZoom) {
		minZoom = newMinZoom;
		maxZoom = newMaxZoom;
		transform.position = Vector3.zero;
	}
}
