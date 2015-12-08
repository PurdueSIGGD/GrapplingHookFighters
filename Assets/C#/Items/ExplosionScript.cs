using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		this.GetComponent<CircleCollider2D> ().radius = 0;
		foreach (grenade g in transform.GetComponentsInChildren<grenade>()) {
			g.SendMessage("Explode");
		}
		this.GetComponent<CircleCollider2D> ().radius = .75f;
		print("ugh");
		//EditorApplication.isPaused = true;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(this.transform.position,this.transform.position + Vector3.right * this.GetComponent<CircleCollider2D> ().radius);
		Animator a = this.GetComponentInChildren<Animator> ();
		if (a.GetTime () > .3) 
			this.GetComponent<CircleCollider2D> ().enabled = false;
		else //this.GetComponent<CircleCollider2D>().radius += 7*Time.deltaTime;
		if (a.GetTime () > 1.6f)
			Destroy (this.gameObject);
	}
	void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.GetComponent<Rigidbody2D>() != null && col.transform.GetComponent<FiredProjectile>() == null) {
			col.transform.GetComponent<Rigidbody2D> ().AddForce (500 * col.transform.GetComponent<Rigidbody2D>().mass * (col.transform.position - this.transform.position));
			col.transform.GetComponent<Rigidbody2D> ().AddForce (200 * col.transform.GetComponent<Rigidbody2D>().mass * Vector2.up);
		}
		if (col.transform.GetComponent<Health> ()) {
			col.transform.SendMessage("hit");
		}
		if (col.transform.GetComponent<grenade>()) {
			col.transform.SendMessage("Explode");
		}
	}

}
