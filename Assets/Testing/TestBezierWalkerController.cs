using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBezierWalkerController : MonoBehaviour {

	[SerializeField] TestBezierWalker[] walkers;
	[SerializeField] TestBezierWalker.TravelMode travelMode;
	[SerializeField] float speed;

	void Start(){
		
	}
	
	void Update(){
		for(int i=0; i<walkers.Length; i++){
			walkers[i].travelMode = travelMode;
			walkers[i].speed = speed;
		}
	}
}
