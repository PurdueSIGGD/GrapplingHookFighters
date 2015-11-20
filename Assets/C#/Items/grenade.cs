using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {

    private bool pullPin = false;
    private bool pinPulled = false;

    public float fuseTime, fuseParticleInterval;
    private float timePassed = 0;

    public GameObject explosion, particle;

    public void Update() {
        if (pullPin) {
            pullPin = false;
            pinPulled = true;
            transform.FindChild("Pin").GetComponent<Rigidbody2D>().isKinematic = false;
			transform.FindChild("Pin").GetComponent<Rigidbody2D>().AddForce(Vector3.up);
            transform.FindChild("Pin").GetComponent<CircleCollider2D>().isTrigger = false;
            transform.FindChild("Pin").transform.parent = null;
        }
        if (pinPulled) {
			fuseParticleInterval += Time.deltaTime;
            timePassed += Time.deltaTime;
			if (fuseParticleInterval > .3f) {
				GameObject particleG =(GameObject) GameObject.Instantiate(particle, transform.FindChild("Cube").transform.position + Vector3.forward * .05f, this.transform.rotation);
				particleG.GetComponent<Rigidbody2D>().AddForce(.015f * (Random.insideUnitCircle));
				particleG.GetComponent<Rigidbody2D>().gravityScale = -.3f;
				particleG.GetComponent<ParticleScript>().time = .35f;
			}
        }
        if (timePassed >= fuseTime) {
            GameObject ex = (GameObject)GameObject.Instantiate(explosion, transform.FindChild("Sphere").transform.position, Quaternion.identity);
            ex.gameObject.layer = this.gameObject.layer;
            Destroy(gameObject);
        }
    }

	public void click() {
        if (!pinPulled)
            pullPin = true;
    }
	void Explode() {
		timePassed = fuseTime;
	}
    public void unclick() {

    }
}
