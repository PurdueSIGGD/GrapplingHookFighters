using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {
    public string useString;
    private float cooldownTime = 1;
    private float time = 0;
    public bool callsFunction;
	public GameObject[] items;
    private Transform childSprite;

	private bool used;
    void Start()
    {
        childSprite = transform.FindChild("ButtonSprite");
    }
    void Update()
    {
        time += Time.deltaTime;
    }
	void DropItem() {
        
		if (!used && !callsFunction) {
			used = true;
			GameObject spawn = (GameObject)GameObject.Instantiate(items[Random.Range(0, items.Length)], transform.FindChild("BoxSpawnPoint").position, Quaternion.identity);
			spawn.GetComponent<Rigidbody2D>().AddForce(80* (Vector2.up + Random.insideUnitCircle));
			spawn.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0, 20));
		}
        if (callsFunction && time > cooldownTime)
        {
            //print("calling function");
            childSprite.GetComponent<Animator>().SetTrigger("Press");
            GameObject.Find("Menus").SendMessage(useString);
            time = 0;
        }
	}
}
