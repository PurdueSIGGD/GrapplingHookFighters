using UnityEngine;
using System.Collections;

public class GravityWell : MonoBehaviour {

    float rad = 10;
    float force = -500;
	// Use this for initialization
	void Start () {
        this.GetComponent<CircleCollider2D>().radius = rad;
	}
	
	// Update is called once per frame
	void Update () {
        if (force < 0)
        {
            force += Time.deltaTime * 33;
            this.rad -= Time.deltaTime * 0.6555f;
            this.GetComponent<CircleCollider2D>().radius = rad;
            //print(force);
            //print(rad);
        }
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if (force < 0)
        {
            //float distanceForce = Vector2.Distance(col.transform.position, this.transform.position) / (rad);
            if (col.transform.GetComponent<Rigidbody2D>() != null && col.transform.GetComponent<FiredProjectile>() == null)
            {
                col.transform.GetComponent<Rigidbody2D>().AddForce((force + 0) * Time.deltaTime * col.transform.GetComponent<Rigidbody2D>().mass * (col.transform.position - this.transform.position));
            }
        }
        else
        {
            if (col.transform.GetComponent<Rigidbody2D>() != null && col.transform.GetComponent<FiredProjectile>() == null)
            {
                col.transform.GetComponent<Rigidbody2D>().AddForce(600 * col.transform.GetComponent<Rigidbody2D>().mass * (col.transform.position - this.transform.position));
                col.transform.GetComponent<Rigidbody2D>().AddForce(600 * col.transform.GetComponent<Rigidbody2D>().mass * Vector2.up);
            }
            Destroy(this.gameObject);
        }
    }

}
