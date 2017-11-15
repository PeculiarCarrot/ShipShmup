﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Ship {
	int dir = 0;

	// Use this for initialization
	public override void DoStart () {
		accel = .3f + Random.value * .1f;
		rotAccel = 100f;
	}
	
	// Update is called once per frame
	public override void DoUpdate () {
		MoveLeft();
		if(Random.value < .05)
			NewDirection();
		velocity.y += (accel / 2f) * dir;
		rotSpeed += rotAccel * dir;
	}

	void OnTriggerEnter (Collider col)
    {
    	PlayerBullet bullet = col.gameObject.GetComponent<PlayerBullet>();
        if(bullet != null)
        {
            bullet.Die();
            GetHurt(bullet.GetDamage());
            return;
        }
    	Player player = col.gameObject.GetComponent<Player>();
    	if(player != null)
    	{
    		player.GetHurt(50);
    		Die();
    		return;
    	}
    }

	private void NewDirection()
	{
		dir = Random.Range(-1, 2);
	}
}
