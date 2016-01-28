using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour {
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public GameObject[] targets;
	public float bufferX = 0, bufferY = 0;
//	private float initialCamSize;
	private Camera cam;
	public float zoomSpeed = 4;
	private float timeGone;
	private int lastCnt; 
	private bool shifting;
	float zooming; //-1 = zooming out, 1 = zooming in, 0 = not zooming
	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera> ();
		//initialCamSize = cam.orthographicSize;
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		if (targets != null)
		{
			float avgX = 0;
			float avgY = 0;
			float maxX = 0, minX = Mathf.Infinity;
			float maxY = 0, minY = Mathf.Infinity;
			Vector3 avgPos = Vector3.zero;
			int i = 0;
			//Vector3 maxDis = transform.position;
			foreach (GameObject g in targets) {
				if (!g.GetComponent<Health>().dead) {
					//if (Vector3.Distance(transform.position, g.transform.position) > Vector3.Distance(transform.position, maxDis)) maxDis = g.transform.position;
					if (Mathf.Abs(g.transform.position.x)  > Mathf.Abs(maxX)) maxX = g.transform.position.x;
					if (Mathf.Abs(g.transform.position.y)  > Mathf.Abs(maxY)) maxY = g.transform.position.y;
					if (Mathf.Abs(g.transform.position.x)  < Mathf.Abs(minX)) minX = g.transform.position.x;
					if (Mathf.Abs(g.transform.position.y)  < Mathf.Abs(minY)) minY = g.transform.position.y;

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
			if (i != lastCnt) shifting = true;
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

				//avgX += GameObject.Find("CenterPlatform").transform.position.x;
				//avgY += GameObject.Find("CenterPlatform").transform.position.y;
				//i++;
				Vector3 avg = avgPos /i;
				//Vector3 avg = new Vector3(avgX/i, avgY/i, this.transform.position.z);
				Debug.DrawLine(avg, Vector3.zero);
				dampTime = 1/Vector2.Distance(this.transform.position, avg);
				//Vector3 point = cam.WorldToViewportPoint(avg);                                      //get the target's position
				//Vector3 delta = avg - cam.ViewportToWorldPoint(new Vector3(.05f, .05f, transform.position.z));   //change in distance
				Vector3 destination = new Vector3(avg.x, avg.y, transform.position.z);
				//Vector3 destination = transform.position + delta;												   //destination vector (messy)
				//destination.Set (destination.x + bufferX, destination.y + bufferY, destination.z);
				//destination.Set (destination.x, destination.y, destination.z);//destination vector (fixed)
				//destination.Set (destination.x + bufferX, destination.y + bufferY, transform.position.z);
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, .7f);  //function to move

				//zoomOutNeeded = false;
				//initialVisCheck();
				/*if (initialVisibility && !allPlayersVisible()) {
				zoomOut();
			}
			else if (allPlayersVisible() && (cam.orthographicSize > initialCamSize)) {
				zoomIn();
			}
			if (AllPlayersInZoomInBounds() && cam.orthographicSize > 5) {
				if (timeGone > .2f) {
					zooming = -.15f/timeGone;
				} else {
					//print("ahahahah");
					zooming = -1;
				}
				//zoomIn();
			}
			else if (AnyPlayersInZoomOutBounds()){
				zooming = 1;
				//zoomOut();
			} else {
				zooming = 0;
			}*/
				//typical movement
				float bufferX = 4;
				float bufferY = 10;
				float sizeX = Mathf.Abs(maxX - minX) + bufferX;
				float sizeY = Mathf.Abs(maxY - minY) + bufferY;
				//print("x: " + sizeX + " y: " + sizeY);
				float camSize = 0.5f * (sizeX > sizeY ? sizeX : sizeY);
				if (shifting) {
					zooming = -13f/(Mathf.Abs(cam.orthographicSize - camSize + 20));
					cam.orthographicSize+=zoomSpeed*zooming*Time.deltaTime;
					if (cam.orthographicSize - camSize < .2f) shifting = false;
				} else if (camSize > 17) {
					cam.orthographicSize = 17;
				} else if (camSize < 4) {
					cam.orthographicSize = 4;
				
				} else {
					cam.orthographicSize = camSize;
				}
				//print(camSize);
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
}
