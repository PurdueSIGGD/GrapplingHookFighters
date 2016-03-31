using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	private bool willexplode = false;
    private Transform originalorientation;
    public float droppedtime = 0.0f;
    public float fusetime = 4.0f;
    //Bug fixes needed
    //Needs to explode on collision

    // Use this for initialization
	void Start () {
        originalorientation = this.transform;
	}
	
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player" && willexplode == true && droppedtime >= fusetime)
        {
            gameObject.GetComponent<ExplosionScript>();
            GameObject.Destroy(this);
        }

    }


 
	// Update is called once per frame
	void Update () {
	    if (/*this. &&*/ this.GetComponent<HeldItem>().timeSinceDropped > 0.0) //focus is player
        {
            willexplode = true;
        }

        if(gameObject.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            droppedtime += Time.deltaTime;     
        }
        //Make sure the landmine falls flat on ground 
        if (this.gameObject.CompareTag("Player") != true) //focus is not player
        {
            transform.rotation = originalorientation.rotation;
        }

    }
}
