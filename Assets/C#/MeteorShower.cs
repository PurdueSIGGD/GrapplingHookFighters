using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MeteorShower : MonoBehaviour {

	public GameObject meteor;
	GameObject boundary;
	public int dropsAtime;
	public int maxDrops;
	float force;
	double maxForce;//something more than 500 so that the meteor speed is fast
	double angleOfDrop;//assumed to be in degrees
	int radius;//How wide will the spread of the meteors fall from the distance
	float time;
	public float lifetime;
	float width,height;
	Vector3 meteorloc;
	Scene scene;

	// Use this for initialization
	void Start () {
		//for now I set them
		scene = SceneManager.GetSceneAt(1);//the Scene that discerns the stage is assumed to be the second scene
		SceneManager.MoveGameObjectToScene (gameObject, scene);
		Debug.Log (scene);
		maxDrops = 25;//if value is at two it only shoots 1 meteor at a time
		determinespawn();
		maxForce = 1000;
		radius = 20;
		angleOfDrop = -135;

		angleOfDrop = (angleOfDrop / 360) * 6.28;//changes to degrees to radians
		dropsAtime = Random.Range (0,maxDrops);
		time = Random.Range (0, 1.5f);
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
					meteorloc = new Vector3 (Random.Range ((width - radius), (width + radius)), height, transform.position.z);
					GameObject thing = (GameObject)Instantiate (meteor,meteorloc, new Quaternion (0, 0, 0, 0));
					SceneManager.MoveGameObjectToScene (thing, scene);
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
		Debug.Log ("The Scene is "+ scene.buildIndex);
		switch(SceneManager.GetActiveScene().buildIndex){//the cases can change depending on the build settings
		case -1://BoxofSword. not used in stages so in impossible switch case
			height = 17;//will spawn inside the box
			width = 23;
			break;
		case 1://Potato
			height = 10;
			width = 20;
			break;
		case 2://Bridges
			width = 19;
			height = 9;
			break;
		case 3://lava fall
			width = 22;
			height = 7;
			break;
		case 4://upwards
			height = 9;
			width = 20;
			break;
		case 6://vertical
			width = 12;
			height = 10;
			break;
		case 8://Roll
			width = 40;
			height = 20;
			break;
		case 9://Scroll
			width = 25;
			height = 70;
			maxDrops = 70;
			break;
		case 12://Arch
			width = 33;
			height = 15;
			break;
		case 13://Buildings
			width = 40;
			height = 35;
			break;
		default://testing stage or any other random stage for now
			width = 31;
			height = 16;
			maxDrops = 30;
			break;
		}
		return;
	}
}
