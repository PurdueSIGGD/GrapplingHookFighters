using UnityEngine;
using System.Collections;

public class LineFader : MonoBehaviour {
	public Color startColor;
	public float time;
	private float startTime;
	LineRenderer[] reds;
	void Start () {
		startTime = time;
		reds = this.GetComponents<LineRenderer>();
	}
	
	void Update () {
		Color ac = new Color(startColor.r, startColor.g, startColor.b, (time/startTime));
		foreach (LineRenderer r in reds) {
			r.SetColors(ac, ac);
		}
		time -= Time.deltaTime;
		if (time <= 0) {
			Destroy(this.gameObject);
		}
	}
}
