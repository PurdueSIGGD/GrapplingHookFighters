﻿using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {
    public string useString;
    private float cooldownTime = 1;
    private float time = 0;
    public bool callsFunction;
	public bool dropsAutomatically;
	public bool randomDispersion;
	public GameObject[] items;
    private Transform childSprite;

	public Sprite openSprite;
	private Sprite closedSprite;

	public float timeTillReset = -1;

	// -1 = no reset, 1 = 1 second to refill

	public bool used;
    void Start()
    {
        childSprite = transform.FindChild("Sprite");
		closedSprite = childSprite.GetComponent<SpriteRenderer>().sprite;
		if (dropsAutomatically) {
			used = true;
			childSprite.GetComponent<SpriteRenderer>().sprite = openSprite;

		}
			

    }
    void Update()
    {
		time += randomDispersion?Time.deltaTime*Random.Range(0f,2f):Time.deltaTime;
		if (time > timeTillReset && timeTillReset > 0 && used) {
			childSprite.GetComponent<SpriteRenderer>().sprite = closedSprite;
			used = false;
			if (dropsAutomatically) use();
		}
    }
	void DropItem() {
        

       if (time > cooldownTime)
        {

			if (callsFunction) {
	            //print("calling function");
				time = 0;
	            childSprite.GetComponent<Animator>().SetTrigger("Press");
	            GameObject.Find("Menus").SendMessage(useString);
				Transform background;
				if (background = transform.FindChild("Background")) 
					GameObject.Destroy(background.gameObject);

			} else {
				if (!used) {
					use();
				}

			}


        }
	}
	void use() {
        AkSoundEngine.PostEvent("ChestOpening", gameObject);
		used = true;
		time = 0;
		childSprite.GetComponent<SpriteRenderer>().sprite = openSprite;
		GameObject spawn = (GameObject)GameObject.Instantiate(items[Random.Range(0, items.Length)], transform.FindChild("BoxSpawnPoint").position, Quaternion.identity);
		spawn.GetComponent<Rigidbody2D>().AddForce(50* (Vector2.up + Random.insideUnitCircle));
		spawn.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0, 20));
	}
}
