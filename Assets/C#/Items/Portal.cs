using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public GameObject bluePortal, orangePortal;
    public Vector2 normal;
    public float maximumVelocity;

    void OnTriggerEnter2D(Collider2D coll) {
        if (bluePortal == null || orangePortal == null || coll.isTrigger)
            return;

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
            coll.gameObject.transform.position = new Vector3(pos.x, pos.y, coll.gameObject.transform.position.z) + (Vector3) bluePortal.GetComponent<Portal>().normal;

            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;

            rb.velocity = magn * bluePortal.GetComponent<Portal>().normal;

        } else if (gameObject == bluePortal) {
            pos = orangePortal.transform.position;
            coll.gameObject.transform.position = new Vector3(pos.x, pos.y, coll.gameObject.transform.position.z) + (Vector3) orangePortal.GetComponent<Portal>().normal;

            float magn = rb.velocity.magnitude;

            if (magn > maximumVelocity)
                magn = maximumVelocity;
            
            rb.velocity = magn * orangePortal.GetComponent<Portal>().normal;
        }
    }

	void ExitPortal(GameObject g) {
		Rigidbody2D rb;

		if (g.GetComponent<Rigidbody2D>() && !g.GetComponent<Rigidbody2D>().isKinematic) {
			rb = g.GetComponent<Rigidbody2D>();
		} else if (g.GetComponentInChildren<Rigidbody2D>() && !g.GetComponentInChildren<Rigidbody2D>().isKinematic) {
			rb = g.GetComponentInChildren<Rigidbody2D>();
		} else {
			return;
		}

		Vector2 pos = new Vector2(0, 0);
		if (gameObject == orangePortal) {
			pos = orangePortal.transform.position;
			g.transform.position = new Vector3(pos.x, pos.y, g.transform.position.z) + (Vector3) orangePortal.GetComponent<Portal>().normal;

			float magn = rb.velocity.magnitude;

			if (magn > maximumVelocity)
				magn = maximumVelocity;

			rb.velocity = magn * orangePortal.GetComponent<Portal>().normal;

		} else if (gameObject == bluePortal) {
			pos = bluePortal.transform.position;
			g.transform.position = new Vector3(pos.x, pos.y, g.transform.position.z) + (Vector3) bluePortal.GetComponent<Portal>().normal;

			float magn = rb.velocity.magnitude;

			if (magn > maximumVelocity)
				magn = maximumVelocity;

			rb.velocity = magn * bluePortal.GetComponent<Portal>().normal;
		}
	}
}
