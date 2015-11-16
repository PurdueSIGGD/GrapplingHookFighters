using UnityEngine;
using System.Collections;

public class HeldItem : MonoBehaviour {
	private bool retrigger;
	private float timeSinceDropped;
	private Collider2D lastCol;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (retrigger) {
			timeSinceDropped += Time.deltaTime;
			if (timeSinceDropped > .2f) {
				if (this.GetComponent<player>()) Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), lastCol, false);
				else Physics2D.IgnoreCollision(this.GetComponent<PolygonCollider2D>(), lastCol, false);
				retrigger = false;
				timeSinceDropped = 0;
			}
			
		}
	}
	void retriggerSoon() {
		retrigger = true;
	}
	void ignoreColl(Collider2D col) {
		lastCol = col;
		if (this.GetComponent<player>()){
			Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), lastCol);
		}
		else { 
			print("hey now");
			Physics2D.IgnoreCollision(this.GetComponent<PolygonCollider2D>(), lastCol);	
		}
	}
}
