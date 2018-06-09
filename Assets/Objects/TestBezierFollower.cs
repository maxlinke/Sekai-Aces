using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class TestBezierFollower : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] float speed;

	float p;

	void Start () {
		
	}
	
	void Update () {
		transform.position = spline.MoveAlongSpline(ref p, speed * Time.deltaTime);
		p -= (int)Mathf.Floor(p);
		transform.rotation = spline.GetRotation(p);
	}
}
