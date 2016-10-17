using UnityEngine;
using System.Collections;

public class PassivePickup : MonoBehaviour {
	public int itemCode; //determines what it does. Y = done, N = not done
	/* 0:Y armor - send message to health script, enable armor which deflects one hit
	 * 1:Y jetpack - jumping adds thrust, limited
	 * 2:Y rollerskates - less friction
	 * 3:Y invisible cloak - sprite becomes invisible, but guns do not
	 * 4:Y spiky boots
	 * 
	 * 
	 */
	public Vector2 offset;
	public float armorHealth = 50;
	private GameObject focus;
	public bool broke;
	private float originalMass;
	private Color focusColor;


	void Pickup(GameObject g) {
		focus = g;
		//print ("Ohhh you got me");
		switch (itemCode) {
		case 0:
			originalMass = g.GetComponent<Rigidbody2D> ().mass;
			//g.GetComponent<Rigidbody2D> ().mass *= 1.3f;
			g.GetComponent<Health> ().SendMessage ("pickUpArmor", this);
			break;
		case 1:
			broke = false;
			g.GetComponent<player> ().jetpack = true;
			if (this.transform.name != "Jetpack") transform.name = "Jetpack";
			this.GetComponentInChildren<ParticleSystem>().Stop();
			this.GetComponentInChildren<Animator>().SetBool("Flying", false);
			break;
		case 2:
			focus.GetComponent<player> ().skateBoard = true;
			g.GetComponent<PolygonCollider2D> ().sharedMaterial.friction = 0;
			g.GetComponent<PolygonCollider2D> ().enabled = false;
			g.GetComponent<PolygonCollider2D> ().enabled = true; //have it reset and rebooted, unity is weird
			g.GetComponent<player> ().maxMoveSpeed = 25;
			break;
		case 3:
                ChangeColors(true);

                break;
		case 4:
			Physics2D.IgnoreCollision (g.GetComponent<Collider2D> (), transform.FindChild ("Hazard").GetComponent<Collider2D>());
			transform.FindChild("Hazard").GetComponent<BoxCollider2D> ().enabled = true;
			break;
		default:
			break;
		}
	}
	void Drop(int i) { //I being whatever you want to say to them
		//print ("Bye");
		switch (itemCode) {
		case 0:
			if (focus)focus.GetComponent<Health> ().SendMessage ("dropArmor", this);
			if (i == 1) {
				broke = true;
			}
			focus.GetComponent<Rigidbody2D> ().mass = originalMass;
			break;
		case 1:
			focus.GetComponent<player> ().jetpack = false;
			if (this.GetComponentInChildren<ParticleSystem>().isPlaying) {
				broke = true;
				this.GetComponentInChildren<ParticleSystem>().Play();
				this.GetComponentInChildren<Animator>().SetBool("Flying", true);
			}
			break;
		case 2:
			focus.GetComponent<player> ().skateBoard = false;
			focus.GetComponent<PolygonCollider2D> ().sharedMaterial.friction = .4f;
			focus.GetComponent<PolygonCollider2D> ().enabled = false;
			focus.GetComponent<PolygonCollider2D> ().enabled = true; //have it reset and rebooted
			focus.GetComponent<player> ().maxMoveSpeed = 10;
			break;
		case 3:
                ChangeColors(false);
			break;
		case 4:
			transform.FindChild("Hazard").GetComponent<BoxCollider2D> ().enabled = false;
			Physics2D.IgnoreCollision (focus.GetComponent<Collider2D> (), transform.FindChild ("Hazard").GetComponent<Collider2D>(), false);

			break;
		default:
			break;
		}

		focus = null;
	}
	void Update() {
		if (broke && itemCode == 0 && GetComponent<HeldItem>()) {
			Destroy (this.GetComponent<HeldItem> ());
			this.gameObject.tag = "Effect";
			this.gameObject.layer += 3;
			this.GetComponentInChildren<SpriteRenderer> ().color = new Color (.25f, .25f, .25f);
		}

		if (broke && itemCode == 1) { //jetpack flinging 
			
			this.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * 500 * Time.deltaTime);
		}
	}
	void OnApplicationQuit() {
		if (focus) {
			focus.GetComponent<PolygonCollider2D> ().sharedMaterial.friction = .4f;
			focus.GetComponent<PolygonCollider2D> ().sharedMaterial.bounciness = 0;
		}
	}
    void ChangeColors(bool on)
    {
        Color c = focus.transform.FindChild("AnimationController").GetComponent<AnimationHandler>().startColor;
        focus.transform.FindChild("AnimationController").GetComponent<AnimationHandler>().startColor = new Color(c.r, c.g, c.b, on?0.2f:1);
        focus.transform.FindChild("AnimationController").GetComponent<AnimationHandler>().ApplyColor();
        this.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, on ? 0.2f : 1);
        // Reticle
        focus.GetComponent<player>().reticle.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, on ? 0.2f : 1);
        //Health and armor bar
        focus.GetComponent<Health>().SendMessage("ApplyColors", on ? 0.2 : 1);
        //grapple 
        focus.GetComponent<GrappleLauncher>().SendMessage("ApplyColors", on ? 0.2 : 1);


        /*focus.transform.FindChild ("Hip").GetComponent<SpriteRenderer> ().color = focusColor;
        this.GetComponentInChildren<SpriteRenderer>().color = new Color (1, 1, 1, 1);
        focus.GetComponent<GrappleLauncher>().firedGrapple.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
        focus.transform.FindChild("AimerBody").GetComponentInChildren<SpriteRenderer>().color = new Color(1,1,1,1);*/
        //	focus.GetComponent<LineRenderer> ().SetColors (new Color (1, 0, 0, 1), new Color (1, 0, 0, 0));

    }
}
