using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler {

	[SerializeField] UnityEvent selectAction;
	[SerializeField] UnityEvent submitAction;
	[SerializeField] UnityEvent deselectAction;

	public void OnPointerEnter(PointerEventData ped){
		InvokeSelectAction();
		Debug.Log("enter | " + Time.time);
	}

	public void OnSelect(BaseEventData bed){
		InvokeSelectAction();
		Debug.Log("select | " + Time.time);
	}

	public void OnPointerClick(PointerEventData ped){
		InvokeSubmitAction();
		Debug.Log("click | " + Time.time);
	}

	public void OnSubmit(BaseEventData bed){
		InvokeSubmitAction();
		Debug.Log("submit | " + Time.time);
	}

	public void OnPointerExit(PointerEventData ped){
		InvokeDeselectAction();
		Debug.Log("exit | " + Time.time);
	}

	public void OnDeselect(BaseEventData bed){
		InvokeDeselectAction();
		Debug.Log("deselect | " + Time.time);
	}

	void InvokeSelectAction(){
		if(selectAction != null) selectAction.Invoke();
	}

	void InvokeSubmitAction(){
		if(submitAction != null) submitAction.Invoke();
	}

	void InvokeDeselectAction(){
		if(deselectAction != null) deselectAction.Invoke();
	}

}
