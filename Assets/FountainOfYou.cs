using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

public class FountainOfYou : MonoBehaviour {

	bool activated = false;
	public ParticleSystem particles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (particles != null && activated == false && GetComponent<HeldItem> ().focus != null) {
			activated = true;
			StartFountain ();
		}
	}

	void StartFountain() {
		particles.Play();
	}
}
