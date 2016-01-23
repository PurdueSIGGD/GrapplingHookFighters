using UnityEngine;
using System.Collections;

public class PortalProjectile : MonoBehaviour {

    public GameObject bluePortalObject, orangePortalObject, gun;
    public int portalColor;
    public Vector2 normal;

    private float time;

    void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			if (portalColor == 1) {
				if (gun.GetComponent<PortalGun>().bluePortal != null) {
					gun.GetComponent<PortalGun> ().bluePortal.GetComponent<Portal> ().SendMessage ("ExitPortal", coll.gameObject);
				}
			} else {
				if (gun.GetComponent<PortalGun>().orangePortal != null) {
					gun.GetComponent<PortalGun> ().orangePortal.GetComponent<Portal> ().SendMessage ("ExitPortal", coll.gameObject);
				}
			}
			Destroy (gameObject);
			return;
		}

		if (coll.gameObject.tag == "Item" || coll.gameObject.tag == "DualItem") {
			Destroy (gameObject);
			return;
		}
		
        if (coll.gameObject.tag == "Platform") {

			gun.GetComponent<PortalGun>().portalColor = Mathf.Abs(portalColor - 1);

            Vector2 point = coll.contacts[0].point;     //poistion of contact
            Vector2 dir = -coll.contacts[0].normal;     //get normal of contact point
            point -= dir;                               //subtract normal from position

            RaycastHit2D hitInfo1 = Physics2D.Raycast(point, dir, 2);       //create a raycast that goes from point back to surface so that we can store it in RaycastHit2D to get more info
            RaycastHit2D hitInfo2 = Physics2D.Raycast(point, dir, 2);       //create second raycast for double checking

            if (hitInfo1.collider != null && hitInfo2.collider != null) {   //check raycast was a success
                while (hitInfo1.normal != hitInfo2.normal) {                 //double check normal because collider sometimes returns wrong normals 
                    hitInfo1 = Physics2D.Raycast(point, dir, 2);
                    hitInfo2 = Physics2D.Raycast(point, dir, 2);
                }
                normal = hitInfo1.normal;                                   //get normal of plane come in contact (straight line perpendicular to surface hit) from RaycastHit2D
                float angle = Vector2.Angle(Vector2.right, normal);         //get angle of surface in refrence to world
                if (normal.y < 0) {                                         //check if angle should be negative (Vector2.Angle won't return negatives)
                    angle *= -1;
                }

				//TODO:adjust the distance from the surface based on the angle so portals are always visible (after position/angle glitch fix)
                Vector3 pos = coll.contacts[0].point + (Vector2)normal * 0.1f;

                GameObject g;
                if (portalColor == 1) {
					//spawn the portal at the calculated position and angle
                    g = (GameObject)Instantiate(orangePortalObject, new Vector3(pos.x, pos.y, transform.position.z), Quaternion.Euler(new Vector3(0, 0, angle)));
					//destory old portal of the same color
                    if (gun.GetComponent<PortalGun>().orangePortal != null) {
                        Destroy(gun.GetComponent<PortalGun>().orangePortal);
                    }
                    gun.GetComponent<PortalGun>().orangePortal = g;

                } else {
                    g = (GameObject)Instantiate(bluePortalObject, new Vector3(pos.x, pos.y, transform.position.z), Quaternion.Euler(new Vector3(0, 0, angle)));
                    if (gun.GetComponent<PortalGun>().bluePortal != null) {
                        Destroy(gun.GetComponent<PortalGun>().bluePortal);
                    }
                    gun.GetComponent<PortalGun>().bluePortal = g;
                }

                float ySize = g.GetComponent<Renderer>().bounds.size.y;
                float ySurfaceSize = coll.gameObject.GetComponent<Renderer>().bounds.size.y;

				//check if the portal is bigger than the surface shot
                if (ySize > ySurfaceSize) {
                    Destroy(g);
                    Destroy(gameObject);
                    return;

					//if the portal is shot off the edge of a surface move it to the edge so it is fully on the surface
					//TODO: account for angle of surface
				} else if ((g.transform.position.y + (ySize / 2)) > (coll.gameObject.transform.position.y + (ySurfaceSize / 2))) {
					g.transform.position -= new Vector3(0, (g.transform.position.y + (ySize / 2)) - (coll.gameObject.transform.position.y + (ySurfaceSize / 2)), 0);
				} else if ((g.transform.position.y - (ySize / 2)) < (coll.gameObject.transform.position.y - (ySurfaceSize / 2))) {
					g.transform.position += new Vector3(0, (coll.gameObject.transform.position.y - (ySurfaceSize / 2)) - (g.transform.position.y - (ySize / 2)), 0);;
                }

				//pass portal pointers to portal fields
                if (gun.GetComponent<PortalGun>().bluePortal != null) {
                    gun.GetComponent<PortalGun>().bluePortal.GetComponent<Portal>().bluePortal = gun.GetComponent<PortalGun>().bluePortal;
                    gun.GetComponent<PortalGun>().bluePortal.GetComponent<Portal>().orangePortal = gun.GetComponent<PortalGun>().orangePortal;
                }

                if (gun.GetComponent<PortalGun>().orangePortal != null) {
                    gun.GetComponent<PortalGun>().orangePortal.GetComponent<Portal>().bluePortal = gun.GetComponent<PortalGun>().bluePortal;
                    gun.GetComponent<PortalGun>().orangePortal.GetComponent<Portal>().orangePortal = gun.GetComponent<PortalGun>().orangePortal;
                }

				//pass portal it's own normal
                g.GetComponent<Portal>().normal = this.normal;
                g.layer = gameObject.layer;
            }

            GameObject.Destroy(this.gameObject);
        }
    }

    void Update() {
        time += Time.deltaTime;
        if (time > 6)
            GameObject.Destroy(this.gameObject);
    }
}
