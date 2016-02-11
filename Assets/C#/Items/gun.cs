using UnityEngine;
using System.Collections;
using UnityEditor;
public class gun : MonoBehaviour, item {

	public bool trigger, death, ejecting, canDual, raycastShoot, automatic, canFire, nonLethal, penetrating, chargedShot, shootIntoWalls, childProjectile, chargedShot, gunGoesPoof;
	public int playerid, bulletsPerShot = 1, ammo;
	public float timeToShoot, projectileSpeed, recoil, damage, spread, timeToCharge;
	private float timeSincelast, maxProjectileSpeed, chargeTime;
	//the point at which bullets come out of
	public Vector3 shootPoint;
	public GameObject projectileGameObject, particle, sparks, splats;
	public Vector2 reticlePos;
	public Sprite shellSprite;
	public PhysicsMaterial2D bulletPhys;
	// Use this for initialization
	void Start () {
		timeSincelast = timeToShoot;

		if (chargedShot) {
			maxProjectileSpeed = projectileSpeed;
			projectileSpeed = 0;
		}
	}

	public void click() {
		if ((canFire || automatic) && !chargedShot) {
			trigger = true;
			canFire = false;
		} else if (chargedShot) {
			trigger = false;
			canFire = true;
			if (chargeTime + Time.deltaTime <= timeToCharge) {
				chargeTime += Time.deltaTime;
			} else {
				chargeTime = timeToCharge;
			}
			projectileSpeed = (maxProjectileSpeed * (chargeTime / timeToCharge));

		} else {
			trigger = false;
		}
    }

	public void unclick(){
		if (!chargedShot) {
			canFire = true;
			trigger = false;
		} else if (chargedShot && canFire && chargeTime > .1f) {
			trigger = true;
			canFire = false;
		} else if (chargedShot) {
			trigger = false;
			projectileSpeed = 0;
			chargeTime = 0;
		}
	}

	//a send message command
	public void SetPlayerID(int playerid){
	//will set an ID number as an Int
		this.playerid = playerid;
	}
	void OnCollisionEnter2D(Collision2D col) {
		/*print(col.transform.name);
		if (col.transform.name == "Particle(Clone)") {
			print(this.transform.name);
			EditorApplication.isPaused = true;
		}*/
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
				reticlePos = GameObject.Find("Reticle" + playerid).transform.position;
				Vector2 gunBase = transform.FindChild("GunBase").position;
				//Vector2 thing = reticlePos - (Vector2)shootPoint;
				Vector2 playerPos = GameObject.Find("Player" + playerid).transform.position;
			
				//Vector2 thing = (Vector2)shootPoint - playerPos;
				Vector2 thing = (Vector2)shootPoint - gunBase;

				transform.parent.parent.GetComponent<Rigidbody2D>().AddForce(-40 * recoil * thing); //Pushing back
				for (int i = 0; i < bulletsPerShot; i++) {
					Vector2 f = thing;
					f += (spread/Random.Range(1,5) * Random.insideUnitCircle) + f;
					f.Normalize();
					int layermask = (1 << this.gameObject.layer) + (1 << (this.gameObject.layer + 5));
				//	print(Vector3.Distance(playerPos, f));
					RaycastHit2D[] rr = Physics2D.RaycastAll(playerPos, shootPoint - (Vector3)playerPos, Vector3.Distance(playerPos, shootPoint), layermask);
					bool inTheWay = false;
					if (rr.Length > 0) {
						foreach (RaycastHit2D ray in rr) {
							//print(ray.transform);
							if (!penetrating && !ray.collider.isTrigger && !ray.transform.GetComponent<ParticleScript>() && !ray.transform.GetComponent<player>() && !ray.transform.GetComponentInParent<player>() && !ray.transform.GetComponent<GrappleScript>() && !ray.transform.GetComponent<gun>()) {
								//print(ray.transform);
								inTheWay = true; //if the player is facing a wall, makes sure they can't shoot through it
							}
						}
					}
					//print (inTheWay);
						if (raycastShoot) {
							if (!inTheWay) {
								//Debug.DrawLine(this.transform.position, ugh);
								//EditorApplication.isPaused = true;
								RaycastHit2D[] r;
								r = Physics2D.RaycastAll (shootPoint, f, 100, layermask);
								Vector3 endPoint = (shootPoint + (Vector3)(f * 100));
								Transform hit = null;
								foreach (RaycastHit2D ray in r) {
									
									if (!(ray.transform.gameObject == this.GetComponent<HeldItem> ().focus) && 
									(!ray.collider.isTrigger || (ray.collider.GetComponent<Hittable>() && ray.collider.GetType() == typeof(PolygonCollider2D))) && //so we can hit select items that someone is holding
									!ray.transform.GetComponent<ParticleScript> ()) {
										if (penetrating) {
											if (hit && hit.GetComponent<player>()) {
												GameObject.Instantiate(splats, endPoint, Quaternion.identity);

											} else {
												GameObject.Instantiate(sparks, endPoint, Quaternion.identity);

											}

											endPoint = ray.point;
											hit = ray.transform;
											if (hit.GetComponent<Rigidbody2D> ()) {
												//print(hit);
												hit.GetComponent<Rigidbody2D> ().AddForce (damage * f);
											}
											if (hit.transform.GetComponent<Hittable>()) {
												hit.transform.SendMessage ("hit");
											} 
											if (hit.GetComponent<grenade> ()) {
												hit.SendMessage ("Explode");
											}
										}	
										if (!hit) {
											hit = ray.transform;
											endPoint = ray.point;
										}
									}
								}
								if (hit && !penetrating) {

									if (hit.GetComponent<Rigidbody2D> ()) {
										//print(hit);
										hit.GetComponent<Rigidbody2D> ().AddForce (damage * f);
									}
									if (hit.transform.GetComponent<Hittable>()) {
										hit.transform.SendMessage ("hit");
									}
									if (hit.GetComponent<grenade> ()) {
										hit.SendMessage ("Explode");
									}
								}
								GameObject g;
								g = (GameObject)GameObject.Instantiate (projectileGameObject, shootPoint, transform.rotation);
								g.GetComponent<FiredProjectile> ().sourcePlayer = GameObject.Find ("Player" + playerid);
								if (childProjectile) {
									g.transform.SetParent (transform);
									g.GetComponent<Rigidbody2D> ().isKinematic = true;
								}
								//g.transform.position = endPoint;
							//EditorApplication.isPaused = true;
								g.GetComponent<LineRenderer> ().SetPosition (0, shootPoint);
								g.GetComponent<LineRenderer> ().SetPosition (1, endPoint);
							if (hit && hit.GetComponent<player>() && hit.GetComponent<Health>().dead) {
									GameObject.Instantiate(splats, endPoint, Quaternion.identity);
								} else {
									GameObject.Instantiate(sparks, endPoint, Quaternion.identity);
								}
							}
							
						} else { //if something is in the way
							Collider2D[] hitColliders = Physics2D.OverlapCircleAll (shootPoint, .1f, layermask);
							bool colliding = false;
							foreach (Collider2D c in hitColliders) {
								if (c.GetComponent<Rigidbody2D> () && !c.isTrigger && !shootIntoWalls) {
									//print (c);
									if (c.gameObject != GameObject.Find ("Player" + playerid)) {
										c.GetComponent<Rigidbody2D> ().AddForce (((Vector2)shootPoint - gunBase) * projectileSpeed * .7f);
										colliding = true;
									}

								}
							}
							if (!colliding) {
								//print ("shooting into wall");
								GameObject g;
								g = (GameObject)GameObject.Instantiate (projectileGameObject, shootPoint, transform.rotation);
								g.GetComponent<FiredProjectile> ().sourcePlayer = GameObject.Find ("Player" + playerid);
								if (childProjectile) {
									g.transform.SetParent (transform);
									g.GetComponent<Rigidbody2D> ().isKinematic = true;
								}
							//UnityEditor.EditorApplication.isPaused = true;
								if (g.GetComponent<HeldItem> ()) {
									g.SendMessage ("ignoreColl", GameObject.Find ("Player" + playerid).GetComponent<Collider2D> ());
									g.SendMessage ("retriggerSoon");
								}
								g.layer = this.transform.gameObject.layer;
								g.GetComponent<FiredProjectile> ().damage = this.damage;
								g.GetComponent<FiredProjectile> ().nonLethal = this.nonLethal;
								if (nonLethal) {
									g.GetComponent<Rigidbody2D> ().AddTorque (5 * Random.Range (0, 5f) * projectileSpeed);
								}

								g.GetComponent<Rigidbody2D> ().AddForce (f * projectileSpeed);
								if (g.GetComponent<grenade> ())
									g.GetComponent<grenade> ().pinPulled = true;
							}
						}			

				}

				if (ejecting) {
					GameObject shelly = (GameObject)GameObject.Instantiate(particle, transform.FindChild("shellEject").transform.position, GetComponentInParent<Transform>().rotation);
					shelly.transform.localScale = new Vector3(shelly.transform.localScale.x / 2.5f, shelly.transform.localScale.z / 2.5f, shelly.transform.localScale.z / 3);
					shelly.GetComponent<SpriteRenderer>().sprite = this.shellSprite;
					if (this.bulletsPerShot > 1) {
						shelly.transform.localScale += Vector3.one/5;
					}
					BoxCollider2D c = shelly.AddComponent<BoxCollider2D>();
					c.sharedMaterial = this.bulletPhys;
					c.size = new Vector2(c.size.x, c.size.y/2);
					shelly.GetComponent<Rigidbody2D>().gravityScale = 1;
					//shelly.GetComponent<Rigidbody2D>().mass = float.MinValue;
					Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), shelly.GetComponent<Collider2D>());
					Physics2D.IgnoreCollision(GameObject.Find("Player" + playerid).GetComponent<Collider2D>(), shelly.GetComponent<Collider2D>());

					shelly.GetComponent<Rigidbody2D>().AddForceAtPosition(200 *  (.1f *Random.insideUnitCircle +  Vector2.up) * shelly.GetComponent<Rigidbody2D>().mass, shelly.transform.position);
					shelly.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-20,20));					//shelly.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + (Quaternion.AngleAxis(90, (Vector3)thing) * thing)));
					shelly.GetComponent<ParticleScript>().time = 2;
					shelly.GetComponent<ParticleScript>().shell = true;
					shelly.layer = this.gameObject.layer == 8 ? 11 : 12; //becomes the respective nocol layer

				}
				if (gunGoesPoof) {
					transform.FindChild("ParticleSmoke").GetComponent<ParticleSystem>().Play();

					/*GameObject particleG =(GameObject) GameObject.Instantiate(particle, shootPoint, this.transform.rotation);
					if (this.transform.parent && this.transform.parent.parent != null) particleG.GetComponent<Rigidbody2D>().velocity = transform.parent.GetComponentInParent<Rigidbody2D>().velocity * .6f;
					particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + thing));
					particleG.GetComponent<Rigidbody2D>().gravityScale = -.3f;
					particleG.GetComponent<ParticleScript>().time = .6f;*/
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
	void NotDeath() {
		death = false;
	}
}
