using UnityEngine;
using System.Collections;

public class HeldItem : MonoBehaviour {
	private bool retrigger;
	private float timeSinceDropped;
	private Collider2D lastCol;
	public GameObject focus; 
	public bool forceHazard;
	public Collider2D hazardCollider;
	public float forceThreshold;
	private Rigidbody2D rb; 
	public float throwForce = 900;
	// Use this for initialization
	void Start () {
		if (forceHazard) {
			rb = GetComponent<Rigidbody2D> ();
		}
	}
	
	// Update is called once per frame
	void Update () {


		if (retrigger) {
			timeSinceDropped += Time.deltaTime;
			if (timeSinceDropped > .2f) {
				Collider2D[] colliders = this.GetComponents<Collider2D> ();
				foreach (Collider2D c in colliders) {
					Physics2D.IgnoreCollision (c, lastCol, false);
				}
				if (hazardCollider != null) {
					Physics2D.IgnoreCollision (hazardCollider, lastCol, false);
				}
				retrigger = false;
				timeSinceDropped = 0;
			}			
		}
	}
	void retriggerSoon() {
		retrigger = true;
	}
	void ignoreColl(Collider2D col) {
		lastCol = col;
		Collider2D[] colliders = this.GetComponents<Collider2D> ();
		foreach (Collider2D c in colliders) {
			Physics2D.IgnoreCollision (c, lastCol, true);
		}
		if (hazardCollider != null) {
			Physics2D.IgnoreCollision (hazardCollider, lastCol, true);
		}
		//else { 
			//print("hey now");
			//Physics2D.IgnoreCollision(this.GetComponent<PolygonCollider2D>(), lastCol);	
		//}
	}
	void NotDeath() {
		object o = 0;
		if (focus) focus.SendMessage("throwWeapont",o);
	}
	void fullClick() {

	}
	void click() {
		//up to object implementation
	}
	void unclick() {

	}

	void OnCollisionEnter2D(Collision2D c) {
		if (forceHazard) {
			ForceHazardHit (c);
		}
	}
	/*void OnCollisionStay2D(Collision2D c) {
		ForceHazardHit (c);
	}*/

	void ForceHazardHit(Collision2D collision) {
		if (!forceHazard || hazardCollider == null || rb == null) {
			return;
		}

		bool hitHazardCol = false;
		foreach (ContactPoint2D c in collision.contacts) {
			if (c.otherCollider == hazardCollider || c.collider == hazardCollider) {
				hitHazardCol = true;
				break;
			}
		}

		if (!hitHazardCol) {
			return;
		}

		//Debug.Log ("slam");

		//float colSpeed = collision.relativeVelocity.magnitude;
		float colSpeed = rb.velocity.magnitude;
		Collider2D col = collision.collider;
		//Debug.Log (gameObject.name + " velocity: " + colSpeed);
		if (colSpeed >= forceThreshold && col.GetComponent<Hittable>() && !col.isTrigger) {
			Debug.Log (gameObject.name + " velocity: " + colSpeed);
			col.transform.SendMessage ("hit");
			if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
			//if (col.transform.GetComponent<Health>()) col.transform.SendMessage("Gib",Random.Range(1,3));
		}
	}
}
