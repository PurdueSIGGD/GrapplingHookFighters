using UnityEngine;
using System.Collections;

public class scrollPlatform : MonoBehaviour {

	public GameObject platform;
	public Vector3 downby = new Vector3(0,-1.5f,0);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += downby * Time.deltaTime;
		if (this.transform.position.y <= -10) {
			transform.position = new Vector3(transform.position.x,70,0);
			//GameObject newplat = (GameObject)Instantiate(platform,new Vector3(transform.position.x,70,0),new Quaternion(0,0,0,0));

			//	Destroy(gameObject);
		}
	}
}
