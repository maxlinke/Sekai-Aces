using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BezierSolution;

public class TrackTrigger : MonoBehaviour {

	[SerializeField] UnityEvent action;
	[SerializeField] BezierSpline curve;
	[SerializeField] [Range(0f,1f)] float positionOnCurve;

	const int testResolution = 10000;
	const int cascades = 3;

	public void ApplyPosition () {
		transform.position = curve.GetPoint(positionOnCurve);
	}

	public void SnapToCurve () {
		float closestPos = 0;
		float closestSqrDist = Mathf.Infinity;
		float start = 0f;
		float end = 1f;
		for(int i=0; i<cascades; i++){
			for(int j=0; j<=testResolution; j++){
				float frac = ((float)j / testResolution);
				float pos = ((1f - frac) * start) + (frac * end);
				float sqrDist = (transform.position - curve.GetPoint(pos)).sqrMagnitude;
				if(sqrDist < closestSqrDist){
					closestSqrDist = sqrDist;
					closestPos = pos;
				}
			}
			start = Mathf.Clamp01(closestPos - (1f / testResolution));
			end = Mathf.Clamp01(closestPos + (1f / testResolution));
		}
		positionOnCurve = closestPos;
		ApplyPosition();
	}

	public void DebugThing(){
		for(int i=0; i<curve.Count; i++){
			float frac = ((float)i / (curve.Count - 1));
			Debug.DrawRay(curve.GetPoint(frac), Vector3.up * 100, Color.red, 1f, false);
		}
	}

	void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("TrackFollower")){
			action.Invoke();
		}else{
			Debug.Log("got triggered by non-track follower \"" + otherCollider.name + "\"");
		}
	}

}
