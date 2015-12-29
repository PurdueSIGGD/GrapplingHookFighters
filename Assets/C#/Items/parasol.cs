﻿using UnityEngine;
using System.Collections;

public class parasol : MonoBehaviour {
	private bool opened;
	public Sprite sOpen, sClosed;
	private float timeSwitched;
	private Transform myParent;
	// Use this for initialization
	void Start () {
		opened = false;
		timeSwitched = Time.time;
	}

	// Update is called once per frame
	void Update () {
		if (this.transform.parent) { 
			this.transform.parent.parent.GetComponent<Rigidbody2D>().gravityScale = opened ? .25f : 1.1f;
			myParent = this.transform.parent.parent;
		} else {
			if (myParent) {
				myParent.GetComponent<Rigidbody2D>().gravityScale = opened ? .25f : 1.1f;
				myParent = null;
			}
		}
		//if (!opened && Time.time - timeSwitched > .06f) opened = true;
		this.GetComponentInChildren<SpriteRenderer>().sprite = opened ? sOpen : sClosed;

	}
	void LateUpdate() {
		if (opened) {
			//transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			//transform.rotation = Quaternion.identity;
		}


	}
	void click() {
		if (Time.time - timeSwitched > .05f) {
			opened = true;
			timeSwitched = Time.time;
		}
	}
	void unclick() {
		if (Time.time - timeSwitched > .05f || !this.transform.parent) {
			transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
			opened = false;
			timeSwitched = Time.time;
		}
	}
}
