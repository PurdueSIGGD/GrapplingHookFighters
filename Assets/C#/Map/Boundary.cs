using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boundary : MonoBehaviour {
	//This class also does programming for the respawn of the characters too
	public GameObject p;
	//public Collider2D b;
	public float timeNow;
	public bool respawning;
	
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
		//b = this.GetComponent<Collider2D>();

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
					if (deathwait[i].player.GetComponent<Health>())  {
						GameObject g = deathwait[i].player;
						Transform rePos = GameObject.Find("Player" + deathwait[i].player.GetComponent<player>().playerid + "Parent").transform;
						g.transform.parent = rePos;
						g.transform.position = rePos.position;
						g.GetComponent<GrappleLauncher>().firedGrapple.transform.position = rePos.position;
						g.GetComponent<GrappleLauncher>().SendMessage("Disconnect");
						g.transform.eulerAngles = Vector3.zero;
						g.GetComponent<Health>().resetPlayer();
						g.GetComponent<player>().death = false;
						g.BroadcastMessage("NotDeath");
						g.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
					}
					//deathwait[i].player.SetActive(true);
					deathwait.RemoveAt(i);
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		//print(col);
		//checks to see if the collider belongs to a player
		if (col.GetComponentInParent<Health>() || col.GetComponent<GrappleScript>() || col.GetComponent<ParticleSystem>() || col.GetComponentInChildren<ParticleSystem>()) {
			if (col.GetComponent<Health>() && !col.GetComponent<Health>().dead && !respawning) SetInRespawnQueue(col.gameObject);
		} else {
			//print("Destroyyy");
			Destroy(col.gameObject);
		}
	}
	void SetInRespawnQueue(GameObject g) {
		if (respawning) {
			if (g.GetComponent<Health>() && !g.GetComponent<Health>().dead) g.GetComponent<Health>().killPlayer();
			//the death waiting time is 5 seconds
			deathwait.Add(new deathtime(g,Time.time+5));
		} else {
			if (g.GetComponent<Health>() && !g.GetComponent<Health>().dead) g.GetComponent<Health>().killPlayer();

		}
	}
}
