﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class TrackFollower : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] float initialSpeed;
	[SerializeField] float initialPosition;

	float position;
	float speed;

	Vector3 currentPos;
	Vector3 lastPos;

	void Update () {
		transform.position = spline.MoveAlongSpline(ref position, speed * Time.deltaTime);
		position -= (int)Mathf.Floor(position);
		transform.rotation = spline.GetRotation(position);

		lastPos = currentPos;
		currentPos = transform.position;
	}

	public void LevelReset () {
		StopAllCoroutines();
		position = initialPosition;
		speed = initialSpeed;
	}

	public Vector3 GetVelocity(){
		return (currentPos - lastPos) / Time.deltaTime;
	}

	public void SetSpeed(float value){
		speed = value;
	}

	public void ChangeSpeed (float newSpeed, float duration) {
		StartCoroutine(SpeedChangeCoroutine(newSpeed, duration));
	}

	public void ChangeSpline (BezierSpline newSpline) {
		spline = newSpline;
		position = newSpline.GetClosestPosition(transform.position);
	}

	IEnumerator SpeedChangeCoroutine(float newSpeed, float timeSpan){
		float startSpeed = speed;
		float startTime = Time.time;
		float progress = 0;
		while(progress < 1){
			speed = (progress * newSpeed) + ((1f - progress) * startSpeed);
			progress = (Time.time - startTime)/timeSpan;
			yield return null;
		}
		speed = newSpeed;
	}

}
