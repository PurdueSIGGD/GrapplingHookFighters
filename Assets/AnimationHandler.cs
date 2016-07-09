using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour {
	public SpriteRenderer hip, legR, legL, torso, head, armL, armR;
	public Animator legRA, legLA, armLA, armRA;
	public RuntimeAnimatorController runningL, runningR, airborneL, airborneR, crouchingL, crouchingR, punchingL, punchingR;
	public Sprite singleR, singleL, dualR, dualL, heavyR, heavyL, legLS, legRS, armLS, armRS;

	public bool airborne, crouching, moving, direction, grappling; //direction: false right, true left;
	public Color startColor = Color.white;

	private bool lastDirection; //used for knowing when to flip
	private bool punchIndex;
	// Use this for initialization
	void Start () {
		hip.color = legR.color = legL.color = torso.color = head.color = armL.color = armR.color = startColor;
		armLA.Stop ();
		armRA.Stop ();
		armL.sprite = armLS;
		armR.sprite = armRS;
	}
	
	// Update is called once per frame
	void Update () {
		hip.flipX = legR.flipX = legL.flipX = torso.flipX = head.flipX/* = armL.flipX = armR.flipX*/ = direction;
		armL.flipY = armR.flipY = direction;
		if (direction != lastDirection) {
			hip.transform.localPosition = new Vector3 (hip.transform.localPosition.x * -1, hip.transform.localPosition.y, hip.transform.localPosition.z);
			legR.transform.localPosition = new Vector3 (legR.transform.localPosition.x * -1, legR.transform.localPosition.y, legR.transform.localPosition.z);
			legL.transform.localPosition = new Vector3 (legL.transform.localPosition.x * -1, legL.transform.localPosition.y, legL.transform.localPosition.z);
			torso.transform.localPosition = new Vector3 (torso.transform.localPosition.x * -1, torso.transform.localPosition.y, torso.transform.localPosition.z);
			armL.transform.localPosition = new Vector3 (armL.transform.localPosition.x * -1, armL.transform.localPosition.y, armL.transform.localPosition.z);
			armR.transform.localPosition = new Vector3 (armR.transform.localPosition.x * -1, armR.transform.localPosition.y, armR.transform.localPosition.z);
		}
		lastDirection = direction;
		armL.enabled = !grappling;
		if (airborne) {
			legRA.runtimeAnimatorController = airborneR;
			legLA.runtimeAnimatorController = airborneL;
		} else {
			if (moving) {
				if (crouching) {
					legRA.runtimeAnimatorController = crouchingR;
					legLA.runtimeAnimatorController = crouchingL;
				} else {
					legRA.runtimeAnimatorController = runningR;
					legLA.runtimeAnimatorController = runningL;
				}
			} else {
				legRA.Stop ();
				legLA.Stop ();
				legL.sprite = legLS;
				legR.sprite = legRS;
			}
		}
			
	}

	public void Punch() {
		punchIndex = !punchIndex;
		armLA.Stop ();
		armRA.Stop ();
		if (punchIndex) {
			armLA.runtimeAnimatorController = punchingL;
			armLA.Play (0);
		} else {
			armRA.runtimeAnimatorController = punchingR;
			armRA.Play (0);
		}
	}

	public void SetSingle() {

	}
	public void SetDual() {

	}
	public void SetHeavy() {

	}
}
