using UnityEngine;
using System.Collections;

public class PointerController : MonoBehaviour {
    public GUITexture[] pointers;
    public Texture pointerVert, pointerHoriz, pointerDiag;
    public Camera myCamera;

    public float lowerBoundX, lowerBoundY, upperBoundX, upperBoundY;

    public Transform[] targets;
    private Health[] targetHealth;
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
        targetHealth = new Health[playerCount];
        for (int i = 1; i <= playerCount; i++)
        {
            targets[i - 1] = GameObject.Find("Player" + i).transform;
            targetHealth[i - 1] = targets[i - 1].GetComponent<Health>();
        }
    }
	
	// Update is called once per frame
	void Update () {

	    for (int i = 0; i < 4; i++)
        {
            if (i > playerCount - 1)
            {
                pointers[i].gameObject.SetActive(false);
                continue;
            } 
            
            Vector2 screenPos = myCamera.WorldToScreenPoint(targets[i].position);
            float xPos = screenPos.x / Screen.width;
            float yPos = screenPos.y / Screen.height;

            float newXPos = xPos, newYPos = yPos;
            float newXScale = 0.025f, newYScale = 0.05f;

            bool lX = false, lY = false, uX = false, uY = false;
            if (xPos < lowerBoundX)
            {
                pointers[i].texture = pointerHoriz;
                newXPos = lowerBoundX;
                lX = true;
            }
            if (yPos < lowerBoundY)
            {
                pointers[i].texture = pointerVert;
                newYScale *= -1;
                newYPos = lowerBoundY;
                lY = true;
            }
            if (xPos > upperBoundX)
            {
                pointers[i].texture = pointerHoriz;
                newXScale *= -1;
                newXPos = upperBoundX;
                uX = true;
            }
            if (yPos > upperBoundY)
            {
                pointers[i].texture = pointerVert;
                newYPos = upperBoundY;
                uY = true;
            }
            if (lX && lY || lX && uY || uX && uY || uX && lY)
            {
                pointers[i].texture = pointerDiag;
            }

            pointers[i].transform.position = new Vector2(newXPos, newYPos);
            pointers[i].transform.localScale = new Vector2(newXScale, newYScale);
            //if changes are found, set the thing to active. Otherwise, not active.
            pointers[i].gameObject.SetActive((xPos != newXPos || yPos != newYPos) && !targetHealth[i].dead);
        }
	}
}
