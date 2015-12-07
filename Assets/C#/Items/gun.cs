﻿using UnityEngine;
using System.Collections;
using UnityEditor;
public class gun : MonoBehaviour, item {

	public bool trigger, death, ejecting, canDual, raycastShoot, automatic, canFire = true, nonLethal;
	public int playerid, bulletsPerShot = 1, ammo;
	public float timeToShoot, projectileSpeed, recoil, damage, gunGoesPoof, spread;
	private float timeSincelast;
	//the point at which bullets come out of
	public Vector3 shootPoint;
	public GameObject projectileGameObject, particle; 
	public Vector2 reticlePos;
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
				reticlePos = GameObject.Find("Reticle" + playerid).transform.position;
				//Vector2 thing = reticlePos - (Vector2)shootPoint;
				Vector2 playerPos = GameObject.Find("Player" + playerid).transform.position;
				Vector2 thing = (Vector2)shootPoint - playerPos;
				transform.parent.parent.GetComponent<Rigidbody2D>().AddForce(-40 * recoil * thing); //Pushing back
				for (int i = 0; i < bulletsPerShot; i++) {
					Vector2 f = (Vector2)shootPoint - playerPos;
					f += (spread/Random.Range(1,5) * Random.insideUnitCircle) + f;
					f.Normalize();
					int layermask = 1 << this.gameObject.layer;
				//	print(Vector3.Distance(playerPos, f));
					RaycastHit2D[] rr = Physics2D.RaycastAll(playerPos, shootPoint - (Vector3)playerPos, Vector3.Distance(playerPos, shootPoint), layermask);
					bool inTheWay = false;
					if (rr.Length > 0) {
						foreach (RaycastHit2D ray in rr) {
							if (!ray.collider.isTrigger && !ray.transform.GetComponent<player>() && !ray.transform.GetComponentInParent<player>() && !ray.transform.GetComponent<GrappleScript>() && !ray.transform.GetComponent<gun>()) {
								print(ray.transform);
								inTheWay = true;
							}
						}
					}
					if (!inTheWay) {
						if (raycastShoot) {

							//Debug.DrawLine(this.transform.position, ugh);
							//EditorApplication.isPaused = true;
							RaycastHit2D[] r;
							r = Physics2D.RaycastAll(shootPoint, f, 100, layermask);
							Vector3 endPoint = (shootPoint + (Vector3)(f * 100));
							Transform hit = null;
							foreach (RaycastHit2D ray in r) {
								if (Vector3.Distance(shootPoint, ray.point) < Vector3.Distance(shootPoint, endPoint) && !ray.collider.isTrigger) {
									//print(ray.transform);
									endPoint = ray.point;
									hit = ray.transform;
								}
							}
							if (hit) {

								if (hit.GetComponent<Rigidbody2D>()) {
									hit.GetComponent<Rigidbody2D>().AddForce(damage * this.GetComponent<Rigidbody2D>().velocity);
								}
								if (hit.transform.GetComponent<Health> ()) {
									hit.transform.SendMessage ("hit");
								}
								if (hit.GetComponent<grenade>()) {
									hit.SendMessage("Explode");
								}
							}
							GameObject g;
							g = (GameObject)GameObject.Instantiate(projectileGameObject, shootPoint, transform.rotation);
							//g.transform.position = endPoint;
							g.GetComponent<LineRenderer>().SetPosition(0, shootPoint);
							g.GetComponent<LineRenderer>().SetPosition(1, endPoint);

							
						} else {
							GameObject g;
							g = (GameObject)GameObject.Instantiate(projectileGameObject, shootPoint, transform.rotation);
							g.layer = this.transform.gameObject.layer;
							g.GetComponent<FiredProjectile>().damage = this.damage;
							g.GetComponent<FiredProjectile>().nonLethal = this.nonLethal;
							g.GetComponent<Rigidbody2D>().AddForce(f*projectileSpeed);
						}			
					}
				}

				if (ejecting) {

					GameObject shelly = (GameObject)GameObject.Instantiate(particle, transform.FindChild("shellEject").transform.position, GetComponentInParent<Transform>().rotation);
					shelly.transform.localScale = new Vector3(shelly.transform.localScale.x / 2.5f, shelly.transform.localScale.z / 2.5f, shelly.transform.localScale.z / 3);
					shelly.GetComponent<SpriteRenderer>().sprite = this.shellSprite;
					shelly.AddComponent<BoxCollider2D>();
					shelly.GetComponent<Rigidbody2D>().gravityScale = 1;
					//shelly.GetComponent<Rigidbody2D>().mass = float.MinValue;
					//Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), shelly.GetComponent<Collider2D>());
					shelly.GetComponent<Rigidbody2D>().AddForceAtPosition(200 *  (.1f *Random.insideUnitCircle +  Vector2.up) * shelly.GetComponent<Rigidbody2D>().mass, shelly.transform.position);
					shelly.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-20,20));					//shelly.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + (Quaternion.AngleAxis(90, (Vector3)thing) * thing)));
					shelly.GetComponent<ParticleScript>().time = 2;
					shelly.GetComponent<ParticleScript>().shell = true;
					shelly.layer = this.transform.gameObject.layer;
				}
				for (int i = 0; i < gunGoesPoof; i++) {
					GameObject particleG =(GameObject) GameObject.Instantiate(particle, shootPoint, this.transform.rotation);
					if (this.transform.parent && this.transform.parent.parent != null) particleG.GetComponent<Rigidbody2D>().velocity = transform.parent.GetComponentInParent<Rigidbody2D>().velocity * .6f;
					particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle + thing));
					particleG.GetComponent<Rigidbody2D>().gravityScale = -.3f;
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
	void NotDeath() {
		death = false;
	}
}
