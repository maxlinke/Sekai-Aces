using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMenuScript : MonoBehaviour {

	[SerializeField] GameObject parentMenu;
	[SerializeField] GameObject firstSelected;

	EventSystem es;

	void Start () {
		
	}

	void OnEnable(){
		es = EventSystem.current;
		if(firstSelected != null){
			es.SetSelectedGameObject(firstSelected);
		}
		Debug.Log(this.gameObject.name + " was just enabled");
	}
	
	void Update () {
		bool esc = Input.GetKeyDown(KeyCode.Escape);
		bool b = Input.GetKeyDown(KeyCode.JoystickButton1);
		if((esc || b) && parentMenu != null){
			Debug.Log("closing " + this.gameObject.name + ", opening " + parentMenu.name);
			parentMenu.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}
}
