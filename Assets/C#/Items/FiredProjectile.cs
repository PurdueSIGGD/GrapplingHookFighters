using UnityEngine;
using System.Collections;

public class FiredProjectile : MonoBehaviour {
	//Rigidbody2D bulletrigid2D;
	//BoxCollider2D bulletBox2D;

	// Use this for initialization
	public float time, damage;
	public bool exploding, dieOnAnyHit;
	public GameObject explosion;
	void Start () {
	//	bulletBox2D = GetComponentInParent<BoxCollider2D>();
	//	bulletrigid2D = GetComponentInParent<Rigidbody2D>();
	}
	void OnTriggerEnter2D(Collider2D col) {
		if (col.GetComponent<player>()) {
			if (exploding) {
				GameObject.Instantiate(explosion, this.transform.position, this.transform.rotation);
			} else {
				col.SendMessage("hit");
			}

		}
		if (dieOnAnyHit) GameObject.Destroy (this.gameObject);

	}
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 6)
			GameObject.Destroy (this.gameObject);
	}
}
