using UnityEngine;
using System.Collections;

public class FiredProjectile : MonoBehaviour {
	//Rigidbody2D bulletrigid2D;
	//BoxCollider2D bulletBox2D;

	// Use this for initialization
	public float time = 6, damage;
	public bool exploding, dieOnAnyHit;
	public GameObject explosion;
	void OnTriggerEnter2D(Collider2D col) {

		if ((!col.isTrigger || col.GetComponent<ExplosionScript>()) && !col.GetComponent<FiredProjectile>()) {
			if (exploding) {
				GameObject ex = (GameObject)GameObject.Instantiate (explosion, this.transform.position, Quaternion.identity);
				ex.gameObject.layer = this.gameObject.layer;
			}
			if (col.GetComponent<Rigidbody2D>()) {
				col.GetComponent<Rigidbody2D>().AddForce(damage * this.GetComponent<Rigidbody2D>().velocity);
			}
			if (col.transform.GetComponent<Health> () && !exploding) {
				col.transform.SendMessage ("hit");
			}
			if (col.GetComponent<grenade>()) {
				col.SendMessage("Explode");
			}
			if (dieOnAnyHit)
				GameObject.Destroy (this.gameObject);
		}
		if (col.shapeCount == 5 && dieOnAnyHit) 
			GameObject.Destroy (this.gameObject);
		//if (col.isTrigger && col.GetComponent<Health>() && dieOnAnyHit)
			//GameObject.Destroy (this.gameObject);

	}
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		if (time <= 0)
			GameObject.Destroy (this.gameObject);
	}
}
