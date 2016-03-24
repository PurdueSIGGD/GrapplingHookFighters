using UnityEngine;
using System.Collections;
//using UnityEditor;
using System.Security.Policy;
using FMOD.Studio;

public class player : MonoBehaviour {
   // private float currentX;
   // private float currentY;
	private float timeSincePickup = 1, punchTime = 1, jumpTime;
    public int playerid;
    private bool jumped, switchedKey, jumpedKey;
	public bool death, joystickController, tempDisabled;
    private bool canMoveRight;
    private bool canMoveLeft, canPickup;
	private bool spinningItem;
	private Transform punchable;
	private int punchableIndex;
	private float lastItemRotation;
	private float crouchTime;
	private bool crouched, isStandingUp;
	public float maxMoveSpeed = 10;
	public GameObject heldItem1, heldItem2, passiveItem;
	public bool jetpack, jetpackPlaying, skateBoard;
    public Vector3 firingVector, savedReticleLocation;
	private Vector2[] standingCol, crouchingCol;


   // static float layer1Position = 0;
   // static float layer2Position = 1;

//    static int layer1Value = 8;
  //  static int layer2Value = 9;

	public int joystickID;
	public int mouseID;


    private float time = 0.0f;

    void Start() {
        GameObject.Find("Reticle" + playerid).transform.position = transform.position;
        //switchedKey = true;
		Vector2[] curCol = 	this.GetComponent<PolygonCollider2D>().points;

		standingCol = new Vector2[curCol.Length];
		crouchingCol = new Vector2[curCol.Length];
		for (int i = 0; i < curCol.Length; i++) {
			standingCol[i] = crouchingCol[i] = curCol[i];
		}
		crouchingCol[4] = new Vector2(crouchingCol[4].x, 0);
		crouchingCol[3] = new Vector2(crouchingCol[3].x, (crouchingCol[2].y-crouchingCol[3].y));
		crouchingCol[2] = new Vector2(crouchingCol[2].x, 0);

        tag = "Player";
        canMoveRight = true;
        canMoveLeft = true;
    }

    void Update() {
        //CrashDetector.SetExePoint("Whateverelse");
        //if (playerid == 1) print("Start update function player");
		if (punchTime < 1) punchTime +=Time.deltaTime;
		if (!death && !tempDisabled) {

           // currentX = transform.position.x;
           // currentY = transform.position.y;
            if (time < 1.0f) time += Time.deltaTime;
            if (timeSincePickup <= .4f) timeSincePickup += Time.deltaTime;

            //layer handling will be able to make us deal with seperate collisions and items and such. Changing position is simply for aestetics.
            if (time >= 1.0f && changePlane()) {
                switchPlanes();
            }
			float maxMoveSpeedRevised = (skateBoard && Mathf.Abs(this.GetComponent<Rigidbody2D>().velocity.y) > .01f) ? maxMoveSpeed/10 : maxMoveSpeed ;
			if (this.GetComponent<Rigidbody2D>().velocity.x > -1 * maxMoveSpeedRevised && canMoveLeft && goLeft()) {
				this.transform.FindChild("Sprite").GetComponent<SpriteRenderer> ().flipX = true;
				float f = Input.GetAxis("HorizontalPD" + joystickID);
				float force = 0;
				if (f != 0) {
					force = -1;
				} else {
					force = (this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : -1);
				}
				GetComponent<Rigidbody2D>().AddForce(force * new Vector3(40, 0, 0));
            }
			if (this.GetComponent<Rigidbody2D>().velocity.x < maxMoveSpeedRevised && canMoveRight && goRight()) {
				this.transform.FindChild("Sprite").GetComponent<SpriteRenderer> ().flipX = false;
				float f = Input.GetAxis("HorizontalPD" + joystickID);
				float force = 0;
				if (f != 0) {
					force = 1;
				} else {
					force = (this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : 1);
				}
				GetComponent<Rigidbody2D>().AddForce(force * new Vector3(40, 0, 0));            
			}
            if (goDown()) {
				if (!crouched) this.GetComponent<PolygonCollider2D>().points = crouchingCol;
				crouched = true;
				this.isStandingUp = false;
                GetComponent<Rigidbody2D>().AddForce(new Vector3(0, -10, 0));
				maxMoveSpeed = 4;
			} else {
				if (crouched) {
					this.isStandingUp = true;
				}
				crouched = false;
				if (isStandingUp) {
					//start slowly moving upwards
					Vector2 offset = this.GetComponent<PolygonCollider2D>().offset;
					offset.y-=4*Time.deltaTime;
					if (offset.y < -1f) {
						//done standing up
						this.GetComponent<PolygonCollider2D>().points = standingCol;
						this.isStandingUp = false;
						offset.y = 0;
						maxMoveSpeed = 10;
					}
					this.GetComponent<PolygonCollider2D>().offset = offset;


				}
			}

			//if (playerid == 1) print(crouched);
			jumpNow(false);
			Transform center = this.gameObject.transform.FindChild("Center");

            Vector3 reticlePos = GameObject.Find("Reticle" + playerid).transform.position;

            reticlePos.z = transform.position.z;
            firingVector = reticlePos - center.position;
            firingVector.Normalize();

			float rotZ = Mathf.Atan2(firingVector.y, firingVector.x) * Mathf.Rad2Deg; //moving the rotation of the center here

			if (heldItem1 && heldItem1.GetComponent<HeldItem>().rotating) {
				if (!spinningItem) {
					spinningItem = true;
					savedReticleLocation = reticlePos - this.transform.position;
				}
				heldItem1.transform.rotation = Quaternion.Euler(0,0,(rotZ + lastItemRotation) * ((heldItem1.transform.position.x - transform.position.x < 0) ? -1 : 1));

			} else {
				if (spinningItem) {
					lastItemRotation = heldItem1.transform.rotation.eulerAngles.z;
					spinningItem = false;
					GameObject.Find("Reticle" + playerid).transform.localPosition = savedReticleLocation;
					reticlePos = GameObject.Find("Reticle" + playerid).transform.position;
					reticlePos.z = transform.position.z;
					firingVector = reticlePos - center.position;
					firingVector.Normalize();
					rotZ = Mathf.Atan2(firingVector.y, firingVector.x) * Mathf.Rad2Deg; //moving the rotation of the center here


				}
				transform.FindChild("AimerBody").rotation = center.rotation = Quaternion.Euler(0, 0, rotZ);

				Vector3 centerScale = center.localScale;
				if(firingVector.x < 0 && (!heldItem1 || !heldItem1.GetComponent<player>())) { //set the y scale to be 0 in order to quickly set the correct orientation of gun when aiming behind yourself
					center.localScale = new Vector3(centerScale.x, -1 * Mathf.Abs(centerScale.y), centerScale.z);
				} else {
					center.localScale = new Vector3(centerScale.x, Mathf.Abs(centerScale.y), centerScale.z);
				}
			}


           
           // GetComponent<LineRenderer>().SetVertexCount(2);
            //GetComponent<LineRenderer>().SetPosition(0, center.position + .1f * Vector3.forward);
			//GetComponent<LineRenderer>().SetPosition(1, center.position + 2 * firingVector + .1f * Vector3.forward);
			//if (pickUpKey()) print(canPickup);
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
	void LateUpdate() {
		
	}
    public void switchPlanes() {
		return;
		//code is obsolete
		/*
        time = 0.0f;
        if (gameObject.layer != layer1Value) {
            //print("augh" + gameObject.layer);
            gameObject.layer = layer1Value;
            //transform.FindChild("TopTrigger").gameObject.layer = layer1Value;
            if (heldItem1 != null)
                heldItem1.layer = layer1Value;
            if (heldItem2 != null)
                heldItem2.layer = layer1Value;
			if (passiveItem != null)
				passiveItem.layer = layer1Value;
            transform.position = new Vector3(currentX, currentY, layer1Position);
        } else
        if (gameObject.layer != layer2Value) {
            //print("oof" + gameObject.layer);
            gameObject.layer = layer2Value;
            //transform.FindChild("TopTrigger").gameObject.layer = layer2Value;
            if (heldItem1 != null)
                heldItem1.layer = layer2Value;
            if (heldItem2 != null)
                heldItem2.layer = layer2Value;
			if (passiveItem != null)
				passiveItem.layer = layer2Value;
            transform.position = new Vector3(currentX, currentY, layer2Position);
        }
        this.GetComponent<GrappleLauncher>().SendMessage("Disconnect");*/
    }
	void throwWeapontr(Transform t) {
		if (heldItem1 == t)
			throwWeapont (0);
		else if (heldItem2 == t)
			throwWeapont (1);
		else if (passiveItem == t)
			throwWeapont (2);
		else return;
	}
	void throwWeapont(int i) { 
		throwWeapon(false, i);
	}
    void throwWeapon(bool b, int i) { //bool for dropping or throwing
		if (i == 0) {
			if (heldItem1 != null) {
				heldItem1.GetComponent<HeldItem> ().focus = null;
				//GameObject.Find ("MouseInput").SendMessage ("playerHasNotItem", playerid);
				heldItem1.SendMessage ("retriggerSoon", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
				if (heldItem1.GetComponent<PolygonCollider2D> ())
					heldItem1.GetComponent<PolygonCollider2D> ().isTrigger = false;
				timeSincePickup = 0;
				heldItem1.GetComponent<Rigidbody2D> ().isKinematic = false;
				if (b)
					heldItem1.GetComponent<Rigidbody2D> ().AddForce (heldItem1.GetComponent<HeldItem> ().throwForce * heldItem1.GetComponent<Rigidbody2D> ().mass * firingVector); //throw weapon
				heldItem1.GetComponent<Rigidbody2D> ().AddTorque (3);
				heldItem1.transform.parent = null;
				heldItem1.transform.localScale = Vector3.one;


				heldItem1.SendMessage ("unclick");
				heldItem1 = null;
			}
		} else if (i == 1) {
			if (heldItem2 != null) {
				heldItem2.GetComponent<HeldItem> ().focus = null;
				//GameObject.Find ("MouseInput").SendMessage ("playerHasNotItem2", playerid);
				heldItem2.SendMessage ("retriggerSoon", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
				if (heldItem2.GetComponent<PolygonCollider2D> ())
					heldItem2.GetComponent<PolygonCollider2D> ().isTrigger = false;

				timeSincePickup = 0;
				heldItem2.GetComponent<Rigidbody2D> ().isKinematic = false;
				if (b)
					heldItem2.GetComponent<Rigidbody2D> ().AddForce (heldItem1.GetComponent<HeldItem> ().throwForce * heldItem2.GetComponent<Rigidbody2D> ().mass * firingVector); //throw weapon
				heldItem2.GetComponent<Rigidbody2D> ().AddTorque (3);
				heldItem2.transform.parent = null;
				heldItem2.transform.localScale = Vector3.one;

				heldItem2.SendMessage ("unclick");
				heldItem2 = null;
				canPickup = false;
			}
		} else if (i == 2) { //passive item
			if (passiveItem != null) {
				passiveItem.SendMessage("Drop",b?0:1);
				passiveItem.GetComponent<HeldItem> ().focus = null;
				passiveItem.SendMessage ("retriggerSoon", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
				if (passiveItem.GetComponent<PolygonCollider2D> ())
					passiveItem.GetComponent<PolygonCollider2D> ().isTrigger = false;

				timeSincePickup = 0;
				passiveItem.GetComponent<Rigidbody2D> ().isKinematic = false;
				if (b)
					passiveItem.GetComponent<Rigidbody2D> ().AddForce (80 * Random.insideUnitCircle); //throw weapon
				passiveItem.GetComponent<Rigidbody2D> ().AddTorque (3);
				passiveItem.transform.parent = null;
				passiveItem.transform.localScale = Vector3.one;
				passiveItem = null;
				canPickup = false;
			}
		}
       // canPickup = true;
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
       // if (name == "Player1" )print(Input.GetAxis("HorizontalPD" + joystickID));

		return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) < 0 || (joystickController?(Input.GetAxis("HorizontalPD" + joystickID) < 0):false));
    }
    bool goRight() {
		return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0 || (joystickController?(Input.GetAxis("HorizontalPD" + joystickID) > 0):false));
    }
    bool goDown() {
		return (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid))  == -1 || (joystickController?(Input.GetAxis("VerticalPD" + joystickID) == -1):false));
    }

    bool jump() {
				if (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) == 1 || (joystickController?(Input.GetAxis("VerticalPD" + joystickID) == 1):false)) {

            return true;
        } else {
            return false;
        }
    }
    bool pickUpKey() {
		return (!death && !tempDisabled && Input.GetAxis("UseP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0);
    }
    void OnTriggerEnter2D(Collider2D col) {
        Rigidbody2D colR = col.GetComponent<Rigidbody2D>();
		if ((!heldItem1 || (heldItem1.CompareTag("DualItem") && col.CompareTag("DualItem"))) &&
			(col.CompareTag("Item") || col.CompareTag("DualItem") || (col.CompareTag("Player") && (!col.GetComponent<player>() || col.GetComponent<Health>().dead))) &&  //It is an item to pick up
            col.GetComponent<HeldItem>() && //it can be held
			(col.GetComponent<HeldItem>().focus == null || col.GetComponent<Health>()) &&  //check focus null so you can't steal weapons
            timeSincePickup > .2f && //had enough time
            !colR.isKinematic) {  //not what I am holding
            canPickup = true;
        } else {
            if (colR && !colR.isKinematic) canPickup = false;
        }

    }
    void OnTriggerExit2D(Collider2D col) {
		if (punchable && col.gameObject == punchable.gameObject) punchable = null;
		Rigidbody2D colR = col.GetComponent<Rigidbody2D>();

		if ((!heldItem1 || (heldItem1.CompareTag("DualItem") && col.CompareTag("DualItem"))) &&
			(col.CompareTag("Item") || col.CompareTag("DualItem") || (col.CompareTag("Player") && (!col.GetComponent<player>() || col.GetComponent<Health>().dead))) &&  //It is an item to pick up
			col.GetComponent<HeldItem>() && //it can be held
			(col.GetComponent<HeldItem>().focus == null || col.GetComponent<Health>()) &&  //check focus null so you can't steal weapons
			timeSincePickup > .2f && //had enough time
			!colR.isKinematic) {  //not what I am holding
            canPickup = false;
        }
    }
    void OnTriggerStay2D(Collider2D col) {

		if (col.GetComponent<player>() || col.GetComponent<ItemBox>()) {
			//print("setting punchable" + col.name);
			punchable = col.transform;
		}
		/*if ((col.CompareTag("Platform") || col.CompareTag("Player") || col.CompareTag("Item")) && !col.isTrigger) {
			jumped = false;
		}*/
		if ((col.CompareTag("Item") || col.CompareTag("DualItem") || (col.CompareTag("Player") && (!col.GetComponent<player>() || col.GetComponent<Health>().dead))) && ((col.GetComponent<HeldItem>() && col.GetComponent<HeldItem>().focus == null) || col.GetComponent<Health>()) && timeSincePickup > .2f) { //check parent null so you can't steal weapons

            if (pickUpKey()) {
				if (col.GetComponent<PassivePickup> ()) {
					if (passiveItem) {
						//drop current passiveItem
						throwWeapon(true, 2);

					}
					col.transform.parent = this.transform;
					col.transform.localPosition = col.GetComponent<PassivePickup> ().offset;
					col.SendMessage("Pickup",this.gameObject);
					Physics2D.IgnoreCollision (col, GetComponent<Collider2D> ());
					timeSincePickup = 0;
					passiveItem = col.gameObject;
					passiveItem.GetComponent<HeldItem> ().focus = this.gameObject;
					if (passiveItem.GetComponent<PolygonCollider2D> ())
						passiveItem.GetComponent<PolygonCollider2D> ().isTrigger = true;
					
					col.SendMessage ("ignoreColl", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
					passiveItem.GetComponent<Rigidbody2D> ().isKinematic = true;
					passiveItem.transform.localScale = Vector3.one;
					passiveItem.transform.rotation = Quaternion.identity;
					canPickup = false;
				} else {
					if (heldItem1 == null) {
						Physics2D.IgnoreCollision (col, GetComponent<Collider2D> ());
						timeSincePickup = 0;
						heldItem1 = col.gameObject;
						heldItem1.GetComponent<HeldItem> ().focus = this.gameObject;
						if (heldItem1.GetComponent<PolygonCollider2D> ())
							heldItem1.GetComponent<PolygonCollider2D> ().isTrigger = true;
						else {
							foreach (BoxCollider2D b in heldItem1.GetComponents<BoxCollider2D>()) {
								b.isTrigger = true;
							}
						}
						col.SendMessage ("ignoreColl", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
						Transform center = this.gameObject.transform.FindChild ("Center");
						heldItem1.GetComponent<Rigidbody2D> ().isKinematic = true;
						heldItem1.transform.SetParent (center, false);
						heldItem1.transform.localScale = Vector3.one;

						//	heldItem1.transform.localScale = new Vector3(Mathf.Abs(heldItem1.transform.localScale.x),Mathf.Abs(heldItem1.transform.localScale.y),Mathf.Abs(heldItem1.transform.localScale.z));
						heldItem1.transform.position = (center.transform.position + .7f * this.firingVector);
						heldItem1.transform.rotation = center.transform.rotation;
						if (heldItem1.GetComponent<gun> () || heldItem1.GetComponent<PortalGun> ()) {
							heldItem1.SendMessage ("SetPlayerID", playerid);
						}
						canPickup = false;
					} else if (heldItem2 == null && !col.GetComponent<player> () && heldItem1.CompareTag ("DualItem") && col.CompareTag ("DualItem")) {

						this.GetComponent<GrappleLauncher> ().SendMessage ("Disconnect");
						Physics2D.IgnoreCollision (col, GetComponent<Collider2D> ());
						timeSincePickup = 0;
						heldItem2 = col.gameObject;

						heldItem2.GetComponent<HeldItem> ().focus = this.gameObject;
						if (heldItem2.GetComponent<PolygonCollider2D> ())
							heldItem2.GetComponent<PolygonCollider2D> ().isTrigger = true;
						else {
							foreach (BoxCollider2D b in heldItem2.GetComponents<BoxCollider2D>()) {
								b.isTrigger = true;
							}
						}
						col.SendMessage ("ignoreColl", this.GetComponent<Collider2D> ().GetComponent<Collider2D> ());
						Transform center = this.gameObject.transform.FindChild ("Center");
						heldItem2.GetComponent<Rigidbody2D> ().isKinematic = true;
						heldItem2.transform.SetParent (center, false);
						heldItem2.transform.localScale = Vector3.one;

						//	heldItem2.transform.localScale = new Vector3(Mathf.Abs(heldItem2.transform.localScale.x),Mathf.Abs(heldItem2.transform.localScale.y),Mathf.Abs(heldItem2.transform.localScale.z));
						heldItem2.transform.position = (center.transform.position + .4f * this.firingVector);
						heldItem2.transform.rotation = center.transform.rotation;
						if (heldItem2.GetComponent<gun> ()) {
							heldItem2.SendMessage ("SetPlayerID", playerid);
						}
						canPickup = false;
					} else {

					}
				}
            }
        }
    }
    void Death() {
        death = true;
        this.GetComponent<Rigidbody2D>().freezeRotation = false;
        //this.GetComponent<LineRenderer>().SetVertexCount(0);
		transform.FindChild("AimerBody").FindChild("Aimer").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        this.gameObject.tag = "Item";
        if (heldItem1 != null) {
            throwWeapon(false, 0);
        }
        if (heldItem2 != null) {
            throwWeapon(false, 1);
        }
		if (passiveItem != null) {
			throwWeapon(false, 2);
		}
    }
    void NotDeath() {
        death = false;
        this.GetComponent<Rigidbody2D>().freezeRotation = true;
		transform.FindChild("AimerBody").FindChild("Aimer").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		//this.GetComponent<LineRenderer>().SetVertexCount(2);
        this.gameObject.tag = "Player";

    }
	void Punch() {
		//print(punchable.name);
		if (punchable != null && punchTime > .3f && !death) {
			//print("I actually can punch");
			punchTime = 0;
			if (punchable.GetComponent<player>()) {
				Rigidbody2D playerPunch = punchable.GetComponent<Rigidbody2D>();
				playerPunch.AddForce(300 * (punchable.gameObject.transform.position - transform.position));
				playerPunch.AddForce(Vector2.up * 450);
				object o = 0;
				punchable.SendMessage("throwWeapont",o);
				o = 1;
				punchable.SendMessage("throwWeapont",o);
			} else {
				//print("drop it");
				punchable.SendMessage("DropItem");
			}

		}
	}
	void jumpNow(bool b) {
		
		if (jetpack) {
			if (jump ()) { //we are using jetpackPlaying because there is a delay for it to stop
				if (transform.FindChild ("Jetpack") && !jetpackPlaying) {
					transform.FindChild ("Jetpack").GetComponentInChildren<ParticleSystem> ().Play ();
					jetpackPlaying = true;
				}
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (0, 3000 * Time.deltaTime, 0));
			} else {
				
				if (transform.FindChild ("Jetpack") && jetpackPlaying) {
					transform.FindChild ("Jetpack").GetComponentInChildren<ParticleSystem> ().Stop ();
					jetpackPlaying = false;
				}
				
			}
		} else {
			Transform center = this.gameObject.transform.FindChild("Center");
			RaycastHit2D[] hits = Physics2D.RaycastAll(center.position, Vector2.down, 1.4f);
			bool hitValid = false;
			foreach (RaycastHit2D hit in hits) {
				Collider2D col = hit.collider;
				if ((col.CompareTag("Platform") || col.CompareTag("Item")) && !col.isTrigger && !col.GetComponent<PassivePickup>()) {
					hitValid = true;
					//Debug.DrawLine (center.position, hit.point, Color.green, 20f);
					//Debug.Log (hit.transform.name);
					break;
				}
			}
			if ((b || jump ()) && hitValid && Time.time - jumpTime > .05f) {
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, 0); //slowing as we hit the floor
				GetComponent<Rigidbody2D> ().AddForce (new Vector3 (0, 800, 0));
				jumpTime = Time.time;
				//jumped = true;
			}
		}

	}
	void DisablePlayers() {
		tempDisabled = true;
	}
	void EnablePlayers() {
		tempDisabled = false;
	}
	void OnDestroy() {
	//	print("wtf why");
	}
}