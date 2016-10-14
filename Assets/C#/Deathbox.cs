using UnityEngine;
using System.Collections;

public class Deathbox : MonoBehaviour {

	public int force;
	public int recoverForce;//force for when one axis is zero
	Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D> ();

		rb.AddForce (new Vector2(Random.Range(-1,2)*force,Random.Range(-1,2)*force));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){

		if((rb.velocity.x>-2&&rb.velocity.x<2)){
			rb.AddForce (new Vector2(Random.Range(-1,2)*force,0));
		}
		if((rb.velocity.y>-2&&rb.velocity.y<2)){
			rb.AddForce (new Vector2(0,Random.Range(-1,2)*force));
		}
		return;
	}
}
