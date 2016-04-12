using UnityEngine;
using System.Collections;

public class LineFader : MonoBehaviour {
	public Color startStartColor, startEndColor;
	public float time;
	private float startTime;
	LineRenderer[] reds;
	void Start () {
		startTime = time;
		reds = this.GetComponents<LineRenderer>();
	}
	
	void Update () {
		Color sc = new Color(startStartColor.r, startStartColor.g, startStartColor.b, (time/startTime));
		foreach (LineRenderer r in reds) {
			r.SetColors(sc, startEndColor);
		}
		time -= Time.deltaTime;
		if (time <= 0) {
			Destroy(this.gameObject);
		}
	}
}
