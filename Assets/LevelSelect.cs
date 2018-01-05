﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public AutoSelect autoselect;
	public GameObject buttonBase;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 12; i++)
		{
			GameObject btn = Instantiate(buttonBase, buttonBase.transform.parent);
			btn.transform.Translate(((Screen.width) / 12) * i, 0, 0);
			btn.GetComponentInChildren<Text>().text = "" + (i + 1);
			int j = i; //For some reason I think the ToLevel callback is keeping a reference to i, so I'm giving it a copy instead
			btn.GetComponent<Button>().onClick.AddListener(() => ToLevel(j));
			if (i == 0)
				autoselect.toSelect = btn;
		}
		buttonBase.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ToLevel(int level)
	{
		PlayerPrefs.SetInt("levelToStart", level);
		PlayerPrefs.Save();
		SceneManager.LoadScene("play");
	}
}
