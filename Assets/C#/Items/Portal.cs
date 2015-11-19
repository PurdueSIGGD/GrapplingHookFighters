using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public GameObject bluePortal, orangePortal;
    public Vector2 normal;
    public float maximumVelocity;

    void OnTriggerEnter2D(Collider2D coll) {
        if (bluePortal == null || orangePortal == null)
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
            pos = bluePortal.transform.position;
            coll.gameObject.transform.position = pos + bluePortal.GetComponent<Portal>().normal;
            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;

            rb.velocity = magn * bluePortal.GetComponent<Portal>().normal;

        } else if (gameObject == bluePortal) {
            pos = orangePortal.transform.position;
            coll.gameObject.transform.position = pos + orangePortal.GetComponent<Portal>().normal;
            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;
            
            rb.velocity = magn * orangePortal.GetComponent<Portal>().normal;
        }
    }
}
