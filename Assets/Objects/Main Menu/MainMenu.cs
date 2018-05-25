using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	[SerializeField] GameObject startScreen;
	[SerializeField] GameObject[] otherScreens;

	void Start () {
		
	}
	
	void Update () {
		
	}

	void OnEnable(){
		startScreen.SetActive(true);
		for(int i=0; i<otherScreens.Length; i++){
			otherScreens[i].SetActive(false);
		}
	}

}
