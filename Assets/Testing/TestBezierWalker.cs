using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class TestBezierWalker : MonoBehaviour {

	public enum TravelMode { Once, Loop };

	[SerializeField] BezierSpline spline;
	public TravelMode travelMode;

	[Range(0f, 1f)]
	[SerializeField] float progress;

	public float speed;

	float p;

	void Start () {
		
	}
	
	void Update () {

		transform.position = spline.MoveAlongSpline(ref p, speed * Time.deltaTime);
		transform.rotation = spline.GetRotation(p);
		if(travelMode == TravelMode.Once){
			p = Mathf.Clamp01(p);
		}else if(travelMode == TravelMode.Loop){
			p -= (int)Mathf.Floor(p);
		}else{
			throw new UnityException("unknown travel mode");
		}

		//float d = spline.GetTangent(progress).magnitude;

		//transform.position = spline.GetPoint(progress);
		//transform.rotation = spline.GetRotation(progress);

		//transform.position = spline.GetNormalizedPoint(progress);
		//transform.position = spline.MoveAlongSpline(ref progress, Time.deltaTime);


	}

}
