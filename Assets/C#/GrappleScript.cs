using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {
	public GameObject focus;
	private bool firing;
	public bool retracting;
	// Use this for initialization

	void OnCollisionEnter2D(Collision2D col) {
		Attach(col.gameObject);
	}

	void OnTriggerEnter2D(Collider2D col) {
		Attach(col.gameObject);
	}

	void Attach(GameObject g) {
		if (g.GetComponent<move>() == null && firing) {
			this.GetComponent<SpringJoint2D>().distance = .5f * Vector3.Distance(this.transform.position, focus.transform.position);
			this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			this.GetComponent<Rigidbody2D>().isKinematic = true;
			this.GetComponent<SpringJoint2D>().enabled = true;
			focus.SendMessage("Attach",g);
		}
	}
	void Release() {
		this.GetComponent<Rigidbody2D>().isKinematic = false;
		this.GetComponent<SpringJoint2D>().enabled = false;

		firing = false;
		retracting = true;
	}
	void Launch(Vector2 firingVector) {
		firing = true;
	}
	void Update() {
		if (retracting) {
			this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.position = Vector3.MoveTowards(this.transform.position, focus.transform.position, 20 * Time.deltaTime );
			//this.transform.position += 50*Time.deltaTime*(focus.transform.position - this.transform.position)/Vector3.Distance(this.transform.position, focus.transform.position);
			
		}
		LineRenderer lr = this.GetComponent<LineRenderer>();
		if (firing || retracting) {
			lr.enabled = true;
			lr.SetPosition(0, this.transform.position);
			lr.SetPosition(1, focus.transform.position);
		} else {
			this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			lr.SetPosition(0, this.transform.position);
			lr.SetPosition(1, this.transform.position);
			lr.enabled = false;
		}

	}
}
