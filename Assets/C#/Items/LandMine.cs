using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	//private bool willexplode = false;
    private Transform originalorientation;
    public float droppedtime = 0.0f;
    public float fusetime = 4.0f;
	public GameObject explosion, beeper;

    // Use this for initialization
	void Start () {
        originalorientation = this.transform;

	}
	
    void OnTriggerStay2D(Collider2D col)
    {
		if(droppedtime >= fusetime && col.GetComponent<Rigidbody2D>() && col.GetComponent<Rigidbody2D>().velocity.magnitude > 2f)
        {
			GameObject.Destroy(this.gameObject);
			GameObject.Instantiate(explosion, transform.position + Vector3.up * .5f, Quaternion.identity);
            
        }

    }


 
	// Update is called once per frame
	void Update () {
		if (droppedtime > fusetime) {
			beeper.SetActive(true);
			Destroy(transform.parent.GetComponent<HeldItem>());
			//GetComponent<SpriteRenderer> ().color = Color.red;
		} else 


		if(transform.parent.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            droppedtime += Time.deltaTime;     
			if (droppedtime < fusetime) {
				transform.position += Time.deltaTime * Vector3.down * .10f;
				beeper.transform.position += Time.deltaTime * Vector3.down * .10f;
			}
        }
        //Make sure the landmine falls flat on ground 
        if (this.gameObject.CompareTag("Player") != true) //focus is not player
        {
            transform.rotation = originalorientation.rotation;
        }

    }

    public void hit()
    {
		GameObject.Instantiate(explosion, transform.position + Vector3.up * .5f, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }
}
