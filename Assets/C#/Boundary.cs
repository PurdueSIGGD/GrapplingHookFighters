using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boundary : MonoBehaviour {
	//This class also does programming for the respawn of the characters too
	public GameObject p;
	public BoxCollider2D b;
	public float timeNow;
	
	public class deathtime{
		public GameObject player;
		public float timeOfDeath;

		public deathtime(GameObject plyr,float tod){
			this.player = plyr;
			this.timeOfDeath = tod;
		}
	}
	deathtime aDeath;

	// keeps track of players and what time they died
	public List<deathtime> deathwait = new List<deathtime>();

	// Use this for initialization
	void Start () {
		p = null;
		b = this.GetComponentInParent<BoxCollider2D>();

	}
	
	// Update is called once per frame
	void Update () {
		//no people are dead just skips
		if (deathwait.Count != 0) {
			timeNow = Time.time;
			for(int i = 0; i < deathwait.Count;i++){
				//checks to see if time passes to respawn the object
				if(timeNow >= deathwait[i].timeOfDeath){
					//The respawn point will be there for now
					deathwait[i].player.transform.position = new Vector3(0,3,0);
					deathwait[i].player.GetComponent<Health>().resetPlayer();
					deathwait[i].player.SetActive(true);
					deathwait[i].player.GetComponent<player>().death = false;

					deathwait.RemoveAt(i);
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D col){
		//checks to see if the collider belongs to a player
		if (col.GetComponentInParent<Health>() != null) {
			//collider->player->player#Parent
			p = col.gameObject;
			p.GetComponent<Health>().killPlayer();
			p.SetActive(false);
			//the death waiting time is 5 seconds
			deathwait.Add(new deathtime(p,Time.time+5));
		}
	}
}
