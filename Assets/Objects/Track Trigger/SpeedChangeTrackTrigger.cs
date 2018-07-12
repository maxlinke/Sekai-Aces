using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChangeTrackTrigger : TrackTrigger {

	[SerializeField] float targetSpeed;
	[SerializeField] float speedChangeDuration;

	protected override void TriggerEnterAction (Collider otherCollider){
		TrackFollower trackFollower = otherCollider.GetComponent<TrackFollower>();
		trackFollower.ChangeSpeed(targetSpeed, speedChangeDuration);
	}

}
