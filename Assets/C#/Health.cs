/*
 * A beginning for Health managment. Will probably need a lot of changes.
 * Maybe some things in Start will change depending on player performance.
 */
using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	//player Health should not exceed 1 and armorHealth should not exceed 2
	public int playerHealth;
	public int armorHealth;

	//Will reduce Player health  or Armor by one
	public void hit() {
		if (armorHealth < 1)
			killPlayer ();
		if (armorHealth > 0)
			armorHealth -= 1;
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
		playerHealth = 0;
		armorHealth = 0;
	}

	//Restore players beginning status
	public void resetPlayer() {
		playerHealth = 1;
		armorHealth = 0;
	}

	// Use this for initialization
	void Start () {

		playerHealth = 1;
		armorHealth = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
