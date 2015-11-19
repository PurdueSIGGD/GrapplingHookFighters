using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour {
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public GameObject[] targets;
	public float bufferX = 0, bufferY = 0;
	private float initialCamSize;
	private Camera cam;
	public float zoomSpeed = 15;
	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera> ();
		initialCamSize = cam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {

		
		if (targets != null)
		{
			float avgX = 0;
			float avgY = 0;
			Vector3 avgPos = Vector3.zero;
			int i = 0;
			foreach (GameObject g in targets) {
				if (g.activeInHierarchy) {
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
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 1);  //function to move

			//zoomOutNeeded = false;
			//initialVisCheck();
			/*if (initialVisibility && !allPlayersVisible()) {
				zoomOut();
			}
			else if (allPlayersVisible() && (cam.orthographicSize > initialCamSize)) {
				zoomIn();
			}*/
			if (AllPlayersInZoomInBounds() && cam.orthographicSize > initialCamSize) {
				zoomIn();
			}
			else if (AnyPlayersInZoomOutBounds()){
				zoomOut();
			}
		}
	}

	void zoomOut() {
		//initialVisibility = false;
		//Debug.Log ("Zooming Out");
		//cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize+0.01f, Time.deltaTime * zoomSpeed);
		cam.orthographicSize+=5*Time.deltaTime;
	}
	void zoomIn() {
		//initialVisibility = false;
		//transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1);
		//Debug.Log ("Zooming In");
		//cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize-0.01f, Time.deltaTime * zoomSpeed);
		cam.orthographicSize-=5*Time.deltaTime;
	}
	
	bool AnyPlayersInZoomOutBounds() {
		foreach (GameObject g in targets) {
			if (g.activeInHierarchy) {
				//Debug.Log("Checking: " + g);
				Vector3 viewPos = cam.WorldToViewportPoint(g.transform.position);
				//Debug.Log (viewPos);
				if (viewPos.x < 0.1f || viewPos.x > 0.9f || viewPos.y < 0.1f || viewPos.y > 0.9f) {
					return true;
				}
			}
		}
		return false;
	}

	bool AllPlayersInZoomInBounds() {
		foreach (GameObject g in targets) {
			if (g.activeInHierarchy) {
				//Debug.Log("Checking: " + g);
				Vector3 viewPos = cam.WorldToViewportPoint(g.transform.position);
				//Debug.Log (viewPos);
				if (viewPos.x < 0.3f || viewPos.x > 0.7f || viewPos.y < 0.3f || viewPos.y > 0.7f) {
					return false;
				}
			}
		}
		return true;
	}
}
