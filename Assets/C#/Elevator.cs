using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {
	public Vector3 top;
	public Vector3 bottom;
	public bool moveUp;
	public float delay;
    public float speed;
	float time;
	// Use this for initialization
	void Start () {
		delay = 0.12f;
		moveUp = false;
		top = new Vector3 (0,25,0);
		bottom = new Vector3 (0,-4,0);
		time = delay;
	}
	
	// Update is called once per frame
	void Update () {
		//if (time <= 0) {
			if (moveUp) {
				if (this.transform.position.y >= top.y) {
					moveUp = !moveUp;
				}
				this.transform.position += Vector3.up * Time.deltaTime * speed;
			} else {
				if (this.transform.position.y <= bottom.y) {
					moveUp = !moveUp;
				}
				this.transform.position += Vector3.down * Time.deltaTime * speed;
			}
			time = delay; 
		//}
		//time -= Time.deltaTime;
	}
}
