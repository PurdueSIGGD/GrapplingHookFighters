using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {
	// Use this for initialization
	//already uses held item for physics script makes sure it kills itself
	HeldItem hi;
	bool hit;
	bool grabbed;
	float countdown;
	Rigidbody2D rigid;


	void Start () {
		hi = gameObject.GetComponentInChildren<HeldItem> ();
		if (hi == null) {
			Destroy(this.gameObject);
		}
		hi.hazardCollider = gameObject.GetComponentInChildren<PolygonCollider2D> ();
		hi.forceHazard = true;
		hit = false;
		grabbed = false;
		countdown = 10f;
		rigid = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (hi.focus == null) {
			grabbed = false;
			hi.forceHazard = true;
		} else {
			grabbed = true;
			hi.forceHazard = false;
		}

		if (rigid.velocity.y >= -.05) {
			hi.forceHazard = false;
		}

		if (grabbed) {
		} else {
			//if (hit) {
				//hi.forceHazard = false;
				/*if(countdown <= 0){
					if(this.gameObject != null){
						Destroy (this.gameObject);
					}
				}
				countdown -= Time.deltaTime;*/
			//}
		}

	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.name == "Boundary") {
			if (this.gameObject != null) {
				Destroy (this.gameObject);
			}
		} else if (col.gameObject.layer == 13) {
			hit = true;
		} else {
			//hi.SendMessage ("ignoreColl", col);
		}
	}
}
