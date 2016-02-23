using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {
	//a script you just put on a cube and the platform now works as a conveyor belt
	BoxCollider2D one;
	bool left; //boolean value for if the conveyor belt is going left
	void Start () {
		one = gameObject.GetComponentInChildren<BoxCollider2D> ();
		if (one == null) {
			//not sure what to put here it works for me!!
		}
		left = true;
	}

	void Update () {
	}

	void OnCollisionStay2D(Collision2D col){
		Rigidbody2D body = col.gameObject.GetComponentInChildren<Rigidbody2D> ();
		if (body == null) {
		} else {
			if (left) {
				body.AddForce (new Vector2 (30, 0));
			} else {
				body.AddForce (new Vector2 (-30, 0));
			}
		}
	}
}
