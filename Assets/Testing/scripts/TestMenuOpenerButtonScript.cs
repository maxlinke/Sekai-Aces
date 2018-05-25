using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMenuOpenerButtonScript : MonoBehaviour {

	[SerializeField] GameObject thisMenu;
	[SerializeField] GameObject childMenu;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void OpenChildMenu(){
		thisMenu.SetActive(false);
		childMenu.SetActive(true);
	}

}
