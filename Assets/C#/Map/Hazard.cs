using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		if (col.GetComponent<Hittable>() && !col.isTrigger) {
			col.GetComponent<Rigidbody2D>().drag = 50;
			col.GetComponent<Rigidbody2D>().angularDrag = 3;
			col.transform.SendMessage ("hit");
			if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
		}
	}
	void OnTriggerStay2D(Collider2D col) {
		if (col.transform.GetComponent<Hittable> () && !col.isTrigger) {
			col.transform.SendMessage ("hit");
		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (col.GetComponent<Hittable>() && (!col.isTrigger || col.GetComponent<player>())) { //if player is held
			col.GetComponent<Rigidbody2D>().drag = .5f;
			col.GetComponent<Rigidbody2D>().angularDrag = .05f;

			//col.transform.SendMessage ("hit");
			//if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
		}
	}
}