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
			Debug.Log ("Hit a player");
			col.gameObject.GetComponent<Health>().hit(100f);//detects a player
		}
			Destroy (this);
		
	}
}
