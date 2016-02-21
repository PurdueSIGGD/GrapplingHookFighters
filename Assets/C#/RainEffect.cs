using UnityEngine;
using System.Collections;

public class RainEffect : MonoBehaviour {
	/**
	 *Preface stuff
	Have to set the particle area in the particle System
	*/
	ParticleSystem rainSys;
	//It gets the particles
	ParticleSystem.Particle[] particles;



	drop[] drops;
	int dropNum;//number of raindrops

	/******************************************************
	* In order to add a collider to each particle I decided to create a struct
	* 
	* this struct is aboslutely worthless though so I don't know.
	*******************************************************/
	struct drop{
		public ParticleSystem.Particle part;
		public BoxCollider2D dropPhy;

		public drop(ParticleSystem.Particle p,BoxCollider2D bc2d){
			part = p;
			dropPhy = bc2d;

			dropPhy.transform.position = new Vector3(part.position.x,part.position.y,part.position.z);
			dropPhy.isTrigger = true;
		}

		public void dropUpdate(){

			dropPhy.transform.position = new Vector3(part.position.x,part.position.y,part.position.z);
			if (part.lifetime == 0) {
				Destroy (dropPhy);
			}
		}
	}
	/**********************************
	* End of struct
	***********************************/

	void Start () {
//		rainSys = gameObject.GetComponentInChildren<ParticleSystem> ();
//		dropNum = rainSys.GetParticles (particles);
//		drops = new drop[rainSys.maxParticles];
	}
	
	// Update is called once per frame
	void Update () {
//		dropNum = rainSys.GetParticles (particles);


		//set the changes to the rain particles
//		rainSys.SetParticles (particles, dropNum);
	}

	void initDrops(ParticleSystem.Particle[] parts, int dropSize){
		for(int i = 0; i < dropNum;){
			//check and see if the drop is new so a new physics2D collider can be added to it
			if((parts[i].startLifetime - parts[i].lifetime)<2){
				//drops [findEmptyDropIndex ()] = new drop (parts[i],new BoxCollider2D());
			}

		}
	}
	/*
	 * returns and index where you can get the null drops array
	 * 
	 * doesn't check for whether the array is full. I don't expect it to happen
	*/
	int findEmptyDropIndex(){
		int i = 0;
		while(!(drops[i++].part.lifetime == 0)&&(i<rainSys.maxParticles)){}
		return i;
	}

	void OnParticleCollision(GameObject gO){
		Debug.Log (gO.name);
		if (gO.tag == "Player") {
			//do nothing because it is rain
			Debug.Log("I hit player");
		}else if(gO.name == "Boundary"){
			Debug.Log("I hit boundary");
			Destroy (this);
		}
	}
}
