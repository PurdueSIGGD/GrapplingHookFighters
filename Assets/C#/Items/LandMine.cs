using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	private bool willexplode = false;
    public float fusetime = 4.0f;
    private float droppedtime = 0.0f;
    //Bug fixes needed
    //Can use landmine like jetpack 

    // Use this for initialization
	void Start () {
        
	}
	
    void OncollisionEnter2d(Collision2D col)
    {
        if(col.gameObject.tag == "Player" && willexplode == true && droppedtime >= fusetime)
        {
            GetComponent<ExplosionScript>();
            GameObject.Destroy(this.gameObject);
        }
        //Make sure the landmine falls flat on ground 
        if (col.gameObject.tag != this.gameObject.tag)
        {
            gameObject.GetComponent<LandMine>().transform.rotation.eulerAngles.Set(0.0f, 0.0f, 0.0f);
        }
    }

 
	// Update is called once per frame
	void Update () {
	    if (this.gameObject.tag == "Player" && gameObject.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            willexplode = true;
        }

        if(gameObject.GetComponent<HeldItem>().timeSinceDropped > 0.0)
        {
            droppedtime += Time.deltaTime;      
        }

        
	}
}
