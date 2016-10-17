/*
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
	private Transform healthIcon, armorIcon;
	public float deadTime;
	public bool ignorePosition;
	public Vector3 boundaryPlace;

	public GameObject gibHolder, splatterGib;
	public Sprite deadSprite;
	public Sprite aliveSprite;

	private GameObject ragdoll;

	private Rigidbody2D myRigid;
	private PolygonCollider2D myPolygon;
	private SpriteRenderer mySprite, healthSprite;


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
		//print(dmgAmount);
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
							//Gib((int)(diff / 50));
						}
					} else {

						playerHealth += diff;
					}
				} 

			} else {
				killPlayer();
			}

		}
	}

	//Adds armor to armorHealth
	public void pickUpArmor(PassivePickup armor) {
		armorHealth += armor.armorHealth;
	}
	//drops armor
	public void dropArmor(PassivePickup armor) {
		armor.armorHealth = armorHealth;
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
		
			//this.boundaryPlace = transform.position;
		if (!dead) {
			myRigid.AddForce(200 * (Vector2.up + Random.insideUnitCircle));
			//transform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-15,15));
			aliveSprite = mySprite.sprite;
			mySprite.sprite = deadSprite;
			alivePoints = myPolygon.points;
			myPolygon.points = deadPoints;
			playerHealth = 0;
			armorHealth = 0;
			dead = true;
			box = this.gameObject.GetComponent<BoxCollider2D>();
			box.size = 2* (Vector2.up + Vector2.right);
			box.isTrigger = true;
			ragdoll = (GameObject)GameObject.Instantiate(gibHolder, transform.position, Quaternion.identity);
			ragdoll.transform.parent = transform;
			ragdoll.transform.localPosition = new Vector3(-.088f,0,0);
			Rigidbody2D[] rg = ragdoll.GetComponentsInChildren<Rigidbody2D>();
			foreach (Rigidbody2D r in rg) {
				Physics2D.IgnoreCollision(r.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
			}
			SpriteRenderer[] sp = ragdoll.GetComponentsInChildren<SpriteRenderer>();
			Color c = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, 1);
            foreach (SpriteRenderer s in sp) {
				s.color = c;
			}
			foreach (Sticky s in transform.GetComponentsInChildren<Sticky>()) {
				//stickybombs, arrows, or anything else stuck in our legs iwll move to them
				if (s.transform.localPosition.y < 0) {
					if (s.transform.localPosition.x > 0) {
						s.transform.parent = ragdoll.transform.FindChild("Leg1");
					} else {
						s.transform.parent = ragdoll.transform.FindChild("Leg2");
					}
				}
			}

			this.BroadcastMessage("Death");
			//print("dying");
			if (GameObject.Find("SceneController")) {
				GameObject.Find("SceneController").SendMessage("AddDeath"); 
				//print("adding death");
			}
			//else
				//if (GameObject.Find("Boundary").GetComponent<Boundary>().respawning) GameObject.Find("Boundary").SendMessage("SetInRespawnQueue", this.gameObject);

		}
	}
	public void Gib(Vector3 t) { //t being where the explosion was
		if (dead) {
			int length = Random.Range(0,4);
			for (int j = 0; j < length; j++) {
				int range = Random.Range (0, 5 + 3); //5 for each limb, 3 for position based gibbing

				if (range >= 5) {
					if (Random.Range(0,1) == 1) {
						if (t.y > transform.position.y) 
							range = 0;
							//head
						if (t.y < transform.position.y) 
							range = Random.Range(0,1)==1?3:4;
						//head
					} else {
						if (t.x < transform.position.x) 
							range = Random.Range(0,1)==1?2:4;
							//left arm or left leg
						else if (t.x > transform.position.x)
							range = Random.Range(0,1)==1?1:3;
							//right arm or right leg
					}
				}
				//print(range);
				if (!usedGibs [range]) {
					GameObject g = null;
					switch (range) 
					{
						case 0: //headz
						g = ragdoll.transform.FindChild("Head").gameObject;
						ragdoll.transform.FindChild("HeadHome").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 1: //arm1
						g = ragdoll.transform.FindChild("Arm1").gameObject;
						ragdoll.transform.FindChild("ArmHome1").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 2: //arm2
						g = ragdoll.transform.FindChild("Arm2").gameObject;
						ragdoll.transform.FindChild("ArmHome2").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 3: //leg1
						g = ragdoll.transform.FindChild("Leg1").gameObject;
						ragdoll.transform.FindChild("Hip").FindChild("LegHome1").GetComponent<HingeJoint2D>().connectedBody = null;
							break;
						case 4: //leg2
						g = ragdoll.transform.FindChild("Leg2").gameObject;
						ragdoll.transform.FindChild("Hip").FindChild("LegHome2").GetComponent<HingeJoint2D>().connectedBody = null;
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
					g.GetComponent<Rigidbody2D> ().AddForce (Random.insideUnitCircle + myRigid.velocity);
					g.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0,  10));
					BoxCollider2D b = g.AddComponent<BoxCollider2D>();
					b.isTrigger = true;
					b.size = new Vector2(.4f, .4f);
					g.AddComponent<HeldItem>();
                    Color cc = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, 1);
					g.transform.GetComponentInChildren<SpriteRenderer> ().color = cc;
					usedGibs [range] = true;
				}
			}
		}
	}
	//Restore players beginning status
	public void resetPlayer() {
		playerHealth = 100;
		armorHealth = 0;
		deadTime = 0;
		ignorePosition = false;
		dead = false;
		for (int i = 0; i < usedGibs.Length; i++)
			usedGibs [i] = false;
		//```Destroy(box);
		GameObject.Destroy(ragdoll);
		mySprite.sprite = aliveSprite;
		//transform.FindChild("GibHolder").gameObject.SetActive(false);
		myPolygon.points = alivePoints;
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
		
		usedGibs = new bool[5];

		playerHealth = 100;
		armorHealth = 0;
		healthIcon = transform.FindChild("HealthIcon");
		armorIcon = transform.FindChild("ArmorIcon");
		myRigid = this.GetComponent<Rigidbody2D>();
		myPolygon = transform.GetComponent<PolygonCollider2D>();
		healthSprite = healthIcon.FindChild("Color").GetComponent<SpriteRenderer>();
		mySprite = transform.FindChild("AnimationController").FindChild("Legs").GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		//healthIcon;
		healthIcon.localScale = new Vector3(playerHealth/100, 1, 1);
		healthSprite.color = new Color(1-playerHealth/100,playerHealth/100,0, healthSprite.color.a);

		armorIcon.localScale = new Vector3(armorHealth/100, 1, 1);
		if (dead) {
			deadTime += Time.deltaTime;
		} else {
			deadTime = 0;
		}
	}
	void Bleed() {
		if (!transform.FindChild ("ParticleBleed") && playerHealth <= 0) {
			/*GameObject g = (GameObject)GameObject.Instantiate (part, this.transform.position, Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360))));

			g.transform.parent = this.transform;
			g.transform.localScale = Vector3.one;
			g.name = "ParticleBleed";*/
			//lets not bleed for now
		}

	}
    public void ApplyColors(float f)
    {
        Color c = new Color(1, 1, 1, f);
        healthIcon.GetComponentInChildren<SpriteRenderer>().color = new Color(healthSprite.color.r, healthSprite.color.g, healthSprite.color.b, f);
        armorIcon.GetComponentInChildren<SpriteRenderer>().color = c;

    }
}
