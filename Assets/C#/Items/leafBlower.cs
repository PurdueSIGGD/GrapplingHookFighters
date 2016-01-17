using UnityEngine;
using System.Collections;

public class leafBlower : MonoBehaviour {
	private bool blowing;
	private float spool;
	ParticleSystem p;
	void Start() {
		p = this.GetComponentInChildren<ParticleSystem>();
	}
	void Update() {
		if (blowing && transform.parent.parent) transform.parent.parent.GetComponent<Rigidbody2D>().AddForce(transform.parent.parent.GetComponent<player>().firingVector * -2 * spool);
	}
	// Use this for initialization
	void click() {
		blowing = true;
		if (spool < 5) spool+=7*Time.deltaTime;
		if (!p.isPlaying) p.Play();
	}
	void unclick() {
		blowing = false;
		if (spool > 0) spool-=4 *Time.deltaTime;
		if (p.isPlaying) p.Stop();

	}
	void OnTriggerStay2D(Collider2D col) {
		//print(blowing);
		if (blowing && col.GetComponent<Rigidbody2D>()) {
			//print("move dammit");
			
			col.GetComponent<Rigidbody2D>().AddForce(spool * 3f * (col.transform.position - this.transform.position));
		}
	}
}
