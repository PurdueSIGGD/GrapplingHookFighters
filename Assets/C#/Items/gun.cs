using UnityEngine;
using System.Collections;
//using UnityEditor;
public class gun : MonoBehaviour, item {

	public bool trigger, death, ejecting, canDual, raycastShoot, automatic, canFire, nonLethal, penetrating, chargedShot, shootIntoWalls, childProjectile, gunGoesPoof, gunGoesFlare, yelling;
	public int playerid, bulletsPerShot = 1, ammo;
	public float timeToShoot, projectileSpeed, recoil, aimRecoil, damage, spread, timeToCharge, minChargeTime, bulletDistance = 100, chance = 0;
	private float timeSincelast, maxProjectileSpeed, chargeTime, gunShaking, nextAngle, lastAngle;
	private float emptyTime;
	public float deleteEmptyTime = 5;
	//the point at which bullets come out of
	public Vector3 shootPoint;
	public GameObject projectileGameObject, particle, sparks, splats, disappear;
	public Vector2 reticlePos;
	public Sprite shellSprite;
	public PhysicsMaterial2D bulletPhys;
	public Sprite emptySprite;

	private HeldItem myHeldItem;
	private Transform butthole, reticle, gunbase;
	private ParticleSystem particleSmoke;
    private ParticleSystem particleMuzzleFlare;
    private bool isrpg;

	public GameObject critGameObject;

	// Use this for initialization
	void Start () {
		timeSincelast = timeToShoot;
		if (chargedShot) {
			maxProjectileSpeed = projectileSpeed;
			projectileSpeed = 0;
		}
		chargeTime = minChargeTime;

		butthole = transform.FindChild("Butthole");
		gunbase = transform.FindChild("GunBase");
		myHeldItem = this.GetComponent<HeldItem>();
		Transform smoke, flare;
		if (smoke = transform.FindChild("ParticleSmoke")) 
			particleSmoke = smoke.GetComponent<ParticleSystem>();
        isrpg = true;
        if (transform.name != "RPG")
            isrpg = false;
        if (isrpg == false)
        {
            if (flare = transform.Find("ParticleMuzzleFlare"))
                particleMuzzleFlare = flare.GetComponent<ParticleSystem>();
        }
        else
        {
            if (flare = transform.Find("ParticleMuzzleFlareRPG"))
                particleMuzzleFlare = flare.GetComponent<ParticleSystem>();
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
			chargeTime = minChargeTime;
		}
		emptyTime = 0;
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
		if (reticle == null && playerid != -1) {
			reticle = GameObject.Find("Player" + playerid).GetComponent<player>().reticle;
		}
		//checked to see if there was a mouseplayer click
		//this could be resource intensive as it is calling a method each update so the click()&unclick() method
		//could be removed from the item interface
		//update shooting
		timeSincelast += Time.deltaTime;
		//if (yelling) print(gunShaking);
		if (!trigger) {
			if (gunShaking > 0) gunShaking -= (aimRecoil)*Time.deltaTime;
			else gunShaking = 0;
		}
		if (ammo <= 0) {
			emptyTime += Time.deltaTime;
			if (emptyTime > deleteEmptyTime && this.playerid < 0) {
				if (disappear != null) GameObject.Instantiate(disappear, transform.position, Quaternion.identity);
				else Debug.LogWarning("Missing disappear particles for item " + transform.name);
				Destroy(this.gameObject);
			}
		}
		if (!death && trigger && playerid != -1) { // checking the playerid not -1 is if the weapon is not picked up
			if (timeSincelast > timeToShoot) {
				if (ammo > 0) {

					ammo--;
					if (ammo == 0 && emptySprite != null) {
						this.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = emptySprite;
					}
					shootPoint = butthole.position; //only need to set when player decides to shoot
					reticlePos = reticle.position;
					Vector2 gunBase = gunbase.position;
					//Vector2 thing = reticlePos - (Vector2)shootPoint;
					Vector2 playerPos = GameObject.Find("Player" + playerid).transform.position;

					//Vector2 thing = (Vector2)shootPoint - gunBase;
					Vector2 thing = Vector2Extension.Deg2Vector(transform.parent.eulerAngles.z);
					//Vector2 thing = Vector2.zero;
					//if (yelling) print(thing);
					if (gunShaking < .5f) gunShaking += (aimRecoil);
					//thing += (gunShaking * Random.insideUnitCircle);
					//float angle = Vector2Extension.Vector2Deg(thing);
					//transform.rotation = Quaternion.Euler(0,0,angle);	
					//print(gunShaking);
					if (gunShaking > 0) transform.parent.GetComponentInParent<RecoilSimulator>().SendMessage("AddTorque",gunShaking);
					thing = (Vector2)shootPoint - gunBase;

					if (transform.parent && transform.parent.parent) transform.parent.parent.parent.GetComponent<Rigidbody2D>().AddForce(-40 * recoil * thing); //Pushing back
					for (int i = 0; i < bulletsPerShot; i++) {
						Vector2 f = thing;
						f += (spread/Random.Range(1f,5f) * Random.insideUnitCircle) + f;
						f.Normalize();
						//print(f + " " + Vector2Extension.Vector2Deg(f));
						int layermask = (1 << this.gameObject.layer) + (1 << (this.gameObject.layer + 5)) + (1 << 15);
						//    print(Vector3.Distance(playerPos, f));
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
							if (!inTheWay || !this.raycastShoot) {
								//Debug.DrawLine(this.transform.position, ugh);
								//EditorApplication.isPaused = true;
								RaycastHit2D[] r;
								float thisDistance = bulletDistance + Random.Range(-1f,1f);
								r = Physics2D.RaycastAll (shootPoint, f, thisDistance, layermask);
								Vector3 endPoint = (shootPoint + (Vector3)(f * thisDistance));
								Transform hit = null;
								foreach (RaycastHit2D ray in r) {

									if (!(ray.transform.gameObject == myHeldItem.focus) && 
										(!ray.collider.isTrigger || (ray.collider.GetComponent<Hittable>() && (!ray.collider.GetComponent<HeldItem>() || ray.collider.GetComponent<HeldItem>().focus != myHeldItem.focus) && ray.collider.GetType() == typeof(PolygonCollider2D))) && //so we can hit select items that someone is holding
										!ray.transform.GetComponent<ParticleScript> () &&
										ray.transform != this.transform) {
										if (penetrating) {
											//print(ray.transform.name);
											Damage(ray.transform, ray.point, f);
											hit = ray.transform;
										} else    
										if (!hit) {
											hit = ray.transform;
											endPoint = ray.point;
										}
									}
								}
								if (hit && !penetrating) {

									Damage(hit.transform, endPoint, f);

								}
								GameObject g;
								g = (GameObject)GameObject.Instantiate (projectileGameObject, shootPoint, transform.rotation);
								FiredProjectile fp = g.GetComponent<FiredProjectile> ();
								if (fp != null) {
									fp.sourcePlayer = GameObject.Find ("Player" + playerid);
								}
								if (childProjectile) {
									g.transform.SetParent (transform);
									g.GetComponent<Rigidbody2D> ().isKinematic = true;
								}
								//g.transform.position = endPoint;
								//EditorApplication.isPaused = true;
								foreach (LineRenderer rrr in g.GetComponents<LineRenderer>()) {
									rrr.SetPosition (0, shootPoint);
									rrr.SetPosition (1, endPoint);
								}


							} else {
								Collider2D[] hitColliders = Physics2D.OverlapCircleAll (shootPoint, .1f, layermask);

								foreach (Collider2D c in hitColliders) {
									if (!c.isTrigger && c.GetComponent<Hittable>()) {
										if (!c.GetComponent<player>() || c.GetComponent<player>().playerid != this.playerid) {
											//if we are sticking the end of our gun into something, cant be us
											//print(3);
											Damage(c.transform, shootPoint, f);
										}
									}
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
							if (!colliding || true) {

								GameObject g;
								g = (GameObject)GameObject.Instantiate (projectileGameObject, shootPoint, transform.rotation);
								FiredProjectile fp = g.GetComponent<FiredProjectile>();
								fp.sourcePlayer = myHeldItem.focus;

								if (childProjectile) {
									g.transform.SetParent (transform);
									g.GetComponent<Rigidbody2D> ().isKinematic = true;
								}
								//UnityEditor.EditorApplication.isPaused = true;
								if (g.GetComponent<HeldItem> ()) {
									g.SendMessage ("ignoreColl", myHeldItem.focus.GetComponent<Collider2D> ());
									g.SendMessage ("retriggerSoon");
								}
								if (!this.raycastShoot) g.layer = this.transform.gameObject.layer; //want it to be light if not a projectile
								fp.damage = this.damage;
								fp.nonLethal = this.nonLethal;

								if (GetOdds()) { //lethal round, potato salad
									fp.damage = 150;
									//fp.dieOnAnyHit = true;
									fp.nonLethal = false;
									fp.exploding = true;
									Destroy (g.GetComponent<HeldItem> ()); // you aint holding this
									GameObject cg = (GameObject)GameObject.Instantiate (critGameObject, g.transform.position, g.transform.rotation);
									cg.transform.parent = g.transform;
								}


								g.GetComponent<Rigidbody2D> ().AddForce (f * projectileSpeed);
								grenade gre;
								if (gre = g.GetComponent<grenade> ())
									gre.GetComponent<grenade> ().pinPulled = true;
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
						Rigidbody2D shellyRigid = shelly.GetComponent<Rigidbody2D>();
						shellyRigid.gravityScale = 1;
						shellyRigid.mass = float.MinValue;
						Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), shellyRigid.GetComponent<Collider2D>());
						Physics2D.IgnoreCollision(myHeldItem.focus.GetComponent<Collider2D>(), shelly.GetComponent<Collider2D>());

						shellyRigid.AddForceAtPosition(200 *  (.1f *Random.insideUnitCircle +  Vector2.up) * shelly.GetComponent<Rigidbody2D>().mass, shelly.transform.position);
						shellyRigid.AddTorque(Random.Range(-20,20));                    //shelly.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + (Quaternion.AngleAxis(90, (Vector3)thing) * thing)));
						ParticleScript ps = shelly.GetComponent<ParticleScript>();
						ps.time = 2;
						ps.shell = true;
						shelly.layer = this.gameObject.layer == 8 ? 11 : 12; //becomes the respective nocol layer

					}
					if (gunGoesPoof) {
						particleSmoke.Play();
					}
                    if (gunGoesFlare && particleMuzzleFlare != null)
                    {
                        particleMuzzleFlare.Play();
                    }
					timeSincelast = 0;
				} else {
					//print("Click");
				}
			} else {
				
			}
		} else {
			//gracefully go back to rotation of 0
		}

	}
	void Death() {
		death = true;
	}
	void NotDeath() {
		death = false;
	}
	void Damage(Transform t, Vector2 endPoint, Vector2 angle) {
		if (t) {
			if (t.tag == "Player" || t.tag == "PlayerGibs") {
				GameObject.Instantiate(splats, endPoint,Quaternion.Euler(0,90,Vector2Extension.Vector2Deg(angle)));

			} else {
				if (!t.GetComponent<ShootableItem>()) GameObject.Instantiate(sparks, endPoint, Quaternion.Euler(0,90,Vector2Extension.Vector2Deg(angle)));
			}
		}
		Rigidbody2D rb;
		if (rb = t.GetComponent<Rigidbody2D> ()) {
			rb.AddForceAtPosition (1 * damage * (angle + Vector2.up), endPoint);
		}
		if (t.GetComponent<Hittable>()) {
			//print(1);
			t.SendMessage ("hit", damage);
		} 
        if (t.GetComponent<Deathbox>())
        {
            t.SendMessage("hit", angle);
        }
    }
	bool GetOdds() { 
		float rd = Random.value;
		return (rd < chance);
	}

}