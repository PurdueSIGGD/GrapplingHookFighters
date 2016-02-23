using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		//print ("starting");
		//this.GetComponent<CircleCollider2D> ().radius = 0;
		foreach (grenade g in transform.GetComponentsInChildren<grenade>()) {
			g.SendMessage("Explode");
		}
		//this.GetComponent<CircleCollider2D> ().radius = .75f;
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, 3);
		foreach (Collider2D c in hitColliders) {
			int layermask = (1 << this.gameObject.layer) + (1 << 13) + (1 << 15);
			RaycastHit2D[] rr = Physics2D.RaycastAll(transform.position, c.transform.position - transform.position, Vector2.Distance(transform.position, c.transform.position), layermask);
			bool hit = true;
			//bool tooCloseToCare = Vector2.Distance(transform.position, c.transform.position) < .2f;
			//if (c.GetComponent<player>()) print(Vector2.Distance(transform.position, c.transform.position));
			//if (!tooCloseToCare) {
				foreach (RaycastHit2D r in rr) {
					if (r.transform != c.transform && 
						r.transform != transform && 
						!r.transform.GetComponent<FiredProjectile>() &&
						!r.transform.GetComponentInParent<player>() &&
					Vector2.Distance(r.point, transform.position) > .05f) //last one is to make sure if it is basically inside an object, the object isnt counted
					{
						//print(r.transform.name);
						hit = false;
						break;
					}

				}
			//}
			if (hit) {
			//print (c.name); 
				if (c.transform.GetComponent<grenade>()) {
					c.transform.SendMessage("Explode");
				}
				if (c.transform.GetComponent<Rigidbody2D>() != null && c.transform.GetComponent<FiredProjectile>() == null) {
					c.transform.GetComponent<Rigidbody2D> ().AddForce (500 * c.transform.GetComponent<Rigidbody2D>().mass * (c.transform.position - this.transform.position));
					c.transform.GetComponent<Rigidbody2D> ().AddForce (200 * c.transform.GetComponent<Rigidbody2D>().mass * Vector2.up);
				}
				if (c.transform.GetComponent<Hittable> ()) {
					c.transform.SendMessage("hit");
					if (c.transform.GetComponent<Health>()) c.transform.SendMessage("Gib",Random.Range(1,3));
				}
				if (c.GetComponent<ShootablePlatform> ())
					c.SendMessage ("hit", 20 /Vector2.Distance(this.transform.position, c.transform.position));
			}
		}
		if (transform.childCount > 1) {
			for (int i = 0; i < transform.childCount; i++) {
				if (!transform.GetChild(i).GetComponent<Animator>()) {
					//print(transform.GetChild(i).name);
					GameObject.Destroy(transform.GetChild(i).gameObject);
				
				}
					
			}
		}
		//print("ugh");
		//EditorApplication.isPaused = true;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawLine(this.transform.position,this.transform.position + Vector3.right * this.GetComponent<CircleCollider2D> ().radius);
		Animator a = this.GetComponentInChildren<Animator> ();
		if (a.GetTime () > .3) {
			this.GetComponent<CircleCollider2D> ().enabled = false;

		}
		if (a.GetTime () > 1.6f)
				Destroy (this.gameObject);
	}
	void OnCollisionEnter2D(Collision2D col) {
		//print(col.transform.name);


		/*if (col.transform.GetComponent<ShootablePlatform> ()) {
			col.transform.SendMessage ("hit", 7);
		}*/

	}

}
