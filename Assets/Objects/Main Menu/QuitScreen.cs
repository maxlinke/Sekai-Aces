using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitScreen : MonoBehaviour {

	[SerializeField] StartMenu startMenu;
	[SerializeField] GameObject firstSelected;

	EventSystem es;

	void Start(){
		
	}
	
	void Update(){
		
	}

	void OnEnable(){
		es = EventSystem.current;
		es.SetSelectedGameObject(firstSelected);
	}

	public void QuitGame(){
		Application.Quit();
	}

	public void Abort(){
		startMenu.SetAllButtonsActivated(true);
		startMenu.SelectLast();
		this.gameObject.SetActive(false);
	}

}
