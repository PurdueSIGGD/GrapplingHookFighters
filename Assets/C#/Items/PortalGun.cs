using UnityEngine;
using System.Collections;

public class PortalGun : MonoBehaviour {

    private bool canFire, trigger, death;
    public int portalColor;

    public GameObject bluePortal, orangePortal;
    public float projectileSpeed;
    public GameObject blueProjectile, orangeProjectile;
    public int playerID;


    public void click() {

        if (canFire) {
            canFire = false;
            trigger = true;
        } else {
            trigger = false;
        }
    }

    public void SetPlayerID(int playerID) {
        //will set an ID number as an Int
        this.playerID = playerID;

    }

    public void unclick() {
        canFire = true;
        trigger = false;
    }

    void Update() {
        if (trigger && !death && playerID != -1) {
            Vector3 shootPoint = transform.FindChild("shootPoint").position;
            Vector2 playerPos = GameObject.Find("Player" + playerID).transform.position;

            Vector2 thing = (Vector2)shootPoint - playerPos;
            //creating new gameobject, not setting our last one to be that. It will cause problems in the future.

            thing.Normalize();
            GameObject g;
            if (portalColor == 1) {
                g = (GameObject)GameObject.Instantiate(orangeProjectile, shootPoint, GetComponentInParent<Transform>().rotation);
            } else {
                g = (GameObject)GameObject.Instantiate(blueProjectile, shootPoint, GetComponentInParent<Transform>().rotation);
            }
            
            g.GetComponent<PortalProjectile>().gun = gameObject;
            g.GetComponent<PortalProjectile>().portalColor = portalColor;

            g.layer = this.transform.gameObject.layer;
            g.GetComponent<Rigidbody2D>().AddForce(thing * projectileSpeed);
            
        }
    }

    void Death() {
        death = true;
    }
	void NotDeath() {
		death = false;
	}
}
