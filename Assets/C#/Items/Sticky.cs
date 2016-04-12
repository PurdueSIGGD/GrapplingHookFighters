using UnityEngine;
using System.Collections;

public class Sticky : MonoBehaviour {
	public bool stuck, hasBeenStuck;
	private Transform colliderThing;

	private Rigidbody2D myRigid;
	private Collider2D myCollider;
	//public float stickTime = .05f;
	/* Meant for making objects that will stick into walls or players, and stay until they disappear. 
	 * Requirements: Rigidbody2D, collider2D (non trigger)
	 * Oddly enough, this isn't used for stickybombs atm
	 */
	void Start() {
		myRigid = this.GetComponent<Rigidbody2D>();
		myCollider = this.GetComponent<Collider2D>();
	}
	void OnTriggerEnter2D(Collider2D col) {
		if (!col.isTrigger && !col.transform.GetComponent<Sticky>() &&  Vector2.SqrMagnitude(myRigid.velocity) > 30) {
			stuck = true;
			colliderThing = col.transform;
		}
	}
	void Update() {
		
	}
	void LateUpdate() {
		if (stuck && !hasBeenStuck && colliderThing) {
			transform.parent = colliderThing.transform;
			myRigid.freezeRotation = true;
			myRigid.isKinematic = true;
			myCollider.enabled = false;
			hasBeenStuck = true;
			this.GetComponent<FiredProjectile>().SendMessage("Stickem", colliderThing);
		}

	}
}
