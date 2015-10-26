using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	private float currentX;
	private float currentY;
	private float currentZ;	
	public int playerid;
	private bool jumped;
	private bool canMoveRight;
	private bool canMoveLeft;
	// Use this for initialization
	void Start () {
		this.tag = "Player";
		canMoveRight = true;
		canMoveLeft = true;
	}
	
	// Update is called once per frame
	void Update () {
		currentX = GetComponent<Rigidbody>().transform.position.x;
		currentY = GetComponent<Rigidbody>().transform.position.y;
		currentZ = GetComponent<Rigidbody> ().transform.position.z;
		if (changePlaneBack() && currentZ <= -7.5f) {
			transform.position = new Vector3(currentX, currentY, ++currentZ);
		}
		if (changePlaneForward() && currentZ >= -6.53f) {
				transform.position = new Vector3(currentX, currentY, --currentZ);
		}
		if (goLeft () && canMoveLeft) {
			GetComponent<Rigidbody>().AddForce(new Vector3(-25, 0, 0));
		}
		if (goRight() && canMoveRight) {
			GetComponent<Rigidbody>().AddForce(new Vector3(25, 0, 0));
		}
		if (jump() && !jumped) {
			GetComponent<Rigidbody>().AddForce (new Vector3(0, 175, 0));
			jumped = true;
		}
	}
	bool changePlaneBack() {
		if (playerid == 0) return Input.GetKeyDown (KeyCode.W);
		if (playerid == 1) return Input.GetKeyDown (KeyCode.UpArrow);
		return false;
	}
	bool changePlaneForward() {
		if (playerid == 0) return Input.GetKeyDown (KeyCode.S);
		if (playerid == 1) return Input.GetKeyDown (KeyCode.DownArrow);
		return false;
	}
	bool goLeft() {
		if (playerid == 0) return Input.GetKey(KeyCode.A);
		if (playerid == 1)return Input.GetKey (KeyCode.LeftArrow);
		return false;
	}
	bool goRight() {
		if (playerid == 0) return Input.GetKey(KeyCode.D);
		if (playerid == 1) return Input.GetKey (KeyCode.RightArrow);
		return false;
	}
	bool jump() {
		if (playerid == 0) return Input.GetKeyDown(KeyCode.Space);
		if (playerid == 1) return Input.GetKeyDown (KeyCode.Keypad0);
		return false;
	}
	void OnTriggerEnter(Collider col) {
		if(col.CompareTag("Platform"))
			jumped = false;
	}
	void OnTriggerStay(Collider col) {
		if (col.CompareTag("PlatformSideRight")) {
			canMoveLeft = false;
		}
		if (col.CompareTag("PlatformSideLeft")) {
			canMoveRight = false;
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.CompareTag ("PlatformSideRight") ||  col.CompareTag("PlatformSideLeft")) {
			canMoveLeft = true;
			canMoveRight = true;
		}
	}
}
