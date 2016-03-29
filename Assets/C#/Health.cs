﻿/*
 * A beginning for Health managment. Will probably need a lot of changes.
 * Maybe some things in Start will change depending on player performance.
 */
using UnityEngine;
using System.Collections;
//using UnityEditor;
public class Health : MonoBehaviour {

	//player Health should not exceed 1 and armorHealth should not exceed 2
	private float playerHealth;
	private float armorHealth;
	public bool dead;
	public int deathSparkleParticle;
	public GameObject particle;
	public GameObject[] gibs;
	private bool[] usedGibs;
	private BoxCollider2D box;
	public GameObject part;
	public Vector2[] deadPoints;
	private Vector2[] alivePoints;
	private Transform healthIcon;
	public float deadTime;
	public bool ignorePosition;
	public Vector3 boundaryPlace;

	public GameObject gibHolder, splatterGib;
	public Sprite deadSprite;
	public Sprite aliveSprite;



	public float getPlayerHealth() {
		return playerHealth;
	}

	public float getArmorHealth() {
		return armorHealth;
	}

	//Will reduce Player health  or Armor by one
	/*void hit() {
		if (!dead) {
			if (armorHealth < 1)
				killPlayer ();
			if (armorHealth > 0) {
				armorHealth -= 1;
				if (armorHealth == 0) {
					this.SendMessage ("throwWeapont", 2);
				}
			}
		}
	}*/

	//Reduces Player health and/or armor by dmgAmount.
	public void hit(float dmgAmount) {	
		print(dmgAmount);
		if (!dead) {
			//print(dmgAmount + " " + playerHealth + " " + armorHealth);
			//print(dmgAmount >= playerHealth + armorHealth);
			if (dmgAmount < playerHealth + armorHealth) {
				float diff = armorHealth - dmgAmount;
				if (armorHealth > 0) {
					if (diff > 0) { //armor still remains
						armorHealth -= dmgAmount;
					} else { //armor eis gon
						this.SendMessage ("throwWeapont", 2);
						armorHealth = 0;
					}
				} else if (diff < 0) { //more damage than armor
					if (diff * -1 >= playerHealth) {
						killPlayer();
						if (diff < -50) { //lots of damage
							Gib((int)(diff / 50));
						}
					} else {

						playerHealth += diff;
					}
				} 

			} else {
				killPlayer();
			}
			print(playerHealth);

		}
	}

	//Adds armor to armorHealth
	public void pickUpArmor() {
		armorHealth = 50;
	}
	//drops armor
	public void dropArmor() {
		armorHealth = 0;
	}
	//Add armor input to armorHealth
	/*public void pickUpArmor(int armor) {
		if (armor != 0) {
			if (armor == 1) {
				if (armorHealth == 1) 
					armorHealth = 2;
				if (armorHealth == 0)
					armorHealth = 1;
			}
			if (armor >= 2) {
				armorHealth = 2;
			}
		}
	}*/
	public void killPlayer() {
		killPlayer (false);
	}

	//params: b, if b, the player has passed the boundary.
	//kills the player....
	public void killPlayer(bool b) {
		if (b) {
			//passed boundary is used to know if the player has died from a boundary
			this.ignorePosition = true;
			//boundaryplace is used to know the last place before death
			this.boundaryPlace = transform.position;
			//print(b);
		}
		if (!dead) {
			this.boundaryPlace = transform.position;

			transform.GetComponent<Rigidbody2D>().AddForce(200 * (Vector2.up + Random.insideUnitCircle));
			//transform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-15,15));
			aliveSprite = this.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite;
			this.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = deadSprite;
			alivePoints = transform.GetComponent<PolygonCollider2D>().points;
			transform.GetComponent<PolygonCollider2D>().points = deadPoints;
			playerHealth = 0;
			armorHealth = 0;
			dead = true;
			box = this.gameObject.GetComponent<BoxCollider2D>();
			box.size = 2* (Vector2.up + Vector2.right);
			box.isTrigger = true;
			GameObject ragdoll = (GameObject)GameObject.Instantiate(gibHolder, transform.position, Quaternion.identity);
			ragdoll.transform.parent = transform;
			ragdoll.transform.localPosition = new Vector3(.088f,-0.002f,0);
			Rigidbody2D[] rg = transform.FindChild("GibHolder(Clone)").GetComponentsInChildren<Rigidbody2D>();
			foreach (Rigidbody2D r in rg) {
				Physics2D.IgnoreCollision(r.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
			}
			SpriteRenderer[] sp = transform.FindChild("GibHolder(Clone)").GetComponentsInChildren<SpriteRenderer>();
			Color c = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ().color;
			foreach (SpriteRenderer s in sp) {
				s.color = c;
			}
			//g.transform.GetComponentInChildren<SpriteRenderer> ().color = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ().color;

			//EditorApplication.isPaused = true;
		//	transform.FindChild("GibHolder").gameObject.SetActive(true);
			/*for (int i = 0; i < deathSparkleParticle; i++) {
				GameObject particleG = (GameObject)GameObject.Instantiate (particle, this.transform.position + Vector3.back, this.transform.rotation);
				if (this.transform.parent && this.transform.parent.parent != null)
					particleG.GetComponent<Rigidbody2D> ().velocity = transform.parent.GetComponentInParent<Rigidbody2D> ().velocity * .6f;

				particleG.GetComponent<Rigidbody2D> ().gravityScale = .05f;
				particleG.GetComponent<Rigidbody2D> ().AddForce (.02f * (Random.insideUnitCircle + Vector2.up));
				particleG.GetComponent<ParticleScript> ().time = .6f;
			}*/

			this.BroadcastMessage("Death");
			//print("dying");
			if (GameObject.Find("SceneController")) {
				GameObject.Find("SceneController").SendMessage("AddDeath"); 
				//print("adding death");
			}
			else
				if (GameObject.Find("Boundary").GetComponent<Boundary>().respawning) GameObject.Find("Boundary").SendMessage("SetInRespawnQueue", this.gameObject);

		}
	}
	public void Gib(int i) { //i being the number of gibs it requests
		if (dead) {
			for (int j = 0; j < i; j++) {
				int range = Random.Range (0, gibs.Length);
				if (!usedGibs [range]) {
					GameObject g = null;
					switch (range) 
					{
						case 0: //headz
						g = transform.FindChild("GibHolder(Clone)").FindChild("Head").gameObject;
						transform.FindChild("GibHolder(Clone)").FindChild("HeadHome").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 1: //arm1
						g = transform.FindChild("GibHolder(Clone)").FindChild("Arm1").gameObject;
						transform.FindChild("GibHolder(Clone)").FindChild("ArmHome1").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 2: //arm2
						g = transform.FindChild("GibHolder(Clone)").FindChild("Arm2").gameObject;
						transform.FindChild("GibHolder(Clone)").FindChild("ArmHome2").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 3: //leg1
						g = transform.FindChild("GibHolder(Clone)").FindChild("Leg1").gameObject;
						transform.FindChild("GibHolder(Clone)").FindChild("LegHome1").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 4: //leg2
						g = transform.FindChild("GibHolder(Clone)").FindChild("Leg2").gameObject;
						transform.FindChild("GibHolder(Clone)").FindChild("LegHome2").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						default:
							break;
					}
					g.layer = transform.gameObject.layer;
					//GameObject g = (GameObject)GameObject.Instantiate (gibs [range], transform.position, Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360))));
					GameObject splats = (GameObject)GameObject.Instantiate(splatterGib, transform.position, Quaternion.identity); 
					splats.transform.parent = g.transform;
					splats.transform.localScale = Vector3.one;
					splats.transform.localPosition = Vector3.zero;
					g.transform.parent = null;
					//print( this.GetComponent<Rigidbody2D>().velocity);
					//EditorApplication.isPaused = true;
					g.GetComponent<Rigidbody2D> ().AddForce (Random.insideUnitCircle + this.GetComponent<Rigidbody2D>().velocity);
					g.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0, i * 10));
					BoxCollider2D b = g.AddComponent<BoxCollider2D>();
					b.isTrigger = true;
					b.size = new Vector2(.4f, .4f);
					g.AddComponent<HeldItem>();

					g.transform.GetComponentInChildren<SpriteRenderer> ().color = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ().color;
					usedGibs [range] = true;
				}
			}
		}
	}
	//Restore players beginning status
	public void resetPlayer() {
		playerHealth = 100;
		armorHealth = 0;
		ignorePosition = false;
		dead = false;
		for (int i = 0; i < usedGibs.Length; i++)
			usedGibs [i] = false;
		//```Destroy(box);
		GameObject.Destroy(transform.FindChild("GibHolder(Clone)").gameObject);
		transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = aliveSprite;

		//transform.FindChild("GibHolder").gameObject.SetActive(false);
		transform.GetComponent<PolygonCollider2D>().points = alivePoints;
		Transform ch = transform.FindChild ("ParticleBleed");
		if (ch) {
			GameObject.Destroy (ch.gameObject);
		}
		//destroy stray stickybombs, arrows
		FiredProjectile[] ffs = transform.GetComponentsInChildren<FiredProjectile>();
		if (ffs.Length > 0) {
			foreach (FiredProjectile f in ffs) {
				GameObject.Destroy(f.gameObject);
			}
		}
	}

	// Use this for initialization
	void Start () {
		usedGibs = new bool[gibs.Length];

		playerHealth = 100;
		armorHealth = 0;
		healthIcon = transform.FindChild("HealthIcon");
	}
	
	// Update is called once per frame
	void Update () {
		//healthIcon;
		healthIcon.localScale = new Vector3(playerHealth/100, 1, 1);
		healthIcon.FindChild("Color").GetComponent<SpriteRenderer>().color = new Color(1-playerHealth/100,playerHealth/100,0);
		if (dead) {
			deadTime += Time.deltaTime;
		} else {
			deadTime = 0;
		}
	}
	void Bleed() {
		if (!transform.FindChild ("ParticleBleed") && playerHealth <= 0) {
			GameObject g = (GameObject)GameObject.Instantiate (part, this.transform.position, Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360))));
			g.transform.parent = this.transform;
			g.name = "ParticleBleed";
		}

	}
}
