﻿using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	private bool willexplode = false;
    private Transform originalorientation;
    public float droppedtime = 0.0f;
    public float fusetime = 4.0f;
	public GameObject explosion;

    // Use this for initialization
	void Start () {
        originalorientation = this.transform;
	}
	
    void OnTriggerStay2D(Collider2D col)
    {
		if(droppedtime >= fusetime && col.GetComponent<Rigidbody2D>() && col.GetComponent<Rigidbody2D>().velocity.magnitude > 2f)
        {
			GameObject.Instantiate (explosion, transform.position, Quaternion.identity);
			GameObject.Destroy(this.gameObject);
        }

    }


 
	// Update is called once per frame
	void Update () {
		if (droppedtime > fusetime) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		}


		if(transform.parent.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            droppedtime += Time.deltaTime;     
        }
        //Make sure the landmine falls flat on ground 
        if (this.gameObject.CompareTag("Player") != true) //focus is not player
        {
            transform.rotation = originalorientation.rotation;
        }

    }
}
