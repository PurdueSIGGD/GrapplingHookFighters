using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {
	public GameObject[] items;
	private bool used;
	void DropItem() {
		if (!used) {
			used = true;
			GameObject spawn = (GameObject)GameObject.Instantiate(items[Random.Range(0, items.Length)], transform.FindChild("BoxSpawnPoint").position, Quaternion.identity);
			spawn.GetComponent<Rigidbody2D>().AddForce(80* (Vector2.up + Random.insideUnitCircle));
			spawn.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0, 20));
		}
	}
}
