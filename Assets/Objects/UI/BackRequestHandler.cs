using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackRequestHandler : MonoBehaviour {

	public enum ActionType{ENABLEDISABLE, EXECUTEACTION}

	[SerializeField] ActionType action;
	[SerializeField] GameObject enableThisAndDisableSelf;
	[SerializeField] UnityEvent executeThis;

	void Update(){
		bool b = Input.GetKeyDown(KeyCode.JoystickButton1);
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		bool backspace = Input.GetKeyDown(KeyCode.Backspace);
		if(b || escape || backspace){
			if(action == ActionType.ENABLEDISABLE){
				enableThisAndDisableSelf.SetActive(true);
				this.gameObject.SetActive(false);
			}else{
				executeThis.Invoke();
			}
		}
	}

}
