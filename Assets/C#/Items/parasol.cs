using UnityEngine;
using System.Collections;

public class parasol : MonoBehaviour {
	private bool opened;
	public Sprite sOpen, sClosed;
	private float timeSwitched;
	private Transform myParent;
	Quaternion fixedRotation;
	// Use this for initialization
	void Start () {
		opened = true;
		timeSwitched = Time.time;
		fixedRotation = Quaternion.Euler (0, 0, 0);
	}

	// Update is called once per frame
	void Update () {
		this.GetComponentInChildren<SpriteRenderer>().sprite = opened ? sOpen : sClosed;
		if (this.transform.parent) { 
			this.transform.parent.parent.GetComponent<Rigidbody2D>().gravityScale = opened ? .25f : 1.1f;
			myParent = this.transform.parent.parent;
		} else {
			if (myParent) {
				myParent.GetComponent<Rigidbody2D>().gravityScale = opened ? .25f : 1.1f;
				myParent = null;
			}
		}

		if (!opened && Time.time - timeSwitched > .06f) opened = true;
	}
	void LateUpdate() {
		transform.rotation = fixedRotation;

	}
	void click() {
		if (Time.time - timeSwitched > .05f) {
			opened = true;
			timeSwitched = Time.time;
		}
	}
	void unclick() {
		if (Time.time - timeSwitched > .05f || !this.transform.parent) {
			opened = false;
			timeSwitched = Time.time;
		}
	}
}
