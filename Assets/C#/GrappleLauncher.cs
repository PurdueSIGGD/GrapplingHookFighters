using UnityEngine;
using System.Collections;

public class GrappleLauncher : MonoBehaviour {
	private bool firing, retracting, attached, mouseReleased, death;
	private float grappleTimer;
	public GameObject firedGrapple;
	public Transform center;

	private Rigidbody2D myRigid;
	private GrappleScript firedGrappleScript;
	// Use this for initialization
	void Start () {
		myRigid = this.GetComponent<Rigidbody2D>();
		center = this.transform.FindChild("AimingParent").FindChild ("Center");

		firedGrapple = GameObject.Find("Grapple" + this.GetComponent<player>().playerid);
		firedGrappleScript = firedGrapple.GetComponent<GrappleScript>();
		firedGrappleScript.focus = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (grappleTimer > 0) grappleTimer -= Time.deltaTime;
		else grappleTimer = 0;
		if (firedGrapple == null) Disconnect();
		if (!death && firedGrapple != null) {
			
			firedGrapple.gameObject.layer = this.gameObject.layer;
			if (attached) myRigid.AddForce(800 *  Time.deltaTime * Vector2.up);
			if (retracting && !firing) {
				if (Vector3.Distance (center.position, firedGrapple.transform.position) < .3f) {
					retracting = false;
					firedGrappleScript.retracting = false;
					firedGrapple.SendMessage("ResetLast");
				}
			}
			if (!firing && !retracting)
				firedGrapple.transform.position = center.position;
			else {

				//RaycastHit2D r;
				//r = Physics2D.Raycast(transform.position, firedGrapple.transform.position - this.transform.position);
				
				if (!attached && Vector3.Distance (firedGrapple.transform.position, center.position) > 10) {
					//print(r.collider.name);
					Disconnect ();

				}
			}
		} else {
			if (firedGrapple != null) firedGrapple.transform.position = center.position;
		}
	}

    void fire() {
		if (grappleTimer <= 0) {
	        if (!mouseReleased) {
	            return;
	        }
			grappleTimer = .3f;
	        Vector2 firingVector = GetComponent<player>().firingVector; //get the angle in which we want to launch
			if (!firing && !retracting && firedGrappleScript.breakTime <= 0) {
	            this.mouseReleased = false;
	            firing = true;
	            firedGrapple.GetComponent<Rigidbody2D>().AddForce(firingVector * 80); //add force to move it
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
        this.mouseReleased = true;
    }

	void Attach() {
		attached = true;
	}
	void Disconnect() {
		if (firing || retracting) {
			attached = false;
			firing = false;
			retracting = true;
			if (firedGrapple != null) firedGrapple.SendMessage("Release");
		}
	}
	void Death() {
		Disconnect ();
		retracting = false;
		firedGrapple.transform.position = center.position;
		firedGrappleScript.retracting = false;
		firedGrapple.SendMessage("ResetLast");
		death = true;
	}
	void NotDeath() {
		death = false;
	}
}
