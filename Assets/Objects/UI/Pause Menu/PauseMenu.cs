﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	[Header("Pause Components")]
	[SerializeField] GameObject pauseFirstSelected;
	[SerializeField] GameObject pauseElementContainer;

	[Header("Gameover Components")]
	[SerializeField] GameObject gameoverFirstSelected;
	[SerializeField] GameObject gameoverElementContainer;

	[Header("General Components")]
	[SerializeField] Button[] mainScreenButtons;


	[SerializeField] SceneAsset mainMenuScene;

	[HideInInspector] public GameController gameController;

	GameObject firstSelected;
	EventSystem es;

	void Start () {
		
	}
	
	void Update () {
		
	}

	void OnEnable(){
		es = EventSystem.current;
		es.SetSelectedGameObject(firstSelected);
	}

	public void InitializeForPause(){
		firstSelected = pauseFirstSelected;
		pauseElementContainer.SetActive(true);
		gameoverElementContainer.SetActive(false);
	}

	public void InitializeForGameover(){
		firstSelected = gameoverFirstSelected;
		pauseElementContainer.SetActive(false);
		gameoverElementContainer.SetActive(true);
	}

	public void Unpause(){
		gameController.UnpauseGame();
		this.gameObject.SetActive(false);
	}

	public void RestartLevel(){
		gameController.ResetLevel();
		this.gameObject.SetActive(false);
	}

	public void ExitToMenu(){
		Time.timeScale = 1f;
		SceneManager.LoadScene(mainMenuScene.name);
	}

}