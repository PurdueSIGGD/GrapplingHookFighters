using UnityEngine;
using System.Collections;

public class ExplodingProp : MonoBehaviour {

    public GameObject explosion;
    bool flame = false;
    int hitThre = 5;
    float flameCountdown = 10;
	ParticleSystem ps;
    // Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem>();
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
			if (ps) ps.Play();
        }
        hitThre -= 1;
    }
}
