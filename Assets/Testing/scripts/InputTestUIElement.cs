using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class InputTestUIElement : MonoBehaviour {

	Text textField;
	[SerializeField] TestParameter testParameter;

	[SerializeField] KeyCode testKey;
	[SerializeField] string axisName;
	[SerializeField] string buttonName;

	Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
	Color activeColor = new Color(0.8f, 1f, 0.8f);
	Color errorColor = new Color(0.8f, 0.2f, 0.2f);

	void Start () {
		textField = this.GetComponent<Text>();
		string name;
		switch(testParameter){
		case TestParameter.KEYBOARD:
			name = testKey.ToString();
			break;
		case TestParameter.AXIS:
			name = axisName;
			break;
		case TestParameter.BUTTON:
			name = buttonName;
			break;
		default:
			name = "ERROR";
			break;
		}
		textField.gameObject.name = name;
		textField.text = name;
		textField.color = inactiveColor;
	}
	
	void Update () {
		switch(testParameter){
		case TestParameter.KEYBOARD:
			KeyboardTest();
			break;
		case TestParameter.AXIS:
			AxisTest();
			break;
		case TestParameter.BUTTON:
			ButtonTest();
			break;
		default:
			textField.color = errorColor;
			break;
		}
	}

	void KeyboardTest(){
		if(Input.GetKey(testKey)){
			textField.color = activeColor;
		}else{
			textField.color = inactiveColor;
		}
	}

	void AxisTest(){
		float value = Input.GetAxisRaw(axisName);
		Color col = Color.Lerp(inactiveColor, activeColor, Mathf.Abs(value));
		textField.text = axisName + "\n" + value;
		textField.color = col;
	}

	void ButtonTest(){
		if(Input.GetButton(buttonName)){
			textField.color = activeColor;
		}else{
			textField.color = inactiveColor;
		}
	}

	enum TestParameter{ KEYBOARD, AXIS, BUTTON };
}
