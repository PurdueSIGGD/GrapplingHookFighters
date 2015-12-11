using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
    private float currentX;
    private float currentY;
	private float timeSincePickup = 1, punchTime = 1;
    public int playerid;
    private bool jumped, switchedKey, jumpedKey;
    public bool death, joystickController;
    private bool canMoveRight;
    private bool canMoveLeft, canPickup;
	private Rigidbody2D punchable;
	private int punchableIndex;
    public GameObject heldItem1, heldItem2;

    public Vector3 firingVector;

    static float layer1Position = 0;
    static float layer2Position = 1;

    static int layer1Value = 8;
    static int layer2Value = 9;

    public int joystickID = 1;


    private float time = 0.0f;

    void Start() {
        GameObject.Find("Reticle" + playerid).transform.position = transform.position;
        //switchedKey = true;

        tag = "Player";
        canMoveRight = true;
        canMoveLeft = true;
    }

    // Update is called once per frame
    void Update() {
        //CrashDetector.SetExePoint("Whateverelse");
        //if (playerid == 1) print("Start update function player");
		if (punchTime < 1) punchTime +=Time.deltaTime;
		if (!death) {

            currentX = transform.position.x;
            currentY = transform.position.y;
            if (time < 1.0f) time += Time.deltaTime;
            if (timeSincePickup <= .2f) timeSincePickup += Time.deltaTime;

            //layer handling will be able to make us deal with seperate collisions and items and such. Changing position is simply for aestetics.
            if (time >= 1.0f && changePlane()) {
                switchPlanes();
            }
            if (this.GetComponent<Rigidbody2D>().velocity.x > -10 && canMoveLeft && goLeft()) {
                GetComponent<Rigidbody2D>().AddForce((this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : -1) * new Vector3(jumped ? 20 : 40, 0, 0));
            }
            if (this.GetComponent<Rigidbody2D>().velocity.x < 10 && canMoveRight && goRight()) {
                GetComponent<Rigidbody2D>().AddForce((this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : 1) * new Vector3(jumped ? 20 : 40, 0, 0));
            }
            if (goDown()) {
                GetComponent<Rigidbody2D>().AddForce(new Vector3(0, -10, 0));
            }
			jumpNow(false);
            Vector3 reticlePos = GameObject.Find("Reticle" + playerid).transform.position;
            reticlePos.z = transform.position.z;
            firingVector = reticlePos - transform.position;
            firingVector.Normalize();
            //firingVector = (reticlePos - transform.position) / f;
            /* End danger zone
			 * 
			 * 
			 */
            GetComponent<LineRenderer>().SetVertexCount(2);
            GetComponent<LineRenderer>().SetPosition(0, transform.position + .1f * Vector3.forward);
			GetComponent<LineRenderer>().SetPosition(1, transform.position + 2 * firingVector + .1f * Vector3.forward);
			
			if (pickUpKey() && timeSincePickup > .2f && (!canPickup || (heldItem1 && heldItem2))) {
                //drop weapon
                timeSincePickup = 0;
                if (heldItem2 != null)
                    throwWeapon(true, 1);
                else if (heldItem1 != null)
                    throwWeapon(true, 0);
            }
        }

        //if (playerid == 1) print("End update function player");

    }
    public void switchPlanes() {
        time = 0.0f;
        if (gameObject.layer != layer1Value) {
            //print("augh" + gameObject.layer);
            gameObject.layer = layer1Value;
            transform.FindChild("TopTrigger").gameObject.layer = layer1Value;
            if (heldItem1 != null)
                heldItem1.layer = layer1Value;
            if (heldItem2 != null)
                heldItem2.layer = layer1Value;
            transform.position = new Vector3(currentX, currentY, layer1Position);
        } else
        if (gameObject.layer != layer2Value) {
            //print("oof" + gameObject.layer);
            gameObject.layer = layer2Value;
            transform.FindChild("TopTrigger").gameObject.layer = layer2Value;
            if (heldItem1 != null)
                heldItem1.layer = layer2Value;
            if (heldItem2 != null)
                heldItem2.layer = layer2Value;
            transform.position = new Vector3(currentX, currentY, layer2Position);
        }
        this.GetComponent<GrappleLauncher>().SendMessage("Disconnect");
    }
	void throwWeapont(int i) {
		throwWeapon(false, i);
	}
    void throwWeapon(bool b, int i) { //bool for dropping or throwing
        if (i == 0) {
			if (heldItem1 != null) {
				heldItem1.GetComponent<HeldItem>().focus = null;
				GameObject.Find("MouseInput").SendMessage("playerHasNotItem", playerid);
	            heldItem1.SendMessage("retriggerSoon", this.GetComponent<Collider2D>().GetComponent<Collider2D>());
	            if (heldItem1.GetComponent<PolygonCollider2D>()) heldItem1.GetComponent<PolygonCollider2D>().isTrigger = false;
	           
	            timeSincePickup = 0;
	            heldItem1.GetComponent<Rigidbody2D>().isKinematic = false;
	            if (b) heldItem1.GetComponent<Rigidbody2D>().AddForce(900 * heldItem1.GetComponent<Rigidbody2D>().mass * firingVector); //throw weapon
	            heldItem1.GetComponent<Rigidbody2D>().AddTorque(3);
	            heldItem1.transform.parent = null;
	            if (heldItem1.GetComponent<gun>()) heldItem1.GetComponent<gun>().unclick();
	            heldItem1 = null;
			}
        } else {
			if (heldItem2 != null) {
				heldItem2.GetComponent<HeldItem>().focus = null;
				GameObject.Find("MouseInput").SendMessage("playerHasNotItem2", playerid);
	            heldItem2.SendMessage("retriggerSoon", this.GetComponent<Collider2D>().GetComponent<Collider2D>());
	            if (heldItem2.GetComponent<PolygonCollider2D>()) heldItem2.GetComponent<PolygonCollider2D>().isTrigger = false;
	           
	            timeSincePickup = 0;
	            heldItem2.GetComponent<Rigidbody2D>().isKinematic = false;
	            if (b) heldItem2.GetComponent<Rigidbody2D>().AddForce(900 * heldItem2.GetComponent<Rigidbody2D>().mass * firingVector); //throw weapon
	            heldItem2.GetComponent<Rigidbody2D>().AddTorque(3);
	            heldItem2.transform.parent = null;
	            if (heldItem2.GetComponent<gun>()) heldItem2.GetComponent<gun>().unclick();
	            heldItem2 = null;
			}
        }
        canPickup = true;
    }
    bool changePlane() {
        /* If the input has its first time being pressed down   */
        if (!death && Input.GetAxisRaw("Switch" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0) {
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
        //if (name == "Player1" )print(Input.GetAxis("HorizontalP" + playerid));

        return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) < 0);
    }
    bool goRight() {
        return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0);
    }
    bool goDown() {
        return (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) < -.5);
    }

    bool jump() {
        if (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) == 1) {

            return true;
        } else {
            return false;
        }
    }
    bool pickUpKey() {
        return (!death && Input.GetAxis("UseP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0);
    }
    void OnTriggerEnter2D(Collider2D col) {
        Rigidbody2D colR = col.GetComponent<Rigidbody2D>();
		if ((!heldItem1 || (heldItem1.CompareTag("DualItem") && col.CompareTag("DualItem"))) &&
			(col.CompareTag("Item") || col.CompareTag("DualItem")) &&  //It is an item to pick up
            col.GetComponent<HeldItem>() && //it can be held
            (col.transform.parent == null || col.GetComponent<Health>()) &&  //check parent null so you can't steal weapons
            timeSincePickup > .2f && //had enough time
            !colR.isKinematic) {  //not what I am holding
            canPickup = true;
        } else {
            if (colR && !colR.isKinematic) canPickup = false;
        }
        if (col.CompareTag("Platform") || col.CompareTag("Player") || col.CompareTag("Item")) {
            jumped = false;
        }
    }
    void OnTriggerExit2D(Collider2D col) {
		if (punchable && col.gameObject == punchable.gameObject) punchable = null;

		if ((col.CompareTag("Item") || col.CompareTag("DualItem")) &&  //It is an item to pick up
            col.GetComponent<HeldItem>() && //it can be held
            (col.transform.parent == null || col.GetComponent<Health>()) &&  //check parent null so you can't steal weapons
            timeSincePickup > .2f && //had enough time
            !col.GetComponent<Rigidbody2D>().isKinematic) {  //not what I am holding

            canPickup = false;
        }
    }
    void OnTriggerStay2D(Collider2D col) {

		if (col.GetComponent<player>()) punchable = col.GetComponent<Rigidbody2D>();

		if ((col.CompareTag("Item") || col.CompareTag("DualItem")) && (col.transform.parent == null || col.GetComponent<Health>()) && timeSincePickup > .2f) { //check parent null so you can't steal weapons

            if (pickUpKey()) {
                if (heldItem1 == null) {
                    Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                    timeSincePickup = 0;
                    heldItem1 = col.gameObject;
					heldItem1.GetComponent<HeldItem>().focus = this.gameObject;
					if (heldItem1.GetComponent<PolygonCollider2D>()) heldItem1.GetComponent<PolygonCollider2D>().isTrigger = true;
                    else {
                        foreach (BoxCollider2D b in heldItem1.GetComponents<BoxCollider2D>()) {
                            b.isTrigger = true;
                        }
                    }
                    col.SendMessage("ignoreColl", this.GetComponent<Collider2D>().GetComponent<Collider2D>());
                    Transform center = this.gameObject.transform.FindChild("Center");
                    heldItem1.GetComponent<Rigidbody2D>().isKinematic = true;
                    heldItem1.transform.SetParent(center);
                    heldItem1.transform.position = (center.transform.position + .7f * this.firingVector);
                    heldItem1.transform.rotation = center.transform.rotation;
                    if (heldItem1.GetComponent<gun>() || heldItem1.GetComponent<PortalGun>()) {
                        heldItem1.SendMessage("SetPlayerID", playerid);
                    }
                    GameObject.Find("MouseInput").SendMessage("playerHasItem", playerid);
                    canPickup = false;
				} else if (heldItem2 == null && !col.GetComponent<player>() && heldItem1.CompareTag("DualItem") && col.CompareTag("DualItem")) {

					this.GetComponent<GrappleLauncher>().SendMessage("Disconnect");
                    Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                    timeSincePickup = 0;
                    heldItem2 = col.gameObject;
					heldItem2.GetComponent<HeldItem>().focus = this.gameObject;
					if (heldItem2.GetComponent<PolygonCollider2D>()) heldItem2.GetComponent<PolygonCollider2D>().isTrigger = true;
                    else {
                        foreach (BoxCollider2D b in heldItem2.GetComponents<BoxCollider2D>()) {
                            b.isTrigger = true;
                        }
                    }
                    col.SendMessage("ignoreColl", this.GetComponent<Collider2D>().GetComponent<Collider2D>());
                    Transform center = this.gameObject.transform.FindChild("Center");
                    heldItem2.GetComponent<Rigidbody2D>().isKinematic = true;
                    heldItem2.transform.SetParent(center);
                    heldItem2.transform.position = (center.transform.position + .4f * this.firingVector + .2f * Vector3.up);
                    heldItem2.transform.rotation = center.transform.rotation;
                    if (heldItem2.GetComponent<gun>()) {
                        heldItem2.SendMessage("SetPlayerID", playerid);
                    }
                    GameObject.Find("MouseInput").SendMessage("playerHasItem2", playerid);
                    canPickup = false;
                } else {

                }
            }
        }
    }
    void Death() {
        death = true;
        this.GetComponent<Rigidbody2D>().freezeRotation = false;
        this.GetComponent<LineRenderer>().SetVertexCount(0);
        this.gameObject.tag = "Item";
        if (heldItem1 != null) {
            throwWeapon(false, 0);
        }
        if (heldItem2 != null) {
            throwWeapon(false, 1);
        }
    }
    void NotDeath() {
        death = false;
        this.GetComponent<Rigidbody2D>().freezeRotation = true;
        this.GetComponent<LineRenderer>().SetVertexCount(2);
        this.gameObject.tag = "Player";

    }
	void Punch() {
		if (punchable != null && punchTime > .3f && !death) {
			punchTime = 0;
			punchable.AddForce(300 * (punchable.gameObject.transform.position - transform.position));
			punchable.AddForce(Vector2.up * 450);
			object o = 0;
			punchable.SendMessage("throwWeapont",o);
			o = 1;
			punchable.SendMessage("throwWeapont",o);

		}
	}
	void jumpNow(bool b) {

		if ((b || jump()) && !jumped && Mathf.Abs(this.GetComponent<Rigidbody2D>().velocity.y) < .01f) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0); //slowing as we hit the floor
			GetComponent<Rigidbody2D>().AddForce(new Vector3(0, 800, 0));
			jumped = true;
			
		}

	}
}