using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public GameObject bluePortal, orangePortal;
    public Vector2 normal;
    public float maximumVelocity;

    void OnTriggerEnter2D(Collider2D coll) {
        if (bluePortal == null || orangePortal == null || coll.isTrigger)
            return;
        
        if (coll.gameObject.GetComponent<PortalProjectile>() || coll.gameObject.GetComponent<PortalProjectile>()) {
            return;
        }

            Rigidbody2D rb;
        if (coll.gameObject.GetComponent<Rigidbody2D>() && !coll.gameObject.GetComponent<Rigidbody2D>().isKinematic) {
            rb = coll.gameObject.GetComponent<Rigidbody2D>();
        } else if (coll.gameObject.GetComponentInChildren<Rigidbody2D>() && !coll.gameObject.GetComponentInChildren<Rigidbody2D>().isKinematic) {
            rb = coll.gameObject.GetComponentInChildren<Rigidbody2D>();
        } else {
            return;
        }

        Vector2 pos = new Vector2(0, 0);
        if (gameObject == orangePortal) {
            if (coll.gameObject.layer != bluePortal.layer)
                coll.GetComponent<player>().switchPlanes();
            pos = bluePortal.transform.position;
            coll.gameObject.transform.position = new Vector3(pos.x, pos.y, coll.gameObject.transform.position.z) + (Vector3) bluePortal.GetComponent<Portal>().normal;

            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;

            rb.velocity = magn * bluePortal.GetComponent<Portal>().normal;

        } else if (gameObject == bluePortal) {
            if (coll.gameObject.layer != orangePortal.layer)
                coll.GetComponent<player>().switchPlanes();
            pos = orangePortal.transform.position;
            coll.gameObject.transform.position = new Vector3(pos.x, pos.y, coll.gameObject.transform.position.z) + (Vector3) orangePortal.GetComponent<Portal>().normal;

            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;
            
            rb.velocity = magn * orangePortal.GetComponent<Portal>().normal;
        }
    }
}
