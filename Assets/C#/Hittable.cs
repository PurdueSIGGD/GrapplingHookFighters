using UnityEngine;
using System.Collections;

public class Hittable : MonoBehaviour {
    public GameObject explosion;
    public bool exploding;


	void hit() {
        if (exploding)
        {
            GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
            GameObject.Destroy(this.gameObject);
        }
		//print("this script does not have something that responds to hit");
	}
}
