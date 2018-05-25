using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbitScript : MonoBehaviour {

	[SerializeField] GameObject center;
	[SerializeField] float orbitDuration;

	void Start () {
		
	}
	
	void Update () {
		transform.RotateAround(center.transform.position, center.transform.up, Time.deltaTime * (360f / orbitDuration));
	}
}
