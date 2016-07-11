using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {

    private bool pullPin = false;
    public bool pinPulled = false, smokey, usePin = true;
	public bool sticky, exploded;
    public float fuseTime, fuseParticleInterval;
    private float timePassed = 0;

	private Rigidbody2D myRigid;

    public GameObject explosion, particle, smokeBomb;

	Transform pin;

	public void Start() {
		if (usePin) pin = transform.FindChild("Pin");
		myRigid = this.GetComponent<Rigidbody2D> ();
	}

    public void Update() {
		if (sticky && myRigid && timePassed > .2f) {
			int layermask = (1 << this.gameObject.layer) + (1 << 13) + (1 << 15);
			RaycastHit2D[] rr = Physics2D.RaycastAll(transform.position, myRigid.velocity, Time.deltaTime, layermask);
			RaycastHit2D myHit;
			foreach (RaycastHit2D r in rr) {
				if (r.transform != this.transform) {
					//stick it to one frame back
					transform.position = (Vector3)r.point - (Vector3)(myRigid.velocity * 5);//- (Vector3)(myRigid.velocity * Time.deltaTime);
					Stick (r.transform);
					break;
				}
			}
		}

		if (transform.parent && transform.parent.GetComponent<ExplosionScript>()) Explode();
		//if (transform.parent) transform.localScale = new Vector3(1.2f/transform.parent.localScale.x,1.2f/transform.parent.localScale.y,1.2f/transform.parent.localScale.z);

        if (pullPin) {
            pullPin = false;
            pinPulled = true;
			if (usePin) {
				pin.GetComponent<Rigidbody2D> ().isKinematic = false;
				pin.GetComponent<Rigidbody2D> ().AddForce (Vector3.up);
				pin.GetComponent<CircleCollider2D> ().isTrigger = false;
				pin.transform.parent = null;
			}
        }
        if (pinPulled || sticky) {
			fuseParticleInterval += Time.deltaTime;
            timePassed += Time.deltaTime;
			if (fuseParticleInterval > .3f && !exploded) {
				GameObject particleG;
				if (usePin) 
					particleG =(GameObject) GameObject.Instantiate(particle, transform.FindChild("Cube").transform.position + Vector3.forward * .05f, this.transform.rotation);
				else 
					particleG =(GameObject) GameObject.Instantiate(particle, transform.position + Vector3.forward * .05f, this.transform.rotation);

				particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle));
				particleG.GetComponent<Rigidbody2D>().gravityScale = -.3f;
				particleG.GetComponent<ParticleScript>().time = .35f;
			}
        } 
        if (timePassed >= fuseTime && !exploded) {
			exploded = true;
			GameObject ex;
			if (smokey) //smokebomb
				ex = (GameObject)GameObject.Instantiate(smokeBomb, transform.FindChild("Sphere").transform.position, Quaternion.identity);
			else if (!sticky) //regular grenade
           		ex = (GameObject)GameObject.Instantiate(explosion, transform.FindChild("Sphere").transform.position, Quaternion.identity);
            else //stickybomb
				ex = (GameObject)GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
			ex.gameObject.layer = this.gameObject.layer;


			if (!smokey) Destroy(gameObject);
			else {
				ex.transform.parent =  (this.transform);
				ex.transform.localScale = new Vector3(ex.transform.localScale.x,ex.transform.localScale.x,ex.transform.localScale.x);

			}
		}
    }

	public void click() {
        if (!pinPulled)
            pullPin = true;
    }
	public void Explode() {
		timePassed = fuseTime;
	}
	public void hit() {
		Explode();
	}
    public void unclick() {

    }
	void OnCollisionEnter2D(Collision2D col) {
		if (sticky) {
			Stick (col.transform);

		}
	}
	void Stick(Transform col) {
		Destroy(this.GetComponent<Rigidbody2D>());
		this.transform.SetParent (col.transform);
		float scalex = 1 / col.transform.localScale.x ; 
		//float scaley = 1 / col.transform.localScale.y ; 
		//float scalez = 1 / col.transform.localScale.z ; 
		float childOrg = transform.localScale.x; 
		transform.localScale = new Vector3(childOrg*scalex, childOrg*scalex, childOrg*scalex);
	}
	void OnCollisionStay2D(Collision2D col) {
		//print(col);
	}
}
