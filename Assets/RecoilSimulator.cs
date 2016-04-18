using UnityEngine;
using System.Collections;

public class RecoilSimulator : MonoBehaviour {
	public float recoverSpeed = 10, pullDelay = .5f, maxAngle = 45, compressionRecovery = 20, maxCompression = 1;
	public bool valueHolder = true;
	//recover speed is the rate that it takes to bring your gun baack down
	//pull delay is the time in between shooting and starting pulling down
	private int lastFactor;
	private float torque, recoilSpeed, time;
	private bool canAddTorque, rotating, compressing;
	private Transform center, aimerSprite;
	private Vector2 startPos, startH1, startH2;
	// Use this for initialization
	void Start () {
		lastFactor = 0;
		if (!valueHolder) {
			StopRotation();
			center = transform.FindChild("Center");
			Transform aimerBody = transform.FindChild("AimerBody");
			aimerSprite = aimerBody.FindChild("Aimer").transform;
			startPos = aimerSprite.transform.localPosition;
			startH1 = aimerBody.FindChild("Held1").transform.localPosition;
			startH2 = aimerBody.FindChild("Held2").transform.localPosition;
		}
	}
	
	void LateUpdate () {
		if (!valueHolder) {
			
			int factor = center.localEulerAngles.z  < 90 || center.localEulerAngles.z > 270 ? 1:-1;
			if (lastFactor != factor) {
				transform.rotation = Quaternion.Euler(Vector3.zero);
				rotating = false;
				torque = 0;
			}
			lastFactor = factor;
			//print(factor);
			//print(torque);
			time+=Time.deltaTime;
			if (rotating) {
				if (time > pullDelay) torque -= Mathf.Pow(recoverSpeed, 2) * Time.deltaTime;
				//print("lowering torque to " + torque);
				Vector3 ang = transform.localEulerAngles;

				if ((factor==1 && ang.z + (factor * torque * Time.deltaTime) < 0)|| factor==-1 && ang.z + (factor * torque * Time.deltaTime) > 360) {
					//print("from " + ang.z + " to " + (ang.z + (factor * torque * Time.deltaTime)) + " with torque " + torque);

					StopRotation();
				} else if ((factor == 1 && ang.z + (factor * torque * Time.deltaTime) > factor * maxAngle) ) {
					transform.localRotation = Quaternion.Euler(ang.x, ang.y, maxAngle);
					torque = 0;
					//print("Max rotation hit" + ang.z);
					//canAddTorque = false;
				} else {
					canAddTorque = true;
					transform.localRotation = Quaternion.Euler(ang.x, ang.y, ang.z + (factor * torque * Time.deltaTime));
				}
			}
			if (compressing) {
				//print(recoilSpeed);
				recoilSpeed -= compressionRecovery * Time.deltaTime;
				//print("stopping " + (aimerSprite.transform.localPosition + Vector3.left * recoilSpeed).x + " " + (startPos.x - maxCompression));

				if ((aimerSprite.transform.localPosition + Vector3.left * recoilSpeed).x > startPos.x) {
					StopCompression();
					SetRecoilPos(0);
					print("stopping");

				} else if ((aimerSprite.transform.localPosition + Vector3.left * recoilSpeed).x < startPos.x - maxCompression) {
					//recoilSpeed = 0;
					SetRecoilPos(maxCompression);
				} else {
					SetRecoilPos(recoilSpeed);
				}
			}

		}
	}
	void SetRecoilPos(float factor) {
		aimerSprite.transform.localPosition = startPos + Vector2.left*factor;
		//set for both weapons, if they exist
		if (center.childCount == 2) {
			center.GetChild(1).transform.localPosition = startH2 + Vector2.left*factor;
		} 
		if (center.childCount >= 1) {
			center.GetChild(0).transform.localPosition = startH1 + Vector2.left*factor;
		}
	}
	//Add a force to the local rotation of the item
	void AddTorque(float f) {
		rotating = true;
		if (canAddTorque) {
			if (torque > 150) {
				torque += f*Random.Range(-10/time,2f);
			} else {
				torque += f*Random.Range(0,2f);
			}
		}
		else torque = 0;

		time = 0;
		AddCompression(f/100);
	}
	void AddCompression(float f) {
		if (f > .5f) f = .5f;
		recoilSpeed += f;
		compressing = true;

		print(recoilSpeed);

	}
	void StopCompression() {
		compressing = false;
		recoilSpeed = 0;
	}
	void StopRotation() {
		torque = 0;
		canAddTorque = true;
		rotating = false;
		//Vector3 ang = transform.localEulerAngles;
		//transform.localRotation = Quaternion.Euler(ang.x, ang.y, 0);
	}
}
