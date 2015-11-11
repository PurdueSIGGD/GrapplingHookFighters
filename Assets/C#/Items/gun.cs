using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour, item {

	public bool trigger;
	public float timeToShoot;
	private float timeSincelast;
	public Vector2 itemAngle;
	//the point at which bullets come out of
	public Vector2 shootpoint;
	public Quaternion gunAngle;
	public GameObject kapeeeeewm;
	public Vector2 reticlePos;
	public int playerid;
	public bool automatic;
	private bool canFire = true;

	// Use this for initialization
	void Start () {
		timeSincelast = timeToShoot;
	}

	public void click(){
		if (automatic || canFire) {
			trigger = true;
			canFire = false;
		}
		else
			trigger = false;
       // print("Player " + this.playerid + " clicked");
    }

	public void unclick(){
		canFire = true;
		trigger = false;
	}

	//a send message command
	public void SetPlayerID(int playerid){
	//will set an ID number as an Int
		this.playerid = playerid;

	}

	// Update is called once per frame
	void Update () {
		//checked to see if there was a mouseplayer click
		//this could be resource intensive as it is calling a method each update so the click()&unclick() method
		//could be removed from the item interface
		//update shooting
		timeSincelast += Time.deltaTime;
		if (trigger && (timeSincelast > timeToShoot) && playerid != -1) { // checking the playerid not -1 is if the weapon is not picked up
			shootpoint = transform.FindChild("Butthole").position; //only need to set when player decides to shoot
			reticlePos = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("Reticle" + playerid).position;
			GameObject g = (GameObject)GameObject.Instantiate(kapeeeeewm,shootpoint,GetComponentInParent<Transform>().rotation);
			//creating new gameobject, not setting our last one to be that. It will cause problems in the future.
			Vector2 thing = reticlePos - shootpoint;
			thing.Normalize();
			g.GetComponent<Rigidbody2D>().AddForce(thing*800f);
			timeSincelast = 0;
		}

	}
}
