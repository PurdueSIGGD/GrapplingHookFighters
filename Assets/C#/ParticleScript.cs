using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {
	public float time = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		Color c = this.GetComponent<SpriteRenderer>().color;
		this.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, time);
		if (time <= 0) {
			GameObject.Destroy(this);
		}
	}
}
