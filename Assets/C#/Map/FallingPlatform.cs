using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour {
	public float life = 4;
	private bool steppedOn;
	void Update() {
		if (steppedOn)
			life -= Time.deltaTime;
		if (life <= 0 && !this.GetComponent<Rigidbody2D>()) {
			this.gameObject.AddComponent<Rigidbody2D> ();
			this.gameObject.layer -= 2;
			ParticleScript p = this.gameObject.AddComponent<ParticleScript> (); //delete itself
			p.time = 4;
		}
	}
	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player") && !col.isTrigger)
			steppedOn = true;

	}
	void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("Player") && !col.isTrigger)
			steppedOn = false; 

		}
}
