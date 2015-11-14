using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {
	public GameObject focus;
	private bool firing, connected;
	public bool retracting;

	private SpringJoint2D toPlayer, toPickup;
	// Use this for initialization
	void Start() {
		toPlayer = this.transform.GetComponent<SpringJoint2D>();
	}
	void OnCollisionEnter2D(Collision2D col) {
		Attach(col.gameObject);
	}

	void OnTriggerEnter2D(Collider2D col) {
		Attach(col.gameObject);
	}

	void Attach(GameObject g) {
		if (g.GetComponent<player>() == null && g.GetComponentInParent<player>() == null &&  (firing || retracting)) {
//			if (focus.name == "Player1" )print(g.name);

			toPlayer.distance = .2f * Vector3.Distance(this.transform.position, focus.transform.position);
			this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			if (g.GetComponent<gun>()) {
				//DistanceJoint2D dj = this.gameObject.AddComponent<DistanceJoint2D>();
				//dj.distance = 0;
				//dj.connectedBody = g.GetComponent<Rigidbody2D>();
			} else {
				this.GetComponent<Rigidbody2D>().isKinematic = true;
			}
			toPlayer.enabled = true;
			focus.SendMessage("Attach",g);
			retracting = false;
			connected = true;
		}
	}
	void Release() {
		if (!this.GetComponent<Rigidbody2D>().isKinematic) {
			//Destroy (this.GetComponent<DistanceJoint2D>());
		} else {
			this.GetComponent<Rigidbody2D>().isKinematic = false;
		}		
		toPlayer.enabled = false;
		connected = false;
		firing = false;
		retracting = true;
	}
	void Launch(Vector2 firingVector) {
		firing = true;
	}
	void Update() {
		RaycastHit2D[] r;
		if (connected) {
			r = Physics2D.RaycastAll(focus.transform.position, (this.transform.position - focus.transform.position), Vector3.Distance(this.transform.position, focus.transform.position) - .2f);
			foreach (RaycastHit2D ray in r) {
				if (ray.transform.gameObject.layer == this.gameObject.layer 
				    && ray.transform.GetComponent<player>() == null 
				    && ray.transform.GetComponentInParent<player>() == null 
				    && ray.transform.GetComponent<GrappleScript>() == null
				    && ray.transform.GetComponent<gun>() == null) {
					focus.SendMessage("Disconnect"); //Temporarily disabled for now
				}
			}
		}


		if (retracting) {
			this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.position = Vector3.MoveTowards(this.transform.position, focus.transform.position, 20 * Time.deltaTime );
			//this.transform.position += 50*Time.deltaTime*(focus.transform.position - this.transform.position)/Vector3.Distance(this.transform.position, focus.transform.position);
		}
		LineRenderer lr = this.GetComponent<LineRenderer>();
		if (firing || retracting || this.GetComponent<Rigidbody2D>().isKinematic == true) {
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
