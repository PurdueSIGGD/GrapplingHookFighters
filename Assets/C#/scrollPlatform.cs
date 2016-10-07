using UnityEngine;
using System.Collections;

public class scrollPlatform : MonoBehaviour {

	public GameObject platform;
	Vector3 downby = new Vector3(0,-.07f,0);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += downby;
		if (this.transform.position.y <= -60) {
			GameObject newplat = (GameObject)Instantiate(platform,new Vector3(transform.position.x,30,0),new Quaternion(0,0,0,0));
				Destroy(gameObject);
		}
	}
}
