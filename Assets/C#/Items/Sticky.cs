using UnityEngine;
using System.Collections;

public class Sticky : MonoBehaviour {
	public bool stuck, hasBeenStuck;
	private Transform colliderThing;
	//public float stickTime = .05f;
	/* Meant for making objects that will stick into walls or players, and stay until they disappear. 
	 * Requirements: Rigidbody2D, collider2D (non trigger)
	 * Oddly enough, this isn't used for stickybombs atm
	 */
	void OnTriggerEnter2D(Collider2D col) {
		if (!col.isTrigger && !col.transform.GetComponent<Sticky>()) {
			stuck = true;
			colliderThing = col.transform;
		}
		//this.GetComponent<Rigidbody2D>().velocity = col.transform.GetComponent<Rigidbody2D>().velocity;
	}
	void Update() {
		//if (Vector2.SqrMagnitude(this.GetComponent<Rigidbody2D>().velocity) < 5) this.GetComponent<Rigidbody2D>().isKinematic = true;
		//if (stuck) stickTime -= Time.deltaTime;
	}
	void LateUpdate() {
		if (stuck && !hasBeenStuck && colliderThing) {
			transform.parent = colliderThing.transform;
			this.GetComponent<Rigidbody2D>().freezeRotation = true;
			//this.GetComponent<Rigidbody2D>().drag = 30;
			//this.GetComponent<Rigidbody2D>().gravityScale = 0;
			this.GetComponent<Collider2D>().enabled = false;
			//if (stickTime <= 0) {
			hasBeenStuck = true;
			this.GetComponent<Rigidbody2D>().isKinematic = true;
			this.GetComponent<FiredProjectile>().SendMessage("Stickem", colliderThing);
			//}
		}

	}
}
