using UnityEngine;
using System.Collections;

public class RecoilSimulator : MonoBehaviour {
	public float recoverSpeed = 10, pullDelay = .5f, maxAngle = 45;
	public bool valueHolder = true;
	//recover speed is the rate that it takes to bring your gun baack down
	//pull delay is the time in between shooting and starting pulling down

	private float torque, time;
	private bool canAddTorque, rotating;

	// Use this for initialization
	void Start () {
		StopRotation();
	}
	
	void LateUpdate () {
		if (!valueHolder) {
			Transform center = transform.FindChild("Center");
			int factor = center.localEulerAngles.z  < 90 || center.localEulerAngles.z > 270 ? 1:-1;
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

		rotating = false;
		//Vector3 ang = transform.localEulerAngles;
		//transform.localRotation = Quaternion.Euler(ang.x, ang.y, 0);
	}
}
