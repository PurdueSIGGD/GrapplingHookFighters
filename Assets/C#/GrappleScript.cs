using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {
	public GameObject focus;
//	private EdgeCollider2D lineCol;
	private bool firing, connected;
	public bool retracting, disconnectMe;
	private Transform lastGrab;
	public Transform center;
	public float breakTime, timeRetracting, timeFiring;

	private Rigidbody2D myRigid;
	private SpringJoint2D toPlayer, toPickup;
	// Use this for initialization
	void Start() {
		myRigid = this.GetComponent<Rigidbody2D>();
		toPlayer = this.transform.GetComponent<SpringJoint2D>();

	}
	void OnCollisionEnter2D(Collision2D col) {
		Attach(col.gameObject);
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (!col.isTrigger) Attach(col.gameObject);
	}

	void Attach(GameObject g) {
		if (g.transform != lastGrab && 
			g.GetComponent<ExplosionScript>() == null && 
			g.GetComponent<player>() == null && 
			(g.GetComponentInParent<player>()) == null && 
			g.tag != "Item" && 
			g.tag != "DualItem" && 
			g.tag != "Grapple" &&
			g.tag != "NoGrapple" &&
			(firing)) {
			//print(g.name);
		/*	lineCol = this.gameObject.AddComponent<EdgeCollider2D>();
			Vector2[] vee = new Vector2[2];
			vee[0] = Vector2.zero;
			vee[1] = focus.transform.position - this.transform.position;
			lineCol.points = vee;*/
			this.transform.parent = g.transform;
            if (!g.GetComponent<GrappleDetacher>()) g.AddComponent<GrappleDetacher>(); //this is a script that will always release the grapple before it destroys itself
			transform.localScale = new Vector3(1/g.transform.localScale.x,1/g.transform.localScale.y,1/g.transform.localScale.z);
			lastGrab = g.transform;
			toPlayer.distance = .2f * Vector3.Distance(this.transform.position, focus.transform.FindChild("AimingParent").position);
			myRigid.velocity = Vector2.zero;
			myRigid.isKinematic = true;
			//transform.FindChild("GrappleAttached").GetComponent<Rigidbody2D>().isKinematic = true;

			//toPlayer.enabled = true;
			focus.SendMessage("Attach");
			retracting = false;
			connected = true;
		}
        if ((g.tag == "NoGrapple" ||
            g.tag == "Item" ||
			g.tag == "DualItem" ) &&
			g.name != focus.name

			 ) {
            //bounce back
            //print("bounce back");
			print(g.name + " " + focus.name);
			retracting = true;
			Detach();
		}
		//otherwise, pass through
	}
	void Detach() {
		//for when we release not determined by the grapplelauncher
		disconnectMe = true;

	}
	void Release() {
		disconnectMe = false;
		this.transform.parent = focus.transform.parent; //for if it moves
		transform.localScale = new Vector3(1,1,1);
		if (!myRigid.isKinematic) {
		
		} else {
			myRigid.isKinematic = false;
			//transform.FindChild("GrappleAttached").GetComponent<Rigidbody2D>().isKinematic = false;

		}		
	//	Destroy(lineCol);
		//lineCol = null;
		toPlayer.enabled = false;
		connected = false;
		firing = false;
		retracting = true;
	}
	void Launch(Vector2 firingVector) {
		firing = true;
	}
	void Update() {
		if (focus != null) {
			center = focus.transform.FindChild("AimingParent");

		} else if (connected) {
			this.retracting = true;
		}
		if (breakTime > 0) {
			breakTime -= Time.deltaTime;
		} else {
			breakTime = 0;
		}
		if (!firing) {
			timeFiring += Time.deltaTime;
		} else {
			timeFiring = 0;
		}
		//RaycastHit2D[] r;
		if (connected) {
			
			/*float d = Vector3.Distance(this.transform.position, center.position) * .75f;
			if (d > 1.2f) { //so getting too close won't disconnect
				int layermask = 1 << (this.gameObject.layer + 5);
				r = Physics2D.RaycastAll(center.position, (this.transform.position - center.position), d, layermask);
				foreach (RaycastHit2D ray in r) {
					if (ray.transform.GetComponent<player>() == null 
					    && ray.transform.GetComponentInParent<player>() == null 
					    && ray.transform.GetComponent<GrappleScript>() == null) {
						breakTime = .4f; //so player wont try to disconnect and shoot again
						//print("raycast disconnect " + ray.transform.name);
						//print(d);
						focus.SendMessage("Disconnect"); //Temporarily disabled for now
					}
				}
			}*/
		}
		/*if (lineCol != null) {
			Vector2[] vee = new Vector2[2];
			vee[0] = Vector2.zero;
			vee[1] = focus.transform.position - this.transform.position;
			lineCol.points = vee;
		}*/

		if (retracting) {
			timeRetracting += Time.deltaTime;
			myRigid.velocity = Vector2.zero;
			transform.position = Vector3.MoveTowards(this.transform.position, center.position, timeRetracting * 30 * Time.deltaTime );
			//this.transform.position += 50*Time.deltaTime*(focus.transform.position - this.transform.position)/Vector3.Distance(this.transform.position, focus.transform.position);
		} else {
			timeRetracting = 1;
		}
		/*LineRenderer lr = this.GetComponent<LineRenderer>();
		if (firing || retracting || myRigid.isKinematic == true) {
			lr.enabled = true;
			lr.SetPosition(0, this.transform.position);
			lr.SetPosition(1, center.position);
		} else {
			myRigid.velocity = Vector2.zero;
			lr.SetPosition(0, this.transform.position);
			lr.SetPosition(1, this.transform.position);
			lr.enabled = false;
		}*/

	}
	void ResetLast() { //lastgrab is the last object we grabbed, we make sure the line doesnt grab anything on its way back
		lastGrab = null;
	}

}