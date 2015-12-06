using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		if (col.transform.GetComponent<Health> ()) {
			col.transform.SendMessage ("hit");
		}
	}
}