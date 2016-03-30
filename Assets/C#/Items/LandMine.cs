using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	private bool willexplode = false;
    private Transform originalorientation;
    private float droppedtime = 0.0f;
    public float fusetime = 4.0f;
    //Bug fixes needed
    //Can use landmine like jetpack 

    // Use this for initialization
	void Start () {
        originalorientation = this.transform;
	}
	
    void OnCollisionEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && willexplode == true && droppedtime >= fusetime)
        {
            GetComponent<ExplosionScript>();
            GameObject.Destroy(this);
        }

    }


 
	// Update is called once per frame
	void Update () {
	    if (this.tag == "Player" && gameObject.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            willexplode = true;
        }

        if(gameObject.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            droppedtime += Time.deltaTime;      
        }
        //Make sure the landmine falls flat on ground 
        if (this.gameObject.transform.IsChildOf(transform))
        {
            gameObject.GetComponent<LandMine>().transform.rotation = originalorientation.rotation;
        }

    }
}
