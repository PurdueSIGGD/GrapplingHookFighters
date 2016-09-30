using UnityEngine;
using System.Collections;
public enum Direction {
	VERTICAL,
	HORIZONTAL,
	ANY
}
public class MenuNode : MonoBehaviour {
	public Direction direction;
	public int depth;
	public void Traverse(GameObject nextObj) {
		MenuNode next = nextObj.GetComponent<MenuNode>();
		int c1 = depth;
		int c2 = next.depth;
		nextObj.GetComponent<Animator>().SetBool("Insta", false);
		GetComponent<Animator>().SetBool("Insta", false);
		if (direction == Direction.HORIZONTAL || (direction == Direction.ANY && next.direction == Direction.HORIZONTAL)) {
			if (c1 < c2) {
				nextObj.GetComponent<Animator>().SetBool(c1<0?"Left":"Right", false);
				print("Going to the right " + c1 + " " + c2);
			} else {
				GetComponent<Animator>().SetBool(c1<0?"Left":"Right", true);
				print("Going to the left" + c1 + " " + c2);
			}
		} else if (direction == Direction.VERTICAL || (direction == Direction.ANY && next.direction == Direction.VERTICAL)) {
			if (c1 < c2) {
				nextObj.GetComponent<Animator>().SetBool(c1<0?"Bottom":"Top", false);
				print("Going upwards " + c1 + " " + c2);
			} else {
				GetComponent<Animator>().SetBool(c1<0?"Bottom":"Top", true);

				print("Going to the downwards" + c1 + " " + c2);
			}
		}
	}
}


