using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {

	public bool active = true;
	ArrayList stuckers;

	void Start() {
		stuckers = new ArrayList();
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (active && col.GetComponent<Hittable>() && !col.isTrigger) {
			col.GetComponent<Rigidbody2D>().drag = 50;
			col.GetComponent<Rigidbody2D>().angularDrag = 3;
			col.transform.SendMessage ("hit");
			if (col.transform.GetComponent<Health> () ) {
				col.transform.SendMessage ("Bleed");
				stuckers.Add(col.gameObject);
			}
		}
	}
	void OnTriggerStay2D(Collider2D col) {
		if (active && col.transform.GetComponent<Hittable> () && !col.isTrigger) {
			col.transform.SendMessage ("hit");
		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (active && col.GetComponent<Hittable>() && (!col.isTrigger || col.GetComponent<player>())) { //if player is held
			col.GetComponent<Rigidbody2D>().drag = .5f;
			col.GetComponent<Rigidbody2D>().angularDrag = .05f;
			if (col.transform.GetComponent<Health> () ) {
				stuckers.Remove(col.gameObject);
			}
			//col.transform.SendMessage ("hit");
			//if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
		}
	}

	void OnDestroy() {
		foreach (Object o in stuckers) {
			if (o != null) {
				Rigidbody2D rg = ((GameObject)o).GetComponent<Rigidbody2D>();
				rg.drag = .5f;
				rg.angularDrag = .05f;
			}
		}
	}
}