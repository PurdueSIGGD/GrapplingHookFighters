using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class lightning : MonoBehaviour {

	//flash screen for when lightning effect was made was first put in under the AutoZoomCamera and not part of the
	//lightning effect 

	public Image lightningSprite;
	public Image flashScreen;
	public Canvas effectcanvas;
	Vector3 canvasOffset;
	//Vector3 stageCenter;

	float interval;//how long the lightning is spaced out
	float appear;//how long the lightning is out
	float flashAppear;
	bool hasStruck;//lightning is lightninging and stuff...
	

	void Start () {
		//lightningSprite = this.gameObject.GetComponent<SpriteRenderer>();
		//stageCenter = gameObject.transform.FindChild("Stage Center").transform.position;
		//flashScreen = gameObject.GetComponentsInChildren
		//flashScreen.color = new Color(1,1,1,1);
		canvasOffset = effectcanvas.transform.position;
		flashScreen.transform.position = new Vector3 (canvasOffset.x,canvasOffset.y,canvasOffset.z-5);
		lightningSprite.enabled = false;
		flashScreen.enabled = false;
		interval = Random.Range (50, 100);
		appear = Random.Range (10, 30);
		hasStruck = false;
	}
	
	// Update is called once per frame
	void Update () {
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
				}else{
					flashAppear--;
				}
			}else{
				interval--;
			}
		}
	}
}
