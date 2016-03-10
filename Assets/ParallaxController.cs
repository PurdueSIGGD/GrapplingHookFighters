using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour {
	/* Meant for changing backgrounds in the distance, mostly sprites and biz
	 * Distances - An array of floats which correspond to the children of this object, and will move around
	 * according to their distance. i.e. more distance, less movement
	 */
	public float[] distances;
	Transform holder;

	void Start() {
		holder = GameObject.Find("ParalayerHolder").transform;
	}
	void Update () {
		

		for (int i = 1; i <= distances.Length; i++) {
			Transform currentDude = holder.FindChild("ParaLayer (" + i + ")");
			currentDude.localPosition = -1 * transform.position / distances[i - 1];
		//	currentDude.localScale = Vector3.one * 10/(camSize * distances[i - 1]);
		}
		//float f = 15/transform.FindChild("Main Camera").GetComponent<Camera>().orthographicSize;
		//holder.localScale = new Vector3(f,f,f);
	}
}
