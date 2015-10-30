using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	private float currentX;
	private float currentY;
	private float currentZ;	
	public int playerid;
	private bool jumped, switchedKey, jumpedKey;
	private bool canMoveRight;
	private bool canMoveLeft;

	private Vector3 mousePos;

	static float layer1Position = 0;
	static float layer2Position = 1;

	static int layer1Value = 8;
	static int layer2Value = 9;

	// Use this for initialization
	void Start () {
		//switchedKey = true;
		mousePos = this.transform.position;

		this.tag = "Player";
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
			if (this.gameObject.layer != layer1Value) {
				//print("augh" + this.gameObject.layer);
				this.gameObject.layer = layer1Value;
				transform.position = new Vector3(currentX, currentY, layer1Position);
			} else 
			if (this.gameObject.layer != layer2Value) {
				//print("oof" + this.gameObject.layer);
				this.gameObject.layer = layer2Value;
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
		/*Vector3 pz = Input.mousePosition;
		pz.z = this.transform.position.z;
		Vector3 ppz = Camera.main.ScreenToWorldPoint(this.transform.position); //temporary for just mouse1
		//pz.y -= 6.7f;
		pz.x -= ppz.x;
		pz.y -= ppz.y;
		*/
		mousePos.x += Input.GetAxis("MouseX");
		mousePos.y += Input.GetAxis("MouseY");

	
		//transform.FindChild("Reticle").transform.position = mousePos; //ignore, fuck this stuff
		//float angle = Vector3.Angle(mousePos - this.transform.position, transform.forward);
		Vector3 reticlePos = transform.FindChild("Reticle").transform.position;
		float angle;
		if (this.transform.position.x == transform.FindChild("Reticle").transform.position.x) angle = 0;
		else angle = Mathf.Atan((reticlePos.y - this.transform.position.y)/(reticlePos.x - this.transform.position.x));
		//print(angle);
		angle = angle/2;
		if (angle < 0) {
			angle+= Mathf.PI/2;
		}
		if (reticlePos.y - this.transform.position.y < 0) {
			angle += Mathf.PI/2;
		}
		angle = angle * 2 * Mathf.Rad2Deg; //Don't question this, for some reason all of the other angles are not working
		//this.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
		print(angle);
		this.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
		this.GetComponent<LineRenderer>().SetPosition(1, this.transform.position + 2 * new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle),this.transform.position.z));

	}
	bool changePlane() {
		/* If the input has its first time being pressed down   */
		if (Input.GetAxisRaw("Switch" + this.playerid) > 0) {
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
		return (Input.GetAxis("HorizontalP" + this.playerid) < 0);
	}
	bool goRight() {
		return (Input.GetAxis("HorizontalP" + this.playerid) > 0);

	}
	bool jump() {
		if (Input.GetAxis("VerticalP" + this.playerid) > 0 ) {
			this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, 0); //slowing as we hit the floor

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
