using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour {
	float scaleSiz;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.name == "Boundary") {
			if (this.gameObject != null) {
				Destroy (this.gameObject);
			}
		}
	}
}
