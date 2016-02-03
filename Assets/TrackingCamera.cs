using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour {
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	private Vector2 lastPos, goalPos;
	public float initialCamSize = 10;
	public GameObject[] targets;
	public float bufferX = 4, bufferY = 4;
	private float goalCamSize, halfwayCamSize;
	private Camera cam;
	public float zoomSpeed = 4;
	private float maxSize = 25, minSize = 4;
	private float timeGone;
	private int lastCnt, playerCount; 
	private bool shifting, transition, halfway;
	float zooming; //-1 = zooming out, 1 = zooming in, 0 = not zooming
	// Use this for initialization
	void Start () {
		lastCnt = -1;
		cam = GetComponentInChildren<Camera> ();
		initialCamSize = cam.orthographicSize;
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
	void FixedUpdate () {

		if (transition) {
			transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, 2);  //function to move
			//if (cam.orthographicSize < 17) cam.orthographicSize+=Time.deltaTime*.5f;
			if (halfway) cam.orthographicSize+=Time.deltaTime*.7f;
			else {
				float factor = Vector2.Distance(transform.position, (goalPos + lastPos)/2)/(.5f*Vector2.Distance(goalPos,lastPos));
				//factor will be 0 when at the halfway point, will be 1 when at the final point
				cam.orthographicSize = halfwayCamSize + (goalCamSize - halfwayCamSize)*(factor); 
			}
			if (halfway && Vector2.Distance(goalPos, transform.position) < Vector2.Distance(goalPos, lastPos) * .5f) {
				GameObject.Find("SceneController").SendMessage("RespawnPeeps");
				halfway = false;
				lastCnt = playerCount;
				halfwayCamSize = cam.orthographicSize;
				//calculate the rate which the camera should move, regarding the distance of each players (copy what was later)
				float avgX = 0;
				float avgY = 0;
				float maxX = Mathf.NegativeInfinity, minX = Mathf.Infinity;
				float maxY = Mathf.NegativeInfinity, minY = Mathf.Infinity;
				Vector3 avgPos = Vector3.zero;
				int i = 0;
				//Vector3 maxDis = transform.position;
				foreach (GameObject g in targets) {
					if (!g.GetComponent<Health>().dead) {
						Vector3 tempPlayer = g.transform.position;
						//X Bounds
						if (tempPlayer.x < minX)
							minX = tempPlayer.x;
						if (tempPlayer.x > maxX)
							maxX = tempPlayer.x;
						//Y Bounds
						if (tempPlayer.y < minY)
							minY = tempPlayer.y;
						if (tempPlayer.y > maxY)
							maxY = tempPlayer.y;

						avgX += g.transform.position.x;
						avgY += g.transform.position.y;
						if (i == 0) {
							avgPos = g.transform.position;
						}
						else {
							avgPos+= g.transform.position;
						}
						i++;
					}

				}
		
				float sizeX = Mathf.Abs(maxX - minX) + bufferX;
				float sizeY = Mathf.Abs(maxY - minY) + bufferY;
				//print("x: " + sizeX + " y: " + sizeY);

				float camSize = 0.5f * (sizeX > sizeY ? sizeX : sizeY);
				if (camSize > maxSize) {
					goalCamSize = maxSize;
				} else if (camSize < minSize) {
					goalCamSize = minSize;
				} else {
					goalCamSize = camSize;
				}
				//print(goalCamSize);
				//we have goalCamSize, now we can use it later to calculate 
			}
		
		} else {
		if (targets != null)
		{
			float avgX = 0;
			float avgY = 0;
			float maxX = Mathf.NegativeInfinity, minX = Mathf.Infinity;
			float maxY = Mathf.NegativeInfinity, minY = Mathf.Infinity;
			Vector3 avgPos = Vector3.zero;
			int i = 0;
			//Vector3 maxDis = transform.position;
			foreach (GameObject g in targets) {
				if (!g.GetComponent<Health>().dead) {
					Vector3 tempPlayer = g.transform.position;
					//X Bounds
					if (tempPlayer.x < minX)
						minX = tempPlayer.x;
					if (tempPlayer.x > maxX)
						maxX = tempPlayer.x;
					//Y Bounds
					if (tempPlayer.y < minY)
						minY = tempPlayer.y;
					if (tempPlayer.y > maxY)
						maxY = tempPlayer.y;
						//if (Mathf.Abs(tempPlayer.x) == 0 || Mathf.Abs(tempPlayer.y) == 0) print(g.name);
					avgX += g.transform.position.x;
					avgY += g.transform.position.y;
					if (i == 0) {
						avgPos = g.transform.position;
					}
					else {
						avgPos+= g.transform.position;
					}
					i++;
				}

			}
				if (i != lastCnt && lastCnt != -1) {
					shifting = true;
				//	print("now shifting cuz i = " + i + " and last i was " + lastCnt);

				}
			lastCnt = i;
			if (i == 0) {
				avgPos = this.transform.position;

				i = 1;
				timeGone += Time.deltaTime;
			} else {
				timeGone = 0;
			}


			if (timeGone > 0) {
				//lost everyone, gone
				zooming = -.15f/(timeGone + 1);
				cam.orthographicSize+=zoomSpeed*zooming*Time.deltaTime;
			
			} else {
			
				Vector3 avg = avgPos /i;
				Debug.DrawLine(avg, Vector3.zero);
				dampTime = 1/Vector2.Distance(this.transform.position, avg);
				Vector3 destination = new Vector3(avg.x, avg.y, transform.position.z);
				
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, .7f);  //function to move
				

				//typical movement
				//print(maxX + " " + minX + " " + maxY + " " + minY);
				float sizeX = Mathf.Abs(maxX - minX) + bufferX;
				float sizeY = Mathf.Abs(maxY - minY) + bufferY;
				
				//print("x: " + sizeX + " y: " + sizeY);
				float camSize = 0.5f * (sizeX > sizeY ? sizeX : sizeY);
				if (camSize > maxSize) camSize = maxSize;
				if (camSize < minSize) camSize = minSize;
				//shifting = (Mathf.Abs(cam.orthographicSize - camSize) > .2f && !(camSize > maxSize) && !(camSize < minSize)); //if we are not close enough to the target
				if (shifting) {
					//print("Shifting " + Mathf.Abs(cam.orthographicSize - camSize));
					zooming = -5f/((cam.orthographicSize - camSize + 30));
					cam.orthographicSize+=zoomSpeed*zooming*Time.deltaTime;
					if (Mathf.Abs(cam.orthographicSize - camSize) < .1f) shifting = false;
				} else {
					cam.orthographicSize = camSize;
				}
				//print(camSize);
			}
		}
		}
	}

	bool AnyPlayersInZoomOutBounds() {
		foreach (GameObject g in targets) {
			if (!g.GetComponent<Health>().dead) {
				//Debug.Log("Checking: " + g);
				Vector3 viewPos = cam.WorldToViewportPoint(g.transform.position);
				//Debug.Log (viewPos);
				if (viewPos.x < 0.15f || viewPos.x > 0.85f || viewPos.y < 0.15f || viewPos.y > 0.85f) {
					return true;
				}
			}
		}
		return false;
	}

	bool AllPlayersInZoomInBounds() {
		foreach (GameObject g in targets) {
			if (!g.GetComponent<Health>().dead) {
				//Debug.Log("Checking: " + g);
				Vector3 viewPos = cam.WorldToViewportPoint(g.transform.position);
				//Debug.Log (viewPos);
				if (viewPos.x < 0.25f || viewPos.x > 0.75f || viewPos.y < 0.25f || viewPos.y > 0.75f) {
					return false;
				}
			}
		}
		return true;
	}
	void SetNewPlace(Vector2 pos) {
		
		goalPos = pos;
		lastPos = transform.position;
		transition = true;
		halfway = true;

	}
	void StopNewPlace() {
		transition = false;
		lastCnt = -1;
	}
}
