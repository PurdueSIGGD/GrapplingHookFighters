using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {

    private bool pullPin = false;
    public bool pinPulled = false, smokey;
	public bool sticky, exploded;
    public float fuseTime, fuseParticleInterval;
    private float timePassed = 0;

    public GameObject explosion, particle, smokeBomb;

	public void Start() {
	}

    public void Update() {
		if (transform.parent && transform.parent.GetComponent<ExplosionScript>()) Explode();
		//if (transform.parent) transform.localScale = new Vector3(1.2f/transform.parent.localScale.x,1.2f/transform.parent.localScale.y,1.2f/transform.parent.localScale.z);

        if (pullPin) {
            pullPin = false;
            pinPulled = true;
            transform.FindChild("Pin").GetComponent<Rigidbody2D>().isKinematic = false;
			transform.FindChild("Pin").GetComponent<Rigidbody2D>().AddForce(Vector3.up);
            transform.FindChild("Pin").GetComponent<CircleCollider2D>().isTrigger = false;
            transform.FindChild("Pin").transform.parent = null;
        }
        if (pinPulled || sticky) {
			fuseParticleInterval += Time.deltaTime;
            timePassed += Time.deltaTime;
			if (fuseParticleInterval > .3f && !exploded) {
				GameObject particleG;
				if (!sticky) 
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
			this.transform.parent = null;
			GameObject ex;
			if (smokey) 
				ex = (GameObject)GameObject.Instantiate(smokeBomb,  transform.FindChild("Sphere").transform.position, Quaternion.identity);
			else if (!sticky) 
           		ex = (GameObject)GameObject.Instantiate(explosion, transform.FindChild("Sphere").transform.position, Quaternion.identity);
            else
				ex = (GameObject)GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
			ex.gameObject.layer = this.gameObject.layer;


			if (!smokey) Destroy(gameObject);
			else {
				ex.transform.parent = this.transform;
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
    public void unclick() {

    }
	void OnCollisionEnter2D(Collision2D col) {
		if (sticky) {
			Destroy(this.GetComponent<Rigidbody2D>());
			this.transform.parent = col.transform;
			float scalex = 1 / col.transform.localScale.x ; 
			//float scaley = 1 / col.transform.localScale.y ; 
			//float scalez = 1 / col.transform.localScale.z ; 
			float childOrg = transform.localScale.x; 
			transform.localScale = new Vector3(childOrg*scalex, childOrg*scalex, childOrg*scalex);

		}
	}
	void OnCollisionStay2D(Collision2D col) {
		//print(col);
	}
}
