using UnityEngine;
using System.Collections;

public class PointerController : MonoBehaviour {
    public Transform[] pointers;
    public Sprite pointerVert, pointerHoriz;

    public float lowerBoundX, lowerBoundY, upperBoundX, upperBoundY;

    private Transform[] targets;
    private int playerCount;


	// Use this for initialization
	void Start () {
        if (GameObject.Find("SceneController"))
        {
            playerCount = GameObject.Find("SceneController").GetComponent<SceneController>().playerCount;
        }
        else
        {
            playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        }
        targets = new Transform[playerCount];
        for (int i = 1; i <= playerCount; i++)
        {
            targets[i - 1] = GameObject.Find("Player" + i).transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < playerCount; i++)
        {
            
        }
	}
}
