using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;
//using UnityEditor;

public class FountainOfYou : MonoBehaviour {

	bool activated = false;
	public ParticleSystem particles;
	private HeldItem heldItem;
	/*public Color player1Color;
	public Color player2Color;
	public Color player3Color;
	public Color player4Color;*/

	// Use this for initialization
	void Start () {
		heldItem = GetComponent<HeldItem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (activated && particles.isStopped) {
			Destroy (gameObject);
		}
	}

	void StartFountain() {
		particles.Play();
	}

	public void click() {
		if (particles != null && activated == false && heldItem.focus != null) {
			PickColor();
			activated = true;
			StartFountain ();
		}
	}

	void PickColor() {
		Color newColor = heldItem.focus.transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ().color;
		Debug.Log (newColor);
		particles.startColor = newColor;
	}

	/*void DetermineHolder () {
		string name = heldItem.focus.name;
		switch (name) {
		case ("Player1"):
			{
				particles.startColor = player1Color;
				break;
			}
		case ("Player2"):
			{
				particles.startColor = player2Color;
				break;
			}
		case ("Player3"):
			{
				particles.startColor = player3Color;
				break;
			}
		case ("Player4"):
			{
				particles.startColor = player4Color;
				break;
			}
		}
	}*/
}
	