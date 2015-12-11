using UnityEngine;
using System.Collections;

public class GrappleLauncher : MonoBehaviour {
	private bool firing, retracting, attached, mouseReleased, death;
	private float grappleTimer;
	public GameObject firedGrapple;
	// Use this for initialization
	void Start () {
		firedGrapple = GameObject.Find("Grapple" + this.GetComponent<player>().playerid);
		firedGrapple.GetComponent<GrappleScript>().focus = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (grappleTimer > 0) grappleTimer -= Time.deltaTime;
		else grappleTimer = 0;
		if (!death) {
			firedGrapple.gameObject.layer = this.gameObject.layer;
			if (attached) this.GetComponent<Rigidbody2D>().AddForce(800 *  Time.deltaTime * Vector2.up);
			if (retracting && !firing) {
				//firedGrapple.GetComponent<Rigidbody2D>().AddForce((this.transform.position - firedGrapple.transform.position)/(100 * Vector3.Distance(this.transform.position, firedGrapple.transform.position)));
				if (Vector3.Distance (this.transform.position, firedGrapple.transform.position) < .3f) {
					retracting = false;
					firedGrapple.GetComponent<GrappleScript> ().retracting = false;
					firedGrapple.SendMessage("ResetLast");
				}
			}
			if (!firing && !retracting)
				firedGrapple.transform.position = transform.position;
			else {

				//RaycastHit2D r;
				//r = Physics2D.Raycast(transform.position, firedGrapple.transform.position - this.transform.position);
				
				if (!attached && Vector3.Distance (firedGrapple.transform.position, this.transform.position) > 10) {
					//print(r.collider.name);
					Disconnect ();

				}
			}
		} else {
			firedGrapple.transform.position = transform.position;
		}
	}

    void fire() {
		if (grappleTimer <= 0) {
	        if (!mouseReleased) {
	            return;
	        }
			grappleTimer = .3f;
	        Vector2 firingVector = GetComponent<player>().firingVector; //get the angle in which we want to launch
	        if (!firing && !retracting && firedGrapple.GetComponent<GrappleScript>().breakTime <= 0) {
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
		//this.GetComponent<Rigidbody2D>().AddForce(Vector3.Distance(this.transform.position, firedGrapple.transform.parent.position) * 100 * (this.transform.position.y > firedGrapple.transform.parent.position.y ? Vector3.down : Vector3.up));
		attached = true;
	}
	void Disconnect() {
		if (firing || retracting) {
			attached = false;
			firing = false;
			retracting = true;
			firedGrapple.SendMessage("Release");
		}
	}
	void Death() {
		death = true;
	}
	void NotDeath() {
		death = false;
	}
}
