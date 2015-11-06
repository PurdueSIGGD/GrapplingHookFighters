using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour, item {

	public bool trigger;
	public float timeToShoot = 5;
	private float timeSincelast = 0;
	public Vector2 itemAngle;
	//the point at which bullets come out of
	public Vector3 shootpoint;

	// Use this for initialization
	void Start () {
	
	}

	public void click(){
		trigger = true;
	}

	public void unclick(){
	}

	// Update is called once per frame
	void Update () {
		//update angle

		//update shooting
		timeSincelast += Time.deltaTime;
		if (trigger && (timeSincelast > timeToShoot)) {
			//GameObject.Instantiate();
			timeSincelast -= timeToShoot;
		}

	}
}
