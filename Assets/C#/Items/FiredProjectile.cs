using UnityEngine;
using System.Collections;

public class FiredProjectile : MonoBehaviour {
	//Rigidbody2D bulletrigid2D;
	//BoxCollider2D bulletBox2D;

	// Use this for initialization
	public float time = 6, damage, startTime;
	public bool exploding, dieOnAnyHit, nonLethal, exploded, forceInducedPain, pointsWhenFast, makesHimBleed, sticky, hitsSourcePlayer = true, takeTime, explodesOnPlayerHit;
	public GameObject explosion;
	public GameObject sourcePlayer;
	public float forceThreshold = 30.0f;

	void Start() {
		//print ("starting off my thing");
		startTime = time;
		Collider2D[] cols = GetComponents<Collider2D> ();
		if (!hitsSourcePlayer && sourcePlayer != null) {
			foreach (Collider2D col in cols) {
				Physics2D.IgnoreCollision (sourcePlayer.GetComponent<PolygonCollider2D> (), col, true);
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col) {
		
		if (!hitsSourcePlayer && col.gameObject == sourcePlayer) {
			return;
		}
		
		if ((!col.isTrigger || col.GetComponent<ExplosionScript>()) && !col.GetComponent<FiredProjectile>()) {
			if (exploding && !exploded && !this.GetComponent<HeldItem>()) {
				if (takeTime && startTime - time < .1f) { 
					//turn into item you can pick up
					this.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
					PolygonCollider2D polyBox = this.gameObject.AddComponent<PolygonCollider2D>();
					Vector2[] points = new Vector2[4];
					points[0] = new Vector2(.1f, .1f);
					points[1] = new Vector2(.1f, -.1f);
					points[2] = new Vector2(-.1f, -.1f);
					points[3] = new Vector2(-.1f, .1f);
					polyBox.points = points;
					BoxCollider2D triggerBox = this.GetComponent<BoxCollider2D>();
					triggerBox.offset = Vector2.zero;
					triggerBox.isTrigger = true;
					triggerBox.size = new Vector2(1,1);
					this.gameObject.AddComponent<Hittable>();
					HeldItem h = this.gameObject.AddComponent<HeldItem>();
					h.throwForce = 1200;
					dieOnAnyHit = false;
					GetComponent<Rigidbody2D>().gravityScale = 1;
					this.gameObject.tag = "Item";
				} else {
					//print ("spawning explosion");
					exploded = true;
					GameObject ex = (GameObject)GameObject.Instantiate (explosion, this.transform.position, Quaternion.identity);
					ex.gameObject.layer = this.gameObject.layer;
					//GameObject.DestroyImmediate (this.gameObject);
				}

			}
			//if (col.GetComponent<Rigidbody2D>()) {
			//	col.GetComponent<Rigidbody2D>().AddForce(damage * this.GetComponent<Rigidbody2D>().velocity);
			//}

			if (!sticky && !nonLethal && !this.GetComponent<HeldItem>() && col.transform.GetComponent<Hittable> () && !exploding && (!forceInducedPain || Vector2.SqrMagnitude(this.GetComponent<Rigidbody2D>().velocity) > forceThreshold)) {
				col.transform.SendMessage ("hit", damage);
				if (makesHimBleed && col.GetComponent<Health>()) col.SendMessage("Bleed");
			}
			if (!nonLethal && col.GetComponent<grenade>()) {
				col.SendMessage("Explode");
			}
			if (exploded || (dieOnAnyHit && startTime - time > .05f)) //can die, not too soon
				GameObject.Destroy (this.gameObject);
		}

		
		//if (col.isTrigger && col.GetComponent<Health>() && dieOnAnyHit)
			//GameObject.Destroy (this.gameObject);

	}
	void OnCollisionEnter2D(Collision2D col) {
		//print(this.GetComponent<HeldItem>().timeSinceDropped + " " + Vector2.SqrMagnitude(col.relativeVelocity) + " " + (startTime - time));
		if (explodesOnPlayerHit && col.transform.GetComponent<player>() || 
			(Vector2.SqrMagnitude(col.relativeVelocity) > 50 && exploding && startTime - time > 1 && (!this.GetComponent<HeldItem>() || this.GetComponent<HeldItem>().timeSinceDropped >= .1f))) {
			//print ("spawning explosion");
			exploded = true;
			GameObject ex = (GameObject)GameObject.Instantiate (explosion, this.transform.position, Quaternion.identity);
			ex.gameObject.layer = this.gameObject.layer;
			//GameObject.DestroyImmediate (this.gameObject);
			GameObject.Destroy (this.gameObject);
		}
	}
	// Update is called once per frame
	void Update () {
		
		if (pointsWhenFast) {
			Vector2 vel = this.GetComponent<Rigidbody2D>().velocity;

			if (Vector2.SqrMagnitude(vel) > 10) {
				if (this.GetComponent<Sticky>() && this.GetComponent<Sticky>().stuck) {

				} else {
					
					float rotZ = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg; //moving the rotation of the center here
					transform.rotation = Quaternion.Euler(0, 0, rotZ);
				}
			}
		}
		time -= Time.deltaTime;
		if (time <= 0 && startTime != -1) {
			GameObject.Destroy (this.gameObject);
		}
	}
	void Stickem(Transform t) {
		if (sticky) {
			if (t.GetComponent<Hittable>() ) t.transform.SendMessage ("hit", damage);
			if (t.GetComponent<Health>() && makesHimBleed) t.SendMessage("Bleed");

		}

			
	}
	void hit() {
		if (exploding && !exploded ) {
			//print ("spawning explosion");
			exploded = true;
			GameObject ex = (GameObject)GameObject.Instantiate (explosion, this.transform.position, Quaternion.identity);
			ex.gameObject.layer = this.gameObject.layer;
			//GameObject.DestroyImmediate (this.gameObject);
			GameObject.Destroy (this.gameObject);
		}
	}
}
