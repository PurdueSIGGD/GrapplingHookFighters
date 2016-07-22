using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour {
    /* Notes for changing animations
     * For each player, you have to change the following values if their outfit changes:
     * Change sprites: larm, rarm, rleg, lleg, grapplearm, hip, torso, head
     * Change animators: L1, R1, ArmL, ArmR
     * Change gibholder
     */


	public SpriteRenderer hip, legR, legL, torso, head, armL, armR;
	public Animator legRA, legLA, armLA, armRA;
	public Sprite singleR, singleL, dualR, dualL, heavyR, heavyL, legLS, legRS, armLS, armRS;

	public Transform AimingParent;
	//I need aiming parent to set the rotation to zero if we change direction, because it is causing problems with my resetting of positions

	public bool airborne, crouching, moving, direction, grappling; //direction: false right, true left;
	public Color startColor = Color.white;

	public int heldType; //0: nothing, 1: single, 2: dual, 3: heavy
	private int lastHeldType;


	private bool lastCrouching, lastAirborne, lastDirection, lastMoving; //used for knowing when to flip
	private bool punchIndex, death;

	private Vector2 localArmL, localArmR;
	private Transform centerR, centerL;

	// Use this for initialization
	void Start () {
		hip.color = legR.color = legL.color = torso.color = head.color = armL.color = armR.color = startColor;
		//armLA.Stop ();
		//armRA.Stop ();
		armL.sprite = armLS;
		armR.sprite = armRS;



		localArmL = armL.transform.localPosition;
		localArmR = armR.transform.localPosition;
		centerR = armR.transform.parent;
		centerL = armL.transform.parent;
	}
	void Update() {
		if (death)
			return;
		hip.flipX = legR.flipX = legL.flipX = torso.flipX = head.flipX = armL.flipX = armR.flipX = direction;

		if (direction != lastDirection) {
			//This is my method of hard coding in position when the player changes direction. It uses the hip as a center axis of flipping

			AimingParent.rotation = Quaternion.Euler(Vector3.zero);
			if (direction) {
				legR.transform.position = new Vector3(hip.transform.position.x - (legR.transform.position.x - hip.transform.position.x), legR.transform.position.y, legR.transform.position.z);
				legL.transform.position = new Vector3(hip.transform.position.x - (legL.transform.position.x - hip.transform.position.x), legL.transform.position.y, legL.transform.position.z);
				torso.transform.position = new Vector3(hip.transform.position.x - (torso.transform.position.x - hip.transform.position.x), torso.transform.position.y, torso.transform.position.z);
				head.transform.position = new Vector3(hip.transform.position.x - (head.transform.position.x - hip.transform.position.x), head.transform.position.y, head.transform.position.z);
				centerR.transform.position = new Vector3(hip.transform.position.x - (centerR.transform.position.x - hip.transform.position.x), centerR.transform.position.y, centerR.transform.position.z);
				centerL.transform.position = new Vector3(hip.transform.position.x - (centerL.transform.position.x - hip.transform.position.x), centerL.transform.position.y, centerL.transform.position.z);
				//print ("switching dir");
				armR.transform.localPosition = new Vector3 (0.086f, 0.349f, armR.transform.localPosition.z);
				armR.transform.localEulerAngles = new Vector3 (0, 0, 180);
				armL.transform.localPosition = new Vector3 (-0.222f, 0.349f, armL.transform.localPosition.z);
				armL.transform.localEulerAngles = new Vector3 (0, 0, 180);
				//changing the local position and rotation of the arm to match with each center
				// this feels so dirty
				// -0.072 -> 0.349 (y)
				// 0.021 -> 0.086, -0.222 (x)
				// rotation: 180


				//0.1167769, 01501417 (anchor)
			} else {
				legR.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - legR.transform.position.x), legR.transform.position.y, legR.transform.position.z);
				legL.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - legL.transform.position.x), legL.transform.position.y, legL.transform.position.z);
				torso.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - torso.transform.position.x), torso.transform.position.y, torso.transform.position.z);
				head.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - head.transform.position.x), head.transform.position.y, head.transform.position.z);
				centerR.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - centerR.transform.position.x), centerR.transform.position.y, centerR.transform.position.z);
				centerL.transform.position = new Vector3(hip.transform.position.x + (hip.transform.position.x - centerL.transform.position.x), centerL.transform.position.y, centerL.transform.position.z);
				armL.transform.localPosition = localArmL;
				armR.transform.localPosition = localArmR;
				armR.transform.localEulerAngles = new Vector3 (0, 0, 0);
				armL.transform.localEulerAngles = new Vector3 (0, 0, 0);
			}

		}
	
		//______________Animation Handling for Arms _______________
		armRA.SetInteger("HeldType", heldType);
		if (armLA.isActiveAndEnabled)
			armLA.SetInteger("HeldType", heldType);
		switch (heldType) {
		case 0:


			break;
		case 1:

			break;
		case 2:

			break;
		case 3:

			break;
		}
		lastHeldType = heldType;


		//______________Animation Handling for Legs _______________
		//armL.enabled = !grappling;
		legRA.SetBool ("Airborne", airborne);
		legLA.SetBool ("Airborne", airborne);
		legRA.SetBool ("Crouching", crouching);
		legLA.SetBool ("Crouching", crouching);
		legRA.SetBool ("Running", moving);
		legLA.SetBool ("Running", moving);

		if (airborne) {
			if (airborne != lastAirborne) {
				legRA.Play ("MonkRAirborne");
				legLA.Play ("MonkLAirborne");
			}
		} else {
			
			if (moving) {
				
				if (crouching) {
					if (crouching != lastCrouching) {
						legRA.Play ("MonkRCrouch");
						legLA.Play ("MonkLCrouch");
					}
					//legRA.runtimeAnimatorController = crouchingR;
					//legLA.runtimeAnimatorController = crouchingL;
				} else {
					//legRA.runtimeAnimatorController = runningR;
					//legLA.runtimeAnimatorController = runningL;

					if (moving != lastMoving) {
						legRA.Play ("MonkRRunning");
						legLA.Play ("MonkLRunning");
					}

				}
			} else {
				if (moving != lastMoving && !crouching) {
					//legRA.Stop ();
					//legLA.Stop ();
					//legL.sprite = legLS;
					//legR.sprite = legRS;
					legRA.Play ("MonkRIdle");
					legLA.Play ("MonkLIdle");
				}
			}

		}
		lastDirection = direction;
		lastAirborne = airborne;
		lastMoving = moving;
		lastCrouching = crouching;
	}

	public void Punch() {
		punchIndex = !punchIndex;
		//armLA.Stop ();
		//armRA.Stop ();
		if (!armLA.isActiveAndEnabled) {
			armRA.Play ("MonkRPunch");
			armRA.SetTrigger ("Punch");
		} else {
			

			if (punchIndex) {
				armRA.Play ("MonkRPunch");
				armRA.SetTrigger ("Punch");

			} else {
				armLA.Play ("MonkLPunch");
				armLA.SetTrigger ("Punch");

			}
		}
	}
	public void Swing() {
		armRA.Play ("Swing");
	}
	public void Death() {
		death = true;
		legRA.SetBool ("Airborne", false);
		legLA.SetBool ("Airborne", false);
		legRA.SetBool ("Crouching", false);
		legLA.SetBool ("Crouching", false);
		legRA.SetBool ("Running", false);
		legLA.SetBool ("Running", false);
		moving = airborne = crouching = false;
		//armRA.SetInteger("HeldType", 0);
		//armLA.SetInteger("HeldType", 0);
		hip.enabled = legR.enabled = legL.enabled = torso.enabled = head.enabled = armL.enabled = armR.enabled = false;

	}
	public void NotDeath() {
		heldType = 0;
		death = false;
		legRA.SetBool ("Airborne", false);
		legLA.SetBool ("Airborne", false);
		legRA.SetBool ("Crouching", false);
		legLA.SetBool ("Crouching", false);
		legRA.SetBool ("Running", false);
		legLA.SetBool ("Running", false);

		//armRA.SetInteger("HeldType", 0);
		//armLA.SetInteger("HeldType", 0);
		moving = airborne = crouching = false;
		hip.enabled = legR.enabled = legL.enabled = torso.enabled = head.enabled = armL.enabled = armR.enabled = true;


	}


}
