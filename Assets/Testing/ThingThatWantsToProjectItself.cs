using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingThatWantsToProjectItself : MonoBehaviour {

	void Start(){
		
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.Return)) transform.position = PlayAreaScript.ProjectTowardsCamera(transform.position);
	}
}
