using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAxisDebuggerTest : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		Vector3 start = transform.position + Vector3.forward;
		Vector3 input = new Vector2(Input.GetAxisRaw("JoystickLX"), Input.GetAxisRaw("JoystickLY"));
		Vector3 end = start + transform.TransformDirection(input);
		Debug.DrawLine(start, end, Color.red, 0f, false);
	}
}
