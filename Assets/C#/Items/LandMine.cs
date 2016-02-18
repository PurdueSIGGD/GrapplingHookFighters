using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour {

	private bool willexplode = false;
    
    // Use this for initialization
	void Start () {
        
	}
	
    void OncollisionEnter2d(Collision2D col)
    {
        if(col.gameObject.tag == "Player" && willexplode == true )
        {
            GetComponent<ExplosionScript>();
            GameObject.Destroy(this.gameObject);
        }
    }
	// Update is called once per frame
	void Update () {
	    /*if ()
        {
            
        }
        */
	}
}
