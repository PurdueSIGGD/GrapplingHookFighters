using UnityEngine;
using System.Collections;

public class Deathbox : MonoBehaviour {

	public int force;
	public int recoverForce;//force for when one axis is zero
	Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D> ();

        rb.AddForce(new Vector2(1, 0) * force);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void hit(Vector2 angle)
    {
        rb.velocity = angle.normalized * force/80;

    }
	void OnCollisionEnter2D(Collision2D col){

		/*if((rb.velocity.x> (force * -1) && rb.velocity.x< (force * 1))){
			rb.velocity = (new Vector2(Random.Range(-1,2)*force,0));
		}
		if((rb.velocity.y> (force * -1) && rb.velocity.y< (force * 1))){
			rb.velocity = (new Vector2(0,Random.Range(-1,2)*force));
		}*/
		return;
	}
}
