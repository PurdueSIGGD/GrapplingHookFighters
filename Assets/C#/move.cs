﻿using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	private float currentX;
	private float currentY;
	private float currentZ;	
	public int playerid;
	private bool jumped, switchedKey, jumpedKey;
	private bool canMoveRight;
	private bool canMoveLeft;

	public Vector3 firingVector;

	static float layer1Position = 0;
	static float layer2Position = 1;

	static int layer1Value = 8;
	static int layer2Value = 9;

	// Use this for initialization
	void Start () {
		GameObject.Find("Reticle" + playerid).transform.position = transform.position;
		//switchedKey = true;

		tag = "Player";
		canMoveRight = true;
		canMoveLeft = true;
	}
	
	// Update is called once per frame
	void Update () {
		currentX = transform.position.x;
		currentY = transform.position.y;
		//currentZ = transform.position.z;

		//layer handling will be able to make us deal with seperate collisions and items and such. Changing position is simply for aestetics.
		if (changePlane()) {
			if (gameObject.layer != layer1Value) {
				//print("augh" + gameObject.layer);
				gameObject.layer = layer1Value;
				transform.position = new Vector3(currentX, currentY, layer1Position);
			} else 
			if (gameObject.layer != layer2Value) {
				//print("oof" + gameObject.layer);
				gameObject.layer = layer2Value;
				transform.position = new Vector3(currentX, currentY, layer2Position);
			}
		}

		if (canMoveLeft && goLeft ()) {
			GetComponent<Rigidbody2D>().AddForce(new Vector3(-20, 0, 0));
		}
		if (canMoveRight && goRight()) {
			GetComponent<Rigidbody2D>().AddForce(new Vector3(20, 0, 0));
		}
		if (!jumped && jump()) {
			GetComponent<Rigidbody2D>().AddForce (new Vector3(0, 300, 0));
			jumped = true;
		}
	
		Vector3 reticlePos = GameObject.Find("Reticle" + playerid).transform.position;
		reticlePos.z = transform.z;
		firingVector = (reticlePos-transform.position)/Vector3.Distance(reticlePos,transform.position);
		GetComponent<LineRenderer>().SetPosition(0, transform.position);
		GetComponent<LineRenderer>().SetPosition(1, transform.position + 2 * firingVector);

	}
	bool changePlane() {
		/* If the input has its first time being pressed down   */
		if (Input.GetAxisRaw("Switch" + playerid) > 0) {
			if (switchedKey) {
				switchedKey = false;
				return true;
			} else {
				return false;
			}
		} else {
			switchedKey = true;
			return false;
		}
	}
	bool goLeft() {
		return (Input.GetAxis("HorizontalP" + playerid) < 0);
	}
	bool goRight() {
		return (Input.GetAxis("HorizontalP" + playerid) > 0);

	}
	bool jump() {
		if (Input.GetAxis("VerticalP" + playerid) > 0 ) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0); //slowing as we hit the floor

			return true;
		} else {
			return false;
		}
	}
	void OnTriggerEnter2D(Collider2D col) {
		if(col.CompareTag("Platform") || col.CompareTag("Player")) {
			jumped = false;
		}
	}
	void OnTriggerStay2D(Collider2D col) { //Please explain. Why is this necessary if we can have layer-specific colliders?
		if (col.CompareTag("PlatformSideRight")) {
			canMoveLeft = false;
		}
		if (col.CompareTag("PlatformSideLeft")) {
			canMoveRight = false;
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("PlatformSideRight") ||  col.CompareTag("PlatformSideLeft")) {
			canMoveLeft = true;
			canMoveRight = true;
		}
	}
}
