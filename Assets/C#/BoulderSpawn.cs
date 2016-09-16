using UnityEngine;
using System.Collections;

public class BoulderSpawn : MonoBehaviour {

	public GameObject boulder;
	Vector3 instLoc;
	int dropsAtATime;
	public bool spawnLorR; //true is left of the loc of spawn and right is false
	public int force;
	float spawntime;

	// Use this for initialization
	void Start () {
		dropsAtATime = Random.Range (1,5);
		spawntime = Random.Range(0f,1f);
		instLoc = this.transform.position;
		force = 500;
	}
	
	// Update is called once per frame
	void Update () {
		spawntime -= Time.deltaTime;


		if(spawntime <= 0){//spawntime is zero start shooting out boulders
			for (int i = 0; i < dropsAtATime; i++) {
				GameObject thing;
				thing = (GameObject)Instantiate (boulder, instLoc, new Quaternion (0, 0, 0, 0));
				float scaleSiz = Random.Range (0.1f, 1.3f);
				thing.transform.localScale = new Vector3 (scaleSiz,scaleSiz,1);
				if (spawnLorR) {
					thing.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-force, 0));
				}else{
					thing.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (force, 0));
				}
			}
			spawntime = Random.Range(2f,3f);
		}
	}
}
