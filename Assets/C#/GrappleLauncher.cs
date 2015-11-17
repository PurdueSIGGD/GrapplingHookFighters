using UnityEngine;
using System.Collections;

public class GrappleLauncher : MonoBehaviour {
	private bool firing, retracting, attached, mouseReleased, death;
	private GameObject firedGrapple;
	public GameObject grappleHook;
	// Use this for initialization
	void Start () {
		firedGrapple = GameObject.Find("Grapple" + this.GetComponent<player>().playerid);
		firedGrapple.GetComponent<GrappleScript>().focus = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (!death) {
			firedGrapple.gameObject.layer = this.gameObject.layer;
			if (attached) this.GetComponent<Rigidbody2D>().AddForce(8 * Vector2.up);
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
				
				if (Vector3.Distance (firedGrapple.transform.position, this.transform.position) > 10) {
					//print(r.collider.name);
					Disconnect ();

				}
			}
		} else {
			firedGrapple.transform.position = transform.position;
		}
	}

    void fire() {
        if (!mouseReleased) {
            return;
        }
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

    void mouseRelease() {
        this.mouseReleased = true;
    }

	void Attach(GameObject g) {

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
}
