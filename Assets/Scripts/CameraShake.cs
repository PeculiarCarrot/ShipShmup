﻿using UnityEngine;
using System.Collections;
//written by https://github.com/ftvs
public class CameraShake : MonoBehaviour
{
	private static CameraShake Instance;
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
		Instance = this;
	}

	public static void Shake(float shakeDuration)
	{
		Instance.shakeDuration = shakeDuration;
	}

	public static void Shake(float shakeDuration, float shakeAmount)
	{
		Instance.shakeDuration = shakeDuration;
		Instance.shakeAmount = shakeAmount;
	}

	public static void Shake(float shakeDuration, float shakeAmount, float decreaseFactor)
	{
		Instance.shakeDuration = shakeDuration;
		Instance.shakeAmount = shakeAmount;
		Instance.decreaseFactor = decreaseFactor;
	}

	void Update()
	{
		if (!Options.screenShake)
			return;
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}
}