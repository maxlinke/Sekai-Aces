using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour {

		[Header("Sub Menus")]
	[SerializeField] GameStartMenu gameStartMenu;
	[SerializeField] GameObject noControllersScreen;
	[SerializeField] GameObject settingsMenu;
	[SerializeField] GameObject quitScreen;

		[Header("Components")]
	[SerializeField] GameObject firstSelected;
	[SerializeField] Button[] buttons;

	EventSystem es;
	GameObject lastSelected;
	string[] gamepads;

	void Start(){
		gamepads = Input.GetJoystickNames();
		foreach(string s in gamepads){
			Debug.Log("> " + s);
		}
	}

	void Update(){

	}

	void OnEnable(){
		es = EventSystem.current;
		SetAllButtonsActivated(true);
		SelectLast();
	}

	void RememberLastSelected(){
		lastSelected = es.currentSelectedGameObject;
	}

	public void SetAllButtonsActivated(bool value){
		for(int i=0; i<buttons.Length; i++){
			buttons[i].interactable = value;
		}
	}

	public void SelectLast(){
		if(lastSelected == null){
			es.SetSelectedGameObject(firstSelected);
		}else{
			es.SetSelectedGameObject(lastSelected);
		}
	}

	public void SingleplayerSelected(){
		gameStartMenu.InitializeForSingleplayer();
		OpenScreen(gameStartMenu.gameObject);
	}

	public void CooperativeSelected(){
		if(gamepads.Length > 0){
			gameStartMenu.InitializeForCooperative();
			OpenScreen(gameStartMenu.gameObject);
		}else{
			OpenScreen(noControllersScreen);
		}
	}

	public void SettingsSelected(){
		OpenScreen(settingsMenu);
	}

	public void QuitGameSelected(){
		RememberLastSelected();
		SetAllButtonsActivated(false);
		quitScreen.SetActive(true);
	}

	void OpenScreen(GameObject screen){
		RememberLastSelected();
		screen.SetActive(true);
		this.gameObject.SetActive(false);
	}

}
