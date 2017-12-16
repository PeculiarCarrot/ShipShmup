﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : Ship {

	public AudioClip hit;

	public EnemySpawner spawner;
	public GameObject mesh;
	public GameObject core;
	private PowerUp.PowerUpType currentPowerUp;
	private float remainingPowerUpDuration;
	private float invincibilityDuration;
	private float flickerTimer, flickerDuration = .05f;
	public bool debug;
	[HideInInspector]
	public bool wasDebug;
	public Texture livesTexture;
	private bool regenerating;

	private Rigidbody body;

	// Use this for initialization
	public override void DoStart () {
		wasDebug = debug;
		livesTexture = Resources.Load<Texture>("Materials/health");
		body = GetComponent<Rigidbody>();
	}

	void OnTriggerEnter (Collider col)
    {
    	PowerUp powerUp = col.gameObject.GetComponent<PowerUp>();
        if(powerUp != null)
        {
            powerUp.Die();
            ApplyPowerUp(powerUp.type, powerUp.GetDuration());
            return;
        }
    }

    void OnGUI()
    {
    	float size = 16;
    	for(int i = 0; i < hp; i++)
    	{
    		 GUI.DrawTexture(new Rect(30 + (size + 20) * i, Screen.height - 56, size, size), livesTexture);
		}
		if(debug)
			GUI.Label(new Rect(0, 0, 100, 100), ""+(int)(1.0f / (Time.smoothDeltaTime/Time.timeScale)));    
    }

	public void GetHurt()
	{
		if(!debug)
		{
			GetComponent<AudioSource>().PlayOneShot(hit);
			CameraShake.Shake(.2f, .15f);
			hp -= 1;
			invincibilityDuration = 2f;
			flickerTimer = flickerDuration;
		}
	}

	public new void Die()
	{
		PlayerPrefs.SetInt("diedOnLevel", spawner.level);
		PlayerPrefs.Save();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
    private void ApplyPowerUp(PowerUp.PowerUpType type, float duration)
    {
    	currentPowerUp = type;
    	remainingPowerUpDuration = duration;
    }

	public bool IsInvincible()
	{
		return invincibilityDuration > 0;
	}

	public void Regenerate()
	{
		regenerating = true;
	}

	// Update is called once per frame
	public void Update () {
		Cursor.visible = false;
		DoUpdate();
		if(currentPowerUp != PowerUp.PowerUpType.None)
		{
			remainingPowerUpDuration -= Time.deltaTime;
			if(remainingPowerUpDuration <= 0)
			{
				remainingPowerUpDuration = 0;
				ApplyPowerUp(PowerUp.PowerUpType.None, 0);
			}
		}
		if(invincibilityDuration > 0)
		{
			invincibilityDuration -= Time.deltaTime;
			flickerTimer -= Time.deltaTime;
			if(flickerTimer < - flickerDuration)
				flickerTimer = flickerDuration;
			mesh.GetComponent<Renderer>().enabled = invincibilityDuration <= 0 || flickerTimer < 0;
		}
		Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
       /* Vector3 newRot = transform.rotation.eulerAngles;
        newRot.y -= rotAccel * (target.x - transform.position.x);
        if(newRot.y < 0)
        	newRot.y += 360;
        if(newRot.y < 180)
        	newRot.y = Mathf.Clamp(newRot.y, 0, 45);
        else
        	newRot.y = Mathf.Clamp(newRot.y, 315, 360);
        newRot.y = Mathf.SmoothDampAngle(newRot.y, 0, ref rotSpeed, .3f);
        transform.eulerAngles = newRot;*/
		body.MovePosition(Vector3.Lerp(transform.position, target, .3f));
        if(regenerating)
        {
        	hp += 3f * Time.deltaTime;
        	if(hp >= maxHP)
        	{
        		hp = maxHP;
        		regenerating = false;
        	}
        }

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, Stage.minX, Stage.maxX),
			Mathf.Clamp(transform.position.y, Stage.minY, Stage.maxY), transform.position.z);
		if(hp <= 0)
			Die();
	}
}
