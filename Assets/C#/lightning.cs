using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class lightning : MonoBehaviour {

	//flash screen for when lightning effect was made was first put in under the AutoZoomCamera and not part of the
	//lightning effect 

	public Image lightningSprite;
	public Image flashScreen;
	public Canvas effectcanvas;
	public GameObject lightningstrike;
	Vector3 canvasOffset;
	Vector3 lstrikeLoc;
	//Vector3 stageCenter;

	float interval;//how long the lightning is spaced out
	float appear;//how long the lightning is out
	int strikes;//how many lighting strikes spawn
	int maxStrikes;
	float flashAppear;
	float width,height;//with and height positions for where the ligthing strikes will spawn
	bool hasStruck;//lightning is lightninging and stuff...
	public float lifetime;
	

	void Start () {
		//lightningSprite = this.gameObject.GetComponent<SpriteRenderer>();
		//stageCenter = gameObject.transform.FindChild("Stage Center").transform.position;
		//flashScreen = gameObject.GetComponentsInChildren
		//flashScreen.color = new Color(1,1,1,1);
		maxStrikes = 6;
		determinespawn();//depending on the stage sets the area where lighting strikes  will spawn
		canvasOffset = effectcanvas.transform.position;
		flashScreen.transform.position = new Vector3 (canvasOffset.x,canvasOffset.y,canvasOffset.z-5);
		lightningSprite.enabled = false;
		flashScreen.enabled = false;
		interval = Random.Range (50, 100);
		appear = Random.Range (10, 30);
		strikes = Random.Range (0,maxStrikes);
		hasStruck = false;
		lifetime = Random.Range (120,300);
		transform.rotation = Quaternion.Euler (0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		//it appears for at most 3 seconds then changes location and disappears for at most 10 seconds
		if(hasStruck){
			if(appear <= 0){
			//lightning has struck so setting location and time
				lightningSprite.enabled = false;
				interval = Random.Range (1000,5000)*Time.deltaTime;
				//set flash appear in the negatives so that a flash doesn't appear before every lightning strike
				flashAppear = Random.Range (-2,10)*Time.deltaTime;
				int Larry =  (int)Random.Range(canvasOffset.x-100,canvasOffset.x+200);
				//int Gary =  (int)Random.Range(transform.position.x-50,transform.position.x+50);
				lightningSprite.transform.position = new Vector3(Larry,0,0);
				strikes = Random.Range (0,maxStrikes);
				hasStruck = false;
			}else{
				appear--;
			}
		}else{
			//counting down for the lighting;
			if(interval <= 0){
				flashScreen.enabled = true;
				if(flashAppear<=0){
					flashScreen.enabled = false;
					lightningSprite.enabled = true;
					hasStruck = true;
					appear = Random.Range (10, 30)*Time.deltaTime;
					//creates lighting strikes across the actual stage
					for(int i = 0; i < strikes;i++){
						lstrikeLoc = new Vector3 (Random.Range(-width,width),height,0);
						GameObject thing = (GameObject)Instantiate (lightningstrike,lstrikeLoc,new Quaternion(0,0,0,0));
						thing.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0,-1000));
					}
				}else{
					flashAppear--;
				}
			}else{
				interval--;
			}
		}

		if(lifetime < 0){
			Destroy(this);
		}
	}

	public void determinespawn(){
		Debug.Log ("The Scene is "+ SceneManager.GetActiveScene().buildIndex );
		switch(SceneManager.GetActiveScene().buildIndex){
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
			maxStrikes = 35;
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
			maxStrikes = 20;
			break;
		}
		return;
	}
}
