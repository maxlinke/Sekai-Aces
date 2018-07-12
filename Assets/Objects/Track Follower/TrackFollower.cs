using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class TrackFollower : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] float initialSpeed;

	float positionOnSpline;
	float speed;

	float desiredSpeed;
	float speedChangeDuration;

	Vector3 currentPos;
	Vector3 lastPos;

	void Update () {
		transform.position = spline.MoveAlongSpline(ref positionOnSpline, speed * Time.deltaTime);
		positionOnSpline -= (int)Mathf.Floor(positionOnSpline);
		transform.rotation = spline.GetRotation(positionOnSpline);

		lastPos = currentPos;
		currentPos = transform.position;
	}

	public void LevelReset () {
		StopAllCoroutines();
		positionOnSpline = 0;
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
		positionOnSpline = newSpline.GetClosestPosition(transform.position);
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

	//TODO this thing gets its own layer that only "collides" with triggers along the track... or more like the triggers only get triggered by this thing. i suppose i could just check a tag via script too, probably not many trigger events

}
