using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour, item
{
	public bool canSwing = false;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void fixedUpdate() {
		if (!canSwing) {
			//transform.Rotate (0, 0, 5);
		}
	}

	public void click() {
		canSwing = false;
	}

	public void unclick() {
		canSwing = true;
	}

	public void Swing() {
		
	}


	void OnCollisionStay(Collision col) {
		if (col.gameObject.tag == "Player") {
			Debug.Log ("come on and slam and welcome to the jam");
			col.gameObject.GetComponent<Rigidbody2D> ().AddRelativeForce (new Vector2 (10, 10), ForceMode2D.Impulse);
		}
	}
}

