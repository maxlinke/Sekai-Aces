using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class SplineChangeTrackTrigger : TrackTrigger {

	[SerializeField] BezierSpline targetSpline;

	protected override void TriggerEnterAction (Collider otherCollider){
		TrackFollower trackFollower = otherCollider.GetComponent<TrackFollower>();
		trackFollower.ChangeSpline(targetSpline);
	}

}
