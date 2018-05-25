using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class MainMenuCamera : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] GameObject focus;
	[SerializeField] float speed;

	float p;

	void Start(){
		
	}
	
	void Update(){
		FollowSpline();
		LookAtFocus();
	}

	void FollowSpline(){
		transform.position = spline.MoveAlongSpline(ref p, speed * Time.deltaTime);
		p = p - Mathf.Floor(p);
	}

	void LookAtFocus(){
		Vector3 toFocus = focus.transform.position - this.transform.position;
		transform.rotation = Quaternion.LookRotation(toFocus, Vector3.up);
	}

}
