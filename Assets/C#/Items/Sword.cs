﻿using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour, item
{
	private HeldItem heldItem;
	public Animator anim;

	// Use this for initialization
	void Start ()
	{
	}

	void fixedUpdate() {
	}

	public void click() {
		Swing ();
	}

	public void unclick() {

	}

	public void Swing() {
		anim.Play ("SwordSlice");
	}

}

