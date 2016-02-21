using UnityEngine;
using System.Collections;

public class ExplodingProp : MonoBehaviour {

    public GameObject explosion;
    bool flame = false;
    int hitThre = 5;
    float flameCountdown = 10;
    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //i += Time.deltaTime;
        //print(i);
        if(flame)
            flameCountdown -= Time.deltaTime;
        if (hitThre <= 0)
        {
            GameObject ex = (GameObject)GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
            ex.layer = this.gameObject.layer;
            Destroy(gameObject);
        }
        if (flameCountdown < 0)
            hitThre = 0;    
	}

    void hit()
    {
        if (!flame)
        {
            flame = true;
            this.GetComponentInChildren<ParticleSystem>().Play();
        }
        hitThre -= 1;
    }
}
