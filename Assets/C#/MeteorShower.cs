using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MeteorShower : MonoBehaviour {

	public GameObject meteor;
	public int dropsAtime;
	public int maxDrops;
	float force;
	double maxForce;//something more than 500 so that the meteor speed is fast
	double angleOfDrop;//assumed to be in degrees
	int radius;//How wide will the spread of the meteors fall from the distance
	float time;
	public float lifetime;
	float width,height;

	// Use this for initialization
	void Start () {
		//for now I set them
		determinespawn();
		maxDrops = 15;//if value is at two it only shoots 1 meteor at a time
		maxForce = 1000;
		radius = 20;
		angleOfDrop = -135;

		angleOfDrop = (angleOfDrop / 360) * 6.28;//changes to degrees to radians
		dropsAtime = Random.Range (0,maxDrops);
		time = Random.Range (0, 2);
		lifetime = Random.Range (50, 200);
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		lifetime -= Time.deltaTime;
		if (time <= 0) {
			for (int i = 0; i < dropsAtime; i++) {
				force = Random.Range (500, (float)maxForce);
				if(meteor != null){
					GameObject thing = (GameObject)Instantiate (meteor, new Vector3 (Random.Range ((width - radius), (width + radius)), height, transform.position.z), new Quaternion (0, 0, 0, 0));
				//doesn't check only assumes the gameObject has a rigidbody2d
				//Meteor.GetComponentInChildren<Rigidbody2D>().gravityScale = 1;
				thing.GetComponentInChildren<Rigidbody2D> ().AddForce (new Vector2 ((Mathf.Cos ((float)angleOfDrop) * force), (Mathf.Sin ((float)angleOfDrop) * force)));
				}
			}
			dropsAtime = Random.Range (0, maxDrops);
			time = Random.Range (5, 10);
		}

		if(lifetime < 0){
			Debug.Log ("Meteor Runs out of Time");
			Destroy (gameObject);
		}
	}

	public void determinespawn(){
		Debug.Log ("The Scene is "+ SceneManager.GetActiveScene().buildIndex );
		switch(SceneManager.GetActiveScene().buildIndex){
		case 1://BoxofSword
			height = 17;//will spawn inside the box
			width = 23;
			break;
		case 2://Potato
			height = 10;
			width = 20;
			break;
		case 3://Bridges
			width = 19;
			height = 9;
			break;
		case 4://lava fall
			width = 22;
			height = 7;
			break;
		case 5://upwards
			height = 9;
			width = 20;
			break;
		case 6://stuck
			width = 12;
			height = 10;
			break;
		default://testing stage or any other random stage for now
			width = 31;
			height = 16;
			break;
		}
		return;
	}
}
