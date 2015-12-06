using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {

    private bool pullPin = false;
    private bool pinPulled = false;
	public bool sticky;
    public float fuseTime, fuseParticleInterval;
    private float timePassed = 0;

    public GameObject explosion, particle;

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
			if (fuseParticleInterval > .3f) {
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
        if (timePassed >= fuseTime) {
			this.transform.parent = null;
			GameObject ex;
			if (!sticky) 
           		ex = (GameObject)GameObject.Instantiate(explosion, transform.FindChild("Sphere").transform.position, Quaternion.identity);
            else
				ex = (GameObject)GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
			ex.gameObject.layer = this.gameObject.layer;


            Destroy(gameObject);
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

		}
	}
	void OnCollisionStay2D(Collision2D col) {
		//print(col);
	}
}
