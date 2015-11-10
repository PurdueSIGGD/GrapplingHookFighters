using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {
	//Rigidbody2D bulletrigid2D;
	//BoxCollider2D bulletBox2D;

	// Use this for initialization
	float time;
	void Start () {
	//	bulletBox2D = GetComponentInParent<BoxCollider2D>();
	//	bulletrigid2D = GetComponentInParent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 6)
			GameObject.Destroy (this.gameObject);
	}
}
