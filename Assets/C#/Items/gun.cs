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
	public bullet kapeeeeewm;
	public Vector2 reticlePos;
	public int playerid;

	// Use this for initialization
	void Start () {
		timeSincelast = timeToShoot;
	}

	public void click(){
		trigger = true;
	}

	public void unclick(){
		trigger = false;
	}

	//a send message command
	public void SetPlayerID(int playerid){
	//will set an ID number as an Int
		this.playerid = playerid;

	}

	// Update is called once per frame
	void Update () {
		//this needs to be revised to get an accurate start position for the bullet
		shootpoint.x = this.gameObject.transform.position.x+1.02f;
		shootpoint.y = this.gameObject.transform.position.y+0.05f;
		//this seems to work
		gunAngle = GetComponentInParent<Transform> ().rotation;
		//checked to see if there was a mouseplayer click
		//this could be resource intensive as it is calling a method each update so the click()&unclick() method
		//could be removed from the item interface
		if (Input.GetMouseButtonDown (0)) { 
			click ();
		} else {
			unclick();
		}
		//update shooting
		timeSincelast += Time.deltaTime;
		if (trigger && (timeSincelast > timeToShoot) && playerid != -1) { // checking the playerid not -1 is if the weapon is not picked up
			kapeeeeewm = (bullet)Instantiate(kapeeeeewm,shootpoint,gunAngle);
			//this won't work until we can differentiate mouse clicks
			reticlePos = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("Reticle" + playerid).position;
			Vector2 thing = reticlePos - shootpoint;
			thing.Normalize();
			kapeeeeewm.GetComponent<Rigidbody2D>().AddForce(thing*5f);
			timeSincelast = 0;
		}

	}
}
