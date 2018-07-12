using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public abstract class TrackTrigger : MonoBehaviour {

	public BezierSpline curve;
	[SerializeField] [Range(0f,1f)] float positionOnCurve;

	public void ApplyPosition () {
		Vector3 beforePos = transform.position;
		transform.position = curve.GetPoint(positionOnCurve);
		Vector3 afterPos = transform.position;
		Debug.DrawLine(beforePos, afterPos, Color.magenta, 1f, false);
	}

	public void SnapToCurve () {
		positionOnCurve = curve.GetClosestPosition(transform.position);
		ApplyPosition();
	}

	void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("TrackFollower")){
			TriggerEnterAction(otherCollider);
		}else{
			Debug.LogError("got triggered by non-track follower \"" + otherCollider.name + "\"");
		}
	}

	protected abstract void TriggerEnterAction (Collider otherCollider);

}
