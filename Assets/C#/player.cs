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
	public AnimationHandler myAnim;

	public Vector2 leftGunPos, rightGunPos;


	private Transform center1, center2, aimingParent, aimer, aimerBody, reticle;
	private Rigidbody2D myRigid;
	private Collider2D myCollider;
	private PolygonCollider2D myPolygon;
	private SpriteRenderer mySprite;


   // static float layer1Position = 0;
   // static float layer2Position = 1;

//    static int layer1Value = 8;
  //  static int layer2Value = 9;

	public int joystickID;
	public int mouseID;


    private float time = 0.0f;

    void Start() {
		aimingParent = transform.FindChild("AimingParent"); //aiming parent retains recoil
		center1 = aimingParent.FindChild("CenterR"); //center1 holds items and weapons on one side
		center2 = aimingParent.FindChild("CenterL"); //center holds items and weapons on the other
		aimerBody = aimingParent.FindChild("AimerBody"); //aimerbody is what holds the aimer for easy rotation and scaling
		aimer = aimerBody.FindChild("Aimer"); //aimer is a sprite
		reticle = GameObject.Find("Reticle" + playerid).transform; //where the reticle we will aim to will be
        GameObject.Find("Reticle" + playerid).transform.position = transform.position;
		myCollider = this.GetComponent<Collider2D>();
		mySprite = transform.FindChild("Hip").GetComponent<SpriteRenderer> ();
		myRigid = this.GetComponent<Rigidbody2D>();
        //switchedKey = true;
		myPolygon = this.GetComponent<PolygonCollider2D>();
		Vector2[] curCol = 	myPolygon.points;

		standingCol = new Vector2[curCol.Length];
		crouchingCol = new Vector2[curCol.Length];
		for (int i = 0; i < curCol.Length; i++) {
			standingCol[i] = crouchingCol[i] = curCol[i];
		}
		crouchingCol[4] = new Vector2(crouchingCol[4].x, -.3f);
		crouchingCol[3] = new Vector2(crouchingCol[3].x, (crouchingCol[2].y-crouchingCol[3].y));
		crouchingCol[2] = new Vector2(crouchingCol[2].x, -.3f);

        tag = "Player";
        canMoveRight = true;
        canMoveLeft = true;
    }

    void Update() {
		isAirborne ();
        //CrashDetector.SetExePoint("Whateverelse");
        //if (playerid == 1) print("Start update function player");
		if (tempDisabled) {
			aimingParent.GetComponent<RecoilSimulator>().SendMessage("StopRotation");
			aimingParent.rotation = Quaternion.Euler(0, 0, 0);
			//this.mySprite.flipX = false;

		}

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
			float maxMoveSpeedRevised = (skateBoard && Mathf.Abs(myRigid.velocity.y) > .01f) ? maxMoveSpeed/10 : maxMoveSpeed ;
			float force = 0;
			if (myRigid.velocity.x > -1 * maxMoveSpeedRevised && canMoveLeft && goLeft()) {
				myAnim.direction = true;
				float f = Input.GetAxis("HorizontalPD" + joystickID);
				if (f != 0) {
					force = -1;
				} else {
					force = (this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : -1);
				}
				myRigid.AddForce(force * new Vector3(40, 0, 0));
            }
			if (myRigid.velocity.x < maxMoveSpeedRevised && canMoveRight && goRight()) {
				myAnim.direction = false;
				float f = Input.GetAxis("HorizontalPD" + joystickID);
				if (f != 0) {
					force = 1;
				} else {
					force = (this.joystickController ? (Input.GetAxis("HorizontalPJ" + joystickID)) : 1);
				}
				myRigid.AddForce(force * new Vector3(40, 0, 0));            
			}
			myAnim.moving = (goRight() || goLeft());
		
            if (goDown()) {
				if (!crouched) myPolygon.points = crouchingCol;
				crouched = true;
				myAnim.crouching = true;
				this.isStandingUp = false;
                myRigid.AddForce(new Vector3(0, -10, 0));
				maxMoveSpeed = 0;
			} else {
				if (crouched) {
					this.isStandingUp = true;
				}
				crouched = false;
				myAnim.crouching = false;
				if (isStandingUp) {
					//start slowly moving upwards
					Vector2 offset = myPolygon.offset;
					offset.y-=4*Time.deltaTime;
					if (offset.y < -1f) {
						//done standing up
						myPolygon.points = standingCol;
						this.isStandingUp = false;
						offset.y = 0;
						maxMoveSpeed = 10;
					}
					myPolygon.offset = offset;


				}
			}

			//if (playerid == 1) print(crouched);
			jumpNow(false);
			//Transform center = this.gameObject.transform.FindChild("AimingParent");

            Vector3 reticlePos = reticle.position;

            reticlePos.z = transform.position.z;
            firingVector = reticlePos - aimingParent.position;
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
					reticle.localPosition = savedReticleLocation;
					reticlePos = reticle.position;
					reticlePos.z = transform.position.z;
					firingVector = reticlePos - aimingParent.position;
					firingVector.Normalize();
					rotZ = Mathf.Atan2(firingVector.y, firingVector.x) * Mathf.Rad2Deg; //moving the rotation of the center here


				}
				aimerBody.localRotation = center1.localRotation = center2.localRotation = Quaternion.Euler(0, 0, rotZ);
				//if(firingVector.x < 0 && (!heldItem1 || !heldItem1.GetComponent<player>())) { //set the y scale to be 0 in order to quickly set the correct orientation of gun when aiming behind yourself

				Vector3 centerScale = center1.localScale;
				if (heldItem1) {
					if (mySprite.flipX) {
						heldItem1.transform.localScale = new Vector3 (centerScale.x, -1 * Mathf.Abs (centerScale.y), centerScale.z);
						if (heldItem2) {
							heldItem2.transform.localScale = new Vector3 (centerScale.x, -1 * Mathf.Abs (centerScale.y), centerScale.z);
						}
						/*center1.localScale = new Vector3(centerScale.x, -1 * Mathf.Abs(centerScale.y), centerScale.z);
						center2.localScale = new Vector3(centerScale.x, -1 * Mathf.Abs(centerScale.y), centerScale.z);*/
					} else {
						heldItem1.transform.localScale = Vector3.one;
						if (heldItem2) {
							heldItem2.transform.localScale = Vector3.one;
						}
					}
				} else {
					center1.localScale = new Vector3(centerScale.x, Mathf.Abs(centerScale.y), centerScale.z);
					center2.localScale = new Vector3(centerScale.x, Mathf.Abs(centerScale.y), centerScale.z);
				}
			}
			if (pickUpKey() && timeSincePickup > .2f && (!canPickup || (heldItem1 && heldItem2))) {
                //drop weapon
                timeSincePickup = 0;
                if (heldItem2 != null)
                    throwWeapon(true, 1);
                else if (heldItem1 != null)
                    throwWeapon(true, 0);
            }
        }


    }
	void LateUpdate() {
		
	}
    public void switchPlanes() {
		return;
		//obselete code
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
				this.aimingParent.SendMessage("StopRotation");
				aimingParent.localRotation = Quaternion.Euler(0,0,0);
				heldItem1.GetComponent<HeldItem> ().focus = null;
				//GameObject.Find ("MouseInput").SendMessage ("playerHasNotItem", playerid);
				heldItem1.SendMessage ("retriggerSoon", myCollider);
				PolygonCollider2D pg;
				if (pg = heldItem1.GetComponent<PolygonCollider2D> ())
					pg.isTrigger = false;
				timeSincePickup = 0;
				Rigidbody2D rg = heldItem1.GetComponent<Rigidbody2D>();
				rg.isKinematic = false;
				if (b)
					rg.AddForce (heldItem1.GetComponent<HeldItem> ().throwForce * rg.mass * firingVector); //throw weapon
				rg.AddTorque (3);
				heldItem1.transform.parent = null;
				heldItem1.transform.localScale = Vector3.one;
				RecoilSimulator rs;
				if (rs = heldItem1.GetComponent<RecoilSimulator>()) {
					rs.SendMessage("StopRotation");
					aimingParent.localRotation = Quaternion.Euler(0,0,0);
				}


				heldItem1.SendMessage ("unclick");
				heldItem1 = null;
				myAnim.heldType = 0;
			}
		} else if (i == 1) {
			if (heldItem2 != null) {
				heldItem2.GetComponent<HeldItem> ().focus = null;
				//GameObject.Find ("MouseInput").SendMessage ("playerHasNotItem2", playerid);
				heldItem2.SendMessage ("retriggerSoon",myCollider);
				PolygonCollider2D pg;
				if (pg = heldItem2.GetComponent<PolygonCollider2D> ())
					pg.isTrigger = false;

				timeSincePickup = 0;
				Rigidbody2D rg = heldItem2.GetComponent<Rigidbody2D>();
				rg.isKinematic = false;
				if (b)
					rg.AddForce (heldItem1.GetComponent<HeldItem> ().throwForce * rg.mass * firingVector); //throw weapon
				heldItem2.GetComponent<Rigidbody2D> ().AddTorque (3);
				heldItem2.transform.parent = null;
				heldItem2.transform.localScale = Vector3.one;
				if (heldItem2.GetComponent<RecoilSimulator>()) {
					heldItem2.GetComponent<RecoilSimulator>().SendMessage("StopRotation");
					aimingParent.localRotation = Quaternion.Euler(0,0,0);
				}
				heldItem2.SendMessage ("unclick");
				heldItem2 = null;
				canPickup = false;
				myAnim.heldType = 1;
			}
		} else if (i == 2) { //passive item
			if (passiveItem != null) {
				passiveItem.SendMessage("Drop",b?0:1);
				passiveItem.GetComponent<HeldItem> ().focus = null;
				passiveItem.SendMessage ("retriggerSoon", myCollider);
				if (passiveItem.GetComponent<PolygonCollider2D> ())
					passiveItem.GetComponent<PolygonCollider2D> ().isTrigger = false;

				timeSincePickup = 0;
				Rigidbody2D rg = passiveItem.GetComponent<Rigidbody2D>();
				rg.isKinematic = false;
				if (b)
					rg.AddForce (80 * Random.insideUnitCircle); //throw weapon
				rg.AddTorque (3);
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

		return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) < -.4f || (joystickController?(Input.GetAxis("HorizontalPD" + joystickID) < 0):false));
    }
    bool goRight() {
		return (Input.GetAxis("HorizontalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > .4f || (joystickController?(Input.GetAxis("HorizontalPD" + joystickID) > 0):false));
    }
    bool goDown() {
		return (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid))  < -.5f || (joystickController?(Input.GetAxis("VerticalPD" + joystickID) == -1):false));
    }
	bool jump() {
		return jump(false);
	}
    bool jump(bool usingJetpack) {
		if (!death && Input.GetAxis("VerticalP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) >= 1 || (joystickController?(Input.GetAxis("VerticalPD" + joystickID) == 1):false)) {
			return isAirborne (usingJetpack);
        } else {
			
            return false;
        }
    }
	bool isAirborne() {
		return isAirborne (false);
	}
	bool isAirborne(bool usingJetpack) {
		RaycastHit2D[] hits = Physics2D.BoxCastAll (center1.position, new Vector2(.5f, .5f), 180 , Vector2.down, 1.4f);
		//RaycastHit2D[] hits = Physics2D.RaycastAll(center1.position, Vector2.down, 1.4f);
		bool hitValid = usingJetpack; 
		foreach (RaycastHit2D hit in hits) {
			Collider2D col = hit.collider;
			if ((col.CompareTag("Platform") || (col.CompareTag("Item")) && !col.isTrigger)) {
				hitValid = true;
				//Debug.DrawLine (center.position, hit.point, Color.green, 20f);
				//Debug.Log (hit.transform.name);
				break;
			}
		}
		myAnim.airborne = !hitValid;
		return hitValid; //hitvalid is if we can jump, because something may be in our way
	}
    bool pickUpKey() {
		return (!death && !tempDisabled && Input.GetAxis("UseP" + (joystickController ? "J" : "") + (joystickController ? joystickID : playerid)) > 0);
    }
	void VerifyItemCollision(Collider2D col, bool toPickup) {
		Rigidbody2D colR = col.GetComponent<Rigidbody2D>();
		HeldItem hl = col.GetComponent<HeldItem>();
		if ((!heldItem1 || (heldItem1.CompareTag("DualItem") && col.CompareTag("DualItem"))) && //if no helditem1 or dual items are possible
			(col.CompareTag("Item") || col.CompareTag("DualItem") || ((col.CompareTag("PlayerGibs") || col.CompareTag("Player")) && (!col.GetComponent<player>() || col.GetComponent<Health>().dead))) &&  //It is an item to pick up
			hl && //it can be held
			(hl.focus == null || col.GetComponent<Health>()) &&  //check focus null so you can't steal weapons
			timeSincePickup > .2f && //had enough time
			!colR.isKinematic) {  //not what I am holding
			canPickup = toPickup;
		} else {
			if (toPickup && colR && !colR.isKinematic) canPickup = false;
		}
	}
    void OnTriggerEnter2D(Collider2D col) {
		VerifyItemCollision(col, true);
    }
    void OnTriggerExit2D(Collider2D col) {
		if (punchable && col.gameObject == punchable.gameObject) punchable = null;
		VerifyItemCollision(col, false);
    }
    void OnTriggerStay2D(Collider2D col) {

		VerifyItemCollision(col, true); //makes picking up things easier

		player pl = col.GetComponent<player>();
		HeldItem hi = col.GetComponent<HeldItem>();

		if (col.GetComponent<player>() || col.GetComponent<ItemBox>()) {
			//print("setting punchable" + col.name);
			punchable = col.transform;
		}

		/*if ((col.CompareTag("Platform") || col.CompareTag("Player") || col.CompareTag("Item")) && !col.isTrigger) {
			jumped = false;
		}*/

		if ((col.CompareTag("Item") || col.CompareTag("DualItem") || ((col.CompareTag("PlayerGibs") || col.CompareTag("Player")) && (!pl || col.GetComponent<Health>().dead))) && ((hi && hi.focus == null) || col.GetComponent<Health>()) && timeSincePickup > .2f) { //check parent null so you can't steal weapons

            if (pickUpKey()) {
				PassivePickup p;
				if (p = col.GetComponent<PassivePickup> ()) { //assign values to passiveItem
					if (passiveItem) {
						//drop current passiveItem
						throwWeapon(true, 2);

					}
					col.transform.parent = this.transform;
					col.transform.localPosition = p.offset;
					col.SendMessage("Pickup",this.gameObject);
					Physics2D.IgnoreCollision (col,myCollider);
					timeSincePickup = 0;
					passiveItem = col.gameObject;
					hi.focus = this.gameObject;
					PolygonCollider2D pol;
					if (pol = passiveItem.GetComponent<PolygonCollider2D> ())
						pol.isTrigger = true;
					
					col.SendMessage ("ignoreColl", myCollider);
					passiveItem.GetComponent<Rigidbody2D> ().isKinematic = true;
					passiveItem.transform.localScale = Vector3.one;
					passiveItem.transform.rotation = Quaternion.identity;
					canPickup = false;
				} else {
					if (heldItem1 == null) { //assign values to helditem1
						Physics2D.IgnoreCollision (col, myCollider);
						timeSincePickup = 0;
						heldItem1 = col.gameObject;
						heldItem1.GetComponent<HeldItem> ().focus = this.gameObject;
						PolygonCollider2D pc;
						if (pc = heldItem1.GetComponent<PolygonCollider2D> ())
							pc.isTrigger = true;
						else {
							foreach (BoxCollider2D b in heldItem1.GetComponents<BoxCollider2D>()) {
								b.isTrigger = true;
							}
						}
						col.SendMessage ("ignoreColl", myCollider);
						//Transform center = this.gameObject.transform.FindChild ("Center");
						heldItem1.GetComponent<Rigidbody2D> ().isKinematic = true;
						heldItem1.transform.SetParent (center1, false);
						heldItem1.transform.localScale = Vector3.one;
						//transfer all recoil parts
						RecoilSimulator heldR = heldItem1.GetComponent<RecoilSimulator>();
						RecoilSimulator aimingR = aimingParent.GetComponent<RecoilSimulator>();
						if (heldR) {
							//aimingR.maxAngle = heldR.maxAngle;
							aimingR.pullDelay = heldR.pullDelay;
							aimingR.recoverSpeed = heldR.recoverSpeed;
						}
						//	heldItem1.transform.localScale = new Vector3(Mathf.Abs(heldItem1.transform.localScale.x),Mathf.Abs(heldItem1.transform.localScale.y),Mathf.Abs(heldItem1.transform.localScale.z));
						//heldItem1.transform.position = (center.transform.position + .7f * this.firingVector);
						heldItem1.transform.localPosition = rightGunPos;//aimerBody.transform.FindChild("Held1").transform.localPosition;
						heldItem1.transform.rotation = center1.transform.rotation;
						if (heldItem1.GetComponent<gun> () || heldItem1.GetComponent<PortalGun> ()) {
							heldItem1.SendMessage ("SetPlayerID", playerid);
						}
						canPickup = false;
						if (heldItem1.CompareTag ("DualItem")) {
							myAnim.heldType = 1;
						} else {
							myAnim.heldType = 3;
						}
					} else if (heldItem2 == null && !pl && heldItem1.CompareTag ("DualItem") && col.CompareTag ("DualItem")) { //assign values to helditem2

						this.GetComponent<GrappleLauncher> ().SendMessage ("Disconnect"); 
						Physics2D.IgnoreCollision (col,myCollider);
						timeSincePickup = 0;
						heldItem2 = col.gameObject;

						heldItem2.GetComponent<HeldItem> ().focus = this.gameObject;
						PolygonCollider2D pol;
						if (pol = heldItem2.GetComponent<PolygonCollider2D> ())
							pol.isTrigger = true;
						else {
							foreach (BoxCollider2D b in heldItem2.GetComponents<BoxCollider2D>()) {
								b.isTrigger = true;
							}
						}
						col.SendMessage ("ignoreColl", myCollider);
						//Transform center = this.gameObject.transform.FindChild ("Center");
						heldItem2.GetComponent<Rigidbody2D> ().isKinematic = true;
						heldItem2.transform.SetParent (center2, false);
						heldItem2.transform.localScale = Vector3.one;
						//transfer recoil
						RecoilSimulator heldR = heldItem2.GetComponent<RecoilSimulator>();
						RecoilSimulator aimingR = aimingParent.GetComponent<RecoilSimulator>();
						if (heldR) {
							//aimingR.maxAngle = heldR.maxAngle;
							aimingR.pullDelay = heldR.pullDelay;
							aimingR.recoverSpeed = heldR.recoverSpeed;
						}

						//	heldItem2.transform.localScale = new Vector3(Mathf.Abs(heldItem2.transform.localScale.x),Mathf.Abs(heldItem2.transform.localScale.y),Mathf.Abs(heldItem2.transform.localScale.z));
						//heldItem2.transform.position = (center.transform.position + .4f * this.firingVector);
						heldItem2.transform.localPosition = leftGunPos;//this.aimerBody.transform.FindChild("Held2").transform.localPosition;

						heldItem2.transform.rotation = center2.transform.rotation;
						if (heldItem2.GetComponent<gun> ()) {
							heldItem2.SendMessage ("SetPlayerID", playerid);
						}
						myAnim.heldType = 2;
						canPickup = false;
					} else {

					}
				}
            }
        }
    }
    void Death() {
        death = true;
		myAnim.Death ();
        this.myRigid.freezeRotation = false;
        //this.GetComponent<LineRenderer>().SetVertexCount(0);
		aimer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
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
		//stop crouching
		Vector2 offset = myPolygon.offset;
		offset.y = -1.1f;
		//myPolygon.points = standingCol;
		this.isStandingUp = false;
		offset.y = 0;
		maxMoveSpeed = 10;
		//myPolygon.offset = offset;
    }
    void NotDeath() {
		myAnim.NotDeath ();
        death = false;
		crouched = false;
		myPolygon.points = standingCol;
		this.isStandingUp = false;
		maxMoveSpeed = 10;
		myRigid.freezeRotation = true;
		aimer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		//this.GetComponent<LineRenderer>().SetVertexCount(2);
        this.gameObject.tag = "Player";

    }
	void Punch() {
		//print(punchable.name);
		if (punchTime > .3f && !death) {
			//print("I actually can punch");
			punchTime = 0;
			//punch animation here
			myAnim.Punch ();
			if (punchable) {
				if (punchable.GetComponent<player> ()) {
					
					Rigidbody2D playerPunch = punchable.GetComponent<Rigidbody2D> ();
					playerPunch.AddForce (300 * (punchable.gameObject.transform.position - transform.position));
					playerPunch.AddForce (Vector2.up * 450);
					object o = 0;
					punchable.SendMessage ("throwWeapont", o);
					o = 1;
					punchable.SendMessage ("throwWeapont", o);
				} else {
					//print("drop it");
					punchable.SendMessage ("DropItem");
				}
			}

		}
	}
	void jumpNow(bool b) {
		
		if (jetpack) {
			Transform myJetpack = transform.FindChild("Jetpack");
			if (jump (true)) { //we are using jetpackPlaying because there is a delay for it to stop
				if (myJetpack && !jetpackPlaying) {
					myJetpack.GetComponentInChildren<ParticleSystem> ().Play ();
					jetpackPlaying = true;
				}
				myRigid.AddForce (new Vector3 (0, 3000 * Time.deltaTime, 0));
			} else {
				
				if (myJetpack && jetpackPlaying) {
					myJetpack.GetComponentInChildren<ParticleSystem> ().Stop ();
					jetpackPlaying = false;
				}
				
			}
		} else {
			//Transform center = this.gameObject.transform.FindChild("Center");

			if ((b || jump ()) && Time.time - jumpTime > .05f) {
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