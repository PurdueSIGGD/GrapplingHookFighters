using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour, item
{
	private HeldItem heldItem;
	public Animator anim;

	// Use this for initialization
	void Start ()
	{
        heldItem = this.GetComponent<HeldItem>();
	}

	void fixedUpdate() {
	}

	public void click() {
		Swing ();
        if (heldItem.focus)
        {
            heldItem.focus.transform.FindChild("AnimationController").SendMessage("Swing");
        }
    }

	public void unclick() {
        /*if (heldItem.focus)
        {
            heldItem.focus.transform.FindChild("AnimationController").SendMessage("UnSwing");
        }*/
    }

	public void Swing() {
		anim.Play ("SwordSlice");
        
        
	}

}

