using UnityEngine;
using System.Collections;

public class RecoilSimulator : MonoBehaviour {
	public float recoverSpeed = 10, pullDelay = .5f, maxAngle = 45;
	//recover speed is the rate that it takes to bring your gun baack down
	//pull delay is the time in between shooting and starting pulling down

	private float torque, time;
	private bool canAddTorque, rotating;

	// Use this for initialization
	void Start () {
		StopRotation();
	}
	
	void LateUpdate () {
		//print(torque);
		time+=Time.deltaTime;
		if (rotating) {
			if (time > pullDelay) torque -= Mathf.Pow(recoverSpeed, 2) * Time.deltaTime;
			//print("lowering torque to " + torque);
			Vector3 ang = transform.localEulerAngles;
			if (ang.z + (torque * Time.deltaTime) < 0) {
				StopRotation();
			} else if (ang.z + (torque * Time.deltaTime) > maxAngle) {
				transform.localRotation = Quaternion.Euler(ang.x, ang.y, maxAngle);
				torque = 0;
				//canAddTorque = false;
			} else {
				canAddTorque = true;
				transform.localRotation = Quaternion.Euler(ang.x, ang.y, ang.z + (torque * Time.deltaTime));
			}
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
	}
	void StopRotation() {
		torque = 0;

		canAddTorque = true;
		//Vector3 ang = transform.localEulerAngles;
		rotating = false;
		//transform.localRotation = Quaternion.Euler(ang.x, ang.y, 0);
	}
}
