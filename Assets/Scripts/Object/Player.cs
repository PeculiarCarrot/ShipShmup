﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameAnalyticsSDK;

public class Player : Ship {

	public AudioClip hit, die;

	public EnemySpawner spawner;
	public GameObject mesh;
	public GameObject core;
	private float invincibilityDuration;

	//timer for flickering when we get hit
	private float flickerTimer, flickerDuration = .05f;

	//if we're in debug mode, we're invulnerable can't get hit'
	public bool debug;
	[HideInInspector]
	public bool wasDebug;
	private static Texture livesTexture, progressTexture;
	public Image chargeImage;
	private bool regenerating;

	private float dieTimer, dieTime = .8f;
	private bool dying;
	//The death effect prefab for regular enemies
	private static GameObject deathEffect;

	private Rigidbody body;
	private float charge;
	private float maxCharge = 50;

	private Ability currentAbility;
	public AbilityPicker abilityPicker;

	public AudioClip charged;

	public Color notChargedColor, chargedColor;

	private Vector2 velocity;
	private AudioSource audio;

	public AudioClip[] regenSounds;
	public AudioClip doneRegenSound;

	public WipeEffect wipe;

	// Use this for initialization
	public override void DoStart () {
		wasDebug = debug;
		livesTexture = Resources.Load<Texture>("Materials/health");
		progressTexture = Resources.Load<Texture>("Materials/progress");
		body = GetComponent<Rigidbody>();
		dieTimer = 99999999;
		Time.timeScale = 1;
		if(deathEffect == null)
			deathEffect = Resources.Load<GameObject>("Prefabs/Effects/deathEffect");
		audio = GetComponent<AudioSource>();
	}

	public void AddCharge(float amount)
	{
		bool wasFull = charge >= maxCharge;
		charge += amount;
		if (!wasFull && charge >= maxCharge)
			PlayChargeNotification();
	}

	void FixedUpdate()
	{
		if(Options.keyboardMovement)
		{
			target = transform.position;
			target.x += velocity.x * Time.deltaTime;
			target.y += velocity.y * Time.deltaTime;
			body.MovePosition(target);
		}
		else
			body.MovePosition(Vector3.Lerp(transform.position, target, Options.smoothMovement ? .3f : 1f));
	}

	private void PlayChargeNotification()
	{
		audio.PlayOneShot(charged);
	}

	public bool IsUsingAbility()
	{
		return currentAbility != null;
	}

	public bool IsDying()
	{
		return dying;
	}

	private float playbackCursor;
	private float hpDrawSize;
	private float renderCharge;

    void OnGUI()
    {
		chargeImage.color = charge >= maxCharge ? chargedColor : notChargedColor;
    	float size = 24;
		hpDrawSize = Mathf.Lerp(hpDrawSize, size, 3 * Time.deltaTime);
		for(int i = 0; i < Mathf.Floor(hp); i++)
    	{
			GUI.DrawTexture(new Rect(42 + (size + 30) * i - hpDrawSize / 2f, Screen.height - 58 - hpDrawSize / 2f, hpDrawSize, hpDrawSize), livesTexture);
		}
		charge = Mathf.Clamp(charge, 0, maxCharge);
		renderCharge = Mathf.Clamp(renderCharge, 0, maxCharge);
		chargeImage.fillAmount = (renderCharge / maxCharge);

		float w, h;
		float pad = 50;
		playbackCursor = Mathf.Lerp(playbackCursor, Stage.songProgress, 3 * Time.deltaTime);
		w = Screen.width - pad * 2;
		h = 5;
		GUI.DrawTexture(new Rect(pad, Screen.height - 20, w, h), progressTexture);
		GUI.DrawTexture(new Rect(pad + w * playbackCursor, Screen.height - 20 - h * 1.5f, 7, h * 4), progressTexture);

		//if(debug)
		//	GUI.Label(new Rect(0, 0, 100, 100), ""+(int)(1.0f / (Time.smoothDeltaTime/Time.timeScale)));    
    }

	public void GetHurt()
	{
		if(!debug)
		{
			hp -= 1;
			if(hp > 0)
			{
				audio.PlayOneShot(hit);
				CameraShake.Shake(.2f, .15f);
			}
			invincibilityDuration = 2f;
			flickerTimer = flickerDuration;
		}
	}


	public new void Die()
	{
		CameraShake.Shake(.15f, .05f, 1f);
		dieTimer = dieTime;
		Time.timeScale = .2f;
		dying = true;
		audio.PlayOneShot(die);
		GetComponent<PatternController>().enabled = false;
		GameObject e = Instantiate(deathEffect, transform.position + new Vector3(0, 0, -3), deathEffect.transform.rotation);
		e.GetComponent<LineRenderer>().material = GetComponentInChildren<MeshRenderer>().material;
		GetComponentInChildren<MeshRenderer>().enabled = false;
		GetComponentInChildren<Collider>().enabled = false;
		//transform.position = newPos;
		Debug.Log("FAIL SONG " + spawner.level + " at: " + Mathf.RoundToInt(Stage.stage.song.time));
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Song " + spawner.level, Mathf.RoundToInt(Stage.stage.song.time));
	}

	public void SetChosenAbility()
	{
		PlayerPrefs.SetInt("chosenAbility", abilityPicker.selectedIndex);
	}

	private void Restart()
	{
		if (wipe.IsTransitioning())
			return;
		PlayerPrefs.SetInt("levelToStart", spawner.level);
		PlayerPrefs.SetInt("chosenAbility", abilityPicker.selectedIndex);
		PlayerPrefs.SetInt("reasonForLevelChange", EnemySpawner.DEATH);
		PlayerPrefs.Save();
		BulletFactory.SleepAll();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public bool IsInvincible()
	{
		return invincibilityDuration > 0 || currentAbility is BubbleAbility;
	}

	public void Regenerate()
	{
		regenerating = true;
	}

	public Material powerUpMaterial;

	public void UseAbility()
	{
		charge = 0;
		if (currentAbility != null)
			currentAbility.End();
		currentAbility = abilityPicker.GetNewAbility(this);
		currentAbility.Begin();

		if(currentAbility is BubbleAbility) audio.PlayOneShot(shieldAbilitySound);
		else if(currentAbility is TimeAbility) audio.PlayOneShot(timeAbilitySound);
		else if(currentAbility is LaserAbility)
		{
			((LaserAbility)currentAbility).shootSound = laserShootSound;
			audio.PlayOneShot(laserAbilitySound);
		}

		GameObject e = Instantiate(deathEffect, transform.position + new Vector3(0, 0, -3), deathEffect.transform.rotation);
		e.GetComponent<DeathEffect>().spd = 1.5f;
		e.GetComponent<DeathEffect>().shrink = .3f;
		e.GetComponent<LineRenderer>().material = powerUpMaterial;
	}

	public GameObject bulletDieEffect;

	public void MakeBulletEffect(BulletProperties bullet)
	{
		Material mat = bullet.GetComponentInChildren<Renderer>().material;
		GameObject part = Instantiate(bulletDieEffect, bullet.transform.position + new Vector3(0, 0, -2), bulletDieEffect.transform.rotation);
		ParticleSystem ps = part.GetComponent<ParticleSystem>();
		var main = ps.main;
		foreach (Renderer r in part.GetComponents<Renderer>())
			r.material.color = mat.color;

		GameObject e = Instantiate(deathEffect, bullet.transform.position + new Vector3(0, 0, -3), deathEffect.transform.rotation);
		e.GetComponent<DeathEffect>().spd = 1.8f;
		e.GetComponent<DeathEffect>().shrink = .5f;
		e.GetComponent<LineRenderer>().material = mat;
	}

	public TrailRenderer trailRenderer;

	public AudioClip timeAbilitySound, laserAbilitySound, shieldAbilitySound, laserShootSound;

	private float accel = 10f, slowAccel = 3f;
	private Vector3 target;
	// Update is called once per frame
	public void Update () {


		if (EditorController.editingMode == 1)
			return;

		dieTimer -= Time.deltaTime;
		if(dieTimer <= 0)
		{
			dieTimer = 9999999;
			Restart();
		}

		AddCharge(.5f * Time.deltaTime);

		renderCharge = Mathf.Lerp(renderCharge, charge, 10f * Time.deltaTime);

		DoUpdate();

		if (currentAbility != null)
		{
			currentAbility.MyUpdate();
			if (currentAbility.IsFinished())
				currentAbility = null;
		}

		if(!dying)
		{
			if(invincibilityDuration > 0)
			{
				invincibilityDuration -= Time.deltaTime;
				flickerTimer -= Time.deltaTime;
				if(flickerTimer < - flickerDuration)
					flickerTimer = flickerDuration;
				mesh.GetComponent<Renderer>().enabled = invincibilityDuration <= 0 || flickerTimer < 0;
				trailRenderer.enabled = invincibilityDuration <= 0 || flickerTimer < 0;
			}

			if(Options.keyboardMovement)
			{
				target = transform.position;

				velocity = Vector2.zero;
				Vector2 input = Vector2.zero;
				if (Input.GetKey(KeyCode.LeftArrow))
					input.x -= 1;
				if (Input.GetKey(KeyCode.RightArrow))
					input.x += 1;
				if (Input.GetKey(KeyCode.UpArrow))
					input.y += 1;
				if (Input.GetKey(KeyCode.DownArrow))
					input.y -= 1;
				input.Normalize();
				velocity = input * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? slowAccel : accel);
			}
			else{
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				target.z = transform.position.z;
			}
	
			if(regenerating || EditorController.editingMode != 0)
	        {
				float lastHP = hp;
				hp += 3f * Time.deltaTime;
				if(hp >= maxHP)
				{
					hp = maxHP;
					regenerating = false;
				}
				if(Mathf.Floor(hp) > Mathf.Floor(lastHP))
				{
					audio.PlayOneShot(regenSounds[(int)Mathf.Floor(hp) - 2], .75f);
					hpDrawSize *= 1.5f;
				}
	        }

			if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.F)) && (charge >= maxCharge || Application.isEditor))
				UseAbility();

			transform.position = new Vector3(Mathf.Clamp(transform.position.x, Stage.minX, Stage.maxX),
				Mathf.Clamp(transform.position.y, Stage.minY, Stage.maxY), transform.position.z);
			if(hp <= 0)
				Die();
		}
	}
}
