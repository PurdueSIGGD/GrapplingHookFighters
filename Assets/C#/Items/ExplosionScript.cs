using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		foreach (grenade g in transform.GetComponentsInChildren<grenade>()) {
			g.SendMessage("Explode");
		}
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
			if (Vector2.Distance(c.transform.position,transform.position) < .5f) hit = true;
			if (hit) {
			//print (c.name); 
				if (c.transform.GetComponent<grenade>()) {
					c.transform.SendMessage("Explode");
				}
				Rigidbody2D rg;
				if ((rg = c.transform.GetComponent<Rigidbody2D>()) != null && c.transform.GetComponent<FiredProjectile>() == null) {
					rg.AddForce (300 * rg.mass * (c.transform.position - this.transform.position));
					rg.AddForce (150 * rg.mass * Vector2.up);
				}
				if (c.transform.GetComponent<Hittable> ()) {
					c.transform.SendMessage("hit",  100 /Vector2.Distance(this.transform.position, c.transform.position));
					if (c.transform.GetComponent<Health>()) c.transform.SendMessage("Gib",transform.position);
				}
				if (c.GetComponent<ShootableItem> () && !c.isTrigger) {
					
					c.SendMessage("hit", 150);
				}
			}
		}
		if (transform.childCount > 1) {
			for (int i = 0; i < transform.childCount; i++) {
				if (!transform.GetChild(i).GetComponent<Animator>() && !transform.GetChild(i).GetComponent<SpriteRenderer>() ) { //delete stuck items, but not animator or light
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
		if (a.GetTime () > .4f)
				Destroy (this.gameObject);
	}
	void OnCollisionEnter2D(Collision2D col) {
		//print(col.transform.name);


		/*if (col.transform.GetComponent<ShootablePlatform> ()) {
			col.transform.SendMessage ("hit", 7);
		}*/

	}

}
