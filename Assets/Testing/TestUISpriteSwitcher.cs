using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUISpriteSwitcher : MonoBehaviour {

	[SerializeField] Image image;
	[SerializeField] Sprite sprite1;
	[SerializeField] Sprite sprite2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad1)) image.sprite = sprite1;
		if(Input.GetKeyDown(KeyCode.Keypad2)) image.sprite = sprite2;
	}
}
