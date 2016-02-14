using UnityEngine;
using System.Collections;

public class HeldItem : MonoBehaviour {
	private bool retrigger;
	private float timeSinceDropped;
	private Collider2D lastCol;
	public GameObject focus; 
	public bool forceHazard;
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
				if (this.GetComponent<player>()) Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), lastCol, false);
				else Physics2D.IgnoreCollision(this.GetComponent<PolygonCollider2D>(), lastCol, false);
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
		if (this.GetComponent<player>()){
			Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), lastCol);
		}
		else { 
			//print("hey now");
			Physics2D.IgnoreCollision(this.GetComponent<PolygonCollider2D>(), lastCol);	
		}
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

	void OnCollisionEnter2D(Collision2D collision) {
		ForceHazardHit (collision);
	}
	void OnCollisionStay2D(Collision2D collision) {
		ForceHazardHit (collision);
	}

	void ForceHazardHit(Collision2D collision) {
		float colSpeed = collision.relativeVelocity.magnitude;
		Collider2D col = collision.collider;
		//Debug.Log ("Velocity" + rb.velocity.magnitude);
		if (forceHazard && rb != null && colSpeed >= forceThreshold && col.GetComponent<Hittable>() && !col.isTrigger) {
			Debug.Log (gameObject.name + "Velocity" + colSpeed);
			col.transform.SendMessage ("hit");
			if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
		}
	}
}
