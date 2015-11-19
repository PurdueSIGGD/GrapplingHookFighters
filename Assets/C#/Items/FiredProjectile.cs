﻿using UnityEngine;
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
		if (!col.isTrigger || col.GetComponent<ExplosionScript>()) {
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
			if (dieOnAnyHit)
				GameObject.Destroy (this.gameObject);
		}
	

	}
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 6)
			GameObject.Destroy (this.gameObject);
	}
}
