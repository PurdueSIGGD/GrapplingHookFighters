using UnityEngine;
using System.Collections;

public class FiredProjectile : MonoBehaviour {
	//Rigidbody2D bulletrigid2D;
	//BoxCollider2D bulletBox2D;

	// Use this for initialization
	public float time = 6, damage, startTime;
	public bool exploding, dieOnAnyHit, nonLethal, exploded, forceInducedPain, pointsWhenFast, makesHimBleed;
	public GameObject explosion;
	void Start() {
		//print ("starting off my thing");
		startTime = time;
	}
	void OnTriggerEnter2D(Collider2D col) {
		
		if ((!col.isTrigger || col.GetComponent<ExplosionScript>()) && !col.GetComponent<FiredProjectile>()) {
			if (exploding && !exploded) {
				//print ("spawning explosion");
				exploded = true;
				GameObject ex = (GameObject)GameObject.Instantiate (explosion, this.transform.position, Quaternion.identity);
				ex.gameObject.layer = this.gameObject.layer;
				//GameObject.DestroyImmediate (this.gameObject);

			}
			if (col.GetComponent<Rigidbody2D>()) {
				col.GetComponent<Rigidbody2D>().AddForce(damage * this.GetComponent<Rigidbody2D>().velocity);
			}

		/*if (!nonLethal && col.transform.GetComponent<Health> () && !exploding && (!forceInducedPain || Vector2.SqrMagnitude(this.GetComponent<Rigidbody2D>().velocity) > 30)) {
				col.transform.SendMessage ("hit");
				if (makesHimBleed) col.SendMessage("Bleed");
			}*/
			if (!nonLethal && col.GetComponent<grenade>()) {
				col.SendMessage("Explode");
			}
			if (exploded || dieOnAnyHit && (startTime - time > .05f || exploding)) //can die, not too soon
				GameObject.Destroy (this.gameObject);
		}
		if (col.shapeCount == 5 && dieOnAnyHit) //uhh I forgot what this does. Something about polygon colliders and biz
			GameObject.Destroy (this.gameObject);
		
		//if (col.isTrigger && col.GetComponent<Health>() && dieOnAnyHit)
			//GameObject.Destroy (this.gameObject);

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
		if (time <= 0)
			GameObject.Destroy (this.gameObject);
	}
	void Stickem(Transform t) {
		if (t.GetComponent<player>()) {
			t.transform.SendMessage ("hit");
			if (makesHimBleed) t.SendMessage("Bleed");
		}
	}
}
