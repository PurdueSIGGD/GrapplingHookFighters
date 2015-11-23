using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(this.transform.position,this.transform.position + Vector3.right * this.GetComponent<CircleCollider2D> ().radius);
		Animator a = this.GetComponent<Animator> ();
		if (a.GetTime () > .3) 
			this.GetComponent<CircleCollider2D> ().enabled = false;
		else //this.GetComponent<CircleCollider2D>().radius += 7*Time.deltaTime;
		if (a.GetTime () > 1.6f)
			Destroy (this.gameObject);
	}
	void OnTriggerEnter2D(Collider2D col) {
		if (col.GetComponent<Rigidbody2D>() != null && col.GetComponent<FiredProjectile>() == null) {
			col.GetComponent<Rigidbody2D> ().AddForce (500 * col.GetComponent<Rigidbody2D>().mass * (col.transform.position - this.transform.position));
			col.GetComponent<Rigidbody2D> ().AddForce (200 * col.GetComponent<Rigidbody2D>().mass * Vector2.up);
		}
		if (col.GetComponent<Health> ()) {
			col.SendMessage("hit");
		}
		if (col.GetComponent<grenade>()) {
			col.SendMessage("Explode");
		}
	}
}
