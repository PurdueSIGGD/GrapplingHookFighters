﻿using UnityEngine;
using System.Collections;

public class PortalProjectile : MonoBehaviour {

    public GameObject bluePortal, orangePortal, gun;
    public int portalColor;
    public Vector2 normal;

    private float time;

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Platform") {

            Vector2 point = coll.contacts[0].point;     //poistion of contact
            Vector2 dir = -coll.contacts[0].normal;     //get normal of contact point
            point -= dir;                               //subtract normal from position

            RaycastHit2D hitInfo1 = Physics2D.Raycast(point, dir, 2);       //create a raycast that goes from point back to surface so that we can store it in RaycastHit2D to get more info
            RaycastHit2D hitInfo2 = Physics2D.Raycast(point, dir, 2);       //create second raycast for double checking

            if (hitInfo1.collider != null && hitInfo2.collider != null) {   //check raycast was a success
                while(hitInfo1.normal != hitInfo2.normal) {                 //double check normal because collider sometimes returns wrong normals 
                    print(hitInfo1.normal + ", " + hitInfo2.normal);
                    hitInfo1 = Physics2D.Raycast(point, dir, 2);
                    hitInfo2 = Physics2D.Raycast(point, dir, 2);
                }
                normal = hitInfo1.normal;                                   //get normal of plane come in contact (straight line perpendicular to surface hit) from RaycastHit2D
                float angle = Vector2.Angle(Vector2.right, normal);         //get angle of surface in refrence to world
                if (normal.y < 0) {                                         //check if angle should be negative (Vector2.Angle won't return negatives)
                    angle *= -1;
                }

                Vector3 pos;
                if (angle < 0)      //changing position of portals if they are on a bottom surface due to camera angle hiding them, adjustment changes proprotionally to angle of surface with a min of + 0.1
                    pos = coll.contacts[0].point + (Vector2)(normal * 0.1f) + (Vector2)(normal * (-angle / 90) * 0.15f);
                else
                    pos = coll.contacts[0].point + (Vector2)normal * 0.1f;

                GameObject g;
                if (portalColor == 1) {
                    g = (GameObject)Instantiate(orangePortal, pos, Quaternion.Euler(new Vector3(0, 0, angle)));
                    print(g.GetComponent<Renderer>().bounds.size);
                    if (gun.GetComponent<PortalGun>().orangePortal != null) {
                        Destroy(gun.GetComponent<PortalGun>().orangePortal);
                    }
                    gun.GetComponent<PortalGun>().orangePortal = g;

                } else {
                    g = (GameObject)Instantiate(bluePortal, pos, Quaternion.Euler(new Vector3(0, 0, angle)));
                    if (gun.GetComponent<PortalGun>().bluePortal != null) {
                        Destroy(gun.GetComponent<PortalGun>().bluePortal);
                    }
                    gun.GetComponent<PortalGun>().bluePortal = g;
                }


                float ySize = g.GetComponent<Renderer>().bounds.size.y;
                float ySurfaceSize = coll.gameObject.GetComponent<Renderer>().bounds.size.y;
                if (ySize > ySurfaceSize) {
                    Destroy(g);
                    Destroy(gameObject);
                    return;
                } else if ((g.transform.position.y + ySize) > (coll.gameObject.transform.position.y + ySurfaceSize)) {
                    g.transform.position -= (Vector3)(((g.transform.position.y + ySize) - (coll.gameObject.transform.position.y + ySurfaceSize)) * normal);
                } else if ((g.transform.position.y - ySize) < (coll.gameObject.transform.position.y - ySurfaceSize)) {
                    g.transform.position += (Vector3)(((coll.gameObject.transform.position.y - ySurfaceSize) - (g.transform.position.y - ySize)) * normal);
                }


                if (gun.GetComponent<PortalGun>().bluePortal != null) {
                    gun.GetComponent<PortalGun>().bluePortal.GetComponent<Portal>().bluePortal = gun.GetComponent<PortalGun>().bluePortal;
                    gun.GetComponent<PortalGun>().bluePortal.GetComponent<Portal>().orangePortal = gun.GetComponent<PortalGun>().orangePortal;
                }

                if (gun.GetComponent<PortalGun>().orangePortal != null) {
                    gun.GetComponent<PortalGun>().orangePortal.GetComponent<Portal>().bluePortal = gun.GetComponent<PortalGun>().bluePortal;
                    gun.GetComponent<PortalGun>().orangePortal.GetComponent<Portal>().orangePortal = gun.GetComponent<PortalGun>().orangePortal;
                }

                g.GetComponent<Portal>().normal = this.normal;
                g.layer = gameObject.layer;
            }

            GameObject.Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Item")
            Physics2D.IgnoreCollision(coll.GetComponent<Collider2D>(), GetComponent<CircleCollider2D>()); //not working
    }

    void Update() {
        time += Time.deltaTime;
        if (time > 6)
            GameObject.Destroy(this.gameObject);
    }
}
