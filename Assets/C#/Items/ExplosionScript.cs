using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		//print ("starting");
		this.GetComponent<CircleCollider2D> ().radius = 0;
		foreach (grenade g in transform.GetComponentsInChildren<grenade>()) {
			g.SendMessage("Explode");
		}
		this.GetComponent<CircleCollider2D> ().radius = .75f;
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, 3);
		foreach (Collider2D c in hitColliders) {
			//print (c.name);
			if (c.GetComponent<ShootablePlatform> ())
				c.SendMessage ("hit", 20 /Vector2.Distance(this.transform.position, c.transform.position));
		}
		if (transform.childCount > 1) {
			for (int i = 0; i < transform.childCount; i++) {
				if (!transform.GetChild(i).GetComponent<Animator>()) {
					print(transform.GetChild(i).name);
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

		if (col.transform.GetComponent<grenade>()) {
			col.transform.SendMessage("Explode");
		}
		if (col.transform.GetComponent<Rigidbody2D>() != null && col.transform.GetComponent<FiredProjectile>() == null) {
			col.transform.GetComponent<Rigidbody2D> ().AddForce (500 * col.transform.GetComponent<Rigidbody2D>().mass * (col.transform.position - this.transform.position));
			col.transform.GetComponent<Rigidbody2D> ().AddForce (200 * col.transform.GetComponent<Rigidbody2D>().mass * Vector2.up);
		}
		if (col.transform.GetComponent<Hittable> ()) {
			col.transform.SendMessage("hit");
			if (col.transform.GetComponent<Health>()) col.transform.SendMessage("Gib",Random.Range(1,3));
		}
		/*if (col.transform.GetComponent<ShootablePlatform> ()) {
			col.transform.SendMessage ("hit", 7);
		}*/

	}

}
