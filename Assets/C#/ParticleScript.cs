using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {
	public float time = 1;
	public bool shell;
	private SpriteRenderer sp;

	// Update is called once per frame
	void Start() {
		sp = this.GetComponent<SpriteRenderer> () ? this.GetComponent<SpriteRenderer> () : transform.GetComponentInChildren<SpriteRenderer> ();
	}

	void Update () {
		time -= Time.deltaTime;
		Color c = sp.color;
		if (!shell)  sp.color = new Color(c.r, c.g, c.b, time);
		if (time <= 0) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
