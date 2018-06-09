using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameGUI : MonoBehaviour {

	[SerializeField] PlayerGUI p1GUI;
	[SerializeField] PlayerGUI p2GUI;

	void Awake () {
		p1GUI.gameObject.SetActive(false);
		p2GUI.gameObject.SetActive(false);
	}

	void Start () {
		
	}
	
	void Update () {
		
	}

	public PlayerGUI ActivateAndGetPlayerGUI(int playerNumber){
		switch(playerNumber){
		case 1:
			p1GUI.gameObject.SetActive(true);
			return p1GUI;
		case 2:
			p2GUI.gameObject.SetActive(true);
			return p2GUI;
		default:
			throw new UnityException("Unsupported playernumber \"" + playerNumber.ToString() + "\"");
		}
	}
}
