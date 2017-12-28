﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	public enum PowerUpType {
		None,
		TripleShot,
		FastShot,
		HomingShot
	}

	private float moveSpeed = 2f;
	private float rotSpeed = 200f;

	public PowerUpType type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(-Time.deltaTime * moveSpeed, 0, 0, Space.World);
		transform.Rotate(0, 0, Time.deltaTime * rotSpeed, Space.World);
	}

	public float GetDuration()
	{
		switch(type)
		{
			case PowerUpType.TripleShot:
				return 10f;
			case PowerUpType.FastShot:
				return 10f;
			case PowerUpType.HomingShot:
				return 10f;
			default:
				return 10f;
		}
	}

	public void Die()
	{
		Destroy(gameObject);
	}
}