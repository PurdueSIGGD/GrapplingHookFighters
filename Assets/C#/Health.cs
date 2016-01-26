﻿/*
 * A beginning for Health managment. Will probably need a lot of changes.
 * Maybe some things in Start will change depending on player performance.
 */
using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	//player Health should not exceed 1 and armorHealth should not exceed 2
	private int playerHealth;
	private int armorHealth;
	public bool dead;
	public int deathSparkleParticle;
	public GameObject particle;
	public GameObject[] gibs;
	private bool[] usedGibs;
	private BoxCollider2D box;
	public GameObject part;



	public int getPlayerHealth() {
		return playerHealth;
	}

	public int getArmorHealth() {
		return armorHealth;
	}

	//Will reduce Player health  or Armor by one
	void hit() {
		if (armorHealth < 1)
			killPlayer ();
		if (armorHealth > 0) {
			armorHealth -= 1;
			if (armorHealth == 0) {
				this.SendMessage ("throwWeapont", 2);
			}
		}
	}

	//Reduces Player health and/or armor by dmgAmount.
	public void hit(int dmgAmount) {	
		if (dmgAmount >= playerHealth + armorHealth)
			killPlayer ();
		if (dmgAmount == armorHealth)
			armorHealth = 0;
	}

	//Adds armor to armorHealth
	public void pickUpArmor() {
		if (armorHealth < 2) 
			armorHealth += 1;
	}
	//drops armor
	public void dropArmor() {
		armorHealth = 0;
	}
	//Add armor input to armorHealth
	public void pickUpArmor(int armor) {
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
	}

	//kills the player....
	public void killPlayer() {
		if (!dead) {
			playerHealth = 0;
			armorHealth = 0;
			dead = true;
			box = this.gameObject.GetComponent<BoxCollider2D>();
			box.size = 2* (Vector2.up + Vector2.right);
			box.isTrigger = true;

			/*for (int i = 0; i < deathSparkleParticle; i++) {
				GameObject particleG = (GameObject)GameObject.Instantiate (particle, this.transform.position + Vector3.back, this.transform.rotation);
				if (this.transform.parent && this.transform.parent.parent != null)
					particleG.GetComponent<Rigidbody2D> ().velocity = transform.parent.GetComponentInParent<Rigidbody2D> ().velocity * .6f;

				particleG.GetComponent<Rigidbody2D> ().gravityScale = .05f;
				particleG.GetComponent<Rigidbody2D> ().AddForce (.02f * (Random.insideUnitCircle + Vector2.up));
				particleG.GetComponent<ParticleScript> ().time = .6f;
			}*/

			this.BroadcastMessage("Death");
			GameObject.Find("Boundary").SendMessage("SetInRespawnQueue", this.gameObject);

		}
	}
	public void Gib(int i) { //i being the number of gibs it requests
		if (dead) {
			for (int j = 0; j < i; j++) {
				int range = Random.Range (0, gibs.Length);
				if (!usedGibs [range]) {
					GameObject g = (GameObject)GameObject.Instantiate (gibs [range], transform.position, Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360))));
					g.GetComponent<Rigidbody2D> ().AddForce (Random.insideUnitCircle * i);
					g.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0, i * 10));
					g.transform.FindChild ("GameObject").GetComponent<SpriteRenderer> ().color = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ().color;
					usedGibs [range] = true;
				}
			}
		}
	}
	//Restore players beginning status
	public void resetPlayer() {
		playerHealth = 1;
		armorHealth = 0;
		dead = false;
		for (int i = 0; i < usedGibs.Length; i++)
			usedGibs [i] = false;
		//```Destroy(box);
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

		playerHealth = 1;
		armorHealth = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Bleed() {
		if (!transform.FindChild ("ParticleBleed")) {
			GameObject g = (GameObject)GameObject.Instantiate (part, this.transform.position, Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360))));
			g.transform.parent = this.transform;
			g.name = "ParticleBleed";
		}

	}
}
