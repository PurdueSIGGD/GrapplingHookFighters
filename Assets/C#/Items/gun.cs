using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour, item {

	public bool trigger;
	public float timeToShoot, projectileSpeed, damage;
	private float timeSincelast;
	public Vector2 itemAngle;
	//the point at which bullets come out of
	public Vector2 shootPoint;
	public Quaternion gunAngle;
	public GameObject projectileGameObject, particle;
	public Vector2 reticlePos;
	public int playerid;
	public bool automatic, raycastShoot;
	private bool canFire = true;
	private LineRenderer lr;
	public int ammo;

	// Use this for initialization
	void Start () {
		lr = this.GetComponent<LineRenderer>();
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
			if (ammo > 0) {

				ammo--;
				shootPoint = transform.FindChild("Butthole").position; //only need to set when player decides to shoot

				reticlePos = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("Reticle" + playerid).position;
				GameObject g;
				
				g = (GameObject)GameObject.Instantiate(projectileGameObject, shootPoint, GetComponentInParent<Transform>().rotation);
				FiredProjectile FP = g.GetComponent<FiredProjectile>();
				FP.damage = this.damage;
				
				g.layer = this.transform.gameObject.layer;
				Vector2 thing = reticlePos - shootPoint;
				//creating new gameobject, not setting our last one to be that. It will cause problems in the future.
				Vector2 playerPos = GameObject.Find("Player" + playerid).transform.position;
				if (Vector2.Distance(reticlePos, playerPos) < Vector2.Distance(shootPoint, playerPos)) {
					thing *= -1;
				}
				thing.Normalize();			
				g.GetComponent<Rigidbody2D>().AddForce(thing*projectileSpeed);
				for (int i = 0; i < 5; i++) {
					GameObject particleG =(GameObject) GameObject.Instantiate(particle, shootPoint, this.transform.rotation);
					particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + thing));
					particleG.GetComponent<ParticleScript>().time = .6f;
				}
				timeSincelast = 0;
			} else {
				print("Click");
			}

		}
		
	}
}
