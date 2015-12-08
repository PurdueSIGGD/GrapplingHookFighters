using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		if (col.transform.GetComponent<Health> () && !col.isTrigger) {
			col.GetComponent<Rigidbody2D>().drag = 4;
			col.transform.SendMessage ("hit");
		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (col.transform.GetComponent<Health> () && !col.isTrigger) {
			col.GetComponent<Rigidbody2D>().drag = .5f;
			col.transform.SendMessage ("hit");
		}
	}
}