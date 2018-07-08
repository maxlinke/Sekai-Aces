using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BezierSolution;

public class TrackTrigger : MonoBehaviour {

	[SerializeField] UnityEvent action;

	[Space(50f)]

	public BezierSpline curve;
	[SerializeField] [Range(0f,1f)] float positionOnCurve;

	const int pointsPerSegment = 100;
	const int iterations = 2;
	const int pointsPerIteration = 10;

	public void ApplyPosition () {
		Vector3 beforePos = transform.position;
		transform.position = curve.GetPoint(positionOnCurve);
		Vector3 afterPos = transform.position;
		Debug.DrawLine(beforePos, afterPos, Color.magenta, 1f, false);
	}

	public void SnapToCurve () {
		float closestPos = 0;
		float minSqrDist = Mathf.Infinity;

		int segmentCount = (curve.loop ? curve.Count : (curve.Count - 1));
		int numberOfTestPoints = (segmentCount * pointsPerSegment) + 1;

		for(int i=0; i<numberOfTestPoints; i++){
			float pos = ((float)i / (numberOfTestPoints - 1));
			float sqrDist = (curve.GetPoint(pos) - transform.position).sqrMagnitude;
			if(sqrDist < minSqrDist){
				closestPos = pos;
				minSqrDist = sqrDist;
			}
//			Debug.DrawRay(curve.GetPoint(pos), Vector3.up, Color.yellow, 1f, false);
		}

		float stepDistance = 1f / numberOfTestPoints;
		float start = Mathf.Clamp01(closestPos + stepDistance);
		float end = Mathf.Clamp01(closestPos - stepDistance);

		for(int i=0; i<iterations; i++){
			for(int j=0; j<pointsPerIteration; j++){
				float frac = ((float)j / (pointsPerIteration - 1));
				float pos = ((1f - frac) * start) + (frac * end);
				float sqrDist = (transform.position - curve.GetPoint(pos)).sqrMagnitude;
				if(sqrDist < minSqrDist){
					minSqrDist = sqrDist;
					closestPos = pos;
				}
//				Vector3 drawPoint = curve.GetPoint(pos) + (Vector3.down * (0.1f * i));
//				Vector3 drawVec = Vector3.down * (1f / (i + 1));
//				Color drawCol = ((i % 2 == 0) ? Color.red : Color.blue);
//				Debug.DrawRay(drawPoint, drawVec, drawCol, 1f, false);
			}
			stepDistance /= (pointsPerIteration - 1);
			start = Mathf.Clamp01(closestPos - (2f * stepDistance));
			end = Mathf.Clamp01(closestPos + (2f * stepDistance));
		}

		positionOnCurve = closestPos;
		ApplyPosition();
	}

	void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("TrackFollower")){
			action.Invoke();
		}else{
			Debug.Log("got triggered by non-track follower \"" + otherCollider.name + "\"");
		}
	}

}
