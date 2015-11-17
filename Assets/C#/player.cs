using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
	private float currentX;
	private float currentY;
	private float timeSincePickup = 1;
	public int playerid;
	private bool jumped, switchedKey, jumpedKey;
	public bool death;
	private bool canMoveRight;
	private bool canMoveLeft;

	public GameObject heldItem;

	public Vector3 firingVector;

	static float layer1Position = 0;
	static float layer2Position = 1;

	static int layer1Value = 8;
	static int layer2Value = 9;
	
	
	private float time = 0.0f;

	// Use this for initialization
	void Start () {

	/*	gun[] guns = GameObject.FindObjectsOfType<gun>();
		foreach (gun g in guns) {
			Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), g.transform.GetComponent<Collider2D>());
		}*/

		GameObject.Find("Reticle" + playerid).transform.position = transform.position;
		//switchedKey = true;

		tag = "Player";
		canMoveRight = true;
		canMoveLeft = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!death) {
			currentX = transform.position.x;
			currentY = transform.position.y;
			time += Time.deltaTime;
			timeSincePickup += Time.deltaTime;

			//layer handling will be able to make us deal with seperate collisions and items and such. Changing position is simply for aestetics.
			if (time >= 1.0f && changePlane ()) {
				time = 0.0f;
				if (gameObject.layer != layer1Value) {
					//print("augh" + gameObject.layer);
					gameObject.layer = layer1Value;
					transform.FindChild ("TopTrigger").gameObject.layer = layer1Value;
					if (heldItem != null)
						heldItem.layer = layer1Value;
					transform.position = new Vector3 (currentX, currentY, layer1Position);
				} else 
				if (gameObject.layer != layer2Value) {
					//print("oof" + gameObject.layer);
					gameObject.layer = layer2Value;				
					transform.FindChild ("TopTrigger").gameObject.layer = layer2Value;
					if (heldItem != null)
						heldItem.layer = layer2Value;
					transform.position = new Vector3 (currentX, currentY, layer2Position);
				}
				this.GetComponent<GrappleLauncher> ().SendMessage ("Disconnect");

			}
			if (this.GetComponent<Rigidbody2D> ().velocity.x > -10 && canMoveLeft && goLeft ()) {
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (jumped?-20:-40, 0, 0));
			}
			if (this.GetComponent<Rigidbody2D> ().velocity.x < 10 && canMoveRight && goRight ()) {
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (jumped?20:40, 0, 0));
			} 
			if (goDown ()) {
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (0, -10, 0));
			}
			if (Mathf.Abs (this.GetComponent<Rigidbody2D> ().velocity.y) < .1f && !jumped && jump ()) {
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (0, 500, 0));
				jumped = true;
			}

			Vector3 reticlePos = GameObject.Find ("Reticle" + playerid).transform.position;
			reticlePos.z = transform.position.z;
			firingVector = (reticlePos - transform.position) / Vector3.Distance (reticlePos, transform.position);
			GetComponent<LineRenderer> ().SetVertexCount(2);
			GetComponent<LineRenderer> ().SetPosition (0, transform.position);
			GetComponent<LineRenderer> ().SetPosition (1, transform.position + 2 * firingVector);


			if (pickUpKey () && heldItem != null && timeSincePickup > .2f) {
				//drop weapon
				throwWeapon(true);
			}
		}

    }
	void throwWeapon(bool b) { //bool for dropping or throwing
		GameObject.Find ("MouseInput").SendMessage ("playerHasNotItem", playerid);
		heldItem.SendMessage("retriggerSoon", this.GetComponent<BoxCollider2D>().GetComponent<Collider2D>());
		if (heldItem.GetComponent<PolygonCollider2D>()) heldItem.GetComponent<PolygonCollider2D>().isTrigger = false;
		else {
			foreach (BoxCollider2D bbbb in heldItem.GetComponents<BoxCollider2D>()) {
				if (bbbb.size == new Vector2(1,1)) bbbb.isTrigger = false;
			}
		}
		timeSincePickup = 0;
		heldItem.GetComponent<Rigidbody2D> ().isKinematic = false;
		if (b) heldItem.GetComponent<Rigidbody2D> ().AddForce (500 * heldItem.GetComponent<Rigidbody2D>().mass * firingVector); //throw weapon
		heldItem.GetComponent<Rigidbody2D> ().AddTorque (3);
		heldItem.transform.parent = null;
		if (heldItem.GetComponent<gun> ()) heldItem.GetComponent<gun> ().unclick ();
		heldItem = null;
	}
	bool changePlane() {
		/* If the input has its first time being pressed down   */
		if (!death && Input.GetAxisRaw("Switch" + playerid) > 0) {
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
	bool goDown() {
		return (!death && Input.GetAxis("VerticalP" + playerid) < 0);
	}

	bool jump() {
		if (!death && Input.GetAxis("VerticalP" + playerid) == 1) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0); //slowing as we hit the floor

			return true;
		} else {
			return false;
		}
	}
	bool pickUpKey() {
		return (!death && Input.GetAxis("UseP" + playerid) > 0);
	}
	void OnTriggerEnter2D(Collider2D col) {
		if(col.CompareTag("Platform") || ((col.CompareTag("Player") || col.CompareTag("Item")) && col.GetComponent<Rigidbody2D>() && col.GetComponent<Rigidbody2D>().velocity.y < .2f)) {
			jumped = false;
		} else {
            //rint("hit");
        }
	}
	void OnTriggerStay2D(Collider2D col) {
		if (col.CompareTag("Item") && col.GetComponent<HeldItem>() && pickUpKey() && (col.transform.parent == null || col.GetComponent<Health>()) && heldItem == null && timeSincePickup > .2f) { //check parent null so you can't steal weapons
			timeSincePickup = 0;
			heldItem = col.gameObject;
			if (heldItem.GetComponent<PolygonCollider2D>()) heldItem.GetComponent<PolygonCollider2D>().isTrigger = true;
			else {
				foreach (BoxCollider2D b in heldItem.GetComponents<BoxCollider2D>()) {
					b.isTrigger = true;
				}
			}
			col.SendMessage("ignoreColl",this.GetComponent<BoxCollider2D>().GetComponent<Collider2D>());
			Transform center = this.gameObject.transform.FindChild("Center");
			heldItem.GetComponent<Rigidbody2D>().isKinematic = true;
			heldItem.transform.SetParent(center);
			heldItem.transform.position = center.transform.position;
			heldItem.transform.rotation = center.transform.rotation;
			if (heldItem.GetComponent<gun>()) heldItem.SendMessage("SetPlayerID", playerid); 
			GameObject.Find("MouseInput").SendMessage("playerHasItem", playerid);
		}
	}
	void Death() {
		death = true;
		this.GetComponent<LineRenderer> ().SetVertexCount (0);
		this.gameObject.tag = "Item";
		if (heldItem != null) {
			throwWeapon(false);
		}
	}
}
