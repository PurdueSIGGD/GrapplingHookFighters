using UnityEngine;
using System.Collections;

public class MeteorShower : MonoBehaviour {

	public GameObject meteor;
	public int dropsAtime;
	public int maxDrops;
	float force;
	double maxForce;//something more than 500 so that the meteor speed is fast
	double angleOfDrop;//assumed to be in degrees
	int radius;//How wide will the spread of the meteors fall from the distance
	float time;

	// Use this for initialization
	void Start () {
		//for now I set them
		maxDrops = 5;//if value is at two it only shoots 1 meteor at a time
		maxForce = 1000;
		radius = 20;
		angleOfDrop = -135;

		angleOfDrop = (angleOfDrop / 360) * 6.28;//changes to degrees to radians
		dropsAtime = Random.Range (0,maxDrops);
		time = Random.Range (0, 3);
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		if (time < 0) {
			for (int i = 0; i < dropsAtime; i++) {
				force = Random.Range (500, (float)maxForce);
				if(meteor != null){
				GameObject thing = (GameObject)Instantiate (meteor, new Vector3 (Random.Range ((transform.position.x - radius), (transform.position.x + radius)), transform.position.y, transform.position.z), new Quaternion (0, 0, 0, 0));
				//doesn't check only assumes the gameObject has a rigidbody2d
				//Meteor.GetComponentInChildren<Rigidbody2D>().gravityScale = 1;
				thing.GetComponentInChildren<Rigidbody2D> ().AddForce (new Vector2 ((Mathf.Cos ((float)angleOfDrop) * force), (Mathf.Sin ((float)angleOfDrop) * force)));
				}
			}
			dropsAtime = Random.Range (0, maxDrops);
			time = Random.Range (5, 10);
		}
	}
}
