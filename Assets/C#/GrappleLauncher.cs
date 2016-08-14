using UnityEngine;
using System.Collections;

public class GrappleLauncher : MonoBehaviour {
	private bool firing, retracting, attached, mouseReleased, death;
	private float grappleTimer;
	public GameObject firedGrapple;
	public Transform center;
	public GameObject[] grapples;
	public Transform grappleArm, armL;
	private Rigidbody2D[] rigids;
	private EdgeCollider2D[] edges;
	private LineRenderer[] lines;
	private SpringJoint2D[] springs;

	private Rigidbody2D myRigid, grappleRigid;
	private GrappleScript firedGrappleScript;
	// Use this for initialization
	void Start () {
		myRigid = this.GetComponent<Rigidbody2D>();
		center = this.transform.FindChild("AimingParent");
		rigids = new Rigidbody2D[grapples.Length];
		edges = new EdgeCollider2D[grapples.Length];
		lines = new LineRenderer[grapples.Length];
		springs = new SpringJoint2D[grapples.Length];
		int i = 0;
		foreach (GameObject g in grapples) {
			rigids[i] = g.GetComponent<Rigidbody2D>();
			edges[i] = g.GetComponent<EdgeCollider2D>();
			lines[i] = g.GetComponent<LineRenderer>();
			springs[i] = g.GetComponent<SpringJoint2D>();
			i++;
		}
		grappleArm.gameObject.SetActive (false);
		firedGrapple = GameObject.Find("Grapple" + this.GetComponent<player>().playerid);
		firedGrappleScript = firedGrapple.GetComponent<GrappleScript>();
		grappleRigid = firedGrapple.GetComponent<Rigidbody2D>();
		firedGrappleScript.focus = this.gameObject;

	}
	
	// Update is called once per frame
	void Update () {
		if (grappleTimer > 0) grappleTimer -= Time.deltaTime;
		else grappleTimer = 0;
		if (firedGrapple == null || firedGrappleScript.disconnectMe) Disconnect();
		if (firedGrapple != null) {
			for (int i = 0; i < grapples.Length; i++) {
				Vector3[] linePoints;
				if (i + 1 >= grapples.Length) {
					linePoints = new Vector3[3];
					lines[i].SetVertexCount(3);
				} else {
					linePoints = new Vector3[2];
					lines[i].SetVertexCount(2);
				}
				if (i == 0) {
					linePoints[0] = transform.position;
				} else {
					linePoints[0] = grapples[i-1].transform.position;
				}
				linePoints[1] = grapples[i].transform.position;
				if (i + 1 >= grapples.Length) {
					linePoints[2] = firedGrapple.transform.position;
				} 
				lines[i].SetPositions(linePoints);
			}


			if (attached) myRigid.AddForce(400 *  Time.deltaTime * (firedGrapple.transform.position - center.position));//Vector2.up);
			if (retracting && !firing) {
				if (Vector3.Distance (center.position, firedGrapple.transform.position) < .3f) {
					retracting = false;
					firedGrappleScript.retracting = false;
					firedGrapple.SendMessage("ResetLast");
					for( int i = 0; i < grapples.Length; i++) {
						grapples[i].transform.localPosition = Vector3.zero;
						grapples[i].transform.localPosition = Vector3.zero;
						edges[i].enabled = false;
						springs[i].distance =  0;//Vector3.Distance (firedGrapple.transform.position, center.position) / 5; //because we have 5 joints
						edges[i].isTrigger = true;
						//set the distance 
					}
					foreach (Rigidbody2D r in rigids) {
						r.velocity = Vector2.zero;
					}
				}
			}
			if (!firing && !retracting) {
				firedGrapple.transform.position = center.position;
				grappleArm.gameObject.SetActive (false);
				armL.gameObject.SetActive (true);
			}
			else {
				
				//if firing or retracting

				//this is to make sure the arm is aiming towards that
				if (!death) {
					grappleArm.gameObject.SetActive (true);
					armL.gameObject.SetActive (false);
					grappleArm.localEulerAngles = new Vector3 (0, 0, Vector2Extension.Vector2Deg (((firing || retracting) ? grapples [grapples.Length - 1].transform.position : grapples [1].transform.position) - center.position) - 85);
				}
				for (int i = 0; i < grapples.Length; i++) {
					Vector2[] points = new Vector2[3];
					if (i == 0) {
						points[0] = transform.position - grapples[i].transform.position;
					} else {
						points[0] = grapples[i-1].transform.position - grapples[i].transform.position;
					}
					points[1] = Vector2.zero;
					if (i + 1 >= grapples.Length) {
						points[2] = Vector2.zero; //if we are going to collide into the fired grapple, that will cause problems with map
					} else {
						points[2] = grapples[i+1].transform.position - grapples[i].transform.position;
					}
					edges[i].points = points;
					springs[i].distance =  attached?Vector3.Distance (firedGrapple.transform.position, center.position) / 30:.2f; 
					//i++;
					//print(i);
					//set the distance 
				}
				if (!attached && Vector3.Distance (firedGrapple.transform.position, center.position) > 10) {
					//print(r.collider.name);
					Disconnect ();

				}
			}
		} else {
			grappleArm.gameObject.SetActive (false);
			if (firedGrapple != null) firedGrapple.transform.position = center.position;
		}
	}

    void fire() {
		if (grappleTimer <= 0) {
	        if (!mouseReleased || death) {
	            return;
	        }
			grappleTimer = .3f;
	        Vector2 firingVector = GetComponent<player>().firingVector; //get the angle in which we want to launch
			if (!firing && !retracting && firedGrappleScript.breakTime <= 0) {

				for (int i = 0; i < grapples.Length; i++) {
					edges[i].enabled = true;
					edges[i].isTrigger = false;
				}
	            this.mouseReleased = false;
	            firing = true;
	            grappleRigid.AddForce(firingVector * 3500); //add force to move it
				//myRigid.AddForce(firingVector * -500); //counteract the throwing force
	            firedGrapple.SendMessage("Launch", firingVector);

	        } else {
	            if (attached || firing) {
	                Disconnect();

	            } else {
	                //retracting = true;
	                //retract grapple
	            }
	        }
		}
    }

    void mouseRelease() {
		if (!death) 
        	this.mouseReleased = true;
    }

	void Attach() {
		attached = true;
		myRigid.AddForce(60 * (firedGrapple.transform.position - transform.position));

	}
	public void Disconnect() {
		firedGrapple.gameObject.layer = this.gameObject.layer;

		if (firing || retracting) {
			
			attached = false;
			firing = false;
			retracting = true;
			if (firedGrapple != null) firedGrapple.SendMessage("Release");
		}
	}
	void Death() {
		/*Disconnect ();
		retracting = false;
		firedGrapple.transform.position = center.position;
		firedGrappleScript.retracting = false;
		firedGrapple.SendMessage("ResetLast");

		for ( int i = 0; i < grapples.Length; i++) {
			lines[i].enabled = false;
		}*/
		death = true;
		grappleArm.gameObject.SetActive (false);
	}
	void NotDeath() {
        Disconnect();
        retracting = false;
        firedGrapple.transform.position = center.position;
        firedGrappleScript.retracting = false;
        firedGrapple.SendMessage("ResetLast");
        death = false;
		grappleArm.gameObject.SetActive (true);
        for (int i = 0; i < grapples.Length; i++)
        {
            //we set kinematic to ignore all freaky physics changes here
            rigids[i].isKinematic = true;
        }
        for ( int i = 0; i < grapples.Length; i++) {
			grapples[i].transform.localPosition = Vector3.zero;
			lines[i].SetVertexCount(2);
			lines[i].SetPositions(new Vector3[2]);
			lines[i].enabled = true;
            springs[i].distance = 0.005f;

        }
        for (int i = 0; i < grapples.Length; i++)
        {
            rigids[i].isKinematic = false;
        }
    }
}
