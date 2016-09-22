using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LightinigStrike : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
		

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent<player> ()){
			//Debug.Log ("Hit a player with trigger");
			col.gameObject.GetComponent<Health>().hit(200f);//detects a player
		}
		//Debug.Log ("Death by trgger");
		Destroy (gameObject);
		
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.GetComponent<player> ()){
			//Debug.Log ("Hit a player with collision");
			col.gameObject.GetComponent<Health>().hit(200f);//detects a player
		}
		//Debug.Log ("death by collision");
		Destroy (gameObject);
	}
}
