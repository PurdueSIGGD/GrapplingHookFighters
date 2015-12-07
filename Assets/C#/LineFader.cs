using UnityEngine;
using System.Collections;

public class LineFader : MonoBehaviour {
	public Color startColor;
	public float time;
	private float startTime;
	void Start () {
		startTime = time;
	}
	
	void Update () {
		Color ac = new Color(startColor.r, startColor.g, startColor.b, (time/startTime));
		this.GetComponent<LineRenderer>().SetColors(ac, ac);
		time -= Time.deltaTime;
		if (time <= 0) {
			Destroy(this.gameObject);
		}
	}
}
