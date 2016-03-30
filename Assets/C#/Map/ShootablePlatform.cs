using UnityEngine;
using System.Collections;

public class ShootablePlatform : MonoBehaviour {
	public int health = 1;
	public GameObject gibs;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void hit(int j) {
		int diff = Mathf.Abs(health - j);
		health-= j;
		if (diff > 30) diff = 30;
		if (health <= 0) {
			GameObject thing = (GameObject)GameObject.Instantiate (gibs, this.transform.position, this.transform.rotation);
			for (int i = 0; i < thing.transform.childCount; i++) {
				Transform t = thing.transform.GetChild (i);
				t.gameObject.layer = this.gameObject.layer - 2;
				if (t.GetComponent<Rigidbody2D> ()) {
					Rigidbody2D rg = t.GetComponent<Rigidbody2D> ();
					rg.AddTorque (Random.Range (0, .5f * diff));
					rg.AddForce (Random.insideUnitCircle * 3 * diff);
				}
			}
			GameObject.Destroy (this.gameObject);
		}
	}
}
