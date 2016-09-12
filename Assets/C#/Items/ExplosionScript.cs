using UnityEngine;
using System.Collections;
//using UnityEditor;
public class ExplosionScript : MonoBehaviour {

	public float samples = 8; //how many pieces of shrapnel will be shot out of the explosion
	public float range = 1.5f; //how far shots go

	// Use this for initialization
	void Start () {
		foreach (grenade g in transform.GetComponentsInChildren<grenade>()) {
			g.SendMessage("Explode");
		}
		for (int i = 0; i < samples; i++) {
			
			Vector2 rand = Random.insideUnitCircle;
			rand = rand / rand.magnitude;
			//turn into unit vector

			int layermask = (1 << this.gameObject.layer) + (1 << 13) + (1 << 15);
			RaycastHit2D[] rr = Physics2D.RaycastAll(transform.position, rand, range, layermask);
			Debug.DrawLine (transform.position, transform.position + range*(Vector3)rand, Color.green, 100f);
			bool hit = false;
			//bool tooCloseToCare = Vector2.Distance(transform.position, c.transform.position) < .2f;
			//if (c.GetComponent<player>()) print(Vector2.Distance(transform.position, c.transform.position));
			//if (!tooCloseToCare) {
			//EditorApplication.isPaused = true;
			RaycastHit2D myHit = new RaycastHit2D();
			foreach (RaycastHit2D r in rr) {
				//print(r.transform.name);
				//reasons that we want to ignore this thing we are hitting 
				if (r.transform != transform && 
					!r.transform.GetComponent<FiredProjectile>() &&
					!r.transform.GetComponent<ExplosionScript>() &&
					!r.collider.isTrigger 
					//(r.transform.gameObject.layer != 13 || Vector2.Distance(r.transform.position, transform.position ) < )
				)
				{
					//print(r.transform.name);
					myHit = r;
					hit = true;
					break;
				}

			}

			if (hit) {
				//print (myHit.transform.name); 
				if (myHit.transform.GetComponent<grenade>()) {
					myHit.transform.SendMessage("Explode");
				}
				Rigidbody2D rg;
				if ((rg = myHit.transform.GetComponent<Rigidbody2D>()) != null && myHit.transform.GetComponent<FiredProjectile>() == null) {
					rg.AddForce (300 * rg.mass * (myHit.transform.position - this.transform.position));
					rg.AddForce (150 * rg.mass * Vector2.up);
				}
				if (myHit.transform.GetComponent<Hittable> ()) {
					myHit.transform.SendMessage("hit",  100 /Vector2.Distance(this.transform.position, myHit.transform.position));
					if (myHit.transform.GetComponent<Health>()) myHit.transform.SendMessage("Gib",transform.position);
				}
				if (myHit.transform.GetComponent<ShootableItem> () && !myHit.transform.GetComponent<Collider2D>().isTrigger) {
					
					myHit.transform.SendMessage("hit", 150);
				}
			}
		}
		if (transform.childCount > 1) {
			for (int i = 0; i < transform.childCount; i++) {
				if (!transform.GetChild(i).GetComponent<Animator>() && !transform.GetChild(i).GetComponent<SpriteRenderer>() ) { //delete stuck items, but not animator or light
					//print(transform.GetChild(i).name);
					GameObject.Destroy(transform.GetChild(i).gameObject);
				
				}
					
			}
		}
		//print("ugh");
		//EditorApplication.isPaused = true;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawLine(this.transform.position,this.transform.position + Vector3.right * this.GetComponent<CircleCollider2D> ().radius);
		Animator a = this.GetComponentInChildren<Animator> ();
		if (a.GetTime () > .3) {
			this.GetComponent<CircleCollider2D> ().enabled = false;

		}
		if (a.GetTime () > .4f)
				Destroy (this.gameObject);
	}
	void OnCollisionEnter2D(Collision2D col) {
		//print(col.transform.name);


		/*if (col.transform.GetComponent<ShootablePlatform> ()) {
			col.transform.SendMessage ("hit", 7);
		}*/

	}

}
