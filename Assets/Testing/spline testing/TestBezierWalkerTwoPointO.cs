using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class TestBezierWalkerTwoPointO : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] float targetSpeed;

	Vector3 lastPos;
	float p;

	//i find it interesting that the actual speed is seeminly always lower than the targeted one...

	void Start () {
		lastPos = transform.position;
		p = 0f;
	}
	
	void Update () {
		transform.position = spline.MoveAlongSpline(ref p, targetSpeed * Time.deltaTime);
		transform.rotation = spline.GetRotation(p);

		p -= Mathf.Floor(p);

		Vector3 delta = transform.position - lastPos;
		float actualSpeed = delta.magnitude / Time.deltaTime;
		float deltaSpeed = actualSpeed - targetSpeed;
		Debug.Log(deltaSpeed);
		Debug.DrawRay(transform.position, Vector3.up * Mathf.Abs(deltaSpeed), ((deltaSpeed > 0) ? Color.red : Color.green), 100f, false);
		lastPos = transform.position;
	}
}
