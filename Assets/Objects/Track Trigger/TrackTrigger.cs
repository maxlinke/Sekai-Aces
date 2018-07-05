using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackTrigger : MonoBehaviour {

	[SerializeField] UnityEvent action;

	void OnTriggerEnter(Collider otherCollider){
		if(otherCollider.CompareTag("TrackFollower")){
			action.Invoke();
		}else{
			Debug.Log("got triggered by non-track follower \"" + otherCollider.name + "\"");
		}
	}

}
