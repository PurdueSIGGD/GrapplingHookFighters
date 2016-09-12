using UnityEngine;
using System.Collections;

public class RandomWeather : MonoBehaviour {

	public GameObject m;//meteor
	public GameObject r;//rain
	public GameObject l;//lighting
	gun thisgun;
	int weather;
	// Use this for initialization
	void Start () {
		weather = Random.Range (0,3);
		thisgun = gameObject.GetComponent<gun> ();
		switch (weather) {
		case 0:
			thisgun.projectileGameObject = m;
			break;
		case 1:
			thisgun.projectileGameObject = r;
			break;
		case 2:
			thisgun.projectileGameObject = l;
			break;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
