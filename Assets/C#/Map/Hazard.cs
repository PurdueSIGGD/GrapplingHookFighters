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
			Rigidbody2D rg = col.GetComponent<Rigidbody2D>();
			rg.drag = 50;
			rg.angularDrag = 3;
			col.transform.SendMessage ("hit", 150);
			if (col.transform.GetComponent<Health> () ) {
				col.transform.SendMessage ("Bleed");
				stuckers.Add(col.gameObject);
			}
		}
	}
	void OnTriggerStay2D(Collider2D col) {
		if (active && col.transform.GetComponent<Hittable> () && !col.isTrigger) {
			col.transform.SendMessage ("hit", 150);
		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (active && col.GetComponent<Hittable>() && (!col.isTrigger || col.GetComponent<player>())) { //if player is held
			Rigidbody2D rg = col.GetComponent<Rigidbody2D>();
			rg.drag = .5f;
			rg.angularDrag = .05f;
			if (col.transform.GetComponent<Health> () ) {
				stuckers.Remove(col.gameObject);
			}

			//if (col.transform.GetComponent<Health> () ) col.transform.SendMessage ("Bleed");
		}
	}

	void OnDestroy() {
		foreach (Object o in stuckers) {
			Rigidbody2D rg;
			if (o != null) {
				rg = ((GameObject)o).GetComponent<Rigidbody2D>();
				rg.drag = .5f;
				rg.angularDrag = .05f;
			}
		}
	}
}