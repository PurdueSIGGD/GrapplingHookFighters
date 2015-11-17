using UnityEngine;
using System.Collections;

public class grenade : MonoBehaviour {

    private bool pullPin = false;
    private bool pinPulled = false;

    public float fuseTime;
    private float timePassed = 0;

    public GameObject explosion;

    public void Update() {
        if (pullPin) {
            pullPin = false;
            pinPulled = true;
            transform.FindChild("Pin").GetComponent<Rigidbody2D>().isKinematic = false;
            transform.FindChild("Pin").GetComponent<CircleCollider2D>().isTrigger = false;
            transform.FindChild("Pin").transform.parent = null;
        }
        if (pinPulled) {
            timePassed += Time.deltaTime;
        }
        if (timePassed >= fuseTime) {
            GameObject ex = (GameObject)GameObject.Instantiate(explosion, this.transform.position, Quaternion.identity);
            ex.gameObject.layer = this.gameObject.layer;
            Destroy(gameObject);
        }
    }

	public void click() {
        if (!pinPulled)
            pullPin = true;
    }

    public void unclick() {

    }
}
