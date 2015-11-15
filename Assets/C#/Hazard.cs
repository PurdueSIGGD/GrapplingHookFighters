using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			col.SendMessage("Death",SendMessageOptions.DontRequireReceiver);
		}
	}
}