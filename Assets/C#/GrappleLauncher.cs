using UnityEngine;
using System.Collections;

public class GrappleLauncher : MonoBehaviour {
	private bool firing, retracting, attached;
	private GameObject firedGrapple;
	public GameObject grappleHook;
	// Use this for initialization
	void Start () {
		firedGrapple = GameObject.Find("Grapple" + this.GetComponent<move>().playerid);
		firedGrapple.GetComponent<GrappleScript>().focus = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		firedGrapple.gameObject.layer = this.gameObject.layer;
		Vector2 firingVector = GetComponent<move>().firingVector; //get the angle in which we want to launch

		if (Input.GetMouseButtonDown(1)) {
			if (!firing && !retracting) {
				firing = true;
				firedGrapple.GetComponent<Rigidbody2D>().AddForce(firingVector * 80); //add force to move it
				firedGrapple.SendMessage("Launch",firingVector);
			} else {
				if (attached || firing) {
					Disconnect();

				} else {
					//retracting = true;
					//retract grapple
				}
			}
		}
		if (retracting && !firing) {
			//firedGrapple.GetComponent<Rigidbody2D>().AddForce((this.transform.position - firedGrapple.transform.position)/(100 * Vector3.Distance(this.transform.position, firedGrapple.transform.position)));
			if (Vector3.Distance(this.transform.position, firedGrapple.transform.position) < .3f) {
				retracting = false;
				firedGrapple.GetComponent<GrappleScript>().retracting = false;
			}
		}
		if (!firing && !retracting) firedGrapple.transform.position = transform.position;
		else {

			//RaycastHit2D r;
			//r = Physics2D.Raycast(transform.position, firedGrapple.transform.position - this.transform.position);
			
			if (Vector3.Distance(firedGrapple.transform.position, this.transform.position) > 10) {
				//print(r.collider.name);
				Disconnect();

			}
		}
	}

	void Attach(GameObject g) {

		attached = true;

	}
	void Disconnect() {
		if (firing || retracting) {
			firing = false;
			retracting = true;
			firedGrapple.SendMessage("Release");
		}
	}
}
