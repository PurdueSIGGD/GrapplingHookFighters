﻿using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {
	public float time;
	// Use this for initialization
	void Start () {
	
	}
	void OnCollisionEnter2D(Collision2D col) {
		print ("bam");
		if (col.transform.GetComponent<Health> ()) {
			col.transform.SendMessage("hit");
		}
		Destroy (this.gameObject);
	}
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 6)
			GameObject.Destroy (this.gameObject);
	}
}
