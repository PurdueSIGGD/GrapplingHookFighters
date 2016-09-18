using UnityEngine;
using System.Collections;

public class GravityWell : MonoBehaviour {

    float rad = 10;
    float force = -500;
    // Use this for initialization

    ParticleSystem ps;
    ParticleSystem.ShapeModule shape;
 
	void Start () {
        this.GetComponent<CircleCollider2D>().radius = rad;
        this.ps = this.gameObject.GetComponent<ParticleSystem>();
        this.shape = ps.shape;
	}
	
	// Update is called once per frame
	void Update () {
        force += Time.deltaTime * 80;
        //print(force);
        if (force < 0)
        {
            this.rad -= Time.deltaTime * 0.6555f * 2;
            this.GetComponent<CircleCollider2D>().radius = rad;
            ps.startSpeed += Time.deltaTime * 0.667f * 2;
            this.shape.radius -= Time.deltaTime * 0.667f * 2;
        }else if(force > 50)
        {
            //print("destroy");
            Destroy(this.gameObject);
        }
        else if(force > 0 && force < 35)
        {
            //print("exploding state");
            this.shape.radius = 4f;
            ps.startSpeed = 30;
        }
        else
        {
			ps.Stop();
        }
	}

    void OnTriggerStay2D(Collider2D col)
    {
		Rigidbody2D colR = col.GetComponent<Rigidbody2D>();
        if (force < 0)
        {
			if (colR != null && col.transform.GetComponent<FiredProjectile>() == null && !colR.CompareTag ("Grapple"))
            {
				colR.AddForce((force)* 2 * Time.deltaTime * colR.mass * (col.transform.position - this.transform.position));
            }
        }
        else if(force > 0 && force < 35 / 2)
        {
            this.GetComponent<CircleCollider2D>().radius = 3f;
			if (colR != null && col.transform.GetComponent<FiredProjectile>() == null && !colR.CompareTag ("Grapple"))
            {
				colR.AddForce(100 * colR.mass * (col.transform.position - this.transform.position));
				colR.AddForce(100 * colR.mass * Vector2.up);
            }
        }
    }
}
