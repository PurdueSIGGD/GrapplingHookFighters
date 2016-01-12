using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour {
	private Vector2 launchDir;
	public float factor = 1;
	// Use this for initialization
	void Start () {
		
		launchDir = factor * 20 * transform.TransformDirection(Vector3.left);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D col) {
		if (!col.isTrigger && col.transform.GetComponent<Rigidbody2D>()) {
			col.transform.GetComponent<Rigidbody2D>().velocity = (launchDir);
		}
	}
}
