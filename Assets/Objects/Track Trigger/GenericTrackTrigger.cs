using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrackTrigger : TrackTrigger {

	[SerializeField] UnityEvent action;

	protected override void TriggerEnterAction (Collider otherCollider){
		action.Invoke();
	}

}
