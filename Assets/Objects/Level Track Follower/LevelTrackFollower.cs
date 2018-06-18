using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class LevelTrackFollower : MonoBehaviour {

	[SerializeField] BezierSpline spline;
	[SerializeField] float speed;

	float p;

	Vector3 currentPos;
	Vector3 lastPos;

	void Start () {

	}

	void Update () {
		transform.position = spline.MoveAlongSpline(ref p, speed * Time.deltaTime);
		p -= (int)Mathf.Floor(p);
		transform.rotation = spline.GetRotation(p);

		lastPos = currentPos;
		currentPos = transform.position;
	}

	public Vector3 GetVelocity(){
		return (currentPos - lastPos) / Time.deltaTime;
	}

	public void SetSpeed(float value){
		speed = value;
	}

	//TODO this thing gets its own layer that only "collides" with triggers along the track... or more like the triggers only get triggered by this thing. i suppose i could just check a tag via script too, probably not many trigger events

}
