using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour, item {

	public bool trigger, death, ejecting;
	public float timeToShoot, projectileSpeed, damage, gunGoesPoof, spread;
	private float timeSincelast;
	public Vector2 itemAngle;
	//the point at which bullets come out of
	public Vector3 shootPoint;
	public Quaternion gunAngle;
	public GameObject projectileGameObject, particle;
	public Vector2 reticlePos;
	public int playerid, bulletsPerShot = 1;
	public bool automatic, raycastShoot;
	private bool canFire = true;
	public int ammo;
	public Sprite shellSprite;
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

		if (!death && trigger && (timeSincelast > timeToShoot) && playerid != -1) { // checking the playerid not -1 is if the weapon is not picked up
			if (ammo > 0) {

				ammo--;
				shootPoint = transform.FindChild("Butthole").position; //only need to set when player decides to shoot
				reticlePos = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("Reticle" + playerid).position;
				//Vector2 thing = reticlePos - (Vector2)shootPoint;
				Vector2 playerPos = GameObject.Find("Player" + playerid).transform.position;

				Vector2 thing = (Vector2)shootPoint - playerPos;
				//creating new gameobject, not setting our last one to be that. It will cause problems in the future.

				for (int i = 0; i < bulletsPerShot; i++) {
					Vector2 f = thing;
					thing += spread * Random.insideUnitCircle;
					thing.Normalize();	
					GameObject g;
					g = (GameObject)GameObject.Instantiate(projectileGameObject, shootPoint, GetComponentInParent<Transform>().rotation);
					FiredProjectile FP = g.GetComponent<FiredProjectile>();
					g.layer = this.transform.gameObject.layer;
					FP.damage = this.damage;

					g.GetComponent<Rigidbody2D>().AddForce(thing*projectileSpeed);
					thing = f.normalized;
				}
				transform.parent.GetComponentInParent<Rigidbody2D>().AddForce(-40 * damage * thing); //Pushing back
				if (ejecting) {
					GameObject shelly = (GameObject)GameObject.Instantiate(particle, transform.FindChild("shellEject").transform.position, GetComponentInParent<Transform>().rotation);
					shelly.transform.localScale = new Vector3(shelly.transform.localScale.x / 2.5f, shelly.transform.localScale.z / 2.5f, shelly.transform.localScale.z / 3);
					shelly.GetComponent<SpriteRenderer>().sprite = this.shellSprite;
					shelly.AddComponent<PolygonCollider2D>();
					shelly.GetComponent<Rigidbody2D>().gravityScale = 1;
					shelly.GetComponent<Rigidbody2D>().mass = float.MinValue;
					Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), shelly.GetComponent<Collider2D>());
					shelly.GetComponent<Rigidbody2D>().AddForceAtPosition(200 * Vector2.up * shelly.GetComponent<Rigidbody2D>().mass, shelly.transform.position);
					//shelly.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + (Quaternion.AngleAxis(90, (Vector3)thing) * thing)));
					shelly.GetComponent<ParticleScript>().time = 2;
					shelly.GetComponent<ParticleScript>().shell = true;
					shelly.layer = this.transform.gameObject.layer;
				}
				for (int i = 0; i < gunGoesPoof; i++) {
					GameObject particleG =(GameObject) GameObject.Instantiate(particle, shootPoint, this.transform.rotation);
					if (this.transform.parent.parent != null) particleG.GetComponent<Rigidbody2D>().velocity = transform.parent.GetComponentInParent<Rigidbody2D>().velocity * .6f;
					particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + thing));
					particleG.GetComponent<ParticleScript>().time = .6f;
				}
				timeSincelast = 0;
			} else {
				//print("Click");
			}

		}
		
	}
	void Death() {
		death = true;
	}

}
